using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private PlayerController player;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        col.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null || enemy.IsDead) return;
        
        col.enabled = false;
        
        enemy.TakeDamage(35);
        
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 impactNormal = (hitPoint - other.transform.position).normalized;
        Quaternion hitRotation = Quaternion.LookRotation(impactNormal);
        ManagerObjectPool.Instance.Spawn(ObjectPoolType.ClawHit, hitPoint, hitRotation);
        
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(SoundManager.Instance.ClawsImpact, gameObject);
    }
}