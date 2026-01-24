using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.RB.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);
    }
}