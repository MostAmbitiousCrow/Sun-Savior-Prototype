using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave_Manager : MonoBehaviour
{
    [Header("Wave Controls")]
    [SerializeField] bool waveStarted;
    public enum WaveMode { WavesInOrder, WavesEndless }
    [SerializeField] WaveMode mode = WaveMode.WavesInOrder;

    [System.Serializable] public class Wave
    {
        [System.Serializable] public struct Spawn 
        {
            [Tooltip("Which spawner (from 0 to max spawners) should the enemies spawn from")]
            public int activeSpawner;
            [Tooltip("Maximum number of enemies to spawn this wave, from the given spawner")]
            public int maxEnemiesToSpawn;
            [Tooltip("Time, in seconds, it takes for an enemy to spawn")]
            public float spawnRate;
        }
        [SerializeField] public List<Spawn> spawnInfo = new();

        //[Tooltip("Duration of the Wave")]
        //public float duration; // Duration of the wave
        //[Tooltip("Spawn rate of enemies, in seconds")]
        //public float spawnRate; // The maximum amount of enemies that can spawn
        //[Tooltip("The maximum amount of enemies that can spawn")]
        //public int maxEnemies; // Falling Objects to spawn
        //[Tooltip("")]
        //public int[] activeSpawners; // What spawners will be active, allowing enemies to spawn
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
        waveStarted = true;
        print("New" + waveInfo[currentWave].spawnInfo.Count);
        for (int i = 0; i < waveInfo[currentWave].spawnInfo.Count; i++)
        {
            StartCoroutine(SpawnEnemies(waveInfo[currentWave].spawnInfo[i]));
        }
        currentWave++; // Update Current Wave
        
        while (enemiesLeft != 0) yield return null;

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

            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnpos, spawnrot);
            Enemy_AI sEScript = spawnedEnemy.GetComponent<Enemy_AI>();
            sEScript.tower = tower;
            sEScript.waveManager = this;
            activeEnemies.Add(spawnedEnemy);
            enemiesLeft++;
            yield return null;
        }
        print("Coroutine Completed");

        yield break;
    }

    //void SpawnEnemy(int spawner)
    //{
    //    Transform st = enemySpawners[spawner].transform;
    //    activeEnemies.Add(Instantiate(enemyPrefab, st.position, st.rotation));
    //}

    public void RemoveEnemy(GameObject enemy)
    {
        Debug.Log("Removed " + enemy.name);
        activeEnemies.Remove(enemy);
        enemiesLeft--;  
    }
    public void ClearEnemies()
    {
        foreach (var item in activeEnemies) Destroy(item);
        activeEnemies.Clear();
    }
}
