using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    [Header("Enemy Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float damage = 1;
    [SerializeField] private float attackRate = .5f;
    [SerializeField] private bool canAttackGroups;
    private Quaternion direction;
    private bool attacking;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField]
    enum EnemyState { Moving, Attacking }
    [SerializeField] EnemyState state = EnemyState.Moving;

    [Header("Detection Variables")]
    [SerializeField] Vector3 attackBoxSize = new(1,1,1);
    [SerializeField] Collider[] hitColliders;
    [SerializeField] int prevLength = 0;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform detectBoxPos;
    [SerializeField] private Character_Health_Script hpScript;
    // public Wave_Manager waveManager;
    public Transform tower;

    Coroutine routine;

    void Start()
    {
        if (tower == null) tower = GameObject.FindWithTag("Tower")?.transform; // Keep in Tutorial 3, remove for Tutorial 4.
        direction = Quaternion.LookRotation(tower.position - transform.position);
        transform.rotation = direction;
        rb.velocity = transform.forward * speed;
        hpScript.triggerEvent.AddListener(() => Wave_Manager.instance.RemoveEnemy(gameObject)); // Add for tutorial 4
    }

    private void FixedUpdate()
    {
        AttackBoxDetect();
        if (!attacking) Moving();
    }

    void Moving() => rb.velocity = transform.forward * speed;

    void AttackBoxDetect()
    {
        hitColliders = Physics.OverlapBox(detectBoxPos.position, attackBoxSize / 2, detectBoxPos.rotation, unitLayer);
        int length = hitColliders.Length;
        if (length == 0)
        {
            state = EnemyState.Moving;
            attacking = false;
            prevLength = 0;
            if (routine != null) StopCoroutine(routine);
        }
        else if (length != prevLength)
        {
            state = EnemyState.Attacking;
            prevLength = length;
            attacking = true;
            rb.velocity = new();
            routine = StartCoroutine(Attack());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = detectBoxPos.localToWorldMatrix;
        Gizmos.color = Color.red;
        //Gizmo draw a cube where the OverlapBox is (positioned where your GameObject is as well as the given size)
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }

    IEnumerator Attack()
    {
        List<Character_Health_Script> HSs = null;
        Character_Health_Script HS = null;

        // Select variable based on attack mode
        if (canAttackGroups)
        {
            HSs = new List<Character_Health_Script>();
            foreach (var item in hitColliders)
            {
                var healthScript = item.GetComponent<Character_Health_Script>();
                if (healthScript != null) HSs.Add(healthScript); // Cache non-null scripts
            }
        }
        else
        {
            HS = hitColliders[0]?.GetComponent<Character_Health_Script>();
        }

        // Start attack loop based on attack mode
        if (canAttackGroups)
        {
            while (attacking)
            {
                foreach (var item in HSs)
                {
                    item.Damage(damage);
                }
                yield return new WaitForSeconds(attackRate);
            }
        }
        else
        {
            while (attacking && HS != null)
            {
                HS.Damage(damage);
                yield return new WaitForSeconds(attackRate);
            }
        }
    }

    // public void UpdateWaveManager() => Wave_Manager.instance.RemoveEnemy(gameObject);
}
