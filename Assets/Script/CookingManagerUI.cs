using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CookingUIManager : MonoBehaviour
{
    public CookingStation station;
    public List<Image> ingredientSlots;  // Inspector (3 slots)
    public Sprite defaultSprite;
    public List<Sprite> ingredientSprites; // follow the enum order
    public TextMeshProUGUI testText;

    private int currentSlot = 0;

    void Start()
    {
        station.OnCooked += ShowCookResult;
        station.OnIngredientPicked += HandleIngredientPicked;
        station.OnFeedback += ShowFeedback;
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
        testText.text = RecipeDB.DishDisplayName(dish);
    }
    public void ResetSlots()
    {
        foreach (var slot in ingredientSlots)
        {
            slot.sprite = defaultSprite;
        }
        currentSlot = 0;
    }
}
