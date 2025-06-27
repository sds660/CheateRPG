using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnRange = 0.5f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField][Range(1,100)] private int amountEnemies = 5;
    private List<GameObject> enemies;
    private Transform _myTransform;
    private float time;

    private void Awake()
    {
        _myTransform = GetComponent<Transform>();
        enemies = new List<GameObject>();
    }

    private void Start()
    {
        SpawnRandom();
        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= spawnInterval && enemies.Count() < amountEnemies)
        {
            SpawnRandom();
            time = 0f;
        }
    }

    public void KillEnemies(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        { 
            Destroy(enemy);
        }
        enemies.Clear();
    }

    private void SpawnRandom()
    {
        GameObject enemyPrefab = enemyPrefabs[(int)Random.Range(0, enemyPrefabs.Length)];
        Vector3 spawnOffset = new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), 0);
        GameObject enemyClone = Instantiate(enemyPrefab, _myTransform.position + spawnOffset, _myTransform.rotation);
        enemies.Add(enemyClone);
        GameObject lastEnemy = enemies.Last<GameObject>();
        lastEnemy.AddComponent<MapFollow>();
        lastEnemy.tag = "Enemy";
        lastEnemy.AddComponent<EnemyAttack>();
        lastEnemy.GetComponent<EnemyAttack>().spawnObject = this.gameObject;
    }
}
