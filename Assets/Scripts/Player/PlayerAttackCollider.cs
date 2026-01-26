using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            enemy.enemyBehaviour.TakeDamage(35, new Vector3(1, transform.position.y, other.transform.position.z));

        }
    }
}
