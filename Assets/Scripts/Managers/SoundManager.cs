using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundManager : Singleton<SoundManager>
{
    [Header("BossAttacks")]
    [Space]
    public EventReference BasicBossAttack;
    public EventReference FireBlast;
    public EventReference ShockWaveJump;
    public EventReference StoneBossJump;
    [Header("PlayerAttacks")]
    [Space]
    public EventReference ChargedAttack;
    public EventReference ClawsAttack;
    public EventReference RangedAttack;
    public EventReference SwordAttack;

    [Header("Footsteps")]
    [Space]

    public EventReference BossSteps;
    public EventReference EnemyFootSteps;
    public EventReference PlayerFootSteps;

    [Header("Impacts")]
    [Space]

    public EventReference BasicBossAttackImpact;
    public EventReference ChargeAttackImpact;
    public EventReference ClawsImpact;
    public EventReference RangedImpact;
    public EventReference ShockWaveImpact;
    public EventReference SwordImpactImpact;

    [Header("On Death Sounds")]
    [Space]

    public EventReference BossDies;
    public EventReference EnemyDies;
    public EventReference PlayerDies;

    [Header("On Hurt Sound")]
    [Space]

    public EventReference EnemyHurt;
    public EventReference PlayerHurt;
    public EventReference Eating;

    [Header("On Hurt Sound")]
    [Space]

    public EventReference MenuSelect;


    public void PlaySound(EventReference eventReference, string parameterSheetName, string parameterName)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        inst.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        inst.setParameterByNameWithLabel(parameterName, parameterName);
        inst.start();
        inst.release();
    }
    public void PlaySound(EventReference eventReference)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        inst.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        inst.start();
        inst.release();
    }
    public void PlaySoundOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }
}
