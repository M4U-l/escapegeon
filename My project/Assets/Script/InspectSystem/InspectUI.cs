using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InspectUI : MonoBehaviour
{
    [Header("Main UI")]
    public GameObject inspectPanel;
    public Button descriptionButton;
    public Button cancelButton;

    [Header("Description Panel")]
    public GameObject descriptionPanel;
    public TMP_Text descriptionText;

    private InspectSystem inspectSystem;
    private InspectableObject currentObject;
    

    void Start()
    {
        inspectSystem = FindFirstObjectByType<InspectSystem>();

        inspectPanel.SetActive(false);
        descriptionPanel.SetActive(false);

        descriptionButton.onClick.AddListener(ShowDescription);
        cancelButton.onClick.AddListener(CancelInspect);
    }


    void Update()
    {
        if (descriptionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            descriptionPanel.SetActive(false);
            inspectPanel.SetActive(true);

            inspectSystem.allowRotation = true;
        }
    }

    public void OpenPanel(InspectableObject obj)
    {
        currentObject = obj;

        inspectPanel.SetActive(true);
        descriptionPanel.SetActive(false);

        // Llenar la descripción
        if (obj != null && obj.data != null)
        {
            descriptionText.text = obj.data.description;
        }
    }

    void ShowDescription()
    {
        descriptionPanel.SetActive(true);
        inspectSystem.allowRotation = false;
    }

    void CancelInspect()
    {
        if (currentObject != null)
        {
            if (currentObject.canBeCollected)
            {
                InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
                if (inventory != null)
                {
                    inventory.Store(currentObject);
                    Debug.Log("Object added to inventory");
                    // ClosePanel();
                    inspectSystem.StopInspect();
                }
            }
            else
            {
                inspectSystem.StopInspect();
            }
        }

        ClosePanel();
    }

    public void ClosePanel()
    {
        inspectPanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }
}
