using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f; // 3 saniye sonra yok olsun
    public int damage = 10;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Mermiyi oluşturulduğu an ileriye (karakterin baktığı yöne) fırlat
        // Unity 6 kullanıyorsan linearVelocity, eskiyse velocity kullan
        rb.linearVelocity = transform.forward * speed;

        // Sonsuza kadar gitmesin, süre dolunca yok et
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Burada ileride düşmana hasar verme kodunu yazacağız.
        // Şimdilik sadece duvara (Layer: Ground) çarpınca yok olsun.
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}