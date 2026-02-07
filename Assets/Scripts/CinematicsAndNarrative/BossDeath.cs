using UnityEngine;

public class BossDeath : MonoBehaviour
{
    public Collider[] allColliders;

    private void Awake()
    {
        allColliders = GetComponentsInChildren<Collider>();
    }

    public void DisableColliders()
    {
        foreach (Collider col in allColliders)
        {
            col.enabled = false;
        }
    }
}
