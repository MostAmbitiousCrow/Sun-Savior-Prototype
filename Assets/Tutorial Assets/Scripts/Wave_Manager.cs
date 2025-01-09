using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave_Manager : MonoBehaviour
{
    public static Wave_Manager instance;
    [Header("Wave Controls")]
    [SerializeField] bool waveStarted;
    public enum WaveMode { WavesInOrder, WavesEndless }
    [SerializeField] WaveMode mode = WaveMode.WavesInOrder;

    [System.Serializable] public class Wave
    {
        [Tooltip("List of individual Spawner Info. The list is limited to how many sides the shape has.")]
        [System.Serializable] public class Spawn 
        {
            [Tooltip("Which spawner (from 0 to max spawners) should the enemies spawn from")]
            public int activeSpawner = 0;
            [Tooltip("Maximum number of enemies to spawn this wave, from the given spawner")]
            public int maxEnemiesToSpawn = 1;
            [Tooltip("Time, in seconds, it takes for an enemy to spawn")]
            public float spawnRate = 1;
        }
        [SerializeField] public List<Spawn> spawnInfo = new();
    }
    [SerializeField] List<Wave> waveInfo = new();


    [SerializeField] float minMaxEnemySpawnRange = 1;

    [Header("Game Stats")]
    [SerializeField] float timeElapsed = 0;
    public bool gameStarted = false;
    private int wavesCount;
    [SerializeField] int currentWave;

    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] List<GameObject> activeEnemies = new();
    [SerializeField] Transform tower;
    [SerializeField] int enemiesLeft = 0;

    [Header("Spawners Setup")]
    [SerializeField] MeshFilter groundMeshFilter;
    [SerializeField] MeshCollider groundMeshCollider;
    [SerializeField] int numberOfSides = 5;
    [SerializeField] Mesh[] groundMeshes;
    [SerializeField] float spawnerDistance = 15;
    [SerializeField] Transform spawnerFolder;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] List<Transform> enemySpawners;
    private int activeSpawnersCount = 0;

    void Start() // Create Enemy Spawners
    {
        wavesCount = waveInfo.Count;
        for (int i = 0; i < numberOfSides; i++)
        {
            float rot = (360 / numberOfSides) * i;
            Quaternion rota = Quaternion.Euler(0, rot, 0);

            GameObject spawner = Instantiate(spawnerPrefab, new Vector3(), rota, spawnerFolder);
            spawner.name = spawner.name + " " + i;
            Transform spawnerT = spawner.transform;
            enemySpawners.Add(spawnerT);
            Vector3 forwardPos = spawnerT.forward * spawnerDistance;
            forwardPos.y = .5f;
            spawnerT.position = forwardPos;
            Debug.Log(spawnerT.name + " Spawned at: " + spawnerT.position);
        }
        instance = this;
    }

    #region "Validation Check"
    private void OnValidate()
    {
        if (waveInfo.Count == 0) return;
        for (int i = 0; i < waveInfo.Count; i++)
        {
            List<Wave.Spawn> spawn = waveInfo[i].spawnInfo;
            if (spawn.Count > numberOfSides)
            {
                spawn.RemoveAt(numberOfSides);
                Debug.Log("Too many spawners assigned to Wave. Max number of spawners are: " + numberOfSides);
            }

            List<int> availableSpawns = new();
            for (int j = 0; j < numberOfSides; j++) availableSpawns.Add(j);
            //for (int j = 0; j < numberOfSides; j++) print(availableSpawns[j]);

            for (int element = 0; element < spawn.Count; element++)
            {
                Wave.Spawn cSpawn = spawn[element];
                if (availableSpawns.Contains(cSpawn.activeSpawner)) availableSpawns.Remove(cSpawn.activeSpawner);
                else cSpawn.activeSpawner = availableSpawns.First();
                
                if (cSpawn.spawnRate < 0.01f)
                {
                    cSpawn.spawnRate = 0.01f;
                    Debug.Log("Enemy Spawn Rate can't be lower than 0.01.");
                }
                if (cSpawn.activeSpawner < 0)
                {
                    cSpawn.activeSpawner = 0;
                    Debug.Log("Selected Spawner can't be lower than 0.");
                }
                if (cSpawn.activeSpawner > numberOfSides)
                {
                    cSpawn.activeSpawner = numberOfSides;
                    Debug.Log("Selected Spawner can't be higher than the max Number of Sides.");
                }
                if (cSpawn.maxEnemiesToSpawn < 1)
                {
                    cSpawn.maxEnemiesToSpawn = 1;
                    Debug.Log("At least one enemy needs to spawn.");
                }
            }
        }
        // Clamp the number of sides to the valid range
        numberOfSides = Mathf.Clamp(numberOfSides, 3, 8);

        // Map the number of sides to the corresponding mesh index
        int meshIndex = numberOfSides - 3;

        // Assign the corresponding mesh to the mesh filter and collider
        groundMeshFilter.mesh = groundMeshes[meshIndex];
        groundMeshCollider.sharedMesh = groundMeshes[meshIndex];
    }
    #endregion

    public void StartNextWave()
    {
        if (currentWave != wavesCount) StartCoroutine(WaveTimer());
        else print("No more Waves");
    }

    public void StopCurrentWave()
    {
        ClearEnemies();
        StopCoroutine(WaveTimer());
    }

    IEnumerator WaveTimer()
    {
        activeEnemies.Clear();
        timeElapsed = 0;
        for (int i = 0; i < waveInfo[currentWave].spawnInfo.Count; i++)
        {
            StartCoroutine(SpawnEnemies(waveInfo[currentWave].spawnInfo[i]));
            activeSpawnersCount++;
        }
        currentWave++; // Update Current Wave
        waveStarted = true;

        while (activeSpawnersCount > 0) yield return null;
        while (enemiesLeft > 0) yield return null;

        yield return new WaitForSeconds(3); // Delay before the wave ends
        waveStarted = false;

        yield break;
    }

    IEnumerator SpawnEnemies(Wave.Spawn spawnInfo)
    {
        float spawnrate = spawnInfo.spawnRate;
        enemySpawners[spawnInfo.activeSpawner].GetPositionAndRotation(out Vector3 spawnpos, out Quaternion spawnrot);
        int enemiesToSpawn = spawnInfo.maxEnemiesToSpawn;

        Transform spawner = enemySpawners[spawnInfo.activeSpawner];
        Vector3 spawnerPos = spawner.position;

        for (int i  = 0; i  < enemiesToSpawn; i ++)
        {
            timeElapsed += spawnrate;
            //print("Waited for " + spawnrate);
            yield return new WaitForSeconds(spawnrate); // Enemy spawn cooldown

            SpawnEnemy(spawnpos + (spawner.right * Random.Range(-minMaxEnemySpawnRange, minMaxEnemySpawnRange)), spawnrot);

            yield return null;
        }
        //print("Coroutine Completed");
        activeSpawnersCount--;
        yield break;
    }

    void SpawnEnemy(Vector3 spawnpos, Quaternion spawnrot)
    {
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnpos, spawnrot);
        Enemy_AI sEScript = spawnedEnemy.GetComponent<Enemy_AI>();
        sEScript.tower = tower;
        // sEScript.waveManager = this;
        activeEnemies.Add(spawnedEnemy);
        enemiesLeft++;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        Debug.Log("Removed " + enemy.name);
        activeEnemies.Remove(enemy);
        enemiesLeft = activeEnemies.Count;
    }
    public void ClearEnemies()
    {
        foreach (var item in activeEnemies) Destroy(item);
        activeEnemies.Clear();
    }
}
