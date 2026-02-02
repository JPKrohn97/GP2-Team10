using UnityEngine;

namespace BehaviorTree
{
    public class TaskMeleeBossAttack : Node
    {
        private Transform transform;
        private Animator animator;
        
        private float lightCooldown;
        private int lightDamage;
        private float lightRange;
        
        private float heavyCooldown;
        private int heavyDamage;
        private float shockwaveRadius;
        private float shockwaveSpeed;
        private float heavyChance;
        private float attackTelegraphDelay; // Time before shockwave spawns after animation starts

        private float lightTimer = 0f;
        private float heavyTimer = 0f;
        private bool isPerformingHeavyAttack = false;
        private float heavyAttackTimer = 0f;

        public TaskMeleeBossAttack(Transform transform, Animator animator,
            float lightCooldown, int lightDamage, float lightRange,
            float heavyCooldown, int heavyDamage, float shockwaveRadius, 
            float shockwaveSpeed, float heavyChance, float attackTelegraphDelay = 0.5f)
        {
            this.transform = transform;
            this.animator = animator;
            this.lightCooldown = lightCooldown;
            this.lightDamage = lightDamage;
            this.lightRange = lightRange;
            this.heavyCooldown = heavyCooldown;
            this.heavyDamage = heavyDamage;
            this.shockwaveRadius = shockwaveRadius;
            this.shockwaveSpeed = shockwaveSpeed;
            this.heavyChance = heavyChance;
            this.attackTelegraphDelay = attackTelegraphDelay;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            EnemyFacing.FaceTarget(transform, target);

            lightTimer -= Time.deltaTime;
            heavyTimer -= Time.deltaTime;

            // Handle heavy attack telegraph timing
            if (isPerformingHeavyAttack)
            {
                heavyAttackTimer -= Time.deltaTime;
                if (heavyAttackTimer <= 0f)
                {
                    isPerformingHeavyAttack = false;
                    SpawnGroundShockwave();
                }
                return state = NodeState.Running;
            }

            // Attempt heavy ground pound attack
            if (heavyTimer <= 0 && Random.value < heavyChance)
            {
                heavyTimer = heavyCooldown;
                animator?.SetTrigger("GroundPound");
                
                // Start telegraph delay
                isPerformingHeavyAttack = true;
                heavyAttackTimer = attackTelegraphDelay;
                
                Debug.Log($"Boss starting Ground Pound! Shockwave in {attackTelegraphDelay}s");
                return state = NodeState.Running;
            }

            // Light melee attack
            if (lightTimer <= 0)
            {
                lightTimer = lightCooldown;
                animator?.SetTrigger("LightAttack");
                LightMeleeAttack();
            }

            return state = NodeState.Running;
        }

        private void LightMeleeAttack()
        {
            // Check if player is in range
            Collider[] hits = Physics.OverlapSphere(transform.position, lightRange);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    var playerHealth = hit.GetComponentInParent<IDamageable>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(lightDamage);
                        Debug.Log($"Boss Light Melee Attack! Damage: {lightDamage}");
                    }
                }
            }
        }

        private void SpawnGroundShockwave()
        {
            Debug.Log($"Boss Ground Pound Impact! Creating shockwave...");
            
            // Create shockwave object
            GameObject shockwave = new GameObject("Shockwave");
            shockwave.transform.position = transform.position;
            
            var shockwaveComponent = shockwave.AddComponent<GroundShockwave>();
            shockwaveComponent.Initialize(heavyDamage, shockwaveRadius, shockwaveSpeed);
        }
    }
}