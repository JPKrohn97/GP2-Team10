using UnityEngine;

public class MagmaWorm : MonoBehaviour
{
    public float appearRadius = 4f;
    public float activeTime = 1.5f;
    public int damage = 8;

    private Vector3 hiddenPos;

    void Start()
    {
        hiddenPos = transform.position;
        gameObject.SetActive(false);
    }

    public void SpawnNear(Transform target)
    {
        Vector2 offset = Random.insideUnitCircle.normalized * appearRadius;

        transform.position = new Vector3(
            target.position.x + offset.x,
            hiddenPos.y,
            target.position.z + offset.y
        );

        gameObject.SetActive(true);
        Invoke(nameof(Hide), activeTime);
    }

    void Hide()
    {
        transform.position = hiddenPos;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
            dmg.TakeDamage(damage);
    }
}
