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
    public EnemyHealth CurrentDeadEnemy { get; private set; }
    
    #endregion

    [Header("Settings")]
    public float moveSpeed = 8f;
    public float jumpHeight = 3f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    
    [Space]
    public float groundCheckDistance = 0.15f;
    public LayerMask groundLayer;
    private Collider col;


    public bool IsGrounded { get; private set; }
    private float groundCheckTimer;
    private const float GroundCheckInterval = 0.02f; 

    [Header("Mutation Interaction")]
    public LayerMask enemyPartLayer;

    public GameObject projectilePrefab;
    public Transform firePoint;
    

    public bool IsOnDeadEnemy { get; private set; }
    public bool IsFinalComboActive { get; set; }
    


    // Input System Class
    public PlayerControls InputHandler;
    public Vector2 CurrentMovementInput { get; private set; }

    #region InputActions
    public InputAction MoveAction { get; private set; }
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


    private int enemyPartContacts = 0;

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        InputHandler = new PlayerControls();

        RB = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (Combat == null) Combat = GetComponent<PlayerCombat>();
        if (AnimationEvents == null) AnimationEvents = GetComponent<PlayerAnimations>();
        if (Animator == null) Animator = GetComponent<Animator>();
        
        MoveAction = InputHandler.Player.Move;
        JumpAction = InputHandler.Player.Jump;
        RangeAction = InputHandler.Player.Range;
        AttackAction = InputHandler.Player.Attack;
        InteractAction = InputHandler.Player.Interact;

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
        StateMachine.Initialize(IdleState);
    }

    private void OnEnable()
    {
        InputHandler.Enable();
        
        MoveAction.performed += OnMove;
        MoveAction.canceled += OnMoveCanceled;


        IsGrounded = CheckIfGrounded();
        groundCheckTimer = 0f;
    }

    private void OnDisable()
    {
        MoveAction.performed -= OnMove;
        MoveAction.canceled -= OnMoveCanceled;

        InputHandler.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx) => CurrentMovementInput = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => CurrentMovementInput = Vector2.zero;

    private void Update()
    {
        IsGrounded = CheckIfGrounded();

        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }


    public bool CheckIfGrounded()
    {
        if (col == null) return false;

        Vector3 origin = col.bounds.center;
        origin.y = col.bounds.min.y + 0.05f;

        float distance = groundCheckDistance + 0.1f;

        return Physics.Raycast(
            origin,
            Vector3.down,
            distance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) == 0) return;

        enemyPartContacts++;
        IsOnDeadEnemy = true;
        
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
    

        if (enemy != null && enemy.IsDead)
        {
            CurrentDeadEnemy = enemy;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) == 0) return;

        enemyPartContacts = Mathf.Max(0, enemyPartContacts - 1);
    

        if (enemyPartContacts == 0)
        {
            IsOnDeadEnemy = false;
            CurrentDeadEnemy = null; 
        }
    }
    
    public void SpawnProjectile()
    {
        GameObject projectile = ManagerObjectPool.Instance.Spawn(ObjectPoolType.Projectile, firePoint.position, transform.rotation);

        if (projectile != null)
        {
            Rigidbody prb = projectile.GetComponent<Rigidbody>();
        
  
            prb.linearVelocity = Vector3.zero;
            prb.angularVelocity = Vector3.zero;
            
            prb.linearVelocity = transform.forward * 30f;
        }
    }
    
    public void ApplyKnockback(Vector3 dir, float force)
    {
        dir.y = 0f;
        RB.AddForce(dir.normalized * force, ForceMode.Impulse);
        DOVirtual.DelayedCall(0.5f,() => RB.linearVelocity = new Vector3(0,RB.linearVelocity.y,0));
    }
    
}
