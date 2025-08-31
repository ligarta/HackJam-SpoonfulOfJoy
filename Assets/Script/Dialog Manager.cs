using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DialogManager : MonoBehaviour
{

    public GameObject panel;
    private bool inChoice = false;
    bool isChosed = false;
    [SerializeField] private float anxStat;
    Choice nextNode;
    // public TextMeshProUGUI timer;
    [Header("UI Elements")]
    [SerializeField] TMP_Text speakerNametext;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] GameObject dialogPanel;
    //[SerializeField] Button choiceButtonPrefab;
    [SerializeField] public Button progresButton;


    [Header("Audio")]
    public AudioClip voiceAudioSource;
    public AudioClip effectAudioSource;
    [SerializeField] float textSpeed = 0.05f;
    [SerializeField] AudioClip audioClick;
    [SerializeField] AudioClip audioTimer;
    [SerializeField] AudioClip shuffleAudio;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioType;

    [Header("Setting")]

    //[SerializeField] string NextSceneDialog;
    DialogNode currentNode;
    int currentLineIndex = 0;
    bool isTyping = false;
    public GameObject shuffleBtn;
    bool isTimeOver = false;
    [SerializeField] private String NextScene;
    [SerializeField] private String chatWith;
    //[SerializeField] GameObject PopUpGameOver;



    /// <summary>
    /// Digunakan untuk inisialisasi dialog manager
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // timer.SetText("");
        dialogPanel.SetActive(false);
        //choicePanel.SetActive(false);

        ///Tambahkan listener pada tombol progress
        progresButton.onClick.AddListener(clicking);

    }

    /// <summary>
    /// Digunakan untuk memulai dialog pada game
    /// </summary>
    /// <param name="startNode"></param>
    public void StartDialog(DialogNode startNode)
    {
        dialogPanel.SetActive(true);
        currentNode = startNode;
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    /// <summary>
    /// Digunakan untuk menampilkan line dialog saat ini
    /// </summary>
    void DisplayCurrentLine()
    {
        ///Cek apakah dialog sudah selesai
        if (currentNode.lines.Length == currentLineIndex && currentNode.isChoiceNull())
        {
            ///Cek apakah ada node selanjutnya
            if (currentNode.nextNode == null)
            {
                PlayerPrefs.SetFloat("anxStat", anxStat);
                EndDialog();
                PlayerPrefs.SetString("sceneBefore", "chat");

                //SceneManager.LoadScene(currentNode.nextScene);
            }
            ///Jika ada node selanjutnya
            else
            {
                ///Pindah ke node selanjutnya
                currentNode = currentNode.nextNode;
                currentLineIndex = 0;
                DisplayCurrentLine();
            }
            return;
        }
        ///Jika masih ada line dialog yang tersisa
        else if (currentLineIndex < currentNode.lines.Length || currentNode.isChoiceNull())
        {
            ///Tampilkan line dialog
            DialogLine line = currentNode.lines[currentLineIndex];
            speakerNametext.text = line.speakerName;
            StartCoroutine(AnimateAndType(line));

        }
        ///Jika tidak ada line dialog yang tersisa, tampilkan pilihan
        else
        {

        }
    }

    // private Image GetTargetImage(DialogTarget targetImage)
    // {
    //     switch (targetImage)
    //     {
    //         case DialogTarget.RightImage:
    //             imageBefore = "right";
    //             return rightImage;

    //         case DialogTarget.LeftImage:
    //             imageBefore = "left";
    //             return leftImage;
    //         //case DialogTarget.CenterImage: return centerImage;
    //         default: return null;
    //     }
    // }

    /// <summary>
    /// Digunakan untuk menganimasikan dan mengetik teks dialog
    /// </summary>
    /// <param name="line"></param>
    /// <param name="targetImage"></param>
    /// <returns></returns>
    IEnumerator AnimateAndType(DialogLine line)
    {
        if (line.animationType != Animation.None)
        {

            yield return new WaitForSeconds(line.durasi);
        }
        yield return StartCoroutine(TypeText(line.text));
    }

    /// <summary>
    /// Digunakan untuk mengetik teks dialog satu per satu karakter
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Digunakan untuk menangani klik pada tombol progress
    /// </summary>
    public void clicking()
    {
        audioSource.clip = audioClick;
        audioSource.Play();
        ///Jika sedang mengetik
        if (isTyping)
        {
            ///Hentikan semua coroutine
            StopAllCoroutines();
            DialogLine currentLine = currentNode.lines[currentLineIndex];///Dapatkan line dialog saat ini
                                                                         ///Tampilkan seluruh teks dialog sekaligus
            dialogText.text = currentLine.text;
            dialogText.maxVisibleCharacters = currentLine.text.Length;
            isTyping = false;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        ///Jika tidak sedang mengetik
        else
        {
            ///Pindah ke line dialog berikutnya
            currentLineIndex++;
            DisplayCurrentLine();

        }
        ///Disable button jika sedang dalam pilihan
        if (inChoice)
        {
            progresButton.interactable = false;
        }
    }

    /// <summary>
    /// Digunakan untuk menyembunyikan kartu yang dipilih dan menampilkan node selanjutnya
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <param name="nextNodeIndex"></param>





    /// <summary>
    /// Digunakan untuk memperbarui status dialog manager setiap frame
    /// </summary>
    async void Update()
    {
        ///Jika kartu sudah dipilih
        if (isChosed)
        {
            ///Tunggu beberapa saat sebelum menampilkan node selanjutnya
            // timer.text = "";
            inChoice = false;
            isChosed = false;
            int a = 2000;
            if (isTimeOver) a = 0;
            await Task.Delay(a);
            await Task.Delay(500);
            panel.SetActive(false);
            shuffleBtn.SetActive(true);
            ///Mulai dialog pada node selanjutnya
            StartDialog(nextNode.nextNode);
            progresButton.interactable = true;
            isTimeOver = false;
        }
    }
    //END DIALOG
    void EndDialog()
    {
        dialogText.text = "";
        speakerNametext.text = "";
        panel.SetActive(false);

    }

    
    public void Restart()
    {
        PlayerPrefs.SetFloat("anxStat", 80);
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

