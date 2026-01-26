using UnityEngine;
using BehaviorTree;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private EnemyAnimatorController animatorController;

    private BehaviorTreeBase behaviorTree;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        behaviorTree = GetComponent<BehaviorTreeBase>();
        
        if (animatorController == null)
            animatorController = GetComponentInChildren<EnemyAnimatorController>();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(25);
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died");

        // Enable ragdoll (handles all physics components)
        if (animatorController)
        {
            animatorController.EnableRagdoll();
        }

        // Disable behavior tree
        if (behaviorTree) 
            behaviorTree.enabled = false;

        // Destroy after delay
        Destroy(gameObject, 5f);
    }
}