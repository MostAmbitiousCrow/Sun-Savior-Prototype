using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave_Manager : MonoBehaviour
{
    [Header("Wave Controls")]
    [SerializeField] bool waveStarted;
    public enum WaveMode { WavesInOrder, WavesEndless }
    [SerializeField] WaveMode mode = WaveMode.WavesInOrder;

    [System.Serializable] public struct Wave
    {
        public float duration; // Duration of the wave
        public float spawnRate; // Time per rain pattern
        public int maxEnemies; // Falling Objects to spawn
        public int[] activeSpawners; // What spawners will be active, allowing enemies to spawn
    }
    [SerializeField] List<Wave> waveInfo = new();
    [SerializeField] int currentWave;

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

    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] List<GameObject> activeEnemies = new();

    [Header("Spawners Setup")]
    [SerializeField] int numberOfSides = 5;
    [SerializeField] float spawnerDistance = 15;
    [SerializeField] Transform spawnerFolder;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] List<Transform> enemySpawners;

    // Update is called once per frame
    void Start() // Create Enemy Spawners
    {
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
        StartCoroutine(WaveTimer());
    }

    public void StopCurrentWave()
    {
        activeEnemies.Clear();
        StopCoroutine(WaveTimer());
    }

    IEnumerator WaveTimer()
    {
        timeElapsed = 0;
        waveStarted = true;
        Wave activeWave = waveInfo[currentWave];
        float duration = activeWave.duration;
        float spawnRate = activeWave.spawnRate;
        int maxEnemies = activeWave.maxEnemies;
        int[] activeSpawners = activeWave.activeSpawners;
        int activeSpawnersCount = activeSpawners.Length;
        activeEnemies.Clear();

        while (waveStarted)
        {
            timeElapsed += spawnRate;

            yield return new WaitForSeconds(spawnRate); // Enemy spawn cooldown
            int s = Random.Range(0, activeSpawnersCount);
            Transform st = enemySpawners[activeSpawners[s]];
            activeEnemies.Add(Instantiate(enemyPrefab, st.position, st.rotation));

            if (timeElapsed >= activeWave.duration)
            {
                waveStarted = false;
            }

            yield return null;
        }

        yield break;
    }

    void SpawnEnemy(int spawner)
    {
        Transform st = enemySpawners[spawner].transform;
        activeEnemies.Add(Instantiate(enemyPrefab, st.position, st.rotation));
    }
}
