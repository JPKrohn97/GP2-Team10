using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    public int damage = 20; 
    public float lifeTime = 5f;

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
        ManagerObjectPool.Instance.Despawn(ObjectPoolType.Projectile, gameObject);
    }

    IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }
}