// File: CookingStation.cs
using UnityEngine;
using System.Collections.Generic;
using System;

public class CookingStation : MonoBehaviour
{
    public event Action<IngredientType> OnIngredientPicked;
    public event Action<DishType> OnCooked; 
    public event Action<string> OnFeedback;      // UI message

    private List<IngredientType> buffer = new();
    public GameObject dialogPanel;

    public void PickIngredient(int ingredientInt) // taro ke UI buttons
    {
        var ing = (IngredientType)ingredientInt;

        if (buffer.Count >= 3)
        {
            OnFeedback?.Invoke("Sudah 3 bahan. Masak dulu ya.");
            return;
        }
        OnIngredientPicked?.Invoke(ing);
        buffer.Add(ing);
        OnFeedback?.Invoke($"Tambahkan {ing}.");
    }

    public void Cook()
    {
        if (buffer.Count != 3)
        {
            OnFeedback?.Invoke("Butuh tepat 3 bahan.");
            return;
        }

        var dish = RecipeDB.MatchRecipe(buffer);
        buffer.Clear();

        if (dish == DishType.None)
        {
            OnFeedback?.Invoke("Resep tidak dikenal. (Gagal)");
            OnCooked?.Invoke(DishType.None);
            LeanTween.scale(dialogPanel, Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() =>
            {
                dialogPanel.SetActive(false);
            });
        }
        else
        {
            dialogPanel.SetActive(false);
            OnFeedback?.Invoke($"Masak {RecipeDB.DishDisplayName(dish)}");
            OnCooked?.Invoke(dish);
        }
    }
    public void ClearBuffer()
    {
        buffer.Clear();
        OnFeedback?.Invoke("Batal. Buffer kosong.");
    }
}
