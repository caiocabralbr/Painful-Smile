using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 10f;
    public float speed = 10f;
    public GameObject explosionFX;

    public ShootedBy shootedBy;

    public enum ShootedBy
    {
        Player,
        Enemy
    }
    private void Start() => Invoke(nameof(Destroy), timeToDestroy);

    
    private void OnBecameInvisible()
    {
        BulletPooling.instance.Death(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet"))
        {
            if (collision.CompareTag("Player") && shootedBy == ShootedBy.Player) return;
            if (collision.CompareTag("Enemy") && shootedBy == ShootedBy.Enemy) return;

            var fx = Instantiate(explosionFX, gameObject.transform.position, Quaternion.identity, collision.transform);
            BulletPooling.instance.Death(gameObject);
        }

        if (collision.CompareTag("Player") && shootedBy == ShootedBy.Enemy) collision.GetComponent<BoatController>().GetHit();
        if (collision.CompareTag("Enemy") && shootedBy == ShootedBy.Player) collision.GetComponent<EnemyBoatAI>().GetHit(40);
    }

    private void Destroy() => BulletPooling.instance.Death(gameObject);
}
