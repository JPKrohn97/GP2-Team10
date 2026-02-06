using UnityEngine;
using System.Collections;
using DG.Tweening;
public class OrbTrap : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waypointReachDistance = 0.2f;
    
    [Header("Damage")]
    [SerializeField] private int damage = 25;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Visibility Cycle")]
    [SerializeField] private float visibleDuration = 5f;
    [SerializeField] private float invisibleDuration = 3f;
    [SerializeField] private float warningDuration = 1f; // Blinking before appearing
    [SerializeField] private float warningBlinkSpeed = 0.15f;
    
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float respawnDelay = 2f;
       
    private int currentWaypointIndex = 0;
    private bool isVisible = true;
    private bool isWarning = false;
    private OrbState currentState = OrbState.Moving;
    private Tween scaleTween;
    private Tween delayTween;
    private enum OrbState
    {
        Moving,
        Invisible,
        Warning,
        Exploding
    }

    private void Start()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("OrbTrap: No patrol points assigned!");
            enabled = false;
            return;
        }
        
        // Start at first patrol point
        transform.position = patrolPoints[0].position;

        Warning();
    }

    private void Update()
    {
        if (currentState == OrbState.Moving && isVisible && !isWarning)
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

    private void Warning()
    {
        isWarning = true;
        scaleTween = transform.DOScale(0.1f, 0.3f).SetEase(Ease.InOutSine).SetLoops(3, LoopType.Yoyo).
            OnComplete(()=> 
            {
                transform.localScale = Vector3.one*0.17f;
                isWarning = false;
                delayTween = DOVirtual.DelayedCall(Random.Range(3,5f), () => { Warning();});
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only explode when fully visible
        if (!isVisible || isWarning || currentState != OrbState.Moving)
            return;
        
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Explode(other);
        }
    }
    private void Explode(Collider playerCollider)
    {
        currentState = OrbState.Exploding;
        
        IDamageable damageable = playerCollider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }   
            GameObject explosion = ManagerObjectPool.Instance.Spawn(
                ObjectPoolType.OrbExplosion,
                transform.position,
                Quaternion.identity
            );
        delayTween?.Kill();
        scaleTween?.Kill();

        
        gameObject.SetActive(false);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.OrbExplosion, gameObject);
        }

    }


    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;
        
        // Draw patrol path
        Gizmos.color = Color.cyan;
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
        
        // Draw explosion radius at current position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
