using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject RightPanel;
    [SerializeField] GameObject LeftPanel;
    [SerializeField] GameObject TransitionPanel;
    [SerializeField] AudioSource clickAudio;
    [SerializeField] AudioClip SwipeSound;
    [SerializeField] AudioClip OpenDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
    }
    void Update()
    {
        
    }
    public void gameStart()
    {
        clickAudio.PlayOneShot(SwipeSound);
        LeanTween.moveX(mainMenu, -1920f, 0.8f).setEaseInExpo().setOnComplete(() =>
        {
            mainMenu.SetActive(false);
        });
        LeanTween.alpha(Panel.GetComponent<RectTransform>(), 0f, 0.8f).setEaseInExpo();
        LeanTween.delayedCall(1f, () =>
        {
            clickAudio.PlayOneShot(OpenDoor);
        });
        LeanTween.moveX(RightPanel, 50f, 2f).setEaseInOutQuad().setDelay(1f);
        LeanTween.moveX(LeftPanel, -50f, 2f).setEaseInOutQuad().setDelay(1f);
        LeanTween.alpha(TransitionPanel, 1f, 2f).setEaseInOutExpo().setDelay(0.3f).setOnComplete(() =>
        {
            LeanTween.delayedCall(1.5f, () =>
            {
                SceneManager.LoadScene("IntroScene");
            });
        });
    }
}
