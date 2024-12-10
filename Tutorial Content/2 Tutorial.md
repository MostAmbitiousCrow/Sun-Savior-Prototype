# 

# Creating the Unit Prefab:

### Materials:
First, let's create a material for the Turret Unit using a Shader Graph. The graph will be simple, we're aiming to add a colour and transparency controller.

<img width="2352" alt="Shader Graph" src="https://github.com/user-attachments/assets/22704b81-3258-46bc-9377-69880f120b59">

> [!NOTE]
> If you're using Unity URP, you'll want to make sure you select **Create > Shader Graph > URP > Lit Shader Graph**

Then, create two new materials with the name "Turret Material", one ending with "(Opaque)" and the second one ending with "(Transparent)" . You can then apply the material shader from the shader graph by selecting **Shader: Shader Graphs/Turret Material Shader URP**.

> [!IMPORTANT]
> PERSONAL NOTE: Change this to mention how you created two basic materials and changed the surface colour and transparency values.

# Placing Units:

### Adding Variables

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

> [!NOTE]
> Be aware of the differences when checking for mouse button inputs. **GetMouseButton()** will constantly return true if the mouse button is being held down. While **GetMouseButtonDown()** will return true on the exact frame the mouse button is pressed, and **GetMouseButtonUp()** will return true on the exact frame the mouse button is released!


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


We'll then need to check if the users cursor is pointing at the ground by checking for a collider and, to prevent detecting the sides of the ground, add an **Approximately** function when detecting the hitPositions y value to avoid any imprecisions.

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

> [!TIP]
> The **Approximately** function can help avoid potential errors caused by floating-point imprecision, especially for positions. For example: creating an if statement where x == 5 requires an exact result, so it won't return true if x = 5.001, whilst checking for an approximate will.

We can then enable or disable the ghost turret and detection box if the raycast has hit the ground or not.
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
Since we use this action excessively, we can create a custom function to reduce clutter.
```cs
void AllowTurretBuild(bool state)
{
	ghostTurretObj.SetActive(state);
	detectionBox.enabled = state;
	canCreateTurret = state; // Has been removed #Notice
}
```


Now to make sure we detect the ground, and only the ground, you'll want to add a **Ground** Layer. Assign your ground object to this layer after the layer has been added.

<img width="430" alt="Layers" src="https://github.com/user-attachments/assets/22d0ffb4-35d1-4c5e-a7be-d0a8f963fbb0">

### Checking if There's Space Available:

You might have realised that you're able to place units inside one-another. We can fix this by adding a collider to check if the space the player wishes to place a unit isn't occupied by an enemy or player unit.

<img width="432" alt="Screenshot 2024-10-26 at 17 48 18" src="https://github.com/user-attachments/assets/a675d40f-2ded-4366-baba-7861b1c13390">

Here we can utilise the **UnityEvent** Variable to trigger an event from any script we assign it. In this case, we're going to use it to trigger the **ColliderTrigger()** function to stop the user from placing any turrets.
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

<img width="433" alt="Unit Event Inspector View" src="https://github.com/user-attachments/assets/207f52ec-2a34-49bc-a5d1-97e616cce0fe">

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



https://github.com/user-attachments/assets/f5563a62-13ba-495c-bd8a-6128561380d3


# Economy:

Right now, the player can add as many turrets as they want. But if we want to stop that, we'll need to implement a simple economy system and put a price on each turret placed. 

To do this, let's add a **GameManager** object to the scene and give it a brand new script, call it  "GameManager". Here we'll store how much money the player has in a singleton variable.

> [!IMPORTANT]
> Singletons are...

```cs
public static int Money {get ; set;}
```

Create a new Bool Event. Using the GameManagers Money singleton, we can check if the player has enough money to create a new Unit!
```cs
bool SufficentMoney() // Check if the player has enough money
{
	if (GameManager.Money > 0) return true;
	else return false;
}
```

