using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening; 

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    #region References
    public Rigidbody RB;
    public Animator Animator;
    public PlayerCombat Combat;    
    public PlayerAnimations AnimationEvents; 
    
    #endregion
    
    [Header("Settings")]
    public float moveSpeed = 8f;
    public float jumpHeight = 3f;
    public float fallMultiplier = 2.5f;     
    public float lowJumpMultiplier = 2f;
    
    [Header("Ground Check (Raycast)")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    private Collider col;
    
    public bool IsGrounded => CheckIfGrounded();

    [Header("Mutation Interaction")]
    public LayerMask enemyPartLayer;
    
    public GameObject projectilePrefab;
    public Transform firePoint;
    
    public bool IsOnDeadEnemy { get; private set; }
    //public EnemyBehaviour TargetEnemy { get; private set; } 

    // Input System Class
    public PlayerControls InputHandler; 
    public Vector2 CurrentMovementInput { get; private set; }


    #region InputActions
    public InputAction JumpAction { get; private set; }
    public InputAction RangeAction { get; private set; }
    public InputAction AttackAction { get; private set; }
    public InputAction InteractAction { get; private set; }
    
    #endregion 

    
    #region States

    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerSwordAttackState SwordAttackState { get; private set; }
    public PlayerMutationState MutationState { get; private set; }
    public PlayerRangeAttackState RangeAttackState { get; private set; }
    
    #endregion

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        InputHandler = new PlayerControls();
        
        RB = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if(Combat == null) Combat = GetComponent<PlayerCombat>();
        if(AnimationEvents == null) AnimationEvents = GetComponent<PlayerAnimations>();
        if(Animator == null) Animator = GetComponent<Animator>();

        IdleState = new PlayerIdleState(this, StateMachine);
        RunState = new PlayerRunState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        AirState = new PlayerAirState(this, StateMachine);
        SwordAttackState = new PlayerSwordAttackState(this, StateMachine);
        MutationState = new PlayerMutationState(this, StateMachine);
        RangeAttackState = new PlayerRangeAttackState(this, StateMachine);
    }

    private void Start()
    {

        InputHandler.Player.Move.performed += ctx => CurrentMovementInput = ctx.ReadValue<Vector2>();
        InputHandler.Player.Move.canceled += ctx => CurrentMovementInput = Vector2.zero;


        JumpAction = InputHandler.Player.Jump;
        RangeAction = InputHandler.Player.Range;
        AttackAction = InputHandler.Player.Attack;
        InteractAction = InputHandler.Player.Interact;

        StateMachine.Initialize(IdleState);
    }

    private void OnEnable() => InputHandler.Enable();
    private void OnDisable() => InputHandler.Disable();

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    

    
    /*
    public void TriggerMutationByUI(EnemyBehaviour target)
    {
        if (target == null) return;
        TargetEnemy = target;
        IsOnDeadEnemy = true;
        StateMachine.ChangeState(MutationState);
    }
    */

    public bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position + 0.1f * Vector3.up, Vector3.down, (col.bounds.extents.y + groundCheckDistance), groundLayer);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) != 0)
        {
         
            //EnemyBehaviour foundEnemy = other.GetComponentInParent<EnemyBehaviour>();
            
            /*
            if (foundEnemy != null)
            {
                IsOnDeadEnemy = true;
                TargetEnemy = foundEnemy;
            }
            */
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsOnDeadEnemy = false;
            //TargetEnemy = null;
        }
    }
}