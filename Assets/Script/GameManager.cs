using System.Collections;
using Unity.Cinemachine;
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
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachinePositionComposer cinemachineComposer;


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
            dayManager._CookingEventDone += HandleCookingFinished;
    }

    void OnDestroy()
    {
        if (dialogManager != null)
            dialogManager.OnDialogFinished -= HandleCookingUI;
    }

    private void HandleCookingUI()
    {
        DialogueToCooking();
    }
    private void HandleCookingFinished()
    {
        CookingToDialogue();
    }
    public void DialogueToCooking()
    {
        currentState = GameState.Cooking;
        currentSelectedCustomerSlot = dayManager.getSelectedCharacterSpriteRenderer().gameObject;
        FocusOnCustomerWhileCooking();
    }

    public void CookingToDialogue()
    {
        currentState = GameState.Dialogue;

        LeanTween.cancel(currentCamera.gameObject);
        Vector3 original = new Vector3(0, 0, -10);
        float duration = 2f;

        // Tween position
        LeanTween.move(currentCamera.gameObject, original, duration).setEase(LeanTweenType.easeInOutQuad);

        // Tween zoom reset
        if (currentCamera.orthographic)
        {
            LeanTween.value(currentCamera.gameObject, currentCamera.orthographicSize, 5f, duration)
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

        cinemachineCamera.Follow = currentSelectedCustomerSlot.transform;
        LeanTween.value(gameObject, 5, 3.35f, 1f).setEaseOutQuad()
        .setOnUpdate((float val) => {
            cinemachineCamera.Lens.OrthographicSize = val;
        });
        LeanTween.value(gameObject, 0, -0.3f, 2f).setEaseOutQuad().setOnUpdate((float val) => {
            cinemachineComposer.Composition.ScreenPosition.Set(val, 0);
        });
    }
}
