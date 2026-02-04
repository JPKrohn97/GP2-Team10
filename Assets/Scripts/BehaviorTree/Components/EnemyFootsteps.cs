using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class EnemyFootsteps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyHealth enemyHealth;
    
    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.5f; // Time between steps
    [SerializeField] private float movementThreshold = 0.1f; // Minimum velocity to play footsteps
    [SerializeField] private bool isBoss = false; // Set true for boss enemies
    
    private float stepTimer = 0f;
    private bool isMoving = false;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        
        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        // Don't play footsteps if dead
        if (enemyHealth != null && enemyHealth.IsDead)
            return;

        // Check if enemy is moving based on NavMeshAgent velocity
        float currentSpeed = agent.velocity.magnitude;
        isMoving = currentSpeed > movementThreshold && agent.enabled && !agent.isStopped;

        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            // Reset timer when not moving
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (isBoss)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.BossSteps);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.EnemyFootSteps);
        }
    }
}
