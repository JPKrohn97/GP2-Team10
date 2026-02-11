using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float dashStartTime;
    private float dashDuration = 0.2f;
    private float dashSpeed = 25f;
    private Vector3 dashStartPos; 

    private float damageRadius = 1.0f; 

    private Collider[] hitColliders = new Collider[10]; 
    private GameObject currentTrail;
    
    private float baseDashDuration = 0.2f;
    private float durationPerLevel = 0.05f;

    public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
        int dashLevel = player.SkillController.GetSkillLevel(
            EnemyHealth.EnemyMutationType.Dash
        );

        dashDuration = baseDashDuration + (dashLevel - 1) * durationPerLevel;
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.ChargedAttack, player.gameObject);
        }


        //player.lastDashTime = Time.time;
        dashStartTime = Time.time;
        dashStartPos = player.transform.position; 
        player.AnimationEvents.ShowShieldVisuals();
        
        
        if (player.Combat.dashTrailPrefab != null)
        {
 
            currentTrail = Object.Instantiate(player.Combat.dashTrailPrefab, player.transform);
            currentTrail.transform.localPosition = Vector3.zero; 
            currentTrail.transform.localRotation = Quaternion.identity;
        }
        Physics.IgnoreLayerCollision(player.gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        player.Animator.SetTrigger("ChargeSkill");
        
        player.RB.useGravity = false;
        player.RB.linearVelocity = Vector3.zero; 
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.RB.linearVelocity = player.transform.forward * dashSpeed;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        float t = Time.time - dashStartTime;

        if (t >= dashDuration)
        {
            ApplyPathDamage(); 
            stateMachine.ChangeState(player.IdleState);
        }
    }

    private void ApplyPathDamage()
    {
        
        int numHits = Physics.OverlapCapsuleNonAlloc(
            dashStartPos,              
            player.transform.position, 
            damageRadius,          
            hitColliders,            
            player.Combat.enemyLayer   
        );
        
        bool hasHitAnyone = false;

        for (int i = 0; i < numHits; i++)
        {
            if (hitColliders[i].TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(player.Combat.dashDamage);
                hasHitAnyone = true;
                
                
                if (hasHitAnyone && SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.ChargeAttackImpact, player.gameObject);
                    
                }
          
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        player.AnimationEvents.HideShieldVisuals();
        
        if (currentTrail != null)
        {
 
            currentTrail.transform.parent = null;


            TrailRenderer tr = currentTrail.GetComponent<TrailRenderer>();
            if (tr != null)
            {
                tr.emitting = false;
            }
            
            ParticleSystem ps = currentTrail.GetComponent<ParticleSystem>();
            if(ps != null)
            {
                var main = ps.main;
                main.loop = false;
            }
            Object.Destroy(currentTrail, 2f);
        }

        Physics.IgnoreLayerCollision(player.gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
        
        player.RB.useGravity = true;
        player.RB.linearVelocity = Vector3.zero;
    }
}