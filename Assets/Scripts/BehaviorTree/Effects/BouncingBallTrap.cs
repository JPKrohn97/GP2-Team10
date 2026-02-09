using UnityEngine;
using System.Collections;

public class BouncingBallTrap : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float bounceHeight = 3f; // Maximum height of bounce
    [SerializeField] private float horizontalSpeed = 5f; // Speed left/right
    [SerializeField] private float bounceSpeed = 8f; // Speed of bounce up/down
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Damage Settings")]
    [SerializeField] private int damage = 15;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Visual Effects")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private ParticleSystem bounceParticles;
    
    [Header("Audio")]
    [SerializeField] private bool playBounceSound = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;
    
    private Vector3 velocity;
    private bool movingRight = true;
    private float currentHeight = 0f;
    private bool movingUp = false;
    private Vector3 lastGroundPosition;

    private void Start()
    {
        // Random initial direction
        movingRight = Random.value > 0.5f;
        
        // Find ground beneath
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, groundLayer))
        {
            lastGroundPosition = hit.point;
            transform.position = lastGroundPosition + Vector3.up * 0.5f;
        }
        
        // Start moving up
        movingUp = true;
        currentHeight = 0f;
    }

    private void Update()
    {
        MoveBall();
        CheckForGroundAhead();
    }

    private void MoveBall()
    {
        // Horizontal movement (left/right)
        float horizontalDirection = movingRight ? 1f : -1f;
        Vector3 horizontalMovement = Vector3.right * horizontalDirection * horizontalSpeed * Time.deltaTime;
        
        // Vertical movement (bounce pattern - no gravity loss)
        if (movingUp)
        {
            // Moving up
            currentHeight += bounceSpeed * Time.deltaTime;
            
            if (currentHeight >= bounceHeight)
            {
                currentHeight = bounceHeight;
                movingUp = false;
                
                // Play bounce sound at peak
                PlayBounceSound();
            }
        }
        else
        {
            // Moving down
            currentHeight -= bounceSpeed * Time.deltaTime;
            
            if (currentHeight <= 0f)
            {
                currentHeight = 0f;
                movingUp = true;
                
                // Spawn bounce particles
                SpawnBounceEffect();
                
                // Play bounce sound
                PlayBounceSound();
                
                // Update last ground position
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
                {
                    lastGroundPosition = hit.point;
                }
            }
        }
        
        // Apply movement
        Vector3 targetPosition = lastGroundPosition + Vector3.up * currentHeight + horizontalMovement;
        transform.position = targetPosition;
    }

    private void CheckForGroundAhead()
    {
        // Check if there's ground in the direction we're moving
        float checkDistance = horizontalSpeed * Time.deltaTime + groundCheckDistance;
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        Vector3 rayStart = transform.position;
        
        // Raycast downward from ahead position
        Vector3 checkPosition = rayStart + direction * checkDistance;
        RaycastHit hit;
        
        bool hasGroundAhead = Physics.Raycast(checkPosition, Vector3.down, out hit, bounceHeight + 2f, groundLayer);
        
        if (showDebugRays)
        {
            Debug.DrawRay(checkPosition, Vector3.down * (bounceHeight + 2f), hasGroundAhead ? Color.green : Color.red);
        }
        
        // If no ground ahead, reverse direction
        if (!hasGroundAhead)
        {
            movingRight = !movingRight;
            
            if (showDebugRays)
            {
                Debug.Log($"<color=yellow>BouncingBall reversed direction! Now moving {(movingRight ? "RIGHT" : "LEFT")}</color>");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            // Deal damage to player
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            // Play impact sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.BallImpact, gameObject);
            }
        }
    }

    private void SpawnBounceEffect()
    {
        if (bounceParticles != null)
        {
            bounceParticles.Play();
        }
    }

    private void PlayBounceSound()
    {
        if (playBounceSound && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.BallBounce, gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw bounce height
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * bounceHeight, 0.3f);
        
        // Draw ground level
        Gizmos.color = Color.green;
        if (Application.isPlaying && lastGroundPosition != Vector3.zero)
        {
            Gizmos.DrawWireCube(lastGroundPosition, new Vector3(2f, 0.1f, 2f));
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(2f, 0.1f, 2f));
        }
        
        // Draw direction arrow
        Gizmos.color = Color.cyan;
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, direction * 2f);
        
        // Draw check distance
        Gizmos.color = Color.red;
        float checkDist = horizontalSpeed * 0.1f + groundCheckDistance;
        Vector3 checkPos = transform.position + direction * checkDist;
        Gizmos.DrawLine(checkPos, checkPos + Vector3.down * (bounceHeight + 2f));
    }
}
