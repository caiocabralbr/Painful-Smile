using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyBoatAI : MonoBehaviour
{
    

    public enum EnemyType { Chaser, Shooter }
    public EnemyType enemyType;

    [Header("Moviment Config")]
    public float minSpeed = 0f;
    public float maxSpeed = 1f;
    public float speed = 0f;
    public float rotateSpeed = 90f;
    private Transform playerTransform;
    public float stopDistance = 2;


    [Header("Visual Config")]
    public SpriteRenderer spriteRenderer;
    public List<ShipHealth> spriteHealthList;

    [Header("Physics Config")]
    private Rigidbody2D rb;
    private float angle;

    [Header("Health Config")]
    public RectTransform healthBarRectTransform;
    public GameObject healthBarCanvas;
    public GameObject healthBarPivot;
    public int health = 100;
    public int maxHealth = 100;

    [Header("Attack Config")]
    private float shootTiming;
    public float timeToShoot = 4f;
    public float attackRange = 10f;
    public GameObject frontalShootPosition;

    [System.Serializable]
    public struct ShipHealth
    {
        public Sprite sprite;
        public int health;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    private void Update()
    {
        UpdateSprite();
        SetHealth();

        if (health <= 0) return;

        switch (enemyType)
        {
            case EnemyType.Chaser:
                ChasePlayer();
                break;
            case EnemyType.Shooter:
                ShootPlayer();
                break;
        }
    }

    private void LateUpdate()
    {
        healthBarPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        healthBarCanvas.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }

    private void ChasePlayer()
    {
        if (playerTransform == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        speed += Time.deltaTime;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        rb.velocity = direction * speed;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle+90), Time.deltaTime * rotateSpeed);
    }

    private void ShootPlayer()
    {
        if (playerTransform == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle + 90), Time.deltaTime * rotateSpeed);

        if (Vector3.Distance(transform.position, playerTransform.position) >= stopDistance)
        {
            speed += Time.deltaTime;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (Vector3.Distance(transform.position, playerTransform.position) < attackRange)
        {
            Shoot();
        }
    }

    private void UpdateSprite()
    {
        foreach (ShipHealth spriteHealth in spriteHealthList)
        {
            if (spriteHealth.health <= health)
            {
                spriteRenderer.sprite = spriteHealth.sprite;
                break;
            }
        }
    }

    private void Shoot()
    {
        shootTiming -= Time.deltaTime;
        if (shootTiming <= 0)
        {
            shootTiming = timeToShoot;
            BulletPooling.instance.Shoot(Quaternion.Euler(0, 0, angle-90) * transform.right, frontalShootPosition.transform.position, Bullet.ShootedBy.Enemy, 1.5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (enemyType == EnemyType.Chaser)
            {
                EnemyPooling.instance.Death(this.gameObject);
                collision.gameObject.GetComponent<BoatController>().GetHit(40);
            }
            else
            {
                collision.gameObject.GetComponent<BoatController>().GetHit(10);
                GetHit(10);
            }
        }
    }

    public void GetHit()
    {
        health -= 20;
        if (health <= 0)
        {
            GameManager.instance.AddPoint(1);
            EnemyPooling.instance.Death(this.gameObject);
        }
    }
    public void GetHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.instance.AddPoint(1);
            EnemyPooling.instance.Death(this.gameObject);
        }
    }

    public void SetHealth()
    {
        var currentHealth = Mathf.Clamp(health, 0, maxHealth);
        float newXScale = (float)currentHealth / maxHealth;
        healthBarRectTransform.localScale = new Vector3(newXScale, 1f, 1f);
    }
}
