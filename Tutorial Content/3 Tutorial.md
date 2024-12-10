# Setup:

#Note Enemy Collider was changed to exclude other Enemy Colliders.

# Character Health:

When we implement our player unit and enemy characters, we need a way to store their health values and a way to increase and decrease it. Here, we'll create a **Character Health Script** which we'll assign to our player units and the enemy prefab.

This script will contain variables that contain the characters **Max Health**, current health **(Health)** and a trigger event in case we want to trigger an event
```cs
[Header("Health Variables")]
public float maxHealth = 5;
public float health = 0;
public UnityEvent triggerEvent; // New

void Start() => health = maxHealth;

public void Damage(float damage)
{
	health -= damage;
	if (health < 0) // If health is depleted, trigger the event and destroy
	{
		triggerEvent.Invoke(); // New
		Destroy(gameObject);
	}
}

public void Heal(float heal) => health += heal;
```
Apply the script to the root object of your Player Turret, Player Wall, Enemy and Tower.

> [!TIP]
> Whenever you have a function that only does one thing, you can add a **=>** at the end of the function. This (=>) is know as an *Expression*.

# Player Unit AI:

Let's start by adding our variables:
```cs
[Header("Stats")]
[SerializeField] private float fireRate = 1; // Time each projectile is fired
[SerializeField] private float turnSpeed = 1; // Speed in which the turret rotates towards an enemy
[SerializeField] private float damage = 1; // Damage the turret deals to an enemy
[SerializeField] private float detectionRadius = 6; // Sphere radius where the turret will detect enemy units
[SerializeField] private LayerMask enemyLayer; // Enemy collision layer the turret will detect
[SerializeField] private bool attacking; // Determines if the turret is attacking

[Header("Components")]
[SerializeField] Rigidbody rb; // Roots rigidbody
[SerializeField] Transform projectileSpawn; // Start location where the projectile will emit (the gun)
[SerializeField] Transform body; // The turrets body in which the turret will rotate
[SerializeField] LineRenderer lineRenderer; // Line Renderer component (The Laser)

[Header("Target Info")]
[SerializeField] Transform currentTarget; // Current target in which the turret is firing at
[SerializeField] Collider[] detectedTargets; // List of detected targets in detection radius

public enum TurretState { First, Last, Closest }
[SerializeField] TurretState turretState = TurretState.First; // turrets current state

private Coroutine routine; // Stored Coroutine
[SerializeField] int targetCount = 0; // Count of the list of detected targets
```
## Turret States

Create a new public function named **UpdateState** and assign it a TurretState argument. Our aim here is to to create an event to easily change 
```cs
public void UpdateState(TurretState state)
{
	if (state == turretState) return;
	turretState = state;
}
```

## Picking a Target, Based on the Current State:

Based on our three states, each state is going to target an enemy based on certain conditions. In this tutorial, we're going to do three states: First (selecting the first enemy that comes into range), Last (selecting the last enemy that is in range) and Closest (selecting the closest enemy in range, based off the distance from it to the turret).

```cs
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
```

As for finding the closest target, we'll have to create a separate function to return the Transform component of the closest enemy.
```cs
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
```
## Detecting Enemies

Create a new function and call this **DetectEnemies()**.

In this function we're going to approach detecting enemies using **Physics.OverlapSphere**. This will be used to obtain an array of enemies that entire the towers detection radius.
```cs
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
```

If you check in play mode, you'll notice the Sphere won't be visible. This is because it won't typically appear in the Scene View or the Game. We'll have to use **Gizmos** to visualise it for us:
```cs
void OnDrawGizmos()
{
	Gizmos.color = Color.green;
	Gizmos.DrawWireSphere(transform.position, detectionRadius);
}
```

> [!TIP]
> If you're ever unsure whether or not to use Overlap checks in your projects; here's a simple breakdown of comparisons between using Overlap and colliders:

| **Feature**              | Physics.Overlap...                       | **Sphere Collider**                             |
| ------------------------ | ---------------------------------------- | ----------------------------------------------- |
| **Dynamic checks**       | Ideal for on-demand, momentary queries.  | Requires enabling/disabling collider as needed. |
| **Physics interactions** | No collision or trigger events.          | Triggers collision and trigger events.          |
| **Performance**          | Lightweight for one-time checks.         | Slightly heavier due to simulation overhead.    |
| **Ease of use**          | Code-only, no GameObject setup required. | Requires setup and management in the scene.     |


