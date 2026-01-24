using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening; // Tween kullanıyorsunuz

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Referanslar")]
    public Rigidbody RB;
    public Animator Animator;
    public PlayerCombat Combat;     // Arkadaşının yazdığı Combat scripti
    public PlayerAnimations AnimationEvents; // Animasyon eventleri için
    
    [Header("Ayarlar")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    
    [Header("Ground Check (Raycast)")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    private Collider col;

    [Header("Mutation Interaction")]
    public LayerMask enemyPartLayer;
    public bool IsOnDeadEnemy { get; private set; }
    public Enemy TargetEnemy { get; private set; } // Mutation için hedef

    // Input System Class
    public PlayerControls InputHandler; 
    public Vector2 CurrentMovementInput { get; private set; }

    // --- STATE MACHINE ---
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerSwordAttackState SwordAttackState { get; private set; }
    public PlayerMutationState MutationState { get; private set; } // YENİ: F tuşu için

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        InputHandler = new PlayerControls();
        
        RB = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        // Combat ve Animation referanslarını otomatik alalım veya editörden sürükle
        if(Combat == null) Combat = GetComponent<PlayerCombat>();
        if(AnimationEvents == null) AnimationEvents = GetComponent<PlayerAnimations>();
        if(Animator == null) Animator = GetComponent<Animator>();

        // State Oluşturma
        IdleState = new PlayerIdleState(this, StateMachine);
        RunState = new PlayerRunState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        AirState = new PlayerAirState(this, StateMachine);
        SwordAttackState = new PlayerSwordAttackState(this, StateMachine);
        MutationState = new PlayerMutationState(this, StateMachine);
    }

    private void Start()
    {
        InputHandler.Player.Move.performed += ctx => CurrentMovementInput = ctx.ReadValue<Vector2>();
        InputHandler.Player.Move.canceled += ctx => CurrentMovementInput = Vector2.zero;

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

    // --- YARDIMCI METODLAR ---

    // Arkadaşının yazdığı Raycast mantığının aynısı
    public bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position + 0.1f * Vector3.up, Vector3.down, (col.bounds.extents.y + groundCheckDistance), groundLayer);
    }

    // Enemy Part ile etkileşim (Eski PlayerInput'tan taşındı)
    private void OnTriggerStay(Collider other)
    {
        // Bitwise kontrol
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsOnDeadEnemy = true;
            TargetEnemy = other.GetComponentInParent<Enemy>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyPartLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsOnDeadEnemy = false;
            TargetEnemy = null;
        }
    }
}