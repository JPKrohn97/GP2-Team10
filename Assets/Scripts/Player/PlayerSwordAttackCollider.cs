using UnityEngine;

public class PlayerSwordAttackCollider : MonoBehaviour
{
    private PlayerController player;


    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }
    

    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Enemy")) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null || enemy.IsDead) return; 
        
        enemy.TakeDamage(player.swordSkillDamage);

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