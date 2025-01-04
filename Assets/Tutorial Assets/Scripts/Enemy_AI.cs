using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_AI : MonoBehaviour
{
    [Header("Enemy Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float damage = 1;
    [SerializeField] private float attackRate = .5f;
    private Quaternion direction;
    private bool attacking;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField]
    enum EnemyState { Moving, Attacking }
    [SerializeField] EnemyState state = EnemyState.Moving;

    [Header("Attack Variables")]
    [SerializeField] Vector3 attackBoxSize = new(1,1,1);
    [SerializeField] Collider[] hitColliders;
    [SerializeField] int prevLength = 0;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform detectBoxPos;
    [SerializeField] private Character_Health_Script hpScript;
    public Wave_Manager waveManager;
    public Transform tower;

    Coroutine routine;

    void Start()
    {
        // tower = GameObject.FindWithTag("Tower").transform;
        direction = Quaternion.LookRotation(transform.position - tower.position);
        transform.rotation = direction;
        rb.velocity = transform.forward * -speed;
        hpScript.triggerEvent.AddListener(() => waveManager.RemoveEnemy(gameObject));
    }

    private void FixedUpdate()
    {
        AttackBoxDetect();
        if (!attacking) Moving();
    }

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

    void OnDrawGizmos()
    {
        Gizmos.matrix = detectBoxPos.localToWorldMatrix;
        Gizmos.color = Color.red;
        //Gizmo draw a cube where the OverlapBox is (positioned where your GameObject is as well as the given size)
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }

    void Moving()
    {
        rb.velocity = -transform.forward * speed;
    }

    IEnumerator Attack()
    {
        List<Character_Health_Script> HSs = new();
        foreach (var item in hitColliders) HSs.Add(item.GetComponent<Character_Health_Script>());

        while (attacking)
        {
            foreach (var item in HSs)
            {
                if (item != null) 
                {
                    item.Damage(damage);
                    // Debug.Log(gameObject.name + " Damaged " + item.name);
                }
            }
            yield return new WaitForSeconds(attackRate);
        }
        yield break;
    }

    public void UpdateWaveManager() => waveManager.RemoveEnemy(gameObject);
}
