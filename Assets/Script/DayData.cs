using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DayEvent
{
    public string eventName;
    public DialogNode dialogToStart;   // dialog assigned in Inspector
    public bool isCookingEvent;        // true if this is a cooking minigame

    [Header("Customer Info")]
    public CustomerData customer;      // assign SO (sprite, animations, etc.)
    [Range(1, 3)]
    public int placeIndex;
}

[CreateAssetMenu(fileName = "DayData", menuName = "Game/DayData")]
public class DayData : ScriptableObject
{
    public int dayNumber;
    public List<DayEvent> events;
}
