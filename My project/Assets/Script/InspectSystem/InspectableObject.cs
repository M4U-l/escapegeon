using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    public InspectableData data;

    [Header("Interaction option")]
    public bool canBeCollected = false;
}
