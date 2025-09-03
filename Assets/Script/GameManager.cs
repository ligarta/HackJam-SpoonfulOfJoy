using System.Collections;
using UnityEngine;

public enum GameState
{
    Dialogue,
    Cooking,
    Paused,
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private CookingStation cookingStation;

    [Header("Camera Settings")]
    public Transform OriginalCameraPosition;
    public Camera currentCamera;
    public GameObject currentSelectedCustomerSlot;
    public float zoomSize = 5f;
    public float smoothSpeed = 2f;

    private Vector3 desiredViewportPos = new Vector3(0.25f, 0.5f, 0f);

    public GameState currentState = GameState.Dialogue;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (dayManager != null)
            dayManager._CookingEvent += HandleCookingUI;
        if (cookingStation != null)
            cookingStation.OnCooked += HandleCookingFinished;
    }

    void OnDestroy()
    {
        if (dialogManager != null)
            dialogManager.OnDialogFinished -= HandleCookingUI;

        if (cookingStation != null)
            cookingStation.OnCooked -= HandleCookingFinished;
    }

    private void HandleCookingUI()
    {
        DialogueToCooking();
    }
    private void HandleCookingFinished(DishType dish)
    {
       
    }
    public void DialogueToCooking()
    {
        currentState = GameState.Cooking;
        FocusOnCustomerWhileCooking();
    }

    public void CookingToDialogue()
    {
        currentState = GameState.Dialogue;

        LeanTween.cancel(currentCamera.gameObject);

        float duration = 2f;

        // Tween position
        LeanTween.move(currentCamera.gameObject, OriginalCameraPosition.position, duration).setEase(LeanTweenType.easeInOutQuad);

        // Tween zoom reset
        if (currentCamera.orthographic)
        {
            LeanTween.value(currentCamera.gameObject, currentCamera.orthographicSize, 10f, duration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnUpdate((float val) => currentCamera.orthographicSize = val);
        }
        else
        {
            LeanTween.value(currentCamera.gameObject, currentCamera.fieldOfView, 60f, duration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnUpdate((float val) => currentCamera.fieldOfView = val);
        }
    }

    // Camera focus metod
    public void FocusOnCustomerWhileCooking()
    {
        if (currentCamera == null || currentSelectedCustomerSlot == null) return;

        Vector3 viewportPos = currentCamera.WorldToViewportPoint(currentSelectedCustomerSlot.transform.position);
        Vector3 desiredWorldPos = currentCamera.ViewportToWorldPoint(
            new Vector3(desiredViewportPos.x, desiredViewportPos.y, viewportPos.z)
        );

        Vector3 targetPos = currentCamera.transform.position + (currentSelectedCustomerSlot.transform.position - desiredWorldPos);

        // Kill any existing tweens on the camera to avoid conflicts
        LeanTween.cancel(currentCamera.gameObject);

        float duration = 3f;

        // Tween position
        LeanTween.move(currentCamera.gameObject, targetPos, duration).setEase(LeanTweenType.easeInOutQuad);

        // Tween zoom
        if (currentCamera.orthographic)
        {
            LeanTween.value(currentCamera.gameObject, currentCamera.orthographicSize, zoomSize, duration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnUpdate((float val) =>
                {
                    currentCamera.orthographicSize = val;
                });
        }
        else
        {
            LeanTween.value(currentCamera.gameObject, currentCamera.fieldOfView, zoomSize, duration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnUpdate((float val) =>
                {
                    currentCamera.fieldOfView = val;
                });
        }
    }

}
