using UnityEngine;
using UnityEngine.UI; 

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void Setup(InspectableData data)
    {
        if (icon != null) icon.sprite = data.icon;
    }
}
