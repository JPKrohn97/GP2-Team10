using UnityEngine;

public class StoneGravityWell : MonoBehaviour
{
    public float pushForce = 12f;
    public float destroyDelay = 0.3f;
    public float damage = 5f;   // Optional biome damage

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            rb.AddForce(dir * pushForce, ForceMode.Impulse);
        }

        // Optional damage via interface (safe for biome-only task)
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
            dmg.TakeDamage((int)damage);

        Destroy(gameObject, destroyDelay);
    }
}
