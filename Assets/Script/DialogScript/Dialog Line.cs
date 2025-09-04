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
    public bool isAnonymous = false;

    public bool isDoneEating;               // assign SO (sprite, animations, etc.)
    [Range(0, 5)] //0 gada, 1 place 1, 2 place 2, 3 place 3, 4 player, 5 narasi
    public int placeIndex;
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