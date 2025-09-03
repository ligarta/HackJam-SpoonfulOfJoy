using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DayEvent
{
    public string eventName;

    [Header("Dialogues")]
    public DialogNode dialogToStart;        // Pre-cooking dialog
    public bool isCookingEvent;             // true if this is a cooking minigame
    public DialogNode correctFoodDialog;    // Branch if cooking is correct
    public DialogNode incorrectFoodDialog;  // Branch if cooking is wrong
    public DishType correctDishType;
    [Header("Customer Info")]
    public CustomerData customer;           // assign SO (sprite, animations, etc.)
    [Range(1, 3)]
    public int placeIndex;
}

[CreateAssetMenu(fileName = "DayData", menuName = "Game/DayData")]
public class DayData : ScriptableObject
{
    public int dayNumber;
    public List<DayEvent> events;
}
