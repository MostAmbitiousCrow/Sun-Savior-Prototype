using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave_Manager : MonoBehaviour
{
    [Header("Wave Controls")]
    [SerializeField] bool pauseWaves;
    public enum WaveMode { WavesInOrder, WavesEndless }
    [SerializeField] WaveMode mode = WaveMode.WavesInOrder;

    [System.Serializable] public struct Wave
    {
        public float duration; // Duration of the wave
        public float timePerPattern; // Time per rain pattern
        public float rainSpeed; // Animation Speed of each rain
        public int fallingObjects; // Falling Objects to spawn
    }

    public struct EndlessWave
    {
        public float duration; // Duration of the wave
        public float spawnRate; // Time per rain pattern
        public float rainSpeed; // Animation Speed of each rain
        public int fallingObjects; // Falling Objects to spawn
    }

    [Header("Game Stats")]
    [SerializeField] float timeElapsed = 0;
    [SerializeField] int currentWave;
    private float baseTimeElapsed = 0;
    public bool gameStarted = false;

    [Header("Components")]
    [SerializeField] List<GameObject> enemySpawners;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
