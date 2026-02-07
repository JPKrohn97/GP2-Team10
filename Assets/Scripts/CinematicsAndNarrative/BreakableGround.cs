using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    public MeshRenderer[] parentRenderers;
    public Rigidbody[] Rigidbodies;
    public ParticleSystem dustParticle;
    
    private void Awake()
    {
        Rigidbodies = GetComponentsInChildren<Rigidbody>();
        parentRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].isKinematic = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ManagerCinemachine.Instance.ShakeOnHit(50f);
            ManagerCinemachine.Instance.HitImpact(0.3f, 0.2f);
            BreakTheGround();
        }
    }

    public void BreakTheGround()
    {
        dustParticle.Play();
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
