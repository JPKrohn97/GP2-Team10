using UnityEngine;
using System.Collections;
using DG.Tweening;
public class SpikeTrap : MonoBehaviour
{
    [SerializeField] Animator trapAnimator;
    [Header("Trap Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float warningHeight = 0.3f;
    [SerializeField] private float fullHeight = 2f;
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private float delayBeforeAttack = 0.3f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float retractSpeed = 5f;
    [SerializeField] private float extendSpeed = 8f;
    
    [Header("Cooldown")]
    [SerializeField] private float cooldownDuration = 3f;
    
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("References")]
    [SerializeField] private Transform spikesTransform;
    [SerializeField] private Collider damageCollider;
    
    private Vector3 originalPosition;
    private Vector3 warningPosition;
    private Vector3 fullExtensionPosition;
    private bool isActive = false;
    private bool canTrigger = true;
    private TrapState currentState = TrapState.Idle;
    
    private enum TrapState
    {
        Idle,
        Warning,
        Retracting,
        DelayingAttack,
        Attacking,
        Cooldown
    }

    private void Awake()
    {
        if (spikesTransform == null)
            spikesTransform = transform.GetChild(0);
        
        originalPosition = spikesTransform.localPosition;
        warningPosition = originalPosition + Vector3.up * warningHeight;
        fullExtensionPosition = originalPosition + Vector3.up * fullHeight;
        
        if (damageCollider != null)
            damageCollider.enabled = false;
    }

    private void Update()
    {
        if (!canTrigger || currentState != TrapState.Idle)
            return;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            TriggerTrap();
        }
    }

    private void TriggerTrap()
    {
        if (!canTrigger) return;
        trapAnimator.SetTrigger("Activate");
        canTrigger = false;
        //StartCoroutine(TrapSequence()); 
        //TriggerTrapDoTween();



    }
    private void EnableCol() 
    { 
        damageCollider.enabled = true;  
    }
    private void DisaableCol()
    {
        damageCollider.enabled = false;
    }
    private void TriggerTrapDoTween()
    {
        //spikesTransform.DOLocalMove(new Vector3(0, 0.3f, 0), 0.5f).
        //    OnComplete(() => { spikesTransform.DOLocalMove(new Vector3(0, 0f, 0), 0.3f).SetLoops(1, LoopType.Yoyo); }).
        //    OnComplete(() => { spikesTransform.DOLocalMove(new Vector3(0, 1.5f, 0), 0.6f); }).
        //    OnComplete(() => { spikesTransform.DOLocalMove(new Vector3(0, 0f, 0), 0.5f); });

        //spikesTransform.DOLocalMove(new Vector3(0, 1.5f, 0), 1.5f).SetEase(Ease.OutBack);

    }
    private IEnumerator TrapSequence()
    {
        // PHASE 1: Warning - extend partially (NO DAMAGE)
        currentState = TrapState.Warning;
        yield return StartCoroutine(MoveTo(warningPosition, extendSpeed));
        
        // Warning sound
        SoundManager.Instance.PlaySound(SoundManager.Instance.SpikeTrapWarning, gameObject);
        
        yield return new WaitForSeconds(warningDuration);
        
        // PHASE 2: Retract
        currentState = TrapState.Retracting;
        yield return StartCoroutine(MoveTo(originalPosition, retractSpeed));
        
        // PHASE 3: Short delay
        currentState = TrapState.DelayingAttack;
        yield return new WaitForSeconds(delayBeforeAttack);
        
        // PHASE 4: Full attack extension (DAMAGE ENABLED)
        currentState = TrapState.Attacking;
        if (damageCollider != null)
            damageCollider.enabled = true;
        
        // Attack sound
        SoundManager.Instance.PlaySound(SoundManager.Instance.SpikeTrapAttack, gameObject);
        
        yield return StartCoroutine(MoveTo(fullExtensionPosition, extendSpeed * 1.5f));
        yield return new WaitForSeconds(attackDuration);
        
        // Disable damage
        if (damageCollider != null)
            damageCollider.enabled = false;
        
        // PHASE 5: Retract to original position
        yield return StartCoroutine(MoveTo(originalPosition, retractSpeed));
        
        // PHASE 6: Cooldown
        currentState = TrapState.Cooldown;
        yield return new WaitForSeconds(cooldownDuration);
        
        // Reset
        currentState = TrapState.Idle;
        canTrigger = true;
    }

    private IEnumerator MoveTo(Vector3 targetLocalPosition, float speed)
    {
        while (Vector3.Distance(spikesTransform.localPosition, targetLocalPosition) > 0.01f)
        {
            spikesTransform.localPosition = Vector3.MoveTowards(
                spikesTransform.localPosition,
                targetLocalPosition,
                speed * Time.deltaTime
            );
            yield return null;
        }
        
        spikesTransform.localPosition = targetLocalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius at trap base
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Get spikes position (runtime or edit mode)
        Transform spikes = spikesTransform;
        if (spikes == null && transform.childCount > 0)
            spikes = transform.GetChild(0);
        
        if (spikes != null)
        {
            // Draw current spikes position (green)
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spikes.position, new Vector3(0.5f, 0.05f, 0.5f));
            
            // Draw warning height (blue) - from spikes position
            Gizmos.color = Color.blue;
            Vector3 warningWorldPos = spikes.position + Vector3.up * warningHeight;
            Gizmos.DrawWireCube(warningWorldPos, new Vector3(0.5f, 0.1f, 0.5f));
            
            // Draw full extension height (red) - from spikes position
            Gizmos.color = Color.red;
            Vector3 fullWorldPos = spikes.position + Vector3.up * fullHeight;
            Gizmos.DrawWireCube(fullWorldPos, new Vector3(0.5f, 0.1f, 0.5f));
        }
    }
}
