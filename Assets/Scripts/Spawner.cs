using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesLeftInWave;
    int enemiesLeftAlive;
    float nextSpawnTime;


    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if (enemiesLeftInWave > 0 && Time.time > nextSpawnTime)
        {
            enemiesLeftInWave--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, transform.position, transform.rotation) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }


    void OnEnemyDeath()
    {
        enemiesLeftAlive--;
        if (enemiesLeftAlive == 0)
        {
            NextWave();
        }
    }


    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {

            currentWave = waves[currentWaveNumber - 1];

            enemiesLeftInWave = currentWave.enemyCount;
            enemiesLeftAlive = currentWave.enemyCount;
        }
    }


    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
        
    }    
}
