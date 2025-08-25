using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;

    private List<InspectableData> inventory = new List<InspectableData>();
    private bool isOpen = false;


    void Awake() => Instance = this;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Key Pressed...");
            ToggleInventory();
        }
    }

    public void Store(InspectableObject obj)
    {
        if (!obj.canBeCollected)
        {
            Debug.Log($"{obj.data.name} no puede ser guardado en el inventario.");
            return;
        }

        inventory.Add(obj.data);

        GameObject slotGO = Instantiate(slotPrefab, inventoryPanel.transform);
        slotGO.GetComponent<InventorySlotUI>().Setup(obj.data);

        obj.gameObject.SetActive(false);
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }

    public List<InspectableData> GetInventory() => inventory;

}
