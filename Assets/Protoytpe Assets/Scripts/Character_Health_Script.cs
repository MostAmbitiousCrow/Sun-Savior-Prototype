using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Health_Script : MonoBehaviour
{
    [Header("Health Variables")]
    public float maxHealth = 5;
    public float health = 0;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Damage(.5f);
        }
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Heal(float heal)
    {
        health += heal;
    }
}
