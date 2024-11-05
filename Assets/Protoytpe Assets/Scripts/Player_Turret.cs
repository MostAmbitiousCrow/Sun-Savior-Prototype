using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Turret : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Time each projectile is fired")]
    [SerializeField] private float fireRate = 1;
    [SerializeField] private float turnSpeed = 1;
    [SerializeField] private float damage = 1;

    [Header("Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform body;

    [SerializeField] List<GameObject> cachedProjectiles;

    [Header("Target Info")]
    [SerializeField] private bool emptyTargets = true;
    [SerializeField] Transform currentTarget;
    [SerializeField] List<Transform> detectedTargets;

    public enum TurretState { First, Last, Closest, Strongest }
    [SerializeField] TurretState turretState = TurretState.First;
    private TurretState currentState;

    private Coroutine currentRoutine;
    private int targetCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateState(turretState);
    }

    #region Update Turret State
    public void UpdateState(TurretState state)
    {
        turretState = state;
        if (targetCount == 0) return;
        switch (state)
        {
            case TurretState.First:
                currentTarget = detectedTargets[0];
                BeginFiring(TurretState.First);
                break;
            case TurretState.Last:
                currentTarget = detectedTargets[targetCount - 1];
                BeginFiring(TurretState.Last);
                break;
            case TurretState.Closest:
                currentTarget = FindClosestTarget();
                BeginFiring(TurretState.Closest);
                break;
            case TurretState.Strongest: // Maybe leave this. Could be too complex?

                BeginFiring(TurretState.Strongest);
                break;
        }
    }
    #endregion

    #region Find Closest Target
    // Credit https://discussions.unity.com/t/clean-est-way-to-find-nearest-object-of-many-c/409917
    Transform FindClosestTarget()
    {
        float minDist = Mathf.Infinity;
        Vector3 pos = transform.position;

        Transform target = null;
        foreach (var item in detectedTargets)
        {
            float dist = Vector3.Distance(pos, item.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = item;
            }
        }
        Debug.Log(gameObject.name + "'s Nearest Target is " + target.name);
        return target;
    } // Find closest enemy in list
    #endregion

    #region Begin Firing Routine
    private void BeginFiring(TurretState state)
    {
        if (state == turretState)
        {
            return;
        }
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FiringRoutine());
    }
    #endregion

    #region Firing Coroutine
    private IEnumerator FiringRoutine() // Where the action takes place
    {
        float t = 0;
        Vector3 pos = transform.position;
        Quaternion rot = Quaternion.identity;
        while (!emptyTargets)
        {
            float yRot = Quaternion.LookRotation(currentTarget.position - pos).eulerAngles.y;
            rot = Quaternion.RotateTowards(body.rotation, Quaternion.Euler(0, yRot, 0), turnSpeed);
            body.rotation = rot;

            t += Time.deltaTime;
            if (t > fireRate)
            {
                //Instantiate() // Unfinished
            }

            yield return null;
        }
        yield break;
    }
    #endregion

    #region Trigger Detection Zone
    private void OnTriggerEnter(Collider other)
    {
        if (emptyTargets)
        {
            emptyTargets = false;
            BeginFiring(turretState); // Continue firing
        }
        targetCount += 1;
        detectedTargets.Add(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!emptyTargets)
        {
            emptyTargets = true;
            if (currentRoutine != null) StopCoroutine(currentRoutine); // Stop firing
        }
        targetCount = -1;
        detectedTargets.Remove(other.transform);
    }
    #endregion
}
