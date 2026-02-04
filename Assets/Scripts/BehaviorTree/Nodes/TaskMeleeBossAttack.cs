using UnityEngine;

namespace BehaviorTree
{
    public class TaskMeleeBossAttack : Node
    {
        private Transform transform;
        private Animator animator;
        
        private float lightCooldown;
        private float heavyCooldown;
        private float heavyChance;

        private float lightTimer = 0f;
        private float heavyTimer = 0f;
        private int attacksSinceLastHeavy = 0;

        public TaskMeleeBossAttack(Transform transform, Animator animator,
            float lightCooldown, float heavyCooldown, float heavyChance)
        {
            this.transform = transform;
            this.animator = animator;
            this.lightCooldown = lightCooldown;
            this.heavyCooldown = heavyCooldown;
            this.heavyChance = heavyChance;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            EnemyFacing.FaceTarget(transform, target);

            if (IsAttackAnimationPlaying())
            {
                return state = NodeState.Running;
            }

            lightTimer -= Time.deltaTime;
            heavyTimer -= Time.deltaTime;

            // Only decide which attack when ready to attack
            if (lightTimer <= 0)
            {
                bool heavyAvailable = heavyTimer <= 0;
                bool randomHeavyChance = Random.value < heavyChance;
                bool forcedHeavy = attacksSinceLastHeavy >= 3;
                
                // Heavy attack: if available AND (random OR forced)
                if (heavyAvailable && (forcedHeavy || randomHeavyChance))
                {
                    heavyTimer = heavyCooldown;
                    lightTimer = lightCooldown;
                    attacksSinceLastHeavy = 0;
                    animator?.SetTrigger("BossStomp");
                    SoundManager.Instance.PlaySound(SoundManager.Instance.StoneBossJump);
                    
                    return state = NodeState.Running;
                }
                else
                {
                    lightTimer = lightCooldown;
                    
                    // ONLY count when heavy was available but not chosen
                    if (heavyAvailable)
                    {
                        attacksSinceLastHeavy++;
                    }
                    
                    animator?.SetTrigger("LightAttack");
                    SoundManager.Instance.PlaySound(SoundManager.Instance.BasicBossAttack);
                    return state = NodeState.Running;
                }
            }

            return state = NodeState.Running;
        }

        private bool IsAttackAnimationPlaying()
        {
            if (animator == null) return false;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            bool isPlayingLightAttack = stateInfo.IsName("atk_front01");
            bool isPlayingStomp = stateInfo.IsName("atk_ground02");
            
            return isPlayingLightAttack || isPlayingStomp;
        }
    }
}