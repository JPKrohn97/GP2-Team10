using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    public PlayerCiınematicAnimimationEvents playerCinematicAnimimationEvents;
    public MeshRenderer[] parentRenderers;
    public Rigidbody[] Rigidbodies;
    private MeshCollider[] allBreakableObjects;
    //public ParticleSystem dustParticle;
    private void Awake()
    {
        playerCinematicAnimimationEvents = FindFirstObjectByType<PlayerCiınematicAnimimationEvents>();
        Rigidbodies = GetComponentsInChildren<Rigidbody>();
        allBreakableObjects = GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].isKinematic = true;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&& playerCinematicAnimimationEvents.isSpecialJump)
        {
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
