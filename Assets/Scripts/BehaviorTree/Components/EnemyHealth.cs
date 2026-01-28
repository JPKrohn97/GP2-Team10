using UnityEngine;
using BehaviorTree;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private EnemyAnimatorController animatorController;
    [SerializeField] private Collider interactionCollider;

    private BehaviorTreeBase behaviorTree;
    public int currentHealth;

    public bool IsDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        behaviorTree = GetComponent<BehaviorTreeBase>();

        if (animatorController == null)
            animatorController = GetComponentInChildren<EnemyAnimatorController>();

        if (interactionCollider != null)
            interactionCollider.enabled = false;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        IsDead = true;

        if (animatorController != null)
            animatorController.EnableRagdoll();

        if (behaviorTree != null)
            behaviorTree.enabled = false;

        if (interactionCollider != null)
            interactionCollider.enabled = true;

        Destroy(gameObject, 30f);
    }

    public void ConsumeBody()
    {
        Destroy(gameObject);
    }
}