# Selecting and Placing Units

But what if we want to select a unit? Well, we're going to have to change a few things to make things work.

But first
## Unit Selection UI:

To create a unit select box, first, create a new canvas object and attach it as a child under the 'Player Controls' GameObject. Then add a separate new image object as a child object of the canvas and apply these settings:

![Canvas-Image-Options](https://github.com/user-attachments/assets/51be8ea5-6ad8-4c3a-85ed-724040c2f02a)

Then, add two buttons.

> [!NOTE]
> You might have to import TextMeshPro to do this!

<img width="752" alt="Import TMP" src="https://github.com/user-attachments/assets/8fd5c3a7-f759-4bb2-8a93-72b38a49bc32">


This is what your Hierarchy should look like.

<img width="275" alt="Unit Select UI Hierarchy" src="https://github.com/user-attachments/assets/cf05fa1c-4035-4ee7-95a0-5a24dfc0b936">

This is what your UI should, or could, look like:
<img width="819" alt="Screenshot 2024-10-26 at 17 45 57" src="https://github.com/user-attachments/assets/901f8b78-625e-4705-ae18-fab8d915acbe">


Feel free to modify the UI to better suit your preferences!



## Adding a Second Unit:

If we want to add a second Unit for the player to select and place onto the ground. We'll need to rework the script to make it compatible with using multiple Units.

For this segment of the tutorial, I'll just have two player units: A Turret Unit and a Wall Unit.
```cs
[SerializeField] GameObject turretPrefab; // Player Turret Prefab to create
[SerializeField] GameObject wallPrefab; // Player Wall Prefab to create
[SerializeField] GameObject ghostTurretPrefab; // Player Ghost Turret Prefab to create
[SerializeField] GameObject ghostWallPrefab; // Player Ghost Wall Prefab to create // New
  
[SerializeField] Transform ghostTurret; // Instantiated Ghost Turret Transform component
[SerializeField] Transform ghostWall; // Instantiated Ghost Wall Transform component // New

[SerializeField] Transform ghostObject; // Selected Ghost Object to Move.
[SerializeField] GameObject ghostTurretObj; // Instantiated Ghost Turret GameObject component
[SerializeField] GameObject ghostWallObj; // Instantiated Ghost Wall GameObject component
```

We'll also need to create an **enum** variable to state which Unit we've selected.
```cs
private enum SelectedUnit { Turret, Wall }
[SerializeField] SelectedUnit selectedUnit;
```
Since the **ghostObject** variable will be what ghost object we'll be moving around, we'll have to change it to disable the currently selected ghost object.
```cs
void AllowTurretBuild(bool state) // Disable/Enable the current ghost GameObject and the ghost detection box.
{
	ghostObject.gameObject.SetActive(state);
	detectionBox.enabled = state;
}
```

We'll also need to modify how turrets are created. In this case, we'll use the **switch** statement to determine what Unit we've selected and are going to create.
```cs
private void CreateTurret()
{
	Debug.Log("Turret Created at " + ghostObject.position);
	canCreateTurret = false;
	GameObject unit;
	switch (selectedUnit)
	{
		case SelectedUnit.Turret: unit = turretPrefab; break;
		case SelectedUnit.Wall: unit = wallPrefab; break;
		default: unit = null; break;
	}
	Instantiate(unit, ghostObject.position, newRotation);
	StartCoroutine(Cooldown());
}
```

However, we can make this much simpler by assigning the _unit_ value based on the current _selectedUnit_ enum. Rather than assigning the value by checking each case one by one, we’re using a switch expression to handle this in a single line. This works well here because we only need to set the _unit_ variable without any additional logic.
```cs
GameObject unit = selectedUnit switch
{
	SelectedUnit.Turret => turretPrefab,
	SelectedUnit.Wall => wallPrefab,
	_ => null,
};
```



```cs
[SerializeField] Material ghostMaterialT; // Ghost turret material
[SerializeField] Material ghostMaterialW; // Ghost wall material // New
[SerializeField] Color gMatT; // Ghost turret material colour when canCreateTurret is true
[SerializeField] Color gMatW; // Ghost wall material colour when canCreateTurret is true
[SerializeField] Color gMatF; // Ghost material colour when canCreateTurret is false
```

Finally, to select our Unit
```cs
public void SelectUnit(int unitNum)
{
	switch (unitNum)
	{
		case 0:
			selectedUnit = SelectedUnit.Turret;
			ghostObject = ghostTurret;
			break;
		case 1:
			selectedUnit = SelectedUnit.Wall;
			ghostObject = ghostWall;
			break;
	}
}
```

In the Unit Selection UI we created, in the Button Component, add a new **On Click ()** event and assign the **Player Controller** GameObject to it. Then, if you click on the box next to it, you should see the **Unit_Place_Controller** script component. There, click on the **SelectUnit** and enter 0 for the selection of the Turret Unit, and 1 for the Wall Unit.

<img width="423" alt="Unit Select Button - Unity Event" src="https://github.com/user-attachments/assets/a69a2d82-f739-45dd-8f82-b5441a671d6a">

We'll also need to update the material colours, each colour for their representing turrets colour. You'll have to add new 

```cs
public void ColliderTrigger()
{
	canCreateTurret = false;
	ghostMaterialW.color = gMatF;
	ghostMaterialT.color = gMatF;
}
```

```cs
public void ColliderTriggerExit()
{
	canCreateTurret = true;
	ghostMaterialT.color = gMatT;
	ghostMaterialW.color = gMatW;
}
```


[**Next Tutorial:**](3-Tutorial)






# Final Script

```cs
using System.Collections;
using UnityEngine;

public class Unit_Place_Controller : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] GameObject turretPrefab; // Player Turret Prefab to create
	[SerializeField] GameObject wallPrefab; // Player Wall Prefab to create
	[SerializeField] GameObject ghostTurretPrefab; // Player Ghost Turret Prefab to create
	[SerializeField] GameObject ghostWallPrefab; // Player Ghost Wall Prefab to create // New
	
	[SerializeField] Transform ghostTurret; // Instantiated Ghost Turret Transform component
	[SerializeField] Transform ghostWall; // Instantiated Ghost Wall Transform component // New
	
	[SerializeField] Transform ghostObject; // Selected Ghost Object to Move.
	[SerializeField] GameObject ghostTurretObj; // Instantiated Ghost Turret GameObject component
	[SerializeField] GameObject ghostWallObj; // Instantiated Ghost Wall GameObject component
	
	[SerializeField] Transform tower; // Player Tower Transform Component
	private Vector3 towerPosition; // Saved Tower Vector 3 coords
	[SerializeField] Camera mainCam; // Main Camera component
	[SerializeField] BoxCollider detectionBox; // Collider to check if there's space available
	[SerializeField] Transform detectionBoxT; // Transform for detection box
	
	[Header("Control Variables")]
	[SerializeField] float placementDelay = .1f; // Timed delay between each Player Turret placement
	[SerializeField] bool canCreateTurret; // Check if the player can create a new Turret
	[SerializeField] LayerMask layerMask; // Ghost Movement Layermask
	private enum SelectedUnit { Turret, Wall }
	[SerializeField] SelectedUnit selectedUnit;
	
	[Header("Materials")]
	[SerializeField] Material ghostMaterialT; // Ghost turret material
	[SerializeField] Material ghostMaterialW; // Ghost wall material // New
	[SerializeField] Color gMatT; // Ghost turret material colour when canCreateTurret is true
	[SerializeField] Color gMatW; // Ghost wall material colour when canCreateTurret is true
	[SerializeField] Color gMatF; // Ghost material colour when canCreateTurret is false
	private Quaternion newRotation;
	
	
	#region Start
	void Start()
	{
		towerPosition = tower.position;
		ghostTurretObj = Instantiate(ghostTurretPrefab, new Vector3(), Quaternion.identity); // Create Ghost Turret
		ghostTurret = ghostTurretObj.GetComponent<Transform>();
		ghostTurretObj.SetActive(false);
		
		ghostWallObj = Instantiate(ghostWallPrefab, new Vector3(), Quaternion.identity); // Create Ghost Wall
		ghostWall = ghostWallObj.GetComponent<Transform>();
		ghostWallObj.SetActive(false);
		
		ghostObject = ghostTurretObj.transform;
	}
	
	#endregion
	
	#region Update
	
	void Update()
	{
		InputListener();
	}
	
	#endregion
	
	#region Input Listener
	void InputListener()
	{
		if (Input.GetMouseButton(1))
		{
			GhostTurretMovement();
			if (Input.GetMouseButtonDown(0) && canCreateTurret && SufficentMoney()) {CreateTurret();}
			}
		else
		{
		AllowTurretBuild(false);
		ColliderTriggerExit();
		}
	}
	
	#endregion
	
	#region Ghost Turret Movement
	private void GhostTurretMovement()
	{
		Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, 100, layerMask))
		{
			Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
			Vector3 hitPosition = hit.point;
			
			// Hit Collider and height Approximate check
			if (hit.collider != null && Mathf.Approximately(hitPosition.y, 0.5f))
			{
				// Round y to the nearest tenth
				Vector3 newPos = new(hitPosition.x, Mathf.Round(hitPosition.y * 10) / 10, hitPosition.z);
				
				// Calulate roation to face away from the Tower
				float rotationY = Quaternion.LookRotation(ghostObject.position - towerPosition).eulerAngles.y;
				newRotation = Quaternion.Euler(0, rotationY, 0);
				
				// Set positions and rotations
				ghostObject.SetPositionAndRotation(newPos, newRotation);
				detectionBoxT.SetPositionAndRotation(newPos, newRotation);
				
				// Enable objects
				AllowTurretBuild(true);
			}
			else
			{
			AllowTurretBuild(false);
			}
			}
		else
		{
			AllowTurretBuild(false);
		}
		}
	
	void AllowTurretBuild(bool state)
	{
		ghostObject.gameObject.SetActive(state); // Changed
		detectionBox.enabled = state;
	}
	#endregion
	
	#region Create Turret
	private void CreateTurret()
	{
		Debug.Log("Turret Created at " + ghostObject.position);
		canCreateTurret = false;
		GameObject unit = selectedUnit switch
		{
			SelectedUnit.Turret => turretPrefab,
			SelectedUnit.Wall => wallPrefab,
			_ => null,
		};
	
		Instantiate(unit, ghostObject.position, newRotation);
		StartCoroutine(Cooldown());
	}
	#endregion 
	
	#region Cooldown
	IEnumerator Cooldown()
	{
		yield return new WaitForSeconds(placementDelay);
		canCreateTurret = true;
		yield break;
	}
	#endregion
	
	bool SufficentMoney() // Check if the player has enough money
	{
		if (GameManager.Money > 0) return true;
		else return false;
	}
	
	#region Collider Functions
	public void ColliderTrigger()
	{
		canCreateTurret = false;
		ghostMaterialW.color = gMatF;
		ghostMaterialT.color = gMatF;
	}
	
	public void ColliderTriggerExit()
	{
		canCreateTurret = true;
		ghostMaterialT.color = gMatT;
		ghostMaterialW.color = gMatW;
	}
	#endregion

	// New
	public void SelectUnit(int unitNum) // Select Player Unit
	{
		switch (unitNum)
		{
			case 0:
			selectedUnit = SelectedUnit.Turret;
			ghostObject = ghostTurret;
			break;
			case 1:
			selectedUnit = SelectedUnit.Wall;
			ghostObject = ghostWall;
			break;
		}
	}
}
```
