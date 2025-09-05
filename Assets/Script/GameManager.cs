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


    public GameState currentState = GameState.Dialogue;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (dayManager != null)
        {
            dayManager._CookingEvent += HandleCookingUI;
            dayManager._CookingEventDone += HandleCookingFinished;
        }
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

    // Add at class level
    private float originalOrthoSize;
    private Vector2 originalScreenPosition;
    private Transform originalFollowTarget;

    public void FocusOnCustomerWhileCooking()
    {
        if (currentCamera == null || currentSelectedCustomerSlot == null) return;

        originalFollowTarget = cinemachineCamera.Follow;
        originalOrthoSize = cinemachineCamera.Lens.OrthographicSize;
        originalScreenPosition = cinemachineComposer.Composition.ScreenPosition;

        // Focus on customer
        cinemachineCamera.Follow = currentSelectedCustomerSlot.transform;

        LeanTween.value(gameObject, originalOrthoSize, 3.35f, 1f).setEaseOutQuad()
            .setOnUpdate((float val) => {
                cinemachineCamera.Lens.OrthographicSize = val;
            });

        LeanTween.value(gameObject, originalScreenPosition.x, -0.3f, 2f).setEaseOutQuad()
            .setOnUpdate((float val) => {
                // Important: assign a *new Vector2*, don’t call Set()
                cinemachineComposer.Composition.ScreenPosition = new Vector2(val, 0f);
            });
    }

    public void CookingToDialogue()
    {
        currentState = GameState.Dialogue;

        float duration = 2f;

        // Restore follow
        cinemachineCamera.Follow = originalFollowTarget;

        // Tween zoom back to original
        LeanTween.value(gameObject, cinemachineCamera.Lens.OrthographicSize, originalOrthoSize, duration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnUpdate((float val) =>
            {
                cinemachineCamera.Lens.OrthographicSize = val;
            });

        // Tween screen composition back
        Vector2 startPos = cinemachineComposer.Composition.ScreenPosition;
        LeanTween.value(gameObject, 0f, 1f, duration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnUpdate((float t) =>
            {
                cinemachineComposer.Composition.ScreenPosition =
                    Vector2.Lerp(startPos, originalScreenPosition, t);
            });
    }
    }
