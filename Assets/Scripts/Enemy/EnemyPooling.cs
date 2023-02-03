using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooling : MonoBehaviour
{
    public static EnemyPooling instance;

    public int maxObjects = 10;
    public GameObject EnemyPrefab;
    public GameObject explosionFX;

    private class PooledObject
    {
        public GameObject gameObject;
        public EnemyBoatAI.EnemyType type;
    }

    private List<PooledObject> availableObjects = new List<PooledObject>();
    private List<PooledObject> inUseObjects = new List<PooledObject>();


    private void Awake()
    {
        instance = this;
        int maxEnemyCount = PlayerPrefs.GetInt("maxEnemyCount");
        if (maxEnemyCount >= 1) maxObjects = maxEnemyCount;
    }

    public void Spawn(Vector3 position, EnemyBoatAI.EnemyType type)
    {
        if (EnemyPrefab == null) return;
        
        if (inUseObjects.Count >= maxObjects)
        {
            return;
        }

        PooledObject objectToUse = availableObjects.Find(obj => obj.type == type);

        if (objectToUse == null)
        {
            GameObject spawn = Instantiate(EnemyPrefab, position, Quaternion.identity);
            spawn.GetComponent<EnemyBoatAI>().enemyType = type;
            objectToUse = new PooledObject
            {
                gameObject = spawn,
                type = type
            };

            inUseObjects.Add(objectToUse);
        }
        else
        {
            objectToUse.gameObject.SetActive(true);
            objectToUse.gameObject.transform.position = position;

            availableObjects.Remove(objectToUse);
            inUseObjects.Add(objectToUse);
        }
    }

    public void Spawn(Vector3 position, EnemyBoatAI.EnemyType type, GameObject enemyPrefab)
    {
        if (EnemyPrefab == null) return;

        if (inUseObjects.Count >= maxObjects)
        {
            return;
        }

        PooledObject objectToUse = availableObjects.Find(obj => obj.type == type);

        if (objectToUse == null)
        {
            GameObject spawn = Instantiate(enemyPrefab, position, Quaternion.identity);
            spawn.GetComponent<EnemyBoatAI>().enemyType = type;
            objectToUse = new PooledObject
            {
                gameObject = spawn,
                type = type
            };

            inUseObjects.Add(objectToUse);
        }
        else
        {
            objectToUse.gameObject.SetActive(true);
            objectToUse.gameObject.transform.position = position;

            availableObjects.Remove(objectToUse);
            inUseObjects.Add(objectToUse);
        }
    }

    public void Death(GameObject gameObject)
    {
        Instantiate(explosionFX, gameObject.transform.position, Quaternion.identity);
        PooledObject objectToReturn = inUseObjects.Find(obj => obj.gameObject == gameObject);

        if (objectToReturn == null)
        {
            return;
        }

        objectToReturn.gameObject.SetActive(false);

        inUseObjects.Remove(objectToReturn);
        availableObjects.Add(objectToReturn);

        EnemyBoatAI enemy = gameObject.GetComponent<EnemyBoatAI>();
        enemy.health = enemy.maxHealth;

    }
}
