using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogNode", menuName = "Dialog/Node")]
public class DialogNode : ScriptableObject
{
    /// <summary>
    /// Baris dialog yang ada pada node ini
    /// </summary>
    public DialogLine[] lines;
    /// <summary>
    /// Pilihan yang ada pada node ini
    /// </summary>
    public Choice[] choices;
    /// <summary>
    /// Scene yang akan dituju jika dialog ini selesai
    /// </summary>
    public string nextScene;
    /// <summary>
    /// Node yang akan dituju jika dialog ini selesai
    /// </summary>
    public DialogNode nextNode;



/// <summary>
/// Digunakan untuk mendapatkan pilihan berdasarkan index yang diberikan   
/// </summary>
/// <param name="i"></param>
/// <returns></returns>
    public Choice nextNodeIndex(int i)
    {
        if (i >= choices.Length)
        {
            return choices[0];
        }
        return choices[i];
    }
    
    
    /// <summary>
    /// Digunakan untuk mengecek apakah dialog node ini memiliki pilihan atau tidak
    /// </summary>
    /// <returns> true jika dialog node ini memiliki pilihan</returns>
    public bool isChoiceNull()
    {
        return choices.Length == 0;
    }

}

[System.Serializable]
/// Digunakan untuk menyimpan pilihan yang ada pada dialog node
public class Choice
{
    /// <summary>
    /// Teks yang akan ditampilkan pada pilihan
    /// </summary>
    public string choiceText;
    /// <summary>
    /// Node yang akan dituju jika pilihan ini dipilih
    /// </summary>
    public DialogNode nextNode;

}
