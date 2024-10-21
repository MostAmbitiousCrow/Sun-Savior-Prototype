# Page 1
 

 
First, create a scripts folder and create a new C# script with the name 'Camera_Controller' and attach it to the **Camera Controller** GameObject.

### 1. Getting Our Screen Values

First, we need to determine the current screens width and height if we want to be able to calculate at what zone we want the players cursor to rotate the camera.

We can do this by saving the users screen width and height in pixels and creating detection values to trigger the rotation movement. To keep things simple, I like to get a percentage value of the screens width and target that percentage to be my trigger zone.
```
[Tooltip("Right distance detection percentage")] [Range(0f, 100f)]
[SerializeField] private float percentageHorizontalDetectionR = 85f; // Right distance detection

[Tooltip("Left distance detection percentage")] [Range(0f, 100f)]
[SerializeField] private float percentageHorizontalDetectionL = 15f; // Left distance detection

[Tooltip("Upwards distance detection percentage")] [Range(0f, 100f)]
[SerializeField] private float VerticalDetectionU; // Upwards distance detection

[SerializeField] private float horizontalDetectionL = 0;
[SerializeField] private float horizontalDetectionR = 0;
[SerializeField] private float VerticalDetectionU = 0;
```

```
mainCamera = Camera.main; // Save the Main Camera Component
screenSizeX = Screen.width; // Set the float Width value of the users screen resolution
screenSizeY = Screen.height; // Set the float Height value of the users screen resolution
horizontalDetectionR = screenSizeX * (percentageHorizontalDetectionR / 100); // Set the Right Horizontal Detection
horizontalDetectionL = screenSizeX * (percentageHorizontalDetectionL / 100); // Set the Left Horizontal Detection
```

(Add possible improvement here)

You'll need to toggle **Allow Fullscreen Switch** so the screen width and height values don't get changed by the player too often.
![[Screenshot 2024-10-19 at 12.35.32.png]]
### 2. Camera Movement:
#### Horizontal Movement:

To create our Camera Horizontal Rotation Movement, lets create some variables to assign the speed of the rotation and smoothness. 
```
[SerializeField] private float smoothSpeed = 5; // Smooth speed when the rotation is moving towards its target Y Value
[SerializeField] private float rotationSpeed = 50; // Rotation speed
```

We then need to detect the x value of the mouse position.
```
mousePos = Input.mousePosition;
float posX = mousePos.x;
```
Determine if the mouse has exceeded the detection on the right. Making sure that the targetAngleY variable is set back to 180 as to avoid high values. 
```
if (posX > horizontalDetectionR)
{
	if (targetAngleY < -180)
		targetAngleY = 180;
	targetAngleY -= Time.deltaTime * rotationSpeed;
}
```
Determine if the mouse has exceeded the detection on the left. Making sure that the targetAngleY variable is set back to -180 as to avoid high values.
```
else if (posX < horizontalDetectionL)
{
	if (targetAngleY > 180)
		targetAngleY = -180;
	targetAngleY += Time.deltaTime * rotationSpeed;
}
```

```
Quaternion targetRotation = Quaternion.Euler(0, targetAngleY, 0);
transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
```

Your camera controls should look like something like this:
https://github.com/user-attachments/assets/f1ceeb14-b2d4-45fc-a91b-552eb9502186

#### Birds-Eye View:

Now, if we want to add a birds eye, we'll have to rework how our Horizontal Movement works.
To do this, we're going to save each X and Y angle, X for the vertical rotation and Y for the horizontal rotation, and then apply it to the Transforms target rotation.

We'll first add some new variables, two for the X and Y target angles, and three for the Birds Eye function.
```
[SerializeField] private float targetAngleY; // Target value for horizontal rotation
[SerializeField] private float targetAngleX; // Target value for vertical rotation

[Header("Birds Eye Variables")]
[SerializeField] private bool inBirdsEye;
[SerializeField] private float birdsEyeAngle = 60f;
[SerializeField] private float birdsEyeTime = .3f;
```



To trigger the Birds Eye, we're going to use a coroutine. We can use the Ternary Operator to determine whether or not the timer should count down or count upwards depending on if the user is entering or exiting the Birds Eye view.
```
IEnumerator BirdsEyeTransition(bool entering)
{
	float t = entering ? 0f : 1f;
	float targetRotationX = entering ? birdsEyeAngle : 0f;
```

Now, to to transition the camera, we're going to Lerp the angle towards the birds eye angle variable value in a While statement by dividing Time.deltaTime with birdsEyeTime, then multiplying it either with a positive value if it's entering birds eye view, or a negative if the player is exiting birds eye view.
```
	float initialRotation = transform.rotation.x;
	
	// Smoothly transition to or from the birds-eye view
	while ((entering && t < 1f) || (!entering && t > 0f))
	{
		t += Time.deltaTime / birdsEyeTime * (entering ? 1f : -1f);
		targetAngleX = Mathf.Lerp(initialRotation, birdsEyeAngle, Mathf.Clamp01(t));
		yield return null;
	}
```

Then, we can set the target to the targets X rotation to ensure it's properly set at the end of the transition.
```
	// Ensure the final rotation is set precisely at the end of the transition
	targetAngleX = targetRotationX;
	inBirdsEye = entering;
}
```

Now, let's return back to 

