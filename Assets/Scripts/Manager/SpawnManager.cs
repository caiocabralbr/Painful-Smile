using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnWeight
{
    public EnemyBoatAI.EnemyType type;
    public int weight;
    public GameObject enemyPrefab;
}

public class SpawnManager : MonoBehaviour
{
    public List<SpawnWeight> spawnWeights;
    public float spawnTimer;
    public float maxSpawnRadius;
    private float currentTime;
    private Transform playerTransform;

    private void Start()
    {
        currentTime = spawnTimer;
        playerTransform = FindObjectOfType<BoatController>().transform;
        int spawnRateData = PlayerPrefs.GetInt("spawnRate");
        if (spawnRateData >= 1) spawnTimer = spawnRateData;

    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = spawnTimer;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = Vector3.zero;
        int totalWeight = 0;
        foreach (SpawnWeight enemyWeight in spawnWeights)
        {
            totalWeight += enemyWeight.weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        for (int i = 0; i < spawnWeights.Count; i++)
        {
            if (randomWeight < spawnWeights[i].weight)
            {
                Vector3 direction = Random.insideUnitCircle.normalized;
                float distance = Random.Range(0, maxSpawnRadius);
                spawnPosition = playerTransform.position + direction * distance;

                float playerDistance = Vector3.Distance(spawnPosition, playerTransform.position);
                if (playerDistance < maxSpawnRadius / 2)
                {
                    i--;
                    continue;
                }
                EnemyPooling.instance.Spawn(spawnPosition, spawnWeights[i].type, spawnWeights[i].enemyPrefab);

                break;
            }
            else
            {
                randomWeight -= spawnWeights[i].weight;
            }
        }
    }
}
