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
    [SerializeField] private SpriteRenderer characterSprite;

    // private bool inChoice = false;   // removed choice
    // bool isChosed = false;           // removed choice
    // [SerializeField] private float anxStat;   // removed anx stat
    // Choice nextNode;                 // removed choice

    // public TextMeshProUGUI timer;

    [Header("UI Elements")]
    [SerializeField] TMP_Text speakerNametext;   // now optional
    [SerializeField] TMP_Text dialogText;
    [SerializeField] GameObject dialogPanel;
    [SerializeField] GameObject choicePanel;   // removed choice
    [SerializeField] GameObject prefebChoices;   // removed choice
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
    bool isChoice = false;   // removed choice
    [SerializeField] private String NextScene;
    [SerializeField] private String chatWith;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogPanel.SetActive(false);;
    }

    public void StartDialog(DialogNode startNode)
    {
        dialogPanel.SetActive(true);
        LeanTween.scale(dialogPanel, new Vector3(1, 1, 1), 0.5f).setEaseOutExpo();
        currentNode = startNode;
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (currentNode.lines.Length == currentLineIndex && currentNode.isChoiceNull())
        {
            if (currentNode.nextNode == null)
            {
                EndDialog();
                PlayerPrefs.SetString("sceneBefore", "chat");
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
            setSprite(line);
            StartCoroutine(TypeText(line.text));

        }
        //    /// jika baris dialog habis dan memiliki pilihan
        //    else
        //    {
        //        if (!currentNode.isChoiceNull())
        //        {
        //            showChoices();
        //        }
        //        else if (currentNode.nextNode != null)
        //        {
        //            currentNode = currentNode.nextNode;
        //            currentLineIndex = 0;
        //            DisplayCurrentLine();
        //        }
        //        else
        //        {
        //            EndDialog();
        //        }
        //    }
        //}
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
        if (isChoice) return;   // removed choice
        audioSource.clip = audioClick;
        audioSource.Play();

        if (isTyping)
        {
            StopAllCoroutines();
            DialogLine currentLine = currentNode.lines[currentLineIndex];
            dialogText.text = currentLine.text;
            dialogText.maxVisibleCharacters = currentLine.text.Length;
            dialogText.rectTransform.sizeDelta = new Vector2(dialogText.rectTransform.sizeDelta.x, dialogText.preferredHeight);
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
    public void SetdialogText(TextMeshProUGUI text)
    {
        dialogText = text;
    }
    public void SetDialogPanel(GameObject panel)
    {
        dialogPanel = panel;
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
        LeanTween.scale(dialogPanel, new Vector3(0, 0, 0), 0.5f).setEaseInExpo().setOnComplete(() =>
        {
            dialogPanel.SetActive(false);
        });
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
        if (!isShake) return;
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
}
//    public void showChoices()
//    {
//        isChoice = true;   // removed choice
//        choicePanel.SetActive(true);
//        for (int i = 0; i < currentNode.choices.Length; i++)
//        {
//            GameObject choiceObj = Instantiate(prefebChoices, choicePanel.transform);
//            choiceObj.GetComponentInChildren<TMP_Text>().text = currentNode.choices[i].choiceText;
//            int choiceIndex = i;
//            choiceObj.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choiceIndex));
//        }
//        LeanTween.scale(choicePanel, new Vector3(1, 1, 1), 0.5f).setEaseOutExpo();
//    }

//    public void OnChoiceSelected(int index)
//    {
//        LeanTween.scale(choicePanel, new Vector3(0, 0, 0), 0.5f).setEaseInExpo().setOnComplete(() =>
//        {
//            choicePanel.SetActive(false);
//            foreach (Transform child in choicePanel.transform)
//            {
//                Destroy(child.gameObject);
//            }
//        });

//        Debug.Log("Choice selected: " + index);
//        currentNode = currentNode.choices[index].nextNode;
//        currentLineIndex = 0;
//        DisplayCurrentLine();
//        isChoice = false;   // removed choice
//    }
//}
