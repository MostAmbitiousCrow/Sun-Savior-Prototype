using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    [Header("Enemy Variables")]
    [SerializeField] private float speed;
    public Transform tower;
    private Quaternion direction;
    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        tower = GameObject.FindWithTag("Tower").transform;
        direction = Quaternion.LookRotation(transform.position - tower.position);
        //direction.y *= -1;
        transform.rotation = direction;
        rb.velocity = transform.forward * -speed;
    }
}