Add the **DetectEnemies()** function to your **Update()** function.

```cs
void Update() => DetectEnemies();
```
> [!TIP]
> Whenever you have a function that simple does one thing, you can add a **=>** at the end of the function. So, for example, you can use this in your update functi1on only trigger the **DetectEnemies()** function, since this will be the only thing being triggered.
> 
> This (=>) is know as an *Expression*.

## Facing and Shooting Targets

#Note Make sure to mention to untick 'Use World Space' in the Line Renderer.

As part of this tutorial, the player turret will fire lasers at its target. This process will take place in a coroutine.
```cs
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
```

Now, to get the turret to fire at the enemy over a constant sequence of time we need to utilise **Time.deltaTime** to count upwards, then check if that time value has reached over fire rate value; thus triggering the projectile sequence.

We'll also be triggering the damage function on the enemy's **Character_Health_Script** to damage them.
```cs
		t += Time.deltaTime;
		if (t > fireRate)
		{
			t = 0;
			if (prevTarget != currentTarget)
			{
				prevTarget = currentTarget;
				eHP = currentTarget.GetComponent<Character_Health_Script>();
			} 
```

Update the line renderers positions from the projectile spawn position to the current targets position.
```cs
			lineRenderer.SetPosition(0, projectileSpawn.position);
			lineRenderer.SetPosition(1, new(currentTarget.position.x, projectileSpawn.position.y, currentTarget.position.z));
```

Trigger the damage event from the target enemies script with the turrets damage value. Then trigger a new Coroutine named **BulletRoutine**; this will be where we'll control the timing of the renderers visibility.

```cs
			eHP.Damage(damage);
			StartCoroutine(BulletRoutine());
		}
		yield return null;
	}
	yield break;
}
```

In our **BulletRoutine()** we'll calculate how fast the Liner Renderer component should be active by multiplying the fireRate value by 0.1 for a quick laser visual. We'll then use **Mathf.Clamp** limit the fire rate to 0.01. This is to simply make adjustments for the visual aspect of the turret so it doesn't look like it's skipping bullets when firing at an incredibly fast rate.

```cs
IEnumerator BulletRoutine()
{
	lineRenderer.enabled = true;
	yield return new WaitForSeconds(Mathf.Clamp(.1f * fireRate, .01f, .1f));
	lineRenderer.enabled = false;
	yield break;
}
```
# Enemy Unit Setup:


## Enemy Unit AI:

```cs
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
```
## Start Function

```cs
void Start()
{
	tower = GameObject.FindWithTag("Tower").transform;
	direction = Quaternion.LookRotation(transform.position - tower.position);
	transform.rotation = direction;
	rb.velocity = transform.forward * -speed;
}
```
## Moving Towards the Tower:

```cs
private void FixedUpdate()
{
	AttackBoxDetect();
	if (!attacking) Moving();
}
```

## Attacking Player Units:

As done for the Player Unit detection zone, we're going to use **Physics.OverlapBox** to create an invisible cube to detect player units. 
```cs
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
```

We can these use Gizmos to visualise the detection box in front of the. However, it's worth noting that DrawWire won't allow us to rotate the wire. Instead, we'll have to set the set the detect box's local position to World Matrix using **Gizmos.matrix**. Gizmos.matrix is the position, rotation and scale of the Gizmos.
```cs
void OnDrawGizmos()
{
	Gizmos.matrix = detectBoxPos.localToWorldMatrix;
	Gizmos.color = Color.red;
	//Gizmo draw a cube where the OverlapBox is (positioned where your GameObject is as well as the given size)
	Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
}
```

```cs
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
```

# Configure Unit State UI: TO DO????


# Final Scripts
## Character Health Script:

```cs
using UnityEngine;
using UnityEngine.Events;

public class Character_Health_Script : MonoBehaviour
{
	[Header("Health Variables")]
	public float maxHealth = 5;
	public float health = 0;
	public UnityEvent triggerEvent; // New
	
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
			triggerEvent.Invoke(); // New
			Destroy(gameObject);
		}
	}
	
	public void Heal(float heal)
	{
		health += heal;
	}
}
```

## Player Unit AI

```cs
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

```

## Enemy AI

```cs
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
                }
            }
            yield return new WaitForSeconds(attackRate);
        }
        yield break;
    }
}

```

- [] Remember to mention the UpdateWaveManager() function on the Enemy_AI script.
