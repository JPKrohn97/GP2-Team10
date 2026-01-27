using UnityEngine;

namespace BehaviorTree
{
    public class TaskAttack : Node
    {
        private Transform transform;
        private Animator animator;
        private float attackCooldown;
        private float lastAttackTime;
        private int damage;
        private string attackTrigger;

        public TaskAttack(Transform transform, Animator animator, float cooldown, int damage, string attackTrigger = "Attack")
        {
            this.transform = transform;
            this.animator = animator;
            this.attackCooldown = cooldown;
            this.damage = damage;
            this.attackTrigger = attackTrigger;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Instant direction change - left/right only
            EnemyFacing.FaceTarget(transform, target);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                animator?.SetTrigger(attackTrigger);
            }

            return state = NodeState.Running;
        }
    }
}