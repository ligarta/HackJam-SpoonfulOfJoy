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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void gameStart()
    {
        LeanTween.moveX(mainMenu, -1920f, 0.8f).setEaseInExpo().setOnComplete(() =>
        {
            mainMenu.SetActive(false);
        });
        LeanTween.alpha(Panel.GetComponent<RectTransform>(), 0f, 0.8f).setEaseInExpo();
        LeanTween.moveX(RightPanel, 200f, 2f).setEaseInQuad().setDelay(1f);
        LeanTween.moveX(LeftPanel, -200f, 2f).setEaseInQuad().setDelay(1f);
        LeanTween.alpha(TransitionPanel, 1f, 1.5f).setEaseInOutExpo().setDelay(0.8f).setOnComplete(() =>
        {
            LeanTween.delayedCall(1.5f, () =>
            {
                SceneManager.LoadScene("IntroScene");
            });
        });
    }
}
