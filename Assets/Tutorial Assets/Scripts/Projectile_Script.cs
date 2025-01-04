using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Script : MonoBehaviour
{
    [Header("Projectile Variables")]
    public float speed = 5;
    public float damage = .5f;

    [Header("Components")]
    [SerializeField] Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character_Health_Script health_Script = other.GetComponent<Character_Health_Script>();
        health_Script.Damage(damage);
        Destroy(gameObject);
    }
}