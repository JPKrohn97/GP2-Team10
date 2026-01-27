using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            enemy.TakeDamage(35);
            ManagerCinemachine.Instance.CinemachineCameraShake();
        }
    }
}
