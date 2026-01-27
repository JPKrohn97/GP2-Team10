using UnityEngine;

public class StoneGravityWell : MonoBehaviour
{
    public float forceStrength = 12f;
    public float destroyDelay = 0.3f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce(dir * forceStrength, ForceMode2D.Impulse);
            }

            Destroy(gameObject, destroyDelay);
        }
    }
}
