using System.Collections.Generic;
using System.Linq;

public static class RecipeDB
{
    // Each recipe is exactly 3 ingredients (order-insensitive)
    private static readonly Dictionary<DishType, IngredientType[]> _recipes = new()
    {
        { DishType.SotoAyam,   new[] { IngredientType.Ayam, IngredientType.Air, IngredientType.Kunyit } },
        { DishType.SotoDaging, new[] { IngredientType.Daging, IngredientType.Air, IngredientType.Kunyit } },
        { DishType.Rawon,      new[] { IngredientType.Daging, IngredientType.Air, IngredientType.Kluwek } },
        { DishType.SateDaging, new[] { IngredientType.Daging, IngredientType.Daging, IngredientType.Kacang } },
        { DishType.SateAyam,   new[] { IngredientType.Ayam, IngredientType.Ayam, IngredientType.Kacang } },
        { DishType.Seblak,     new[] { IngredientType.Kerupuk, IngredientType.Kerupuk, IngredientType.Air } },
        { DishType.Teh,        new[] { IngredientType.Teh, IngredientType.Teh, IngredientType.Air } },
        { DishType.Kopi,       new[] { IngredientType.Kopi, IngredientType.Kopi, IngredientType.Air } },
    };

    public static DishType MatchRecipe(IList<IngredientType> picked)
    {
        if (picked == null || picked.Count != 3) return DishType.None;
        var sorted = picked.OrderBy(x => x).ToArray();

        foreach (var kv in _recipes)
        {
            var recipeSorted = kv.Value.OrderBy(x => x).ToArray();
            if (recipeSorted.SequenceEqual(sorted))
                return kv.Key;
        }
        return DishType.None;
    }

    public static string DishDisplayName(DishType dish) => dish switch
    {
        DishType.SotoAyam => "Soto Ayam",
        DishType.SotoDaging => "Soto Daging",
        DishType.Rawon => "Rawon",
        DishType.SateDaging => "Sate Daging",
        DishType.SateAyam => "Sate Ayam",
        DishType.Seblak => "Seblak",
        DishType.Teh => "Teh",
        DishType.Kopi => "Kopi",
        DishType.None => "Misterius",
        _ => ""
    };
}
