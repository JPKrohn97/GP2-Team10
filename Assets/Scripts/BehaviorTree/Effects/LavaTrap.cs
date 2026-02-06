using UnityEngine;
using System.Collections;

public class LavaTrap : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointReachDistance = 0.2f;
    
    [Header("Damage Settings")]
    [SerializeField] private int damagePerTick = 10;
    [SerializeField] private float damageInterval = 0.5f; // Damage every 0.5 seconds
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem lavaParticles;
    [SerializeField] private Renderer lavaRenderer;
    
    [Header("Audio")]
    [SerializeField] private bool playSizzleSound = true;
    
    private int currentWaypointIndex = 0;
    private bool playerOnLava = false;
    private Coroutine damageCoroutine;
    private GameObject currentPlayer;

    private void Start()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("LavaTrap: No patrol points assigned! Trap will be stationary.");
        }
        else
        {
            // Start at first patrol point
            transform.position = patrolPoints[0].position;
        }
        
        // Make sure particles are playing
        if (lavaParticles != null)
        {
            lavaParticles.Play();
        }
    }

    private void Update()
    {
        // Always move if patrol points are set
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        if (patrolPoints.Length == 0) return;
        
        Transform targetWaypoint = patrolPoints[currentWaypointIndex];
        
        // Move towards current waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime
        );
        
        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointReachDistance)
        {
            // Move to next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            playerOnLava = true;
            currentPlayer = other.gameObject;
            
            // Start damage over time
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageOverTime());
            }
            
            // Play sizzle sound
            if (playSizzleSound && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.LavaSizzle, gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Keep track that player is still on lava
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            playerOnLava = true;
            currentPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            playerOnLava = false;
            currentPlayer = null;
            
            // Stop damage coroutine
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (playerOnLava)
        {
            // Deal damage
            if (currentPlayer != null)
            {
                IDamageable damageable = currentPlayer.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damagePerTick);
                }
            }
            
            // Wait for next damage tick
            yield return new WaitForSeconds(damageInterval);
        }
        
        damageCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw patrol path if assigned
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] == null) continue;
                
                // Draw waypoint sphere
                Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);
                
                // Draw line to next waypoint
                int nextIndex = (i + 1) % patrolPoints.Length;
                if (patrolPoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                }
            }
        }
        
        // Draw danger area
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
