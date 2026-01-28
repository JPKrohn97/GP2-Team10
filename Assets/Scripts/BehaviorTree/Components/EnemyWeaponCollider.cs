using UnityEngine;
using DG.Tweening;
using System.Collections;
public class EnemyWeaponCollider : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;
   private Collider wepCollider;

    private void Start()
    {
        wepCollider=GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("Enemy weapon collider triggered with player");
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                wepCollider.enabled = false; // Disable collider after hit to prevent multiple hits
                damageable.TakeDamage(damage);
                Debug.Log($"Player hit for {damage} damage");
                
                // Spawn hit effect at collision point
                SpawnHitEffect(other);
            }
        }
    }
    
    private void SpawnHitEffect(Collider other)
    {
      
        
            Vector3 hitPosition = other.ClosestPoint(transform.position);
            GameObject spawnedPart= ManagerObjectPool.Instance.Spawn(ObjectPoolType.TestParticle, hitPosition, Quaternion.identity);
            DOVirtual.DelayedCall(0.6f, () =>
            {
                ManagerObjectPool.Instance.Despawn(ObjectPoolType.TestParticle, spawnedPart);
            });
        
    }
}