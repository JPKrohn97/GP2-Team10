using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    public PlayerCiınematicAnimimationEvents playerCinematicAnimimationEvents;
    public MeshRenderer[] parentRenderers;
    public Rigidbody[] Rigidbodies;
    public MeshCollider[] allBreakableObjects;
    public BoxCollider[] colliders;
    //public ParticleSystem dustParticle;
    private void Awake()
    {
        colliders = GetComponentsInChildren<BoxCollider>();
        playerCinematicAnimimationEvents = FindFirstObjectByType<PlayerCiınematicAnimimationEvents>();
        Rigidbodies = GetComponentsInChildren<Rigidbody>();
        allBreakableObjects = GetComponentsInChildren<MeshCollider>(true);
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].isKinematic = true;
        }
        if (ManagerSave.Instance.SaveState.isFirstBossDefeated)
        {
            gameObject.SetActive(false);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&& playerCinematicAnimimationEvents.isSpecialJump)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;        
            }
            for (int i = 0; i < Rigidbodies.Length; i++)
            {
                Rigidbodies[i].isKinematic = false;
            }
            for (int i = 0; i < allBreakableObjects.Length; i++)
            {
                allBreakableObjects[i].gameObject.SetActive(true);
            }
            other.GetComponentInChildren<PlayerCiınematicAnimimationEvents>().animController.SetTrigger("GroundHitMid");
            ManagerCinemachine.Instance.ShakeOnHit(50f);
            ManagerCinemachine.Instance.HitImpact(0.3f, 0.2f);
            ManagerVibration.Vibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);
            BreakTheGround();
        }
    }

    public void BreakTheGround()
    {
        //dustParticle.Play();
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].isKinematic = false;
        }
        for (int i = 0; i < parentRenderers.Length; i++)
        {
            parentRenderers[i].enabled = false;
        }
    }
}
