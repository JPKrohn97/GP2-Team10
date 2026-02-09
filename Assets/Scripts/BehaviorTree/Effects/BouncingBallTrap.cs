using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BouncingBallTrap : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float bounceHeight = 3f;
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private float bounceSpeed = 8f;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Bounce Behavior")]
    [SerializeField] private float directionChangeChance = 0.5f; // 50% chance to change direction on bounce

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

    private bool movingForward = true;
    private float currentHeight = 0f;
    private bool movingUp = false;
    private float currentHorizontalPosition = 0f;

    private void Start()
    {
        movingForward = Random.value > 0.5f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, groundLayer))
        {
            transform.position = hit.point + Vector3.up * 0.5f;
        }

        movingUp = true;
        currentHeight = 0.5f;
        currentHorizontalPosition = transform.position.z;
        JumpTheBall();
    }

    private void Update()
    {
        //MoveBall();
        CheckForGroundAhead();
    }

    private void MoveBall()
    {
        float horizontalDirection = movingForward ? 1f : -1f;
        currentHorizontalPosition += horizontalDirection * horizontalSpeed * Time.deltaTime;

        if (movingUp)
        {
            currentHeight += bounceSpeed * Time.deltaTime;

            if (currentHeight >= bounceHeight)
            {
                currentHeight = bounceHeight;
                movingUp = false;
                PlayBounceSound();
            }
        }
        else
        {
            currentHeight -= bounceSpeed * Time.deltaTime;

            if (currentHeight <= 0.5f)
            {
                currentHeight = 0.5f;
                movingUp = true;
                SpawnBounceEffect();
                PlayBounceSound();

                // RANDOM DIRECTION CHANGE on each bounce
                if (Random.value < directionChangeChance)
                {
                    movingForward = !movingForward;

                    if (showDebugRays)
                    {
                        Debug.Log($"<color=cyan>Random direction change! Now moving {(movingForward ? "FORWARD" : "BACKWARD")}</color>");
                    }
                }
            }
        }

        Vector3 newPosition = transform.position;
        newPosition.y = GetGroundHeight() + currentHeight;
        newPosition.z = currentHorizontalPosition;
        transform.position = newPosition;
    }

    private float GetGroundHeight()
    {
        RaycastHit hit;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y + 5f, currentHorizontalPosition);

        if (Physics.Raycast(rayStart, Vector3.down, out hit, 100f, groundLayer))
        {
            return hit.point.y;
        }

        return 0f;
    }

    private void CheckForGroundAhead()
    {
        float checkDistance = horizontalSpeed * 0.3f + groundCheckDistance;
        Vector3 direction = movingForward ? Vector3.forward : Vector3.back;

        Vector3 checkPosition = transform.position + direction * checkDistance;
        checkPosition.y += 5f;

        RaycastHit hit;
        bool hasGroundAhead = Physics.Raycast(checkPosition, Vector3.down, out hit, bounceHeight + 10f, groundLayer);

        if (showDebugRays)
        {
            Debug.DrawRay(checkPosition, Vector3.down * (bounceHeight + 10f), hasGroundAhead ? Color.green : Color.red, 0.1f);
        }

        
        if (!hasGroundAhead)
        {
            movingForward = !movingForward;

            if (showDebugRays)
            {
                Debug.Log($"<color=red>Edge detected! Forced reverse to {(movingForward ? "FORWARD" : "BACKWARD")}</color>");
            }
        }
    }
    public void JumpTheBall()
    {
        transform.DOJump(transform.position + Vector3.forward * (movingForward ? 5f : -5f), bounceHeight, 1, bounceSpeed * 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SpawnBounceEffect();
            PlayBounceSound();

            if (Random.value < directionChangeChance)
            {
                movingForward = !movingForward;
                // Force reverse if no ground ahead (edge of platform)
            }
            CheckForGroundAhead();
            JumpTheBall();
            
        });

    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * bounceHeight, 0.3f);

        Gizmos.color = Color.green;
        float groundY = Application.isPlaying ? GetGroundHeight() : transform.position.y;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, groundY, transform.position.z), new Vector3(1f, 0.1f, 1f));

        Gizmos.color = Color.cyan;
        Vector3 direction = movingForward ? Vector3.forward : Vector3.back;
        Gizmos.DrawRay(transform.position, direction * 2f);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            float checkDist = horizontalSpeed * 0.3f + groundCheckDistance;
            Vector3 checkPos = transform.position + direction * checkDist;
            checkPos.y += 5f;
            Gizmos.DrawLine(checkPos, checkPos + Vector3.down * (bounceHeight + 10f));
        }
    }
}
