using UnityEngine;

public class PlayerSwordAttackCollider : MonoBehaviour
{
    private PlayerController player;
    private bool hasHitThisSwing; 
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
    
        hasHitThisSwing = false;
        col.enabled = true; 
    }

    private void OnTriggerEnter(Collider other)
    {
    
        if (hasHitThisSwing) return;

        if (!other.CompareTag("Enemy")) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null || enemy.IsDead) return; 


        col.enabled = false; 
        hasHitThisSwing = true;


        enemy.TakeDamage(100);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.SwordImpactImpact, gameObject);
        }

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 impactNormal = (hitPoint - other.transform.position).normalized;
        Quaternion hitRotation = Quaternion.LookRotation(impactNormal);
        ManagerObjectPool.Instance.Spawn(ObjectPoolType.SwordHit, hitPoint, hitRotation);
        
    }
}