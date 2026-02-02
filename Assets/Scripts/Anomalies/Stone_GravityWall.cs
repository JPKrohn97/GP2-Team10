using UnityEngine;

public class StoneGravityWall : MonoBehaviour
{
    public float pushForce = 20f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        Vector3 pushDir = (other.transform.position - transform.position).normalized;
        rb.AddForce(pushDir * pushForce, ForceMode.Force);
    }
}
