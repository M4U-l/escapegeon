using UnityEngine;

[CreateAssetMenu(fileName = "InspectableData", menuName = "Inspect/Inspectable Data", order = 1)]
public class InspectableData : ScriptableObject
{
    public string objectName;
    [TextArea(3, 6)] public string description;
    public Sprite icon; // preview on the inventory
}