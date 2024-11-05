# Creating the Unit Prefab:

### Materials:
First, let's create a material for the Turret Unit using a Shader Graph. The graph will be simple, we're aiming to add a colour and transparency controller.

<img width="2352" alt="Shader Graph" src="https://github.com/user-attachments/assets/5b9cde5d-6645-4691-a634-57c2f5195ae5">

> Notice! If you're using Unity URP, you'll want to make sure you select Create > Shader Graph > URP > Lit Shader Graph

Then, create two new materials with the name "Turret Material", one ending with "(Opaque)" and the second one ending with "(Transparent)" . You can then apply the material shader from the shader graph by selecting **Shader: Shader Graphs/Turret Material Shader URP**.

**Alternatively, you can just create two basic materials and change the surface colour and transparency from there lmao.**

# Placing Units:

### 

Create a new C# script named **Unit_Place_Controller** and add the script to a new GameObject in your scene with name "Player Controls".

**Let's start by adding our variables:**
```cs
[Header("Components")]
[SerializeField] GameObject turretPrefab; // Player Turret Prefab to create
[SerializeField] GameObject ghostTurretPrefab; // Player Ghost Turret Prefab to create
[SerializeField] Transform ghostTurret; // Instantiated Ghost Turret Transform component
[SerializeField] GameObject ghostTurretObj; // Instantiated Ghost Turret GameObject component
[SerializeField] Transform tower; // Player Tower Transform Component
private Vector3 towerPosition; // Saved Tower Vector 3 coords
[SerializeField] Camera mainCam; // Main Camera component
[SerializeField] BoxCollider detectionBox; // Collider to check if there's space available
[SerializeField] Transform detetionBoxT; // Transform for detection box 

[Header("Control Variables")]
[SerializeField] float placementDelay = .1f; // Timed delay between each Player Turret placement
private bool canCreateTurret; // Check if the player can create a new Turret
[SerializeField] LayerMask layerMask; // Ghost Movement Layermask
```

Now, in our **Start()** function, let's save the location of the players tower and create our **Ghost Turret** using **Instantiate()**.
```cs
void Start()
{
	towerPosition = tower.position;
	ghostTurretObj = Instantiate(ghostTurretPrefab, new Vector3(), Quaternion.identity);
	ghostTurretObj.SetActive(false);
	ghostTurret = ghostTurretObj.GetComponent<Transform>();
}
```
This is going to be our 'ghost' indicator, which will show the player where their turret will be placed.


Then, create our **InputListener()** function. Here, we add **'if'** statements to check for User Input. We first want to check if the player is holding the Right Mouse Button, which would correspond to the value of **'1'**. Then, while the player is still holding down the right button, we'll check for the Left Mouse Button input, which corresponds to the value of '**0**'.
```cs
void InputListener()
{
	if (Input.GetMouseButton(1))
	{
		GhostTurretMovement();
		if (Input.GetMouseButtonDown(0) && canCreateTurret && SufficentMoney()) {CreateTurret();}
	}
	else
	{
		ghostTurretObj.SetActive(false);
		detectionBox.enabled = false;
	}
}
```
Make sure this function is being triggered in the **Update()** function!

> **Note:** Be aware of the differences when checking for mouse button inputs. **GetMouseButton()** will check if the mouse button is being held down. While **GetMouseButtonDown()** will return true on the exact frame the mouse button was pressed which will return true on the exact frame the mouse button is released!


Create a new private function named **GhostTurretMovement()**, this is where we demonstrate where the player Turret will be placed by having a **Ghost** be displayed for us.

We're going to use a Raycast to determine where the player can place their units; done by translating the users cursor position in the screen to the game world.
```cs
Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
if (Physics.Raycast(ray, out RaycastHit hit, 100, layerMask))
{
	Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
	
	Vector3 hitPosition = hit.point;
```

If written correctly, the **Debug.DrawRay** function should visualise a red beam being pointing to wherever your cursor is pointing in the world!


