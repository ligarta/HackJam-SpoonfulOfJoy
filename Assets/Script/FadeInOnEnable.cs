using UnityEngine;

public class FadeInOnEnable : MonoBehaviour
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class SlideFadeInOnEnable : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float slideDistance = 200f;   
        [SerializeField] private float duration = 0.8f;
        [SerializeField] private LeanTweenType easeType = LeanTweenType.easeOutCubic;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Vector2 originalPos;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            originalPos = rectTransform.anchoredPosition;
        }

        void OnEnable()
        {
            rectTransform.anchoredPosition = originalPos + new Vector2(-slideDistance, 0f);
            canvasGroup.alpha = 0f;
            LeanTween.move(rectTransform, originalPos, duration)
                .setEase(easeType);
            LeanTween.value(gameObject, 0f, 1f, duration)
                .setOnUpdate((float val) => canvasGroup.alpha = val);
        }
    }
}
