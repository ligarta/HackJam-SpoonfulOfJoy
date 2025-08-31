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
    void Start()
    {
        Invoke("StartThis", 1f);
    }
    void StartThis()
    {
        manager.StartDialog(startNode);
        
    }

}
