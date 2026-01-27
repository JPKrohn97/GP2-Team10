using UnityEngine;

public class StoneGravityWell : MonoBehaviour
{
    public float forceStrength = 12f;
    public float destroyDelay = 0.3f;

    void OnTriggerEnter(Collider other)   //  3D trigger
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>(); //  3D rigidbody
            if (rb != null)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce(dir * forceStrength, ForceMode.Impulse); //  3D force
            }

            Destroy(gameObject, destroyDelay);
        }
    }
}
