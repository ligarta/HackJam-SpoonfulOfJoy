using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    public List<DayData> days;
    public DialogManager dialogManager;
    public CookingUIManager cookingManagerUI;
    public CookingStation cookingStation;
    public event Action _CookingEvent;
    private int currentDayIndex;
    private int currentEventIndex;
    [SerializeField] TextMeshProUGUI[] dialogText;
    [SerializeField] TextMeshProUGUI[] orderText;
    [SerializeField] TextMeshProUGUI[] dialogtextnametext;
    [SerializeField] TextMeshProUGUI[] orderDialogtextnametext;
    [SerializeField] GameObject[] dialogtextPanels;
    [SerializeField] GameObject[] orderTextPanels;
    [SerializeField] SpriteRenderer[] customerSpriteRenderer;
    [SerializeField] SpriteRenderer[] foodSpriteRenderer;

    void Start()
    {
        Debug.Log("[DayManager] Start() called");
        dialogManager.OnDisplayingNewLine += assignCurrentDialogText;
        StartDay(0);
    }

    void StartDay(int index)
    {
        Debug.Log($"[DayManager] StartDay() index={index}");

        if (index >= days.Count)
        {
            Debug.Log("[DayManager] No more days available!");
            return;
        }

        currentDayIndex = index;
        currentEventIndex = 0;
        RunNextEvent();
    }

    void RunNextEvent()
    {
        Debug.Log($"[DayManager] RunNextEvent() currentDayIndex={currentDayIndex}, currentEventIndex={currentEventIndex}");

        if (currentDayIndex >= days.Count)
        {
            Debug.LogError("[DayManager] currentDayIndex is out of range!");
            return;
        }

        var day = days[currentDayIndex];
        Debug.Log($"[DayManager] Loaded Day {day.dayNumber} with {day.events.Count} events");

        if (currentEventIndex >= day.events.Count)
        {
            Debug.Log($"[DayManager] No more events. Day {day.dayNumber} finished!");
            if (currentDayIndex + 1 < days.Count)
            {
                StartCoroutine(SwitchDayAnimation(currentDayIndex + 1));
            }
            else
            {
                Debug.Log("[DayManager] All days completed!");
                // Handle game completion here
            }
            return;
        }

        var ev = day.events[currentEventIndex];
        Debug.Log($"[DayManager] Running event {currentEventIndex}, isCookingEvent={ev.isCookingEvent}");

        // Safety checks
        if (ev.dialogToStart == null)
        {
            Debug.LogError($"[DayManager] Event {currentEventIndex} has null dialogToStart!");
            currentEventIndex++;
            RunNextEvent();
            return;
        }

        if (ev.dialogToStart.lines == null || ev.dialogToStart.lines.Length == 0)
        {
            Debug.LogError($"[DayManager] Event {currentEventIndex} has no dialog lines!");
            currentEventIndex++;
            RunNextEvent();
            return;
        }

        // Get the place index from the first line of the dialog
        int currentselectedIndex = ev.dialogToStart.lines[0].placeIndex;
        Debug.Log($"[DayManager] currentselectedIndex={currentselectedIndex}");
        if (ev.isCookingEvent)
        {
            Debug.Log("[DayManager] Setting up cooking event");
            assignCurrentOrderDialogText(currentselectedIndex - 1);
        }
        else
        {
            if (currentselectedIndex > 0)
            {
                Debug.Log("[DayManager] Setting up dialog event");
                assignCurrentDialogText(currentselectedIndex - 1);
            }
            else
            {
                Debug.LogWarning("[DayManager] currentselectedIndex is 0, using default dialog setup");
                assignCurrentDialogText(0);
            }
        }

        if (currentselectedIndex >= 1 && currentselectedIndex <= customerSpriteRenderer.Length)
        {
            Debug.Log("[DayManager] Setting character sprite");
            dialogManager.setCharacterSprite(customerSpriteRenderer[currentselectedIndex - 1]);
        }

        Debug.Log("[DayManager] Starting dialog");

        if (ev.isCookingEvent)
        {
            dialogManager.isCooking = true;
            dialogManager.StartDialog(ev.dialogToStart);

            if (cookingStation != null)
            {
                cookingStation.gameObject.SetActive(true);
                cookingStation.OnCooked += HandleCookingFinished;
            }

            _CookingEvent?.Invoke();

            if (cookingManagerUI != null)
            {
                cookingManagerUI.SlideFirstStageCookingUI();
            }
        }
        else
        {
            dialogManager.StartDialog(ev.dialogToStart);
            dialogManager.OnDialogFinished += HandleDialogFinished;
        }
    }

    void HandleDialogFinished()
    {
        Debug.Log("[DayManager] HandleDialogFinished() called");
        dialogManager.OnDialogFinished -= HandleDialogFinished;
        currentEventIndex++;
        RunNextEvent();
    }

    private DishType currentDish;

    void HandleCookingFinished(DishType dish)
    {
        Debug.Log($"[DayManager] HandleCookingFinished() dish={dish}");
        currentDish = dish;
        if (cookingStation != null)
        {
            cookingStation.OnCooked -= HandleCookingFinished;
        }
    }

    public void cookingConfirmed()
    {
        Debug.Log("[DayManager] cookingConfirmed() called");

        if (currentDayIndex >= days.Count || currentEventIndex >= days[currentDayIndex].events.Count)
        {
            Debug.LogError("[DayManager] cookingConfirmed: Invalid day or event index!");
            return;
        }

        var ev = days[currentDayIndex].events[currentEventIndex];
        bool success = false;

        if (ev.correctDishType != null)
        {
            foreach (DishType a in ev.correctDishType)
            {
                if (currentDish == a)
                {
                    success = true;
                    break;
                }
            }
        }

        Debug.Log($"[DayManager] cookingConfirmed result success={success}");

        DialogNode resultDialog = success ? ev.correctFoodDialog : ev.incorrectFoodDialog;

        if (resultDialog != null)
        {
            dialogManager.StartDialog(resultDialog);
        }
        else
        {
            Debug.LogWarning("[DayManager] No result dialog found!");
        }

        dialogManager.isCooking = false;

        // Wait for dialog to start before accessing current line
        StartCoroutine(SetFoodSpriteAfterDialogStart());

        dialogManager.OnDialogFinished += HandleDialogFinished;
    }

    private IEnumerator SetFoodSpriteAfterDialogStart()
    {
        // Wait a frame for dialog to start
        yield return null;

        if (dialogManager.IsDialogActive())
        {
            var currentLine = dialogManager.getCurrentLine();
            if (currentLine != null)
            {
                int index = currentLine.placeIndex - 1;
                if (index >= 0 && index < foodSpriteRenderer.Length && cookingManagerUI != null)
                {
                    Debug.Log($"[DayManager] Setting food sprite at index {index} for dish={currentDish}");
                    foodSpriteRenderer[index].sprite = cookingManagerUI.GetMenuSprite(currentDish);
                }
            }
        }
    }

    public void assignCurrentDialogText(int currentIndex)
    {
        Debug.Log($"[DayManager] assignCurrentDialogText() index={currentIndex}");

        if (currentIndex < 0 || currentIndex >= dialogText.Length)
        {
            Debug.LogError($"[DayManager] assignCurrentDialogText: Invalid index {currentIndex}");
            return;
        }

        resetDialogBox();
        dialogtextPanels[currentIndex].SetActive(true);

        if (currentIndex < dialogtextnametext.Length && currentIndex != 4)
        {
            Debug.Log($"[DayManager] assigning index={currentIndex}");
            dialogManager.SetDialogSpeakerNameText(dialogtextnametext[currentIndex]);
        }
        else
        {
            dialogManager.SetDialogSpeakerNameText(null);
        }

        dialogManager.SetdialogText(dialogText[currentIndex]);
        dialogManager.SetDialogPanel(dialogtextPanels[currentIndex]);
    }

    public void resetDialogBox()
    {
        Debug.Log("[DayManager] resetDialogBox()");

        if (dialogtextPanels != null)
        {
            foreach (GameObject a in dialogtextPanels)
            {
                if (a != null) a.SetActive(false);
            }
        }

        if (orderTextPanels != null)
        {
            foreach (GameObject b in orderTextPanels)
            {
                if (b != null) b.SetActive(false);
            }
        }
    }

    public void assignCurrentOrderDialogText(int currentIndex)
    {
        Debug.Log($"[DayManager] assignCurrentOrderDialogText() index={currentIndex}");

        if (currentIndex < 0 || currentIndex >= orderText.Length)
        {
            Debug.LogError($"[DayManager] assignCurrentOrderDialogText: Invalid index {currentIndex}");
            return;
        }

        resetDialogBox();

        if (orderTextPanels != null && currentIndex < orderTextPanels.Length)
        {
            orderTextPanels[currentIndex].SetActive(true);
            dialogManager.SetDialogPanel(orderTextPanels[currentIndex]);
        }

        if (orderDialogtextnametext != null && currentIndex < orderDialogtextnametext.Length)
        {
            dialogManager.SetDialogSpeakerNameText(orderDialogtextnametext[currentIndex]);
        }

        dialogManager.SetdialogText(orderText[currentIndex]);
    }

    [Header("Day Transition UI")]
    [SerializeField] Image fadePanel;             // black fullscreen Image
    [SerializeField] TextMeshProUGUI dayText;     // child text
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float displayDuration = 1.5f;

    private IEnumerator SwitchDayAnimation(int index)
    {
        Debug.Log($"[DayManager] SwitchDayAnimation() to day index {index}");
        yield return new WaitForSeconds(2f);

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color panelColor = fadePanel.color;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                panelColor.a = Mathf.Clamp01(t / fadeDuration);
                fadePanel.color = panelColor;
                yield return null;
            }

            if (dayText != null && index < days.Count)
            {
                dayText.text = $"Day {days[index].dayNumber}";
                dayText.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(displayDuration);

            t = fadeDuration;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                panelColor.a = Mathf.Clamp01(t / fadeDuration);
                fadePanel.color = panelColor;
                yield return null;
            }

            fadePanel.gameObject.SetActive(false);
        }

        StartDay(index);
    }
}