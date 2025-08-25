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
        inspectSystem.StopInspect(); // volvemos al sistema
    }

    public void ClosePanel()
    {
        inspectPanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }
}
