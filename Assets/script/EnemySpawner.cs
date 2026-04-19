using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // จุด Spawn รอบบันได
    public float spawnInterval = 3f;
    public int maxEnemiesPerWave = 5;
    public float spawnRadius = 1f; // รัศมี Random รอบจุด Spawn
    
    [Header("Wave Settings")]
    public bool useWaves = true;
    public float timeBetweenWaves = 10f;
    public int currentWave = 0;
    public int enemiesPerWave = 3; // เพิ่มทีละ wave
    
    [Header("Auto Generate Spawn Points")]
    public bool autoGenerateSpawnPoints = false;
    public int numberOfSpawnPoints = 8;
    public float spawnDistance = 5f; // ระยะห่างจากศูนย์กลาง
    
    private float lastSpawnTime;
    private int enemiesSpawnedThisWave;
    private bool waveInProgress = false;
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    void Start()
    {
        // สร้างจุด Spawn อัตโนมัติ
        if (autoGenerateSpawnPoints && spawnPoints.Length == 0)
        {
            GenerateSpawnPoints();
        }
        
        if (useWaves)
        {
            StartWave();
        }
    }
    
    void Update()
    {
        // ลบศัตรูที่ตายแล้วออกจาก List
        activeEnemies.RemoveAll(enemy => enemy == null);
        
        if (useWaves)
        {
            // เช็คว่า Wave จบหรือยัง
            if (waveInProgress && activeEnemies.Count == 0 && enemiesSpawnedThisWave >= enemiesPerWave)
            {
                StartCoroutine(WaitForNextWave());
            }
        }
        else
        {
            // Spawn แบบต่อเนื่อง
            if (Time.time - lastSpawnTime >= spawnInterval && activeEnemies.Count < maxEnemiesPerWave)
            {
                SpawnEnemy();
                lastSpawnTime = Time.time;
            }
        }
    }
    
    // เริ่ม Wave
    void StartWave()
    {
        currentWave++;
        enemiesSpawnedThisWave = 0;
        waveInProgress = true;
        
        Debug.Log($"Wave {currentWave} Started! Enemies: {enemiesPerWave}");
        
        StartCoroutine(SpawnWave());
    }
    
    // Spawn ศัตรูทีละตัวใน Wave
    System.Collections.IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            enemiesSpawnedThisWave++;
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    // รอ Wave ถัดไป
    System.Collections.IEnumerator WaitForNextWave()
    {
        waveInProgress = false;
        Debug.Log($"Wave {currentWave} Completed! Next wave in {timeBetweenWaves} seconds...");
        
        yield return new WaitForSeconds(timeBetweenWaves);
        
        // เพิ่มความยากในแต่ละ Wave
        enemiesPerWave += Mathf.RoundToInt(currentWave * 0.5f);
        
        StartWave();
    }
    
    // Spawn ศัตรู 1 ตัว
    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }
        
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned!");
            return;
        }
        
        // สุ่มจุด Spawn
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        // เพิ่ม Random ตำแหน่งเล็กน้อย
        Vector3 spawnPosition = spawnPoint.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );
        
        // สร้างศัตรู
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
        activeEnemies.Add(enemy);
        
        Debug.Log($"Enemy spawned at {spawnPoint.name}");
    }
    
    // สร้างจุด Spawn รอบๆ บันได
    void GenerateSpawnPoints()
    {
        GameObject spawnParent = new GameObject("SpawnPoints");
        spawnParent.transform.SetParent(transform);
        
        List<Transform> points = new List<Transform>();
        
        for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            float angle = i * (360f / numberOfSpawnPoints);
            float radian = angle * Mathf.Deg2Rad;
            
            Vector3 spawnPos = transform.position + new Vector3(
                Mathf.Cos(radian) * spawnDistance,
                0,
                Mathf.Sin(radian) * spawnDistance
            );
            
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
            spawnPoint.transform.position = spawnPos;
            spawnPoint.transform.SetParent(spawnParent.transform);
            
            // หันเข้าหาศูนย์กลาง
            spawnPoint.transform.LookAt(transform.position);
            
            points.Add(spawnPoint.transform);
        }
        
        spawnPoints = points.ToArray();
        
        Debug.Log($"Generated {numberOfSpawnPoints} spawn points");
    }
    
    // วาด Gizmos
    void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                    Gizmos.DrawLine(point.position, point.position + point.forward * 2f);
                }
            }
        }
        
        // วาดวงรอบศูนย์กลาง
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnDistance);
    }
}