using UnityEngine;



[CreateAssetMenu(fileName = "NewDialogLine", menuName = "Dialog/Line")]
/// Digunakan untuk menyimpan baris dialog
public class DialogLine : ScriptableObject 
{
    /// <summary>
    /// Nama pembicara
    /// </summary>
    public string speakerName;
    /// <summary>
    /// Teks dialog
    /// </summary>
    [TextArea(3, 5)] public string text;
    public Sprite charSprite;

    // public DialogTarget targetImage;

    [Header("Audio")]
    public AudioClip spokenText;
    public AudioClip moorOrEffect;

    [Header("Animation")]
    public float durasi = 1;
    public Animation animationType;

}

// public enum DialogTarget
// {
//     LeftImage,
//     RightImage,
//     //CenterImage
// }

public enum Animation
{
    None,
    EnteringScane,
    ExitingScane,
    Jumping,
    Shaking,
    Scaling,
    Rotating
}