using UnityEngine;
using System.Collections;

public class SceneLoadTrigger : MonoBehaviour
{
    public virtual IEnumerator OnSceneLoaded()
    {
        // Put scene setup logic here
        // e.g., wait for camera position, spawn points, music, etc.

        yield return null;
    }
}