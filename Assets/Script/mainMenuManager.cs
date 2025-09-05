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
    [SerializeField] GameObject CreditPanel;

    public void gameStart()
    {
        clickAudio.PlayOneShot(SwipeSound);

        // Gunakan GetComponent<RectTransform>() langsung
        LeanTween.moveX(mainMenu.GetComponent<RectTransform>(),
            mainMenu.GetComponent<RectTransform>().anchoredPosition.x - Screen.width, 0.8f)
            .setEaseInExpo()
            .setOnComplete(() =>
            {
                mainMenu.SetActive(false);
            });

        // Panel fade out
        LeanTween.alpha(Panel.GetComponent<RectTransform>(), 0f, 0.8f).setEaseInExpo();

        // Door sound effect
        LeanTween.delayedCall(1f, () =>
        {
            clickAudio.PlayOneShot(OpenDoor);
        });

        // Panel movements using current positions
        LeanTween.moveX(RightPanel, 50f, 2f).setEaseInOutQuad().setDelay(1f);
        LeanTween.moveX(LeftPanel, -50f, 2f).setEaseInOutQuad().setDelay(1f);

        // Transition fade
        LeanTween.alpha(TransitionPanel, 1f, 2f).setEaseInOutExpo().setDelay(0.3f).setOnComplete(() =>
         {
             LeanTween.delayedCall(1.5f, () =>
             {
                 SceneManager.LoadScene("IntroScene");
             });
         });
    }
    float temp = 0f;

    public void toCredit()
    {
        clickAudio.PlayOneShot(SwipeSound);

        CreditPanel.SetActive(true);
        temp = mainMenu.GetComponent<RectTransform>().anchoredPosition.x;
        LeanTween.moveX(mainMenu.GetComponent<RectTransform>(),
            mainMenu.GetComponent<RectTransform>().anchoredPosition.x - Screen.width, 0.8f)
            .setEaseInOutExpo()
            .setOnComplete(() =>
            {
                mainMenu.SetActive(false);
            });

        LeanTween.moveX(CreditPanel.GetComponent<RectTransform>(),
            0, 0.8f)
            .setEaseInOutExpo();
    }

    public void closeCredit()
    {
        mainMenu.SetActive(true);
        clickAudio.PlayOneShot(SwipeSound);
        LeanTween.moveX(mainMenu.GetComponent<RectTransform>(), temp, 0.8f)
            .setEaseInOutExpo();

        LeanTween.moveX(CreditPanel.GetComponent<RectTransform>(),
            Screen.width, 0.8f)
            .setEaseInOutExpo().setOnComplete(() =>
            {
                CreditPanel.SetActive(false);
            });
    }

    public void quitGame()
    {
        Application.Quit();
    }
}