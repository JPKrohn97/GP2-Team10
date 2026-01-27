using UnityEngine;

public class EnemyWeaponCollider : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log($"Player hit for {damage} damage");
            }
        }
    }
}