I then prefer to add a section to update the objects rotation. You can delete the rotation update method in HorizontalCameraRotation()
```
// Update the objects rotation
private void UpdateRotation()
{
	HorizontalCameraRotation();
	VerticalCameraRotation();
	Quaternion targetRotation = Quaternion.Euler(targetAngleX, targetAngleY, 0);
	transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
}
```

This should be your final result:
https://github.com/user-attachments/assets/72604429-fcc6-457d-855c-25026723f132

This should be your completed script:

```
using System;
using System.Collections;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera mainCamera;

    [Header("Camera Script Variables")]

    [Tooltip("How fast the camera will move when panning")]
    [SerializeField] private float smoothSpeed = 5;        // Smooth speed when the rotation is moving towards its target Y Value
    [SerializeField] private float rotationSpeed = 50;     // Rotation speed 

    [Tooltip("Right distance detection percentage")] [Range(0f, 100f)]
    [SerializeField] private float percentageHorizontalDetectionR = 85f; // Right distance detection
    [Tooltip("Left distance detection percentage")] [Range(0f, 100f)]
    [SerializeField] private float percentageHorizontalDetectionL = 15f; // Left distance detection
    [Tooltip("Upwards distance detection percentage")] [Range(0f, 100f)]
    [SerializeField] private float percentageVerticalDetectionU = 15f;   // Upwards distance detection

    [SerializeField] private float horizontalDetectionL = 0;
    [SerializeField] private float horizontalDetectionR = 0;
    [SerializeField] private float VerticalDetectionU = 0;
    
    private float screenSizeX; // Screen width value
    private float screenSizeY; // Screen height value
    private Vector2 mousePos; // Users mouse position in pixel coords
    private float targetAngleY; // Target value for horizontal rotation
    private float targetAngleX; // Target value for vertical rotation
    
    [Header("Birds Eye Variables")]
    [SerializeField] private bool inBirdsEye;
    [SerializeField] private float birdsEyeAngle = 60f;
    [SerializeField] private float birdsEyeTime = .3f;

    private Coroutine birdsEyeRoutine;

    #region Start
    void Start()
    {
        mainCamera = Camera.main; // Save the Main Camera Component
        UpdateScreenData();
    }
    #endregion

    #region Update
    void Update()
    {
        UpdateRotation();
    }
    #endregion

    #region Horizontal Cam Rotation
    // Rotates the camera pivot based on the direction of what side the players cursor is aimed at.
    private void HorizontalCameraRotation()
    {
        mousePos = Input.mousePosition;
        float posX = mousePos.x;
        if (posX > horizontalDetectionR)
        {
            if (targetAngleY < -180)
                targetAngleY = 180;
            targetAngleY -= Time.deltaTime * rotationSpeed; 
        }
        else if (posX < horizontalDetectionL)
        {
            if (targetAngleY > 180)
                targetAngleY = -180;
            targetAngleY += Time.deltaTime * rotationSpeed;
        }
    }
    #endregion
    
    #region Vertical Cam Rotation
    private void VerticalCameraRotation()
    {
        float posY = mousePos.y;

        // If we're moving to Bird's Eye view
        if (posY > VerticalDetectionU && !inBirdsEye)
        {
            TransitionToBirdsEye(true);
        }
        // If we're moving out of Bird's Eye view
        else if (posY <= VerticalDetectionU && inBirdsEye)
        {
            TransitionToBirdsEye(false);
        }
    }

    // Method to handle the Birds Eye transition
    private void TransitionToBirdsEye(bool enteringBirdsEye)
    {
        inBirdsEye = enteringBirdsEye;

        if (birdsEyeRoutine != null)
            StopCoroutine(birdsEyeRoutine);

        birdsEyeRoutine = StartCoroutine(BirdsEyeTransition(enteringBirdsEye));
    }

    IEnumerator BirdsEyeTransition(bool entering)
    {
        float t = entering ? 0f : 1f;
        float targetRotationX = entering ? birdsEyeAngle : 0f;

        float initialRotation = transform.rotation.x;

        // Smoothly transition to or from the birds-eye view
        while ((entering && t < 1f) || (!entering && t > 0f))
        {
            t += Time.deltaTime / birdsEyeTime * (entering ? 1f : -1f);
            targetAngleX = Mathf.Lerp(initialRotation, birdsEyeAngle, Mathf.Clamp01(t));
            yield return null;
        }
        // Make sure the final rotation is set properly at the end of the transition
        targetAngleX = targetRotationX;
        inBirdsEye = entering;
    }
    #endregion

    #region Update Rotation
    // Update the objects rotation
    private void UpdateRotation()
    {
        HorizontalCameraRotation();
        VerticalCameraRotation();
        Quaternion targetRotation = Quaternion.Euler(targetAngleX, targetAngleY, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
    #endregion

    #region Update Screen Values
    public void UpdateScreenData()
    {
        screenSizeX = Screen.width;                                                  // Set the float Width value of the users screen resolution
        screenSizeY = Screen.height;                                                 // Set the float Height value of the users screen resolution
        horizontalDetectionR = screenSizeX * (percentageHorizontalDetectionR / 100); // Set the Right Horizontal Detection
        horizontalDetectionL = screenSizeX * (percentageHorizontalDetectionL / 100); // Set the Left Horizontal Detection
        VerticalDetectionU = screenSizeY * (percentageVerticalDetectionU / 100); // Set the Upwards Vertical Detection
    }
    #endregion
}
```

# NOTE: REMOVE CAMERA COMPONENT
