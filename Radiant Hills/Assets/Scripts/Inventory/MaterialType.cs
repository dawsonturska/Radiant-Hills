using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Inventory/Material")]
public class MaterialType : ScriptableObject
{
    public string materialName; // Name of the material
    public Sprite icon; // Icon for the material (set this in the inspector)
    public float price;
}
