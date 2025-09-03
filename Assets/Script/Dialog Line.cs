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
    
    [Header("Audio")]
    public AudioClip spokenText;
    public AudioClip moorOrEffect;

    [Header("Animation")]
    public float durasi = 1;
    public bool isShake = false;

}

// public enum DialogTarget
// {
//     LeftImage,
//     RightImage,
//     //CenterImage
// }

public enum Animation
{
    None, fade, shake
}