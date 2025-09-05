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

    private float originalOrthoSize;
    private Vector2 originalScreenPosition;

    void Start()
    {
        if (dayManager != null)
        {
            dayManager._CookingEvent += HandleCookingUI;
            dayManager._CookingEventDone += HandleCookingFinished;
        }
        if (cinemachineCamera != null)
        {
            originalOrthoSize = cinemachineCamera.Lens.OrthographicSize;
        }
        if (cinemachineComposer != null)
        {
            originalScreenPosition = cinemachineComposer.Composition.ScreenPosition;
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

    public void CookingToDialogue()
    {
        currentState = GameState.Dialogue;

        // Clear follow target
        cinemachineCamera.Follow = null;

        float duration = 2f;

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
