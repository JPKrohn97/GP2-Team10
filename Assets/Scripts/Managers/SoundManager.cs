using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundManager : Singleton<SoundManager>
{

    [Header("Music")]
    public EventReference MainMenuMusic;

    public EventReference BossMusic;
    public EventReference Regular;


    [Header("Orb Trap")]
    [Space]
    public EventReference OrbExplosion;
    [Header("Traps")]
    [Space]
    public EventReference SpikeTrapWarning;
    public EventReference SpikeTrapAttack;
    
    [Header("Lava Trap")]
    [Space]
    public EventReference LavaSizzle;

    [Header("Bouncing Ball")]
    [Space]
    public EventReference BallBounce;
    public EventReference BallImpact;

    [Header("BossAttacks")]
    [Space]
    public EventReference BasicBossAttack;
    public EventReference FireBlast;
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

    private EventInstance musicInstance;

    private EventInstance instance;
    public void PlaySoundOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }
    public void PlaySound(EventReference eventReference, string parameterSheetName, string parameterName)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        inst.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        inst.setParameterByNameWithLabel(parameterName, parameterName);
        inst.start();
        inst.release();
    }
    public void PlaySound(EventReference eventReference, string parameterSheetName, string parameterName, Transform playPosition)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(inst, playPosition.gameObject);
        inst.set3DAttributes(RuntimeUtils.To3DAttributes(playPosition));
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
    public void PlaySound(EventReference eventReference, Transform playPosition)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(inst, playPosition.gameObject);

        inst.set3DAttributes(RuntimeUtils.To3DAttributes(playPosition));
        inst.start();
        inst.release();
    }
    public void PlaySound(EventReference eventReference, GameObject playPosition)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(inst, playPosition);

        inst.set3DAttributes(RuntimeUtils.To3DAttributes(playPosition));
        inst.start();
        inst.release();
    }
    public void StartPlaySound(EventReference eventReference, Transform playPosition)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        inst.set3DAttributes(RuntimeUtils.To3DAttributes(playPosition));
        PLAYBACK_STATE playBackState;
        inst.getPlaybackState(out playBackState);

        // Only start if it's NOT already playing
        if (playBackState == PLAYBACK_STATE.STOPPED)
        {

            var attributes = RuntimeUtils.To3DAttributes(transform.position);
            inst.set3DAttributes(attributes);
            inst.start();
        }
    }

    public void StopSound(EventReference eventReference)
    {
        EventInstance inst = RuntimeManager.CreateInstance(eventReference);
        PLAYBACK_STATE playBackState;
        inst.getPlaybackState(out playBackState);
        // Only stop if it's playing
        if (playBackState == PLAYBACK_STATE.PLAYING)
        {
            inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            inst.release();
        }
    }


    public void PlayMusic(EventReference musicEvent)
    {
        // Stop the previous music if it's playing
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }

        // Start the new music
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    public void StopMusic()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }
    }

}
