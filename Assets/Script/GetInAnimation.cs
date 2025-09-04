using UnityEngine;

public class GetInAnimation : MonoBehaviour
{

    /// <summary>
    /// The SpriteRenderer component of the character to animate.
    /// </summary>
    [SerializeField] private SpriteRenderer characterSprite;
    private StartDialog dialog;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialog=gameObject.GetComponent<StartDialog>();
        animate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Animasi karakter dengan fade dan scale up.
    /// </summary>
    void animate()
    {

        Color fromColor = new Color(0.1f, 0.1f, 0.1f, 0f);
        Color toColor = Color.white;
        if (dialog.startNode.lines[0].isAnonymous)toColor=new Color(0.15f, 0.15f, 0.15f, 1f);
        LeanTween.value(gameObject, fromColor, toColor, 5f)
        .setEaseInOutExpo()
        .setOnUpdate((Color val) =>
        {
            characterSprite.color = val;
        });
        LeanTween.scale(transform.gameObject, new Vector3(0.6f, 0.6f, 0.6f), 5f).setEaseInOutExpo().setOnComplete(() =>
        {
            gameObject.GetComponent<StartDialog>().StartThis();
        });
    }
}
