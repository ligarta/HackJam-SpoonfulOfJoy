using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("References")]
    public DialogManager dialogManager;    // Reference to your DialogManager in the scene
    public Image fadeOverlay;              // UI Image that covers the screen for fade effect

    [Header("Settings")]
    public string mainSceneName = "SampleScene"; // Name of the scene to load
    public float fadeDuration = 1.5f;          // How long the fade takes

    private void Start()
    {
        // Subscribe to dialog finish event
        if (dialogManager != null)
        {
            dialogManager.OnDialogFinished += HandleDialogFinished;
        }

        // Make sure overlay starts transparent
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (dialogManager != null)
        {
            dialogManager.OnDialogFinished -= HandleDialogFinished;
        }
    }

    // Called when dialog is finished
    private void HandleDialogFinished()
    {
        StartCoroutine(FadeAndLoad());
    }

    // Coroutine to fade to black and load main scene
    private IEnumerator FadeAndLoad()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            if (fadeOverlay != null)
            {
                Color c = fadeOverlay.color;
                c.a = alpha;
                fadeOverlay.color = c;
            }

            yield return null;
        }

        // Load the main scene after fade
        SceneManager.LoadScene(mainSceneName);
    }
}