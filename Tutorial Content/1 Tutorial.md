First, create a scripts folder and create a new C# script with the name 'Camera_Controller' and attach it to the **Camera Controller** GameObject.

### 1. Getting Our Screen Values

First, we need to determine the current screens width and height if we want to be able to calculate at what zone we want the players cursor to rotate the camera.

We can do this by saving the users screen width and height in pixels and creating detection values to trigger the rotation movement. To keep things simple, I like to get a percentage value of the screens width and target that percentage to be my trigger zone.
```cs
[Tooltip("Right distance detection percentage")] [Range(0f, 100f)]
[SerializeField] private float percentageHorizontalDetectionR = 85f; // Right distance detection

[Tooltip("Left distance detection percentage")] [Range(0f, 100f)]
[SerializeField] private float percentageHorizontalDetectionL = 15f; // Left distance detection

[Tooltip("Upwards distance detection percentage")] [Range(0f, 100f)]
[SerializeField] private float VerticalDetectionU; // Upwards distance detection

[SerializeField] private float horizontalDetectionL = 0;
[SerializeField] private float horizontalDetectionR = 0;
[SerializeField] private float VerticalDetectionU = 0;

private float screenSizeX; // Screen width value
private float screenSizeY; // Screen height value
private Vector2 mousePos; // Users mouse position in pixel coords
private float targetAngleY; // Target value for horizontal rotation
private float targetAngleX; // Target value for vertical rotation
```

>**TIP:** Using [Tooltip()] will give your variable a description, viewable in the inspector by hovering your cursor over the name of the variable, based on a string input. While not always necessary, it can help you remember what variables do without having to open your scripts to find out!

>**TIP:** Using [Range()] will restrict your variable value from the smallest value (left), to the highest value (right). It will also show the value as a slider in the inspector, perfect for easy value changes!

Let's begin by setting our key screen size variables in a new public custom Function named **UpdateScreenData**, and trigger this in the **Start()** function. We'll do this by getting the screen width and height and calculating the percentage values for our Horizontal and Vertical trigger zones.
```cs
screenSizeX = Screen.width; // Set the float Width value of the users screen resolution
screenSizeY = Screen.height; // Set the float Height value of the users screen resolution
horizontalDetectionR = screenSizeX * (percentageHorizontalDetectionR / 100); // Set the Right Horizontal Detection
horizontalDetectionL = screenSizeX * (percentageHorizontalDetectionL / 100); // Set the Left Horizontal Detection
VerticalDetectionU = screenSizeY * (percentageVerticalDetectionU / 100); // Set the Upwards Vertical Detection
```
These trigger zones will represent what area of our screen, in pixels, we can start moving the camera.

**(Add possible improvement here. Mainly to consider screen resolution changes during playtime)**

You'll need to toggle **Allow Fullscreen Switch** as to prevent the user from changing the screen width and height values during their playthrough.

<img width="537" alt="Disable Fullscreen Switch" src="https://github.com/user-attachments/assets/f7e7b116-39d8-457e-b88f-96d6274492c7">

### 2. Camera Movement:
#### Horizontal Movement:

To create our Camera Horizontal Rotation Movement, lets create some variables to assign the speed of the rotation and smoothness. 
```cs
[SerializeField] private float smoothSpeed = 5; // Horizontal smooth speed when the rotation is moving towards its target Y Value
[SerializeField] private float rotationSpeed = 50; // Horizontal rotation speed
```

Then, create a new Function named **HorizontalCameraRotation**. We need to first detect the x value of the mouse position.
```cs
mousePos = Input.mousePosition;
float posX = mousePos.x;
```

