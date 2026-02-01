using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskRangedBossAttack : Node
    {
        private Transform transform;
        private Transform firePoint;
        private Animator animator;
        private NavMeshAgent agent;
        
        private float lightCooldown;
        private int lightDamage;
        private GameObject projectilePrefab;
        private float projectileSpeed;
        
        private float heavyCooldown;
        private int heavyDamage;
        private float aoeRadius;
        private float aoeSpeed;
        private float heavyChance;

        private float lightTimer = 0f;
        private float heavyTimer = 0f;

        public TaskRangedBossAttack(
            Transform transform,
            NavMeshAgent agent,
            Transform firePoint,
            Animator animator,
            float lightCooldown,
            int lightDamage,
            GameObject projectilePrefab,
            float projectileSpeed,
            float heavyCooldown,
            int heavyDamage,
            float aoeRadius,
            float aoeSpeed,
            float heavyChance)
        {
            this.transform = transform;
            this.agent = agent;
            this.firePoint = firePoint;
            this.animator = animator;
            this.lightCooldown = lightCooldown;
            this.lightDamage = lightDamage;
            this.projectilePrefab = projectilePrefab;
            this.projectileSpeed = projectileSpeed;
            this.heavyCooldown = heavyCooldown;
            this.heavyDamage = heavyDamage;
            this.aoeRadius = aoeRadius;
            this.aoeSpeed = aoeSpeed;
            this.heavyChance = heavyChance;

            if (agent != null)
                agent.updateRotation = false;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            if (target == null)
                return state = NodeState.Failure;

            if (agent != null)
                agent.isStopped = true;
                
            EnemyFacing.FaceTarget(transform, target);

            lightTimer -= Time.deltaTime;
            heavyTimer -= Time.deltaTime;

            // Attempt heavy AOE attack
            if (heavyTimer <= 0 && Random.value < heavyChance)
            {
                heavyTimer = heavyCooldown;
                animator?.SetTrigger("HeavyAttack");
                ShootAOEProjectile(target);
                return state = NodeState.Running;
            }

            // Light normal projectile attack
            if (lightTimer <= 0)
            {
                lightTimer = lightCooldown;
                animator?.SetTrigger("Attack");
                ShootNormalProjectile();
            }

            return state = NodeState.Running;
        }

        private void ShootNormalProjectile()
        {
            if (projectilePrefab == null || firePoint == null)
                return;

            GameObject projectile = ManagerObjectPool.Instance.Spawn(
                ObjectPoolType.EnemyProjectile,
                firePoint.position,
                firePoint.rotation
            );

            if (projectile != null)
            {
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = firePoint.forward;
                    rb.linearVelocity = direction * projectileSpeed;
                }

                var projectileComponent = projectile.GetComponent<Projectile>();
                if (projectileComponent != null)
                {
                    projectileComponent.damage = lightDamage;
                }

                Debug.Log($"Boss fired normal projectile! Damage: {lightDamage}");
            }
        }

        private void ShootAOEProjectile(Transform target)
        {
            Debug.Log($"Boss Heavy AOE Attack! Creating expanding projectile...");
            
            // Calculate direction to player
            Vector3 direction = (target.position - firePoint.position).normalized;
            
            GameObject aoeProjectile = new GameObject("AOEProjectile");
            aoeProjectile.transform.position = firePoint.position;
            aoeProjectile.transform.rotation = Quaternion.LookRotation(direction);
            
            var aoeComponent = aoeProjectile.AddComponent<AOEProjectile>();
            aoeComponent.Initialize(heavyDamage, aoeRadius, aoeSpeed, direction);
        }
    }
}