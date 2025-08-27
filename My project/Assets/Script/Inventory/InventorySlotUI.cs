using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    private InspectableData storedData;


    public void Setup(InspectableData data)
    {
        storedData = data;

        if (icon != null) icon.sprite = data.icon;
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(Onclick);
    }
    
    private void Onclick()
    {
        Debug.Log("Equipping: " + storedData.objectName);
        InventoryManager.Instance.EquipItem(storedData);
    }
}
