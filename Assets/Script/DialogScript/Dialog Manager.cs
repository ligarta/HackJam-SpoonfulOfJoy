using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;  // new input system

public class DialogManager : MonoBehaviour
{
    public event Action OnDialogFinished;
    public event Action<int> OnDisplayingNewLine;
    public event Action<int> OnCharacterLeave;
    public GameObject panel;
    [SerializeField] private SpriteRenderer characterSprite;

    [Header("UI Elements")]
    [SerializeField] TMP_Text speakerNametext;   // now optional
    [SerializeField] TMP_Text dialogText;
    [SerializeField] GameObject dialogPanel;
    [SerializeField] GameObject choicePanel;   // removed choice
    [SerializeField] GameObject prefebChoices;   // removed choice

    public DialogLine currentLine;

    [Header("Audio")]
    public AudioClip effectAudioSource;
    [SerializeField] float textSpeed = 0.05f;
    [SerializeField] AudioClip audioClick;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioType;

    [Header("Setting")]
    DialogNode currentNode;
    public int currentLineIndex = 0;
    bool isTyping = false;
    [SerializeField] private String NextScene;
    [SerializeField] private String chatWith;
    public bool isCooking = false;
    [SerializeField] GameObject MenuPanel;
    bool isPause = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentLine = null;
    }

    public void StartDialog(DialogNode node)
    {
        if (node == null || node.lines == null || node.lines.Length == 0)
        {
            Debug.LogError("DialogManager: Invalid node or empty lines.");
            return;
        }

        currentNode = node;
        currentLineIndex = 0;
        LeanTween.cancel(dialogPanel);

        dialogPanel.SetActive(true);
        LeanTween.scale(dialogPanel, Vector3.one, 0.5f).setEaseOutExpo();

        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (currentLineIndex < currentNode.lines.Length && !isCooking)
        {
            OnDisplayingNewLine?.Invoke(currentNode.lines[currentLineIndex].placeIndex - 1);
        }

        if (currentNode == null || currentNode.lines == null)
        {
            Debug.LogError("[DialogManager] DisplayCurrentLine: currentNode or lines is null!");
            if (!isCooking)
            {
                EndDialog();
            }
            return;
        }

        if (currentLineIndex >= currentNode.lines.Length)
        {
            if (isCooking)
            {
                Debug.Log("[DialogManager] Reached end of dialog but cooking is active � keeping dialog open.");
                if (currentNode.lines.Length > 0)
                {
                    currentLine = currentNode.lines[currentNode.lines.Length - 1];
                }
                return;
            }

            if (currentNode.nextNode == null)
            {
                EndDialog();
                PlayerPrefs.SetString("sceneBefore", "chat");
            }
            else
            {
                currentNode = currentNode.nextNode;
                currentLineIndex = 0;
                if (currentNode.lines != null && currentNode.lines.Length > 0)
                {
                    currentLine = currentNode.lines[currentLineIndex];
                }
                DisplayCurrentLine();
            }
            return;
        }

        currentLine = currentNode.lines[currentLineIndex];
        DialogLine line = currentLine;

        if (speakerNametext != null)
        {
            speakerNametext.text = line.speakerName;
        }

        setSprite(line);
        StartCoroutine(TypeText(line.text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogText.text = "";
        int visibleCharCount = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (i % 3 == 0) PlayAudio();
            dialogText.rectTransform.sizeDelta = new Vector2(dialogText.rectTransform.sizeDelta.x, dialogText.preferredHeight);
            if (text[i] == '<')
            {
                int closingTagIndex = text.IndexOf('>', i);
                if (closingTagIndex != -1)
                {
                    dialogText.text += text.Substring(i, closingTagIndex - i + 1);
                    i = closingTagIndex;
                }
            }

            dialogText.text += text[i];
            visibleCharCount++;
            dialogText.maxVisibleCharacters = visibleCharCount;

            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
        if (currentLine != null && currentLine.isLeaving)
        {
            int leavingIndex = currentLine.placeIndex - 1;
            OnCharacterLeave?.Invoke(leavingIndex);
        }
    }

    void PlayAudio()
    {
        if (audioType != null && audioType.isPlaying)
        {
            audioType.Stop();
        }
        if (audioType != null && effectAudioSource != null)
        {
            audioType.clip = effectAudioSource;
            audioType.Play();
        }
    }

    public void clicking()
    {
        if (isPause) return;
        Debug.Log("[DialogManager] Clicking triggered.");

        if (audioSource != null && audioClick != null)
        {
            Debug.Log("[DialogManager] Playing click sound.");
            audioSource.clip = audioClick;
            audioSource.Play();
        }

        if (isTyping)
        {
            Debug.Log("[DialogManager] Currently typing -> skip to end of line.");
            StopAllCoroutines();
            DialogLine currentLine = currentNode.lines[currentLineIndex];
            dialogText.text = currentLine.text;
            dialogText.maxVisibleCharacters = currentLine.text.Length;
            dialogText.rectTransform.sizeDelta = new Vector2(
                dialogText.rectTransform.sizeDelta.x,
                dialogText.preferredHeight
            );
            isTyping = false;

            if (audioSource != null && audioSource.isPlaying)
            {
                Debug.Log("[DialogManager] Stopping typing audio.");
                audioSource.Stop();
            }
        }
        else
        {
            if (isCooking && currentLineIndex >= currentNode.lines.Length - 1)
            {
                Debug.Log("[DialogManager] In cooking mode and last line reached -> return.");
                return;
            }

            currentLineIndex++;
            Debug.Log("[DialogManager] Advancing to line index: " + currentLineIndex);
            DisplayCurrentLine();
        }
    }

    public void SetdialogText(TextMeshProUGUI text)
    {
        dialogText = text;
    }

    public void SetDialogPanel(GameObject panel)
    {
        dialogPanel = panel;
    }

    public void SetDialogSpeakerNameText(TextMeshProUGUI text)
    {
        speakerNametext = text;
    }

    public void setCharacterSprite(SpriteRenderer sr)
    {
        characterSprite = sr;
    }

    void Update()
    {
        bool inputDetected = false;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputDetected = true;
            Debug.Log(IsDialogActive());
        }
        if (inputDetected && !isCooking && IsDialogActive())
        {
            clicking();
        }
    }

    void EndDialog()
    {
        Debug.Log("[DialogManager] EndDialog() called");
        if (isCooking)
        {
            Debug.Log("[DialogManager] EndDialog() blocked because cooking is active");
            return;
        }

        if (dialogText != null)
        {
            dialogText.text = "";
        }
        if (speakerNametext != null)
        {
            speakerNametext.text = "";
        }
        currentLine = null;
        OnDialogFinished?.Invoke();
        // if (dialogPanel != null && !currentNode.nextNode)
        // {
        //     LeanTween.scale(dialogPanel, new Vector3(0, 0, 0), 0.5f).setEaseInExpo().setOnComplete(() =>
        //     {
        //     dialogPanel.SetActive(false);
        //     });
        // }
    }

    public void Restart()
    {
    }

    public void Menu()
    {
        //SceneManager.LoadScene("Start Screen");
    }

    public void setSprite(DialogLine line)
    {
        if (line.charSprite != null && characterSprite != null)
        {
            characterSprite.color = new Color(1f, 1f, 1f, 0f);

            characterSprite.sprite = line.charSprite;

            LeanTween.sequence()
                .append(LeanTween.value(gameObject, 0f, 1f, 0.5f)
                    .setEaseInOutQuad()
                    .setOnUpdate((float val) =>
                    {
                        characterSprite.color = new Color(1f, 1f, 1f, val);
                    }))
                .append(() => ShakeSprite(line.isShake));
        }
    }

    private void ShakeSprite(bool isShake)
    {
        if (!isShake || characterSprite == null) return;

        Vector3 originalPos = characterSprite.transform.position;
        float shakeDuration = 0.5f;
        float shakeAmount = 0.2f;

        LeanTween.value(gameObject, 0f, 1f, shakeDuration)
            .setEaseShake()
            .setOnUpdate((float val) =>
            {
                Vector3 randomPos = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
                characterSprite.transform.position = new Vector3(randomPos.x, randomPos.y, originalPos.z);
            })
            .setOnComplete(() =>
            {
                characterSprite.transform.position = originalPos;
            });
    }

    public DialogLine getCurrentLine()
    {
        return currentLine;
    }

    public bool IsDialogActive()
    {
        bool active = currentLine != null && currentNode != null && dialogPanel != null && dialogPanel.activeInHierarchy;
        Debug.Log($"[DialogManager] IsDialogActive check - currentLine!=null: {currentLine != null}, currentNode!=null: {currentNode != null}, dialogPanel.activeInHierarchy: {dialogPanel?.activeInHierarchy}");
        return active;
    }
    public void openPause()
    {
        MenuPanel.SetActive(true);
        isPause = true;
        Time.timeScale = 0f;
    }
    public void closePause()
    {
        Time.timeScale = 1f;
        isPause = false;
        MenuPanel.SetActive(false);
    }
    public void toMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}