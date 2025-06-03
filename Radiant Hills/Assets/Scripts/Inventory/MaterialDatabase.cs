using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database of MaterialType
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Material Database")]
public class MaterialDatabase : ScriptableObject
{
    [Tooltip("List of all MaterialTypes")]
    public List<MaterialType> allMaterials = new();

    /// <summary>
    /// Lookup Material by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MaterialType FindMaterialByName(string name)
    {
        if (allMaterials == null || allMaterials.Count == 0)
        {
            Debug.LogError("MaterialDatabase: allMaterials list is empty or not assigned!");
            return null;
        }

        MaterialType found = allMaterials.Find(mat => mat.materialName == name);

        if (found == null)
        {
            Debug.LogError($"MaterialDatabase: Material with name '{name}' not found in allMaterials.");
        }

        return found;
    }
}