using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static int Money {get ; set;}
    
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
}
