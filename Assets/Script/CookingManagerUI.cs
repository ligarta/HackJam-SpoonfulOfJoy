using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CookingUIManager : MonoBehaviour
{
    public CookingStation station;
    public List<Image> ingredientSlots;
    public Sprite defaultSprite;
    public List<Sprite> ingredientSprites;
    public List<Sprite> menuSprites;
    public List<TextMeshProUGUI> cookingtextPaper;
    public Image menuResult;
    public TextMeshProUGUI testText;
    private int currentSlot = 0;
    public GameObject dialogPanel;
    public Sprite TransparentPlaceHolder;

    [SerializeField] private List<Sprite> dishCookSequence1;
    [SerializeField] private List<Sprite> dishCookSequence2;
    [SerializeField] private List<Sprite> dishCookSequence3;
    [SerializeField] private DishType currentDish;

    void Start()
    {
        station._UpdateUI += UpdateIngredientSlot;
        station.OnCooked += ShowCookResult;
        station.OnIngredientPicked += HandleIngredientPicked;
        station.OnFeedback += ShowFeedback;

        originalcookingUiPanel = cookingUIPanel;
        originalSecondStageObjectPosition = secondStageObject;
        originalSecondObjectPos = secondStageObject.anchoredPosition;
    }

    void assignDishCookSequence()
    {
        for (int i = 0; i < 3; i++)
        {
            if(currentDish != DishType.None)
            {
                if (i == 0)
                    cookingSequenceImages[i].GetComponent<Image>().sprite = dishCookSequence1[(int)currentDish -1];
                else if (i == 1)
                    cookingSequenceImages[i].GetComponent<Image>().sprite = dishCookSequence2[(int)currentDish -1];
                else if (i == 2)
                    cookingSequenceImages[i].GetComponent<Image>().sprite = dishCookSequence3[(int)currentDish - 1];
            }
            else
            {
                if (i == 0)
                    cookingSequenceImages[i].GetComponent<Image>().sprite = dishCookSequence1[Random.Range(0,System.Enum.GetValues(typeof(DishType)).Length-1)];
                else if (i == 1)
                    cookingSequenceImages[i].GetComponent<Image>().sprite = dishCookSequence2[Random.Range(0, System.Enum.GetValues(typeof(DishType)).Length-1)];
                else if (i == 2)
                    cookingSequenceImages[i].GetComponent<Image>().sprite = dishCookSequence3[Random.Range(0, System.Enum.GetValues(typeof(DishType)).Length-1)];
            }
        }
    }

    void UpdateIngredientSlot(List<IngredientType> buffer)
    {
        for (int i = 0; i < 3; i++)
        {
            ingredientSlots[i].sprite = TransparentPlaceHolder;
            cookingtextPaper[i].text = "";
        }
        for (int a = 0; a < buffer.Count; a++)
        {
            ingredientSlots[a].sprite = ingredientSprites[(int)buffer[a]];
            cookingtextPaper[a].text = buffer[a].ToString();
        }
    }

    void HandleIngredientPicked(IngredientType ing)
    {
        if (currentSlot < ingredientSlots.Count)
        {
            ingredientSlots[currentSlot].sprite = ingredientSprites[(int)ing];
            currentSlot++;
        }
    }

    void ShowFeedback(string msg)
    {
        Debug.Log("UI Message: " + msg);
    }

    void ShowCookResult(DishType dish)
    {
        currentDish = dish;
        assignDishCookSequence();
        SlideSecondStageCookingUI();
        menuResult.sprite = GetMenuSprite(dish);
        testText.text = RecipeDB.DishDisplayName(dish);
    }

    public void ResetSlots()
    {
        foreach (var slot in ingredientSlots)
            slot.sprite = defaultSprite;

        currentSlot = 0;
    }

    [SerializeField] private RectTransform cookingUIPanel;
    [SerializeField] private float slideDuration = 1.50f;
    [SerializeField] private Vector2 targetAnchoredPos = new Vector2(25f, 0f);
    private RectTransform originalcookingUiPanel;

    public void SlideFirstStageCookingUI()
    {
        dialogPanel.SetActive(true);
        LeanTween.scale(dialogPanel, Vector3.one, 0.5f).setEaseOutBack();

        ResetCookingUI();

        if (cookingUIPanel == null) return;
        Vector2 startPos = new Vector2(Screen.width, targetAnchoredPos.y);
        cookingUIPanel.anchoredPosition = startPos;

        LeanTween.move(cookingUIPanel, targetAnchoredPos, slideDuration)
            .setEase(LeanTweenType.easeOutExpo);
    }

    [SerializeField] private RectTransform secondStageObject;
    [SerializeField] private float secondStageMoveAmount = 520f;
    [SerializeField] private float secondStageDuration = 0.8f;
    [SerializeField] private List<GameObject> cookingSequenceImages;
    [SerializeField] private float sequenceDelay = 1f;

    private RectTransform originalSecondStageObjectPosition;
    private Vector2 originalSecondObjectPos;

    public void SlideSecondStageCookingUI()
    {
        if (secondStageObject == null) return;

        Vector2 startPos = secondStageObject.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(0f, secondStageMoveAmount);

        LeanTween.move(secondStageObject, targetPos, secondStageDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() =>
            {
                StartCookingSequence();
            });
    }

    private void StartCookingSequence()
    {
        if (cookingSequenceImages == null || cookingSequenceImages.Count == 0) return;

        foreach (var img in cookingSequenceImages)
            img.SetActive(false);

        float delay = 0f;
        for (int i = 0; i < cookingSequenceImages.Count; i++)
        {
            int index = i;
            LeanTween.delayedCall(delay, () =>
            {
                cookingSequenceImages[index].SetActive(true);
            });
            delay += sequenceDelay;
        }

        LeanTween.delayedCall(delay, () =>
        {
            SlideThirdStageCookingUI();
        });
    }

    public void ResetCookingUI()
    {
        foreach (var img in cookingSequenceImages)
            img.SetActive(false);

        currentSlot = 0;

        if (secondStageObject != null)
            secondStageObject.anchoredPosition = originalSecondObjectPos;

        if (cookingUIPanel != null)
            cookingUIPanel.anchoredPosition = new Vector2(960f, 0f);
    }

    public void debug()
    {
        SlideSecondStageCookingUI();
    }

    [SerializeField] private Vector2 thirdStageTargetPos = new Vector2(-960f, 0f);
    [SerializeField] private float thirdStageDuration = 1.0f;

    public void SlideThirdStageCookingUI()
    {
        if (cookingUIPanel == null) return;

        LeanTween.move(cookingUIPanel, thirdStageTargetPos, thirdStageDuration)
            .setEase(LeanTweenType.easeInOutCubic);
    }

    public Sprite GetMenuSprite(DishType dish)
    {
        return menuSprites[(int)dish];
    }
}
