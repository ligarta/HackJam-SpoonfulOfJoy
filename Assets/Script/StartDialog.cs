using UnityEngine;

public class StartDialog : MonoBehaviour
{
    /// <summary>
    /// Digunakan untuk memulai dialog pada game
    /// </summary>
    [SerializeField] DialogManager manager;
    /// <summary>
    /// Node awal dari dialog
    /// </summary>
    [SerializeField] DialogNode startNode;

    public void StartThis()
    {
        manager.StartDialog(startNode);
        
    }

}
