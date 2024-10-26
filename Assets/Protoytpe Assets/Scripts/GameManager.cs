using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static int Money {get ; set;}
    
    void Start()
    {
        instance = this;
    }

    public void RemoveMoney()
    {

    }

    public void AddMoney()
    {
        
    }
}
