using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class DayManager : MonoBehaviour
{
    public List<DayData> days;  
    public DialogManager dialogManager;
    public CookingStation cookingStation;
    public event Action _CookingEvent;
    private int currentDayIndex;
    private int currentEventIndex;
    [SerializeField] TextMeshProUGUI[] dialogText;
    [SerializeField] TextMeshProUGUI[] orderText;
    [SerializeField] GameObject[] dialogtextPanels;
    [SerializeField] GameObject[] orderTextPanels;
    [SerializeField] Animator[] CustormersAnimator;
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
    private DishType currentDish;
    void HandleCookingFinished(DishType dish)
    {
        currentDish = dish;
        cookingStation.OnCooked -= HandleCookingFinished;
    }
    public void cookingConfirmed()
    {
        var ev = days[currentDayIndex].events[currentEventIndex];
        bool success = (currentDish == ev.correctDishType);

        if (success)
        {
            dialogManager.StartDialog(ev.correctFoodDialog);
        }
        else
        {
            dialogManager.StartDialog(ev.incorrectFoodDialog);
        }
        dialogManager.OnDialogFinished += HandleDialogFinished;
    }
    public void assignCurrentDialogText(int currentIndex)
    {
        resetDialogBox();
        dialogText[currentIndex].gameObject.SetActive(true);
        dialogManager.SetdialogText(dialogText[currentIndex]);
    }
    public void resetDialogBox()
    {
        foreach (TextMeshProUGUI a in dialogText)
        {
            a.gameObject.SetActive(false);
        }
        foreach (TextMeshProUGUI b in orderText)
        {
            b.gameObject.SetActive(false);
        }
    }
}
