using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private int health = 100;
    public Collider[] AllColliders;
    public Collider MainCollider;
    public Animator enemyAnim;
    public Rigidbody[] AllRigidbodies;
    public Rigidbody mainRigidBody;
    public bool isDead;
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        MainCollider = GetComponent<Collider>();
        AllColliders = GetComponentsInChildren<Collider>(true);
        AllRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        mainRigidBody = GetComponent<Rigidbody>();
        DoRagdoll(false, Vector3.zero);

    }

    public void DoRagdoll(bool isRagdoll, Vector3 dir)
    {
        //foreach (var col in AllColliders)
        //    col.enabled = isRagdoll;
        foreach (var rb in AllRigidbodies)
        {
            rb.isKinematic = !isRagdoll;
        }
        MainCollider.enabled = !isRagdoll;
        //GetComponent<Rigidbody>().useGravity = !isRagdoll;
        enemyAnim.enabled = !isRagdoll;
        if (isRagdoll)
        {
            foreach (var rb in AllRigidbodies)
            {
                rb.AddForce(-transform.forward, ForceMode.Impulse);
            }
        }
        isDead = isRagdoll;
    }
    public void TakeDamage(int amount,Vector3 particlePos)
    {
        ManagerObjectPool.Instance.Spawn(ObjectPoolType.HitParticle1, particlePos);
        enemy.enemyAnimation.HitReaction(Random.Range(0, 2));
        health -= amount;
        if (health<=0)
        {
            ManagerObjectPool.Instance.Spawn(ObjectPoolType.HitParticle1, particlePos);

            DoRagdoll(true, -transform.forward);
        }
    }
}