With that value, we determine if the mouse has exceeded the detection on the right. Making sure that the **targetAngleY** variable is set back to 180 as to avoid high values.
```cs
if (posX > horizontalDetectionR)
{
	if (targetAngleY < -180)
		targetAngleY = 180;
	targetAngleY -= Time.deltaTime * rotationSpeed;
}
```
We're changing the Y value because, in a 3D space, the Y rotation would rotate the object by it's **Yaw**. Here is a diagram that visualises this:
![Yaw, Roll and Pitch Diagram](https://github.com/user-attachments/assets/cd209171-a0aa-4ef0-844d-95cc8126887b)


Again, we now determine if the mouse has exceeded the detection on the left. Making sure that the **targetAngleY** variable is set back to -180 as to avoid high values.
```cs
else if (posX < horizontalDetectionL)
{
	if (targetAngleY > 180)
		targetAngleY = -180;
	targetAngleY += Time.deltaTime * rotationSpeed;
}
```


Finally, create another Function named **UpdateRotation**, this is where we'll detect, calculate and apply our Horizontal rotation.
```cs
HorizontalCameraRotation();
Quaternion targetRotation = Quaternion.Euler(targetAngleX, targetAngleY, 0);
transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
```

#### Your camera controls should look like something like this: 

https://github.com/user-attachments/assets/7861f3da-40d6-4e90-95b4-f03c1aa0400e


#### Vertical Movement (Birds-Eye View):

To add our Birds Eye View, we're going to modify our Y target angle variable and apply that to the target rotation.

Let's first create four more variables for the Birds Eye View function.
```cs
[Header("Birds Eye Variables")]
[SerializeField] private bool inBirdsEye; // True if birds eye is active
[SerializeField] private bool birdsEyeLocked; // True if birds eye rotation is locked
[SerializeField] private float birdsEyeAngle = 60f; // The X rotation of the birds eye
[SerializeField] private float birdsEyeTime = .3f; // Time to transition into birds eye
```

Create a new function named **VerticalCameraRotation** and add this to the **UpdateRotation()** method.

Now, let's repeat what we did with the Horizontal Camera movement, but this time, we're checking for the users **Y** axis on their cursor and changing the X rotation of the pivot.
```cs
float posY = mousePos.y;

// If we're moving to Bird's Eye view
if (posY > VerticalDetectionU && !inBirdsEye)
{
TransitionToBirdsEye(true);
}
// If we're moving out of Bird's Eye view
else if (posY <= VerticalDetectionU && inBirdsEye && !birdsEyeLocked)
{
TransitionToBirdsEye(false);
}
```

To trigger the Birds Eye transition, we're going to use a coroutine. We can use the Ternary Operator to determine whether or not the timer should count down or count upwards, depending on if the user is entering or exiting the Birds Eye view.
```cs
IEnumerator BirdsEyeTransition(bool entering)
{
	float t = entering ? 0f : 1f;
	float targetRotationX = entering ? birdsEyeAngle : 0f;
```

Now, to to transition the camera, we're going to Lerp the angle towards the birds eye angle variable value in a While statement by dividing Time.deltaTime with birdsEyeTime, then multiplying it either with a positive value if it's entering birds eye view, or a negative if the player is exiting birds eye view.
```cs
	float initialRotation = transform.rotation.x;
	
	// Smoothly transition to or from the birds-eye view
	while ((entering && t < 1f) || (!entering && t > 0f))
	{
		t += Time.deltaTime / birdsEyeTime * (entering ? 1f : -1f);
		targetAngleX = Mathf.Lerp(initialRotation, birdsEyeAngle, Mathf.Clamp01(t));
		yield return null;
	}
```

>**TIP:** When using a **while** statement, make sure to always end it with **yield return null**. This will repeat the execution every frame until the condition is met. If not used, Unity will freeze and crash, because you'll be trying to call an infinite amount of executions with no frame to render afterwards.

Then, we can set the target to the targets X rotation to ensure it's properly set at the end of the transition.
```cs
	// Ensure the final rotation is set precisely at the end of the transition
	targetAngleX = targetRotationX;
	inBirdsEye = entering;
}
```

You can now add the **VerticalCameraRotation** function to the **UpdateRotation** function!
```cs
// Update the objects rotation
private void UpdateRotation()
{
	HorizontalCameraRotation();
	VerticalCameraRotation();
	Quaternion targetRotation = Quaternion.Euler(targetAngleX, targetAngleY, 0);
	transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
}
```

>**Let's test!**




When testing, you might notice that, yes, the birds eye transition does work, but only remains there while your cursor stays within the trigger area.

To fix this, we're going to add a **Lock** feature, which will allow the user to toggle lock the cameras birds eye rotation, and allow them to move their cursor freely.
```cs
if (inBirdsEye)
{
	if (!birdsEyeLocked && Input.GetMouseButtonDown(2))
	{
	birdsEyeLocked = true;
	}
	else if (Input.GetMouseButtonDown(2))
	{
	birdsEyeLocked = false;
	}
}
```

This should be your final result:

https://github.com/user-attachments/assets/cf3abfa1-c36e-4bd7-8ede-37ddb70404f6


# Final Script

