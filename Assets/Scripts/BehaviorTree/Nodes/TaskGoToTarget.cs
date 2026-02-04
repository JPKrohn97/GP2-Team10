using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskGoToTarget : Node
    {
        private Transform transform;
        private NavMeshAgent agent;
        private float attackRange;
        private Animator animator;
        
        public TaskGoToTarget(Transform transform, NavMeshAgent agent, float attackRange, Animator animator)
        {
            this.transform = transform;
            this.agent = agent;
            this.attackRange = attackRange;
            this.animator = animator;
            agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            // Check if attack animation is playing - if yes, STOP movement AND rotation
            if (animator != null && IsAttackAnimationPlaying())
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                animator.SetBool("isWalking", false);
                // DON'T call EnemyFacing.FaceTarget here - boss should stay locked during attack
                return state = NodeState.Success;
            }

            // Only face target when NOT attacking
            EnemyFacing.FaceTarget(transform, target);

            float distance = Vector3.Distance(transform.position, target.position);
            
            if (distance > attackRange)
            {
                agent.isStopped = false;
                Vector3 destination = EnemyFacing.GetConstrainedPosition(transform.position, target.position);
                agent.SetDestination(destination);
                animator?.SetBool("isWalking", true);

                return state = NodeState.Running;
            }
            
            agent.isStopped = true;
            animator?.SetBool("isWalking", false);
            return state = NodeState.Success;
        }

        private bool IsAttackAnimationPlaying()
        {
            if (animator == null) return false;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // Check for common attack animation states
            return stateInfo.IsName("Attack") ||
                   stateInfo.IsName("LightAttack") ||
                   stateInfo.IsName("HeavyAttack") ||
                   stateInfo.IsName("BossStomp") ||
                   stateInfo.IsName("atk_front01") ||
                   stateInfo.IsName("atk_ground02") ||
                   stateInfo.IsTag("Attack");
        }
    }
}