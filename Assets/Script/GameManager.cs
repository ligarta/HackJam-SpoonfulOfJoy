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
        currentCamera.transform.position = OriginalCameraPosition.position;
        if (currentCamera.orthographic)
            currentCamera.orthographicSize = 10f; 
        else
            currentCamera.fieldOfView = 60f;
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

        StopAllCoroutines();
        StartCoroutine(SmoothFocus(targetPos, zoomSize, 3f));
    }

    private IEnumerator SmoothFocus(Vector3 targetPos, float targetZoom, float duration)
    {
        float elapsed = 0f;

        Vector3 startPos = currentCamera.transform.position;
        float startZoom = currentCamera.orthographic ? currentCamera.orthographicSize : currentCamera.fieldOfView;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            currentCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);

            if (currentCamera.orthographic)
                currentCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);
            else
                currentCamera.fieldOfView = Mathf.Lerp(startZoom, targetZoom, t);

            yield return null;
        }
        currentCamera.transform.position = targetPos;
        if (currentCamera.orthographic)
            currentCamera.orthographicSize = targetZoom;
        else
            currentCamera.fieldOfView = targetZoom;
    }
}
