using UnityEngine;

public class GetInAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer characterSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Color fromColor = new Color(0.36f, 0.36f, 0.36f, 0f); // Abu-abu
        Color toColor = Color.white; // Putih
        LeanTween.value(gameObject, fromColor, toColor, 5f)
        .setEaseInOutExpo()
        .setOnUpdate((Color val) =>
        {
            characterSprite.color = val;
        });
        LeanTween.scale(transform.gameObject, new Vector3(1, 1, 1), 5f).setEaseInOutExpo().setOnComplete(() =>
        {
            gameObject.GetComponent<StartDialog>().StartThis();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
