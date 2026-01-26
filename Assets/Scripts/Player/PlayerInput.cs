using DG.Tweening;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private LayerMask enemyPart;
    Player player;
    public bool isOnDeadEnemy;
    private float moveInput;
    private Enemy targetEnemy;
    private void Start()
    {
        player= GetComponent<Player>();
    }

    private void OnTriggerStay(Collider other)
    {
        // Use bitwise shift to compare the object's layer to the mask
        if ((enemyPart.value & (1 << other.gameObject.layer)) != 0)
        {
            isOnDeadEnemy = true;
            targetEnemy=other.GetComponentInParent<Enemy>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((enemyPart.value & (1 << other.gameObject.layer)) != 0)
        {
            isOnDeadEnemy = false;
            targetEnemy = null;
        }
    }
    void Update()
    {

#if UNITY_MOBILE




#elif UNITY_STANDALONE || UNITY_WEBGL
        moveInput = 0;
        if (Input.GetKey(KeyCode.A)) moveInput = -1;
        else if (Input.GetKey(KeyCode.D)) moveInput = 1;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            player.playerAnimations.SetMovingBool(true);
        }

        player.playerMovement.SetDirection(moveInput);
        player.playerAnimations.SetMovingBool(moveInput != 0);

        if (Input.GetKeyDown(KeyCode.F))
        {
            player.playerMovement.canMove = false;
            player.playerAnimations.SetAnimationTrigger("Bite");
            DOVirtual.DelayedCall(2f, () =>
            {
                player.playerAnimations.MutationSequence();
            });
        }
        if (Input.GetKeyDown(KeyCode.Space)&&player.playerMovement.isGrounded)
        {
            player.playerMovement.Jump();
        }
        
        if (Input.GetMouseButtonDown(0)&&player.playerMovement.isGrounded)
        {
            player.playerCombat.Attack();
        }
    }
}
