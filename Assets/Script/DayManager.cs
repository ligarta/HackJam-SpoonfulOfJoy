using UnityEngine;
using System.Collections.Generic;
using System;

public class DayManager : MonoBehaviour
{
    public List<DayData> days;  
    public DialogManager dialogManager;
    public CookingStation cookingStation;
    public event Action _CookingEvent;
    private int currentDayIndex;
    private int currentEventIndex;

    void Start()
    {
        StartDay(0); 
    }

    void StartDay(int index)
    {
        currentDayIndex = index;
        currentEventIndex = 0;
        RunNextEvent();
    }

    void RunNextEvent()
    {
        var day = days[currentDayIndex];

        if (currentEventIndex >= day.events.Count)
        {
            Debug.Log($"Day {day.dayNumber} finished!");
            return;
        }

        var ev = day.events[currentEventIndex];
        if (ev.isCookingEvent)
        {
            cookingStation.gameObject.SetActive(true);
            cookingStation.OnCooked += HandleCookingFinished;
            _CookingEvent?.Invoke();
        }
        else
        {
            dialogManager.StartDialog(ev.dialogToStart);
            dialogManager.OnDialogFinished += HandleDialogFinished;
        }
    }

    void HandleDialogFinished()
    {
        dialogManager.OnDialogFinished -= HandleDialogFinished;
        currentEventIndex++;
        RunNextEvent();
    }

    void HandleCookingFinished(DishType dish)
    {
        cookingStation.OnCooked -= HandleCookingFinished;
        currentEventIndex++;
        RunNextEvent();
    }
}
