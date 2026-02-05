using UnityEngine;
using DG.Tweening;

public class EnemyWeaponCollider : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;
    private Collider wepCollider;

    private void Start()
    {
        wepCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                wepCollider.enabled = false;
                damageable.TakeDamage(damage);
                SpawnHitEffect(other);
            }
        }
    }
    
    private void SpawnHitEffect(Collider other)
    {
        Vector3 hitPosition = other.ClosestPoint(transform.position);
        GameObject spawnedPart = ManagerObjectPool.Instance.Spawn(ObjectPoolType.EnemyHit, hitPosition, Quaternion.identity);
        DOVirtual.DelayedCall(0.6f, () =>
        {
            ManagerObjectPool.Instance.Despawn(ObjectPoolType.EnemyHit, spawnedPart);
        });
    }
}