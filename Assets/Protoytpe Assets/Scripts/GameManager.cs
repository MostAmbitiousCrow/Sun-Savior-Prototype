using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static int Money {get ; set;}
    public static List<GameObject> activeEnemies;
    public static Camera_Controller camera_Controller;

    [SerializeField] TextMeshProUGUI announcementText;
    [SerializeField] TextMeshProUGUI enemyCountText;
    
    void Start()
    {
        instance = this;
        Money = 10; // Testing purposes
    }

    public void RemoveMoney(int value)
    {
        Money =- (value);
    }

    public void AddMoney(int value)
    {
        Money = -(value);
    }

    public void GameOverEvent()
    {

    }

    public void GameCompleteEvent()
    {

    }

    public void RestartGame()
    {

    }
}
