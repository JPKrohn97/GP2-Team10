using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    
    private int rangeLevel;

    public int damage = 20; 
    public float lifeTime = 5f;

    public void Init(int level)
    {
        rangeLevel = level; 
        damage = 20 + (rangeLevel - 1) * 10; 
    }

    private void OnEnable()
    {
        StartCoroutine(DeactivateRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
      
            IDamageable target = other.GetComponentInParent<IDamageable>();
            
            if (target != null)
            {
                
                if (ManagerObjectPool.Instance != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    ManagerObjectPool.Instance.Spawn(ObjectPoolType.PlayerProjectileExplosion, hitPoint, Quaternion.identity);
                }
                
                target.TakeDamage(damage);
                ReturnToPool(); 
                return;
            }
        }

        if (!other.CompareTag("Player") && !other.isTrigger)
        {
            ReturnToPool();
        }
    }


    private void ReturnToPool()
    {
        StopAllCoroutines(); 
        ManagerObjectPool.Instance.Despawn(ObjectPoolType.PlayerProjectile, gameObject);
    }

    IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }
}