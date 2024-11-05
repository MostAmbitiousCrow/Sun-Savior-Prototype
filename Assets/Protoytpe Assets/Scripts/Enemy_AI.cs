using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    [Header("Enemy Variables")]
    [SerializeField] private float speed;
    public Transform tower;
    private Quaternion direction;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask unitLayer;
    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        tower = GameObject.FindWithTag("Tower").transform;
        direction = Quaternion.LookRotation(transform.position - tower.position);
        //direction = Quaternion.Euler(0, direction.y, 0);
        //direction.y *= -1;
        transform.rotation = direction;
        rb.velocity = transform.forward * -speed;
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 direction = transform.forward;
        Debug.DrawRay(transform.position, direction * rayDistance, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, unitLayer))
        {
            
        }
    }
}
