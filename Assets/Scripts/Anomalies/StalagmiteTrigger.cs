using UnityEngine;

public class StalagmiteTrigger : MonoBehaviour
{
    public StoneStalagmite stalagmite;
    public int damage = 10;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        stalagmite.Activate();

        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
            dmg.TakeDamage(damage);
    }
}
