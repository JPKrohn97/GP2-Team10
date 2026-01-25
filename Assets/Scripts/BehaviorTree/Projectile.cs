using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f;
    public int damage = 10;
    public LayerMask targetLayer;

    [Header("Effects")]
    public GameObject hitEffectPrefab;

    private void Start()
    {
        // Destroy after lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if hit target layer
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // Apply damage if target has health component
            // Example: other.GetComponent<Health>()?.TakeDamage(damage);

            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy on any collision
        SpawnHitEffect();
        Destroy(gameObject);
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}