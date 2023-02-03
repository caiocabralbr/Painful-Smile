using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{

    [Header("Moviment Config")]
    public float speed = 5f;
    public float minSpeed = 0f;
    public float maxSpeed = 5f;

    [Header("Health Config")]
    [SerializeField] private float health = 100f;
    public int maxHealth = 100;

    public RectTransform healthBarRectTransform;
    public GameObject healthBarCanvas;
    public GameObject healthBarPivot;

    [Header("Physics Config")]
    private Rigidbody2D rb;
    private float angle = 0f;
    private Vector3 direction = Vector3.right;

    
    [Header("Visuals Config")]
    public SpriteRenderer spriteRenderer;
    public List<ShipHealth> spriteHealthList;

    [Header("Shoot Config")]
    public float shootTimingFrontal;
    public float timeToShootFrontal = 1f;
    public float shootTimingLateral;
    public float timeToShootLateral = 3f;
    public GameObject frontalShootPosition, spreadShootPosition1, spreadShootPosition2, spreadShootPosition3;

    [System.Serializable]
    public struct ShipHealth
    {
        public Sprite sprite;
        public int health;
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    private void Update()
    {
        MoveForward();
        Rotate();
        ShootFrontal();
        ShootTripleLateral();
        UpdateSprite();
        SetHealth();
    }

    private void LateUpdate()
    {
        healthBarPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        healthBarCanvas.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }

    private void MoveForward()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            speed += Time.deltaTime;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            speed -= Time.deltaTime;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }
        rb.velocity = direction * speed;
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            angle += Time.deltaTime * 90f;
            direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            angle -= Time.deltaTime * 90f;
            direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
        }
        transform.rotation = Quaternion.Euler(0, 0, angle+90);
    }

    private void ShootFrontal()
    {
        if (shootTimingFrontal > 0) shootTimingFrontal -= Time.deltaTime;
        if (shootTimingFrontal <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                shootTimingFrontal = timeToShootFrontal;
                BulletPooling.instance.Shoot(new Vector2(Mathf.Sin(-Mathf.Deg2Rad * (angle - 90)), Mathf.Cos(-Mathf.Deg2Rad * (angle - 90))), frontalShootPosition.transform.position, Bullet.ShootedBy.Player, 1.5f);
            }
        }
    }

    private void ShootTripleLateral()
    {
        if(shootTimingLateral > 0) shootTimingLateral -= Time.deltaTime;
        if (shootTimingLateral <= 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                shootTimingLateral = timeToShootLateral;
                BulletPooling.instance.Shoot(new Vector2(Mathf.Sin(-Mathf.Deg2Rad * (angle)), Mathf.Cos(-Mathf.Deg2Rad * (angle))), spreadShootPosition1.transform.position, Bullet.ShootedBy.Player);
                BulletPooling.instance.Shoot(new Vector2(Mathf.Sin(-Mathf.Deg2Rad * (angle)), Mathf.Cos(-Mathf.Deg2Rad * (angle))), spreadShootPosition2.transform.position, Bullet.ShootedBy.Player);
                BulletPooling.instance.Shoot(new Vector2(Mathf.Sin(-Mathf.Deg2Rad * (angle)), Mathf.Cos(-Mathf.Deg2Rad * (angle))), spreadShootPosition3.transform.position, Bullet.ShootedBy.Player);
            }
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

    public void GetHit()
    {
        health -= 20;
        if (health <= 0) GameManager.instance.GameOver();
    }
    public void GetHit(int damage)
    {
        health -= damage;
        if (health <= 0) GameManager.instance.GameOver();
    }

    public void SetHealth()
    {
        var currentHealth = Mathf.Clamp(health, 0, maxHealth);
        float newXScale = (float)currentHealth / maxHealth;
        healthBarRectTransform.localScale = new Vector3(newXScale, 1f, 1f);
    }
}
