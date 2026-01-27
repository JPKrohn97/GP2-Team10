using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 20; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.linearVelocity = transform.forward * speed; 

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy")) 
        {

            /*
            EnemyBehaviour enemyScript = other.GetComponent<EnemyBehaviour>();
            
            if (enemyScript != null)
            {
 
                enemyScript.TakeDamage(damage, transform.position);
            }
            */

   
            Destroy(gameObject); 
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            
            Destroy(gameObject);
        }
    }
}