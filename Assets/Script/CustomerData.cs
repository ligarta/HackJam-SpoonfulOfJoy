using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "Scriptable Objects/CustomerData")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public Sprite idleSprite;
    public RuntimeAnimatorController animatorController;
}
