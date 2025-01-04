using System.Collections;
using UnityEngine;

public class Player_Turret : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float fireRate = 1; // Time each projectile is fired
    [SerializeField] private float turnSpeed = 1; // Speed in which the turret rotates towards an enemy
    [SerializeField] private float damage = 1; // Damage the turret deals to an enemy
    [SerializeField] private float detectionRadius = 6; // Sphere radius where the turret will detect enemy units
    [SerializeField] private LayerMask enemyLayer; // Enemy collision layer the turret will detect
    [SerializeField] private bool attacking; // Determines if the turret is attacking
    private bool canshoot;

    [Header("Components")]
    [SerializeField] Rigidbody rb; // Roots rigidbody
    [SerializeField] Transform projectileSpawn; // Start location where the projectile will emit (the gun)
    [SerializeField] Transform body; // The turrets body in which the turret will rotate
    [SerializeField] LineRenderer lineRenderer; // Line Renderer component (The Laser)

    [Header("Target Info")]
    [SerializeField] Transform currentTarget; // Current target in which the turret is firing at
    [SerializeField] Collider[] detectedTargets; // List of detected targets in detection radius

    public enum TurretState { First, Last, Closest }
    public TurretState turretState = TurretState.First; // Turrets current state

    private Coroutine routine; // Stored Coroutine
    [SerializeField] int targetCount = 0; // Count of the list of detected targets

    void Start()=> UpdateState(turretState);
    void Update() => DetectEnemies();

    #region Update Turret State
    public void UpdateState(TurretState state)
    {
        if (state == turretState) return;
        turretState = state;
    }
    #endregion

    void GetTarget()
    {
        switch (turretState)
        {
            case TurretState.First:
                currentTarget = detectedTargets[0].transform;
                break;
            case TurretState.Last:
                currentTarget = detectedTargets[targetCount - 1].transform;
                break;
            case TurretState.Closest:
                currentTarget = FindClosestTarget();
                break;
        }
    }

    #region Find Closest Target
    // Credit https://discussions.unity.com/t/clean-est-way-to-find-nearest-object-of-many-c/409917
    Transform FindClosestTarget() // Find closest enemy in list
    {
        float minDist = Mathf.Infinity;
        Vector3 pos = transform.position;

        Transform target = null;
        foreach (var item in detectedTargets)
        {
            Transform itemT = item.transform;
            float dist = Vector3.Distance(pos, itemT.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = itemT;
            }
        }
        Debug.Log(gameObject.name + "'s Nearest Target is " + target.name);
        return target;
    } 
    #endregion

    #region Detect Enemies
    void DetectEnemies()
    {
        detectedTargets = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        targetCount = detectedTargets.Length;
        if (targetCount != 0 && !attacking)
        {
            attacking = true;
            BeginFiring(); // Begin firing
        }
        else if (targetCount == 0)
        {
            attacking = false;
            if (routine != null) StopCoroutine(routine); // Stop firing
        }
    }
    #endregion

    #region Begin Firing Routine
    private void BeginFiring()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FiringRoutine());
        Debug.Log("Began Firing");
    }
    #endregion

    #region Firing Coroutine
    private IEnumerator FiringRoutine() // Where the action takes place
    {
        GetTarget();
        float t = 0;
        Vector3 pos = transform.position;
        Quaternion rot = Quaternion.identity;
        Character_Health_Script eHP = currentTarget.GetComponent<Character_Health_Script>();
        Transform prevTarget = currentTarget;

        while (attacking)
        {
            GetTarget();

            float yRot = Quaternion.LookRotation(currentTarget.position - pos).eulerAngles.y;
            rot = Quaternion.RotateTowards(body.rotation, Quaternion.Euler(0, yRot, 0), turnSpeed);
            body.rotation = rot;
            t += Time.deltaTime;

            if (t > fireRate)
            {
                t = 0;
                canshoot = true;
                if (prevTarget != currentTarget)
                {
                    prevTarget = currentTarget;
                    eHP = currentTarget.GetComponent<Character_Health_Script>();
                }

                lineRenderer.SetPosition(0, projectileSpawn.position);
                lineRenderer.SetPosition(1, new(currentTarget.position.x, projectileSpawn.position.y, currentTarget.position.z));
                eHP.Damage(damage);
                if (canshoot) StartCoroutine(BulletRoutine());
            }
            yield return null;
        }
        yield break;
    }

    IEnumerator BulletRoutine()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(Mathf.Clamp(.1f * fireRate, .01f, .1f));
        lineRenderer.enabled = false;
        canshoot = false;
        yield break;
    }
    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