We'll then need to check if the users cursor is pointing at the ground by checking for a collider and, to prevent detecting the sides of the ground, add an **Approximately** function based on

```cs
	// Hit Collider and height Approximate check
	if (hit.collider != null && Mathf.Approximately(hitPosition.y, 0.5f))
	{
		// Round y to the nearest tenth
		Vector3 newPos = new(hitPosition.x, Mathf.Round(hitPosition.y * 10) / 10, hitPosition.z);
		
		// Calulate roation to face away from the Tower
		float rotationY = Quaternion.LookRotation(ghostTurret.position - towerPosition).eulerAngles.y;
		Quaternion newRotation = Quaternion.Euler(0, rotationY, 0);
		
		// Set positions and rotations
		ghostTurret.SetPositionAndRotation(newPos, newRotation);
		detectionBoxT.SetPositionAndRotation(newPos, newRotation);
```

> **TIP:** The **Approximately** function can help avoid potential errors caused by floating-point imprecision, especially for positions. For example: creating an if statement where x == 5 requires an exact result, so it won't return true if x = 5.001, whilst checking for an approximate will.

We can then enable or disable the ghost turret and detection box if the raycast has hit the ground or not. Since we use this action repeatedly, we can create a custom function to tidy things up.
```cs
		// Enable objects
		AllowTurretBuild(true);
	}
}
else
{
	AllowTurretBuild(false);
}
```

```cs
void AllowTurretBuild(bool state)
{
	ghostTurretObj.SetActive(state);
	detectionBox.enabled = state;
	canCreateTurret = state;
}
```





At this point, you'll want to add a **Ground** Layer and assign your ground object to this layer.
![[Screenshot 2024-10-21 at 17.07.12.png]]

### Checking if There's Space Available:

![[Screenshot 2024-10-26 at 17.48.18.png]]

Here, we can utilise the **UnityEvent** Variable to trigger an event from any script we assign it. In this case, we're going to use it to trigger the **ColliderTrigger()** function to stop the user from placing any turrets.
```cs
[SerializeField] UnityEvent triggerEvent;
[SerializeField] UnityEvent exitEvent;
  
void OnTriggerStay(Collider other)
{
	triggerEvent.Invoke();
}

void OnTriggerExit(Collider other)
{
	exitEvent.Invoke();
}
```

![[Screenshot 2024-10-27 at 17.46.13.png]]

```cs
public void ColliderTrigger()
{
	canCreateTurret = false;
	ghostMaterial.SetColor("_Color", gMatF);
}
  
public void ColliderTriggerExit()
{
	canCreateTurret = true;
	ghostMaterial.SetColor("_Color", gMatT);
}
```


# Economy:

Right now, the player can add as many turrets as they want. But if we want to stop that, we'll need to implement a simple economy and put a price on each turret placed. 

To do this, let's add a **GameManager** object to the scene and give it a script named "GameManager". Here we'll store how much money the player has in a singleton variable.

> Explain what singletons are (!)

```cs
public static int Money {get ; set;}
```

Now, if we 

# Tower Unit AI: #DoNext

```cs
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
```
## States

```cs
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
%%		case TurretState.Strongest: // Maybe leave this. Could be too complex?
		
		BeginFiring(TurretState.Strongest); %%
		break;
	}
}
```
## Detecting Enemies

```cs
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
		StopCoroutine(currentRoutine); // Stop firing
	}
	targetCount = -1;
	detectedTargets.Remove(other.transform);
}

```
## Facing and Shooting Targets



# Configure Unit UI:



# Unit Selection UI:

To create a unit select box, first, create a new canvas object and attach it as a child under the 'Player Controls' GameObject. Then add a separate new image object as a child object of the canvas and apply these settings:

Then, add two buttons.

> Notice! You might have to import TextMeshPro to do this!
![[Screenshot 2024-10-22 at 22.59.08.png]]

This is what your result should look like:
![[Screenshot 2024-10-26 at 17.45.57.png]]

>Feel free to modify the UI to suit your preferences!





