using UnityEngine;

namespace BehaviorTree
{
    public class TaskBossAttack : Node
    {
        private Transform transform;
        private Animator animator;
        
        private float lightCooldown;
        private int lightDamage;
        private float heavyCooldown;
        private int heavyDamage;
        private float heavyChance;

        private float lightTimer = 0f;
        private float heavyTimer = 0f;

        public TaskBossAttack(Transform transform, Animator animator,
            float lightCooldown, int lightDamage,
            float heavyCooldown, int heavyDamage, float heavyChance)
        {
            this.transform = transform;
            this.animator = animator;
            this.lightCooldown = lightCooldown;
            this.lightDamage = lightDamage;
            this.heavyCooldown = heavyCooldown;
            this.heavyDamage = heavyDamage;
            this.heavyChance = heavyChance;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Rotate towards the player
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            lightTimer -= Time.deltaTime;
            heavyTimer -= Time.deltaTime;

            // Attempt heavy attack
            if (heavyTimer <= 0 && Random.value < heavyChance)
            {
                heavyTimer = heavyCooldown;
                animator?.SetTrigger("HeavyAttack");
                Debug.Log($"Boss Heavy Attack! Damage: {heavyDamage}");
                return state = NodeState.Running;
            }

            // Light attack
            if (lightTimer <= 0)
            {
                lightTimer = lightCooldown;
                animator?.SetTrigger("LightAttack");
                Debug.Log($"Boss Light Attack! Damage: {lightDamage}");
            }

            return state = NodeState.Running;
        }
    }
}