using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.InputSystem;  // new input system

public class DialogManager : MonoBehaviour
{
    public event Action OnDialogFinished;

    public GameObject panel;

    // private bool inChoice = false;   // removed choice
    // bool isChosed = false;           // removed choice
    // [SerializeField] private float anxStat;   // removed anx stat
    // Choice nextNode;                 // removed choice

    // public TextMeshProUGUI timer;

    [Header("UI Elements")]
    [SerializeField] TMP_Text speakerNametext;   // now optional
    [SerializeField] TMP_Text dialogText;
    [SerializeField] GameObject dialogPanel;
    //[SerializeField] Button choiceButtonPrefab;   // removed choice
    // [SerializeField] public Button progresButton;   // not needed anymore

    [Header("Audio")]
    // public AudioClip voiceAudioSource;   // not used
    public AudioClip effectAudioSource;
    [SerializeField] float textSpeed = 0.05f;
    [SerializeField] AudioClip audioClick;
    // [SerializeField] AudioClip audioTimer;   // not used
    // [SerializeField] AudioClip shuffleAudio; // not used
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioType;

    [Header("Setting")]
    //[SerializeField] string NextSceneDialog;
    DialogNode currentNode;
    int currentLineIndex = 0;
    bool isTyping = false;
    // public GameObject shuffleBtn;   // not used
    // bool isTimeOver = false;        // removed choice/timer
    [SerializeField] private String NextScene;
    [SerializeField] private String chatWith;
    //[SerializeField] GameObject PopUpGameOver;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // timer.SetText("");
        dialogPanel.SetActive(false);
    }

    public void StartDialog(DialogNode startNode)
    {
        dialogPanel.SetActive(true);
        currentNode = startNode;
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (currentNode.lines.Length == currentLineIndex /*&& currentNode.isChoiceNull()*/)
        {
            if (currentNode.nextNode == null)
            {
                // PlayerPrefs.SetFloat("anxStat", anxStat);   // removed anx stat
                EndDialog();
                PlayerPrefs.SetString("sceneBefore", "chat");
                //SceneManager.LoadScene(currentNode.nextScene);
            }
            else
            {
                currentNode = currentNode.nextNode;
                currentLineIndex = 0;
                DisplayCurrentLine();
            }
            return;
        }
        else if (currentLineIndex < currentNode.lines.Length /*|| currentNode.isChoiceNull()*/)
        {
            DialogLine line = currentNode.lines[currentLineIndex];

            // only set speaker name if it exists
            if (speakerNametext != null)
            {
                speakerNametext.text = line.speakerName;
            }

            StartCoroutine(AnimateAndType(line));
        }
        else
        {
            // choices removed
        }
    }

    IEnumerator AnimateAndType(DialogLine line)
    {
        if (line.animationType != Animation.None)
        {
            yield return new WaitForSeconds(line.durasi);
        }
        yield return StartCoroutine(TypeText(line.text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogText.text = "";
        int visibleCharCount = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (i % 3 == 0) PlayAudio();

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
    }

    void PlayAudio()
    {
        if (audioType.isPlaying)
        {
            audioType.Stop();
        }
        audioType.clip = effectAudioSource;
        audioType.Play();
    }

    public void clicking()
    {
        audioSource.clip = audioClick;
        audioSource.Play();

        if (isTyping)
        {
            StopAllCoroutines();
            DialogLine currentLine = currentNode.lines[currentLineIndex];
            dialogText.text = currentLine.text;
            dialogText.maxVisibleCharacters = currentLine.text.Length;
            isTyping = false;

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else
        {
            currentLineIndex++;
            DisplayCurrentLine();
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)   // New Input System
        {
            clicking();
        }
    }

    void EndDialog()
    {
        dialogText.text = "";

        // clear speaker name only if exists
        if (speakerNametext != null)
        {
            speakerNametext.text = "";
        }
        OnDialogFinished?.Invoke();
    }

    public void Restart()
    {
        // PlayerPrefs.SetFloat("anxStat", 80);   // removed anx stat
        PlayerPrefs.DeleteKey("x");
        PlayerPrefs.DeleteKey("y");
        PlayerPrefs.SetInt(chatWith, 0);
        //SceneManager.LoadScene("SC Chapter 1");
    }

    public void Menu()
    {
        //SceneManager.LoadScene("Start Screen");
    }
}
