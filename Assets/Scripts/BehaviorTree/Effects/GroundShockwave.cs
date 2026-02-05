using UnityEngine;

public class GroundShockwave : MonoBehaviour
{
    private int damage;
    private float maxRadius;
    private float speed;
    private float currentRadius = 0f;
    private bool hasHitPlayer = false;
    private bool initialized = false;

    [Header("Ground Detection")]
    private float shockwaveHeight = 0.5f;
    private float groundCheckHeight = 1.5f;

    public void Initialize(int damage, float maxRadius, float speed)
    {
        this.damage = damage;
        this.maxRadius = maxRadius;
        this.speed = speed;
        initialized = true;
        transform.eulerAngles = new Vector3(-90f, 0f, 0f);
        //Vector3 pos = transform.position;
        //pos.y = shockwaveHeight;
        //transform.position = pos;
    }

    private void Update()
    {
        if (!initialized) return;
        
        currentRadius += speed * Time.deltaTime;

        CheckPlayerHit();

        if (currentRadius >= maxRadius)
        {
            Destroy(gameObject);
        }
    }

    private void CheckPlayerHit()
    {
        if (hasHitPlayer) return;

        float innerRadius = Mathf.Max(0, currentRadius - speed * Time.deltaTime);
        
        Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector3 playerPos = hit.transform.position;
                Vector3 shockwavePos = transform.position;
                
                float distanceXZ = Vector3.Distance(
                    new Vector3(playerPos.x, 0, playerPos.z),
                    new Vector3(shockwavePos.x, 0, shockwavePos.z)
                );
                
                if (distanceXZ <= currentRadius && distanceXZ >= innerRadius)
                {
                    if (IsPlayerOnGround(hit.transform))
                    {
                        var playerHealth = hit.GetComponentInParent<IDamageable>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damage);
                            hasHitPlayer = true;
                        }
                    }
                }
            }
        }
    }

    private bool IsPlayerOnGround(Transform playerTransform)
    {
        RaycastHit hit;
        Vector3 rayStart = playerTransform.position + Vector3.up * 0.1f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckHeight, LayerMask.GetMask("Ground", "Default")))
        {
            return true;
        }

        float heightDifference = Mathf.Abs(playerTransform.position.y - transform.position.y);
        return heightDifference < groundCheckHeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DrawCircleXZ(transform.position, currentRadius);
        
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