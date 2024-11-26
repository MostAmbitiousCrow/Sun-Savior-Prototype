using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.Progress;
using static Wave_Manager;

public class Wave_Manager : MonoBehaviour
{
    [Header("Wave Controls")]
    [SerializeField] bool waveStarted;
    public enum WaveMode { WavesInOrder, WavesEndless }
    [SerializeField] WaveMode mode = WaveMode.WavesInOrder;

    [System.Serializable] public class Wave
    {
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


    [SerializeField]
    struct EndlessWave
    {
        float duration; // Duration of the wave
        float spawnRate; // Time per rain pattern
        int maxEnemies; // Overall number of enemies to spawn
    }

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
    [SerializeField] int numberOfSides = 5;
    [SerializeField] float spawnerDistance = 15;
    [SerializeField] Transform spawnerFolder;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] List<Transform> enemySpawners;
    private int pp = 0;

    private List<Coroutine> spawnerRoutines = new();

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
            
            for (int element = 0; element < spawn.Count; element++)
            {
                Wave.Spawn cSpawn = spawn[element];
                if (availableSpawns.Contains(cSpawn.activeSpawner)) availableSpawns.Remove(cSpawn.activeSpawner);
                else cSpawn.activeSpawner = cSpawn.activeSpawner++;
                
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
        print("New" + waveInfo[currentWave].spawnInfo.Count);
        for (int i = 0; i < waveInfo[currentWave].spawnInfo.Count; i++)
        {
            StartCoroutine(SpawnEnemies(waveInfo[currentWave].spawnInfo[i]));
            pp++;
        }
        currentWave++; // Update Current Wave
        waveStarted = true;

        while (pp > 0) yield return null;
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

        for (int i  = 0; i  < enemiesToSpawn; i ++)
        {
            timeElapsed += spawnrate;
            print("Waited for " + spawnrate);
            yield return new WaitForSeconds(spawnrate); // Enemy spawn cooldown
            SpawnEnemy(spawnpos, spawnrot);

            yield return null;
        }
        //print("Coroutine Completed");
        pp--;
        yield break;
    }

    void SpawnEnemy(Vector3 spawnpos, Quaternion spawnrot)
    {
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnpos, spawnrot);
        Enemy_AI sEScript = spawnedEnemy.GetComponent<Enemy_AI>();
        sEScript.tower = tower;
        sEScript.waveManager = this;
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
