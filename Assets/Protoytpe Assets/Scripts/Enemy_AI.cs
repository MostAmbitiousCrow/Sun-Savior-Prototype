using System.Collections;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    [Header("Enemy Variables")]
    [SerializeField] private float speed;
    public Transform tower;
    private Quaternion direction;
    private bool attacking;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask unitLayer;
    [SerializeField]
    enum EnemyState { Moving, Attacking }
    [SerializeField] EnemyState state = EnemyState.Moving;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    Coroutine routine;

    // Start is called before the first frame update
    void Start()
    {
        tower = GameObject.FindWithTag("Tower").transform;
        direction = Quaternion.LookRotation(transform.position - tower.position);
        transform.rotation = direction;
        rb.velocity = transform.forward * -speed;
    }

    private void FixedUpdate()
    {
        Raycast();
        if (!attacking) Moving();
    }

    void Raycast()
    {
        Vector3 direction = transform.forward;
        Debug.DrawRay(transform.position, direction * rayDistance, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayDistance, unitLayer))
        {
            attacking = true;
            routine = StartCoroutine(Attack(hit.transform));
        }
        else
        {
            attacking = false;
            StopCoroutine(routine);
        }
    }

    void Moving()
    {
        rb.velocity = transform.forward * -speed;
    }

    IEnumerator Attack(Transform target)
    {
        while (true)
        {
            
        }
        yield break;
    }
}
