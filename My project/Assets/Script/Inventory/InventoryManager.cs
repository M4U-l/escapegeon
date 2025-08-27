using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;

    [Header("Equip Settings")]
    [SerializeField] private Transform handTransform; // Punto donde se equipan los objetos
    private GameObject equippedObject;

    private List<InspectableData> inventory = new List<InspectableData>();
    private bool isOpen = false;


    void Awake() => Instance = this;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
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

        if (isOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void EquipItem(InspectableData data)
    {
        if (equippedObject != null)
        {
            equippedObject.SetActive(false);
        }

        if (data.prefab != null)
        {
            equippedObject = Instantiate(data.prefab, handTransform);
            equippedObject.transform.localPosition = Vector3.zero;
            equippedObject.transform.localRotation = Quaternion.identity;
        }
    }

    public List<InspectableData> GetInventory() => inventory;

}
