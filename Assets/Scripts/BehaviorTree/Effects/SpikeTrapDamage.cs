using UnityEngine;

public class SpikeTrapDamage : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private LayerMask playerLayer;
    Collider damageCollider;
    private void Awake()
    {
        damageCollider = GetComponent<Collider>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                damageCollider.enabled= false; // Disable the collider to prevent multiple damage applications

            }
        }
    }
}
