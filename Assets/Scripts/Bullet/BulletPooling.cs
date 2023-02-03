using UnityEngine;
using System.Collections.Generic;
using System;

public class BulletPooling : MonoBehaviour
{
    public static BulletPooling instance;
    public GameObject bulletPrefab;
    public int poolSize = 20;

    public List<GameObject> activeBullets;
    public List<GameObject> inactiveBullets;

    void Start()
    {
        instance = this;
        activeBullets = new List<GameObject>();
        inactiveBullets = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            inactiveBullets.Add(bullet);
        }
    }

    public void Shoot(Vector2 direction, Vector2 position, Bullet.ShootedBy _shootedBy, float size = 1)
        
    {
        GameObject bullet;
        if (inactiveBullets.Count > 0)
        {
            bullet = inactiveBullets[0];
            inactiveBullets.RemoveAt(0);
        }
        else
        {
            if (activeBullets.Count == poolSize) return;
            bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        }

        bullet.transform.position = position;
        bullet.transform.parent = this.transform;
        bullet.transform.localScale = Vector3.one * size;
        bullet.SetActive(true);
        activeBullets.Add(bullet);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bullet.GetComponent<Bullet>().speed;
        bullet.GetComponent<Bullet>().shootedBy = _shootedBy;
    }

    public void Death(GameObject bullet)
    {
        bullet.SetActive(false);
        activeBullets.Remove(bullet);
        inactiveBullets.Add(bullet);
    }

    internal void Shoot(Vector2 vector2, object position, float v)
    {
        throw new NotImplementedException();
    }
}
