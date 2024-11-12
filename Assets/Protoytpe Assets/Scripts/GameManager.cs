using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static int Money {get ; set;}
    public static List<GameObject> activeEnemies;
    
    void Start()
    {
        instance = this;
        Money = 10; // Testing purposes
    }

    public void RemoveMoney()
    {

    }

    public void AddMoney()
    {

    }

    public void GameOverEvent()
    {

    }
}
