using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterialType", menuName = "Inventory/MaterialType")]
public class MaterialType : ScriptableObject
{
    public string materialName; // Name of the material
    public Sprite icon;        // Icon for the material
}
