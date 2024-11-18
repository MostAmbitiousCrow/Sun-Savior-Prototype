using UnityEngine;
using UnityEngine.Events;

public class Character_Health_Script : MonoBehaviour
{
    [Header("Health Variables")]
    public float maxHealth = 5;
    public float health = 0;
    public UnityEvent triggerEvent; // New

    void Start() => health = maxHealth;

    public void Damage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            triggerEvent.Invoke(); // New
            Destroy(gameObject);
        }
    }

    public void Heal(float heal) => health += heal;
}
