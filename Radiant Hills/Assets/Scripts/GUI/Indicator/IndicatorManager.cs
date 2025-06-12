using System.Collections.Generic;
using UnityEngine;

// How to interface:
//
//  IndicatorManager.Instance.ShowIndicator([Name], [Transform]);
//  IndicatorManager.Instance.HideIndicator([Name], [Transform]);

/// <summary>
/// Manages all instances of IndicatorGUI elements
/// </summary>
public class IndicatorManager : MonoBehaviour
{
    /// <summary>
    /// Pairs a name with an Indicator Prefab to key in dictionary
    /// </summary>
    [System.Serializable]
    public class IndicatorEntry
    {
        public string Name;
        public GameObject IndicatorPrefab;
    }

    public static IndicatorManager Instance { get; private set; }

    [Header("Indicator Entries")]
    [SerializeField] private List<IndicatorEntry> indicators = new();

    // Dictionary of all possible indicators, Maps indicator name to prefab; 
    private Dictionary<string, GameObject> prefabDictionary = new();

    // Dictionary of active indicators; maps (target, indicator name) to instantiated indicator
    private Dictionary<(Transform, string), GameObject> activeIndicators = new();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Add each indicator in list to dictionary
        foreach (var entry in indicators)
        {
            if (!string.IsNullOrEmpty(entry.Name) && entry.IndicatorPrefab != null)
            {
                prefabDictionary[entry.Name] = entry.IndicatorPrefab;
            }
        }
    }

    /// <summary>
    /// Shows an indicator of the given name attached to the target
    /// </summary>
    public void ShowIndicator(string name, Transform target)
    {
        var key = (target, name);

        if (activeIndicators.ContainsKey(key))
            return;

        if (!prefabDictionary.TryGetValue(name, out var prefab))
        {
            Debug.LogWarning($"Indicator prefab '{name}' not found.");
            return;
        }

        var instance = Instantiate(prefab, target.position + Vector3.up, Quaternion.identity);
        instance.transform.SetParent(target, true); // Keep world position
        activeIndicators[key] = instance; // Add to active

        // Try to call IndicatorGUI.ShowIndicator()
        var indicatorGUI = instance.GetComponent<IndicatorGUI>();
        if (indicatorGUI != null) indicatorGUI.ShowIndicator();
    }

    /// <summary>
    /// Hides and destroys the indicator of the given name on the target
    /// </summary>
    public void HideIndicator(string name, Transform target)
    {
        var key = (target, name);

        if (activeIndicators.TryGetValue(key, out var instance))
        {
            // Try to get callback from IndicatorGUI.HideIndicator()
            // Object is marked for destruction after any disappearence actions
            var indicator = instance.GetComponent<IndicatorGUI>();
            if (indicator != null)
            {
                indicator.HideIndicator(() =>
                {
                    instance.SetActive(false);
                    Destroy(instance, 0.5f);
                    activeIndicators.Remove(key); // Remove from active
                });
            }
            else
            {
                // Fallback: Just deactivate, then destroy shortly after
                instance.SetActive(false);
                Destroy(instance, 0.5f);
                activeIndicators.Remove(key); // Remove from active
            }
        }
    }
}