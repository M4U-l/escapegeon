using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class InspectSystem : MonoBehaviour
{
    [Header("Config")]
    public float inspectRange = 3f; 
    public Transform inspectAnchor;
    public float rotationSpeed = 100f;
    public GameObject crosshair;
    public bool allowRotation = true;
    private Transform currentObject; 
    private Vector3 originalPosition; 
    private Quaternion originalRotation; 
    private bool isInspecting = false;
    
    private Vector3 previousMousePosition;

    private InspectableObject currentInspectable;
    private InspectUI inspectUI;
    

    void Start()
    {
        inspectUI = FindFirstObjectByType<InspectUI>();
    }

    void Update()
    {
        if (!isInspecting)
        {
            HandleRaycast();
        }
        else
        {
            HandleRotation();
        }
    }


    void HandleRaycast()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, inspectRange))
        {
            if (hit.collider.TryGetComponent(out InspectableObject obj))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartInspect(obj);
                }
            }
        }
    }

    void HandleRotation()
    {
        if (!allowRotation) return;
        
        if (Input.GetMouseButtonDown(0))
            previousMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - previousMousePosition;
            float rotX = delta.y * rotationSpeed * Time.deltaTime;
            float rotY = -delta.x * rotationSpeed * Time.deltaTime;

            currentObject.Rotate(Camera.main.transform.up, rotY, Space.World);
            currentObject.Rotate(Camera.main.transform.right, rotX, Space.World);

            previousMousePosition = Input.mousePosition;
        }
    }

    void StartInspect(InspectableObject obj)
    {
        isInspecting = true;
        currentInspectable = obj;
        currentObject = obj.transform;

        originalPosition = currentObject.position;
        originalRotation = currentObject.rotation;

        currentObject.position = inspectAnchor.position;
        currentObject.rotation = inspectAnchor.rotation;

        FindFirstObjectByType<PlayerMove>().enabled = false;

        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        inspectUI.OpenPanel(obj);
    }

    public void StopInspect()
    {
        isInspecting = false;

        currentObject.position = originalPosition;
        currentObject.rotation = originalRotation;

        currentObject = null;
        currentInspectable = null;

        FindFirstObjectByType<PlayerMove>().enabled = true;

        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inspectUI.ClosePanel();
    }
}
