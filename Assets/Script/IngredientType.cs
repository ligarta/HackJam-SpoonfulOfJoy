using UnityEngine;
public enum IngredientType
{
    Ayam,
    Daging,
    Air,
    DaunJeruk,
    Kluwek,
    Kacang,
    Kerupuk,
    Teh,
    Kopi
}

public enum DishType
{
    None,
    SotoAyam,      // Ayam + Air + DaunJeruk
    SotoDaging,    // Daging + Air + DaunJeruk
    Rawon,         // Daging + Air + Kluwek
    SateDaging,    // Daging + Daging + Kacang
    SateAyam,      // Ayam + Ayam + Kacang
    Seblak,        // Kerupuk + Kerupuk + Air
    Teh,           // Teh + Teh + Air
    Kopi           // Kopi + Kopi + Air
}

