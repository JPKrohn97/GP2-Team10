using UnityEngine;

public class VolcanicLavaPool : MonoBehaviour
{
    [Header("Movement")]
    public float moveRadius = 3f;
    public float moveSpeed = 2f;

    [Header("Damage")]
    public int damage = 5;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;
        PickNewPosition();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            PickNewPosition();
    }

    void PickNewPosition()
    {
        Vector3 offset = new Vector3(
            Random.Range(-moveRadius, moveRadius),
            0f,
            Random.Range(-moveRadius, moveRadius)
        );

        targetPos = startPos + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
            dmg.TakeDamage(damage);
    }
}
