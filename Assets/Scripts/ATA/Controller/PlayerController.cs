using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    
    
    public enum ActiveWeaponType
    {
        Claw,
        Sword
    }
    public ActiveWeaponType CurrentWeapon = ActiveWeaponType.Claw;
    
    #region References
    public Rigidbody RB;
    public Animator Animator;
    public PlayerCombat Combat;
    public PlayerAnimations AnimationEvents;
    public PlayerSkillController SkillController;
    public EnemyHealth CurrentDeadEnemy { get; private set; }
    #endregion

    [Header("Settings")]
    public float moveSpeed = 8f;
    public float jumpHeight = 2.5f;
    public float fallMultiplier = 3.5f;
    
    [Header("Skill Settings")]
    public float swordSkillCooldown = 10f; // 10 saniye bekleme süresi
    private float lastSwordSkillTime = -100f;
    
    [Space]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    private Collider col;
    
    

    public bool IsGrounded { get; private set; }
    public float LastAttackInputTime { get; private set; } = -100f;
    
    [Header("Dash Settings")]
    public float dashCooldown = 1f;
    [HideInInspector] public float lastDashTime = -10f;
 
    [Header("Mutation Interaction")]
    public LayerMask enemyPartLayer;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float mutationRange = 2.2f;
    public bool IsOnDeadEnemy { get; private set; }

    // Input System Class
    public PlayerControls InputHandler;
    public Vector2 CurrentMovementInput { get; private set; }

    #region InputActions
    public InputAction MoveAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public InputAction RangeAction { get; private set; }
    public InputAction AttackAction { get; private set; }
    public InputAction InteractAction { get; private set; }
    public InputAction InteractPause { get; private set; }
    public InputAction DashAction { get; private set; } 
    #endregion

    #region States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerClawAttackState ClawAttackState { get; private set; }
    public PlayerMutationState MutationState { get; private set; }
    public PlayerRangeAttackState RangeAttackState { get; private set; }
    public PlayerDashState DashState { get; private set; } 
    public PlayerSwordAttackState SwordAttackState { get; private set; }
    
    #endregion

    [Header("Pause Game Canvas")]
    public UIPauseGame Script;

    private int enemyPartContacts = 0;

    private void Awake()
    {
        Input.multiTouchEnabled = true;
        
        StateMachine = new PlayerStateMachine();
        InputHandler = new PlayerControls();

        RB = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (Combat == null) Combat = GetComponent<PlayerCombat>();
        if (AnimationEvents == null) AnimationEvents = GetComponent<PlayerAnimations>();
        if (Animator == null) Animator = GetComponent<Animator>();
        if (SkillController == null) SkillController = GetComponent<PlayerSkillController>();
        // Inputs
        MoveAction = InputHandler.Player.Move;
        JumpAction = InputHandler.Player.Jump;
        RangeAction = InputHandler.Player.Range;
        AttackAction = InputHandler.Player.Attack;
        InteractAction = InputHandler.Player.Interact;
        InteractPause = InputHandler.Player.Pause;
        DashAction = InputHandler.Player.Dash; 

        // States
        IdleState = new PlayerIdleState(this, StateMachine);
        RunState = new PlayerRunState(this, StateMachine);
        AirState = new PlayerAirState(this, StateMachine);
        ClawAttackState = new PlayerClawAttackState(this, StateMachine);
        MutationState = new PlayerMutationState(this, StateMachine);
        RangeAttackState = new PlayerRangeAttackState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine); 
        SwordAttackState = new PlayerSwordAttackState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void OnEnable()
    {
        InputHandler.Enable();
        EnhancedTouchSupport.Enable();
        
        MoveAction.performed += OnMove;
        MoveAction.canceled += OnMoveCanceled;
        
        AttackAction.performed += OnAttackInput;
        DashAction.performed += OnDashInput; 
    }

    private void OnDisable()
    {
        MoveAction.performed -= OnMove;
        MoveAction.canceled -= OnMoveCanceled;

        AttackAction.performed -= OnAttackInput;
        DashAction.performed -= OnDashInput; 
        
        InputHandler.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx) => CurrentMovementInput = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => CurrentMovementInput = Vector2.zero;
    private void OnAttackInput(InputAction.CallbackContext ctx) => VirtualClawAttackInput();
    private void OnDashInput(InputAction.CallbackContext ctx) => VirtualDashInput();

    private void Update()
    {
        IsGrounded = CheckIfGrounded();
        StateMachine.CurrentState.LogicUpdate();

        if (InteractPause.WasPressedThisFrame())
        {
            if(Script != null)
            {
                if (Script.IsPaused)
                    Script.UnpauseGame();
                else
                    Script.PauseGame();
            }
        }
        
        if (CurrentDeadEnemy != null)
        {
            float dist = Vector3.Distance(
                transform.position,
                CurrentDeadEnemy.transform.position
            );

            IsOnDeadEnemy = dist <= mutationRange;
        }
        else
        {
            IsOnDeadEnemy = false;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.canPlayerMove == false) return;
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public bool CheckIfGrounded()
    {
        if (col == null) return false;
        Vector3 origin = col.bounds.center;
        origin.y = col.bounds.min.y + 0.05f;
        
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.1f, groundLayer, QueryTriggerInteraction.Ignore);
    }
    
    public void Jump()
    {
        if (!IsGrounded) return;

        Vector3 v = RB.linearVelocity;
        v.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        RB.linearVelocity = v;

        StateMachine.ChangeState(AirState);
    }
    
    
    public void VirtualDashInput()
    {
    
        if (SkillController.GetSkillLevel(EnemyHealth.EnemyMutationType.Dash) > 0)
        {
            if (Time.time >= lastDashTime + dashCooldown && StateMachine.CurrentState != DashState)
                StateMachine.ChangeState(DashState);
        }
    }
    
    public void VirtualJumpInput()
    {
        Jump();
        
    }


    public void VirtualClawAttackInput()
    {
        if (StateMachine.CurrentState == SwordAttackState)
            return;

        LastAttackInputTime = Time.time;

        if (StateMachine.CurrentState != ClawAttackState)
        {
            StateMachine.ChangeState(ClawAttackState);
        }
    }

    public void VirtualSkillSwordInput()
    {
      
        if (SkillController.GetSkillLevel(EnemyHealth.EnemyMutationType.Sword) <= 0)
            return;

        if (Time.time < lastSwordSkillTime + swordSkillCooldown)
            return;

        lastSwordSkillTime = Time.time;

        if (StateMachine.CurrentState != SwordAttackState)
        {
            CurrentWeapon = ActiveWeaponType.Sword;
            StateMachine.ChangeState(SwordAttackState);
        }
    }


    public void ResetToClaw()
    {
        if (CurrentWeapon == ActiveWeaponType.Claw) return;

        CurrentWeapon = ActiveWeaponType.Claw;
        AnimationEvents?.HideSwordVisuals();
    }
    
    
    
    public void VirtualMutationInput()
    {
        if (IsOnDeadEnemy && CurrentDeadEnemy != null)
        {
            StateMachine.ChangeState(MutationState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) == 0) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null && enemy.IsDead)
        {
            CurrentDeadEnemy = enemy;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) == 0) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null && enemy == CurrentDeadEnemy)
        {
            CurrentDeadEnemy = null;
            IsOnDeadEnemy = false;
        }
    }
    
    public void VirtualRangeInput()
    {
        if (SkillController.GetSkillLevel(EnemyHealth.EnemyMutationType.Range) > 0)
        {
            StateMachine.ChangeState(RangeAttackState);
        }
    }
    
    public void SpawnProjectile()
    {
        
        //int rangeLevel = SkillController.GetSkillLevel(EnemyHealth.EnemyMutationType.Range);
        
        GameObject projectile = ManagerObjectPool.Instance.Spawn(ObjectPoolType.PlayerProjectile, firePoint.position, transform.rotation);
    
        if (projectile != null)
        {
            Rigidbody prb = projectile.GetComponent<Rigidbody>();
            if (prb != null)
            {
                prb.linearVelocity = transform.forward * 30f;
            }
        }
    }
    
    public void ApplyKnockback(Vector3 dir, float force)
    {
        dir.y = 0f;
        RB.AddForce(-dir.normalized * force, ForceMode.Impulse);
        DOVirtual.DelayedCall(0.5f,() => {
             if(RB != null) RB.linearVelocity = new Vector3(0, RB.linearVelocity.y, 0);
        });
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, mutationRange);
    }
}