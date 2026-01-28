using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f;
    public int damage = 10;
    public LayerMask targetLayer;

    [Header("Knockback")]
    public float knockbackForce = 1f;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;

    private bool hasHit;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (((1 << other.gameObject.layer) & targetLayer) == 0)
            return;

        hasHit = true;
        
        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target != null)
            target.TakeDamage(damage);
        
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            player.ApplyKnockback(transform.position, knockbackForce);
        
        SpawnHitEffect();
        
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        SpawnHitEffect();
        Destroy(gameObject);
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    }
}