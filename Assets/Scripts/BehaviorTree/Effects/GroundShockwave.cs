using UnityEngine;

public class GroundShockwave : MonoBehaviour
{
    private int damage;
    private float maxRadius;
    private float speed;
    private float currentRadius = 0f;
    private bool hasHitPlayer = false;

    [Header("Ground Detection")]
    private float shockwaveHeight = 0.5f; // Height at which shockwave travels
    private float groundCheckHeight = 1.5f; // Max height player can be to get hit

    public void Initialize(int damage, float maxRadius, float speed)
    {
        this.damage = damage;
        this.maxRadius = maxRadius;
        this.speed = speed;
        
        // Position shockwave at ground level
        Vector3 pos = transform.position;
        pos.y = shockwaveHeight;
        transform.position = pos;
    }

    private void Update()
    {
        currentRadius += speed * Time.deltaTime;

        // Check collision with player
        CheckPlayerHit();

        // Destroy after reaching max radius
        if (currentRadius >= maxRadius)
        {
            Destroy(gameObject);
        }
    }

    private void CheckPlayerHit()
    {
        if (hasHitPlayer) return;

        // Check in ring (from currentRadius-speed*deltaTime to currentRadius)
        float innerRadius = Mathf.Max(0, currentRadius - speed * Time.deltaTime);
        
        Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Calculate distance in XZ plane (ground plane) only
                Vector3 playerPos = hit.transform.position;
                Vector3 shockwavePos = transform.position;
                
                // Distance on ground (ignore Y axis)
                float distanceXZ = Vector3.Distance(
                    new Vector3(playerPos.x, 0, playerPos.z),
                    new Vector3(shockwavePos.x, 0, shockwavePos.z)
                );
                
                // Check if player is in current wave ring on ground
                if (distanceXZ <= currentRadius && distanceXZ >= innerRadius)
                {
                    // Check if player is on the ground (can't jump over it if in air)
                    if (IsPlayerOnGround(hit.transform))
                    {
                        var playerHealth = hit.GetComponentInParent<IDamageable>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damage);
                            hasHitPlayer = true;
                            Debug.Log($"Shockwave hit player! Damage: {damage}");
                        }
                    }
                    else
                    {
                        Debug.Log("Player jumped over shockwave!");
                    }
                }
            }
        }
    }

    private bool IsPlayerOnGround(Transform playerTransform)
    {
        // Method 1: Raycast downward to check if player is grounded
        RaycastHit hit;
        Vector3 rayStart = playerTransform.position + Vector3.up * 0.1f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckHeight, LayerMask.GetMask("Ground", "Default")))
        {
            // Player is close to ground
            return true;
        }

        // Method 2: Check height difference (fallback if no ground layer)
        float heightDifference = Mathf.Abs(playerTransform.position.y - transform.position.y);
        return heightDifference < groundCheckHeight;
    }

    // Optional: visualization in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // Draw circle on ground plane (XZ)
        DrawCircleXZ(transform.position, currentRadius);
        
        // Draw inner ring
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        float innerRadius = Mathf.Max(0, currentRadius - speed * Time.deltaTime);
        DrawCircleXZ(transform.position, innerRadius);
    }

    private void DrawCircleXZ(Vector3 center, float radius)
    {
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}