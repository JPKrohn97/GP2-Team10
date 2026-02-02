using UnityEngine;

public class AOEProjectile : MonoBehaviour
{
    private int damage;
    private float maxRadius;
    private float speed;
    private Vector3 direction;
    private float currentRadius = 0.5f;
    private float maxDistance = 20f;
    private float traveledDistance = 0f;
    private bool hasHitPlayer = false;

    public void Initialize(int damage, float maxRadius, float speed, Vector3 direction)
    {
        this.damage = damage;
        this.maxRadius = maxRadius;
        this.speed = speed;
        this.direction = direction;
    }

    private void Update()
    {
        // Move towards player direction
        float moveStep = speed * Time.deltaTime;
        transform.position += direction * moveStep;
        traveledDistance += moveStep;

        // Gradually increase size (optional)
        currentRadius = Mathf.Lerp(0.5f, maxRadius, traveledDistance / (maxDistance * 0.5f));

        // Check collision with player
        CheckPlayerHit();

        // Destroy after reaching max distance
        if (traveledDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void CheckPlayerHit()
    {
        if (hasHitPlayer) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponentInParent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    hasHitPlayer = true;
                    Debug.Log($"AOE Projectile hit player! Damage: {damage}");
                }
            }
        }
    }

    // Optional: visualization in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentRadius);
    }
}