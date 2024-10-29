using System;
using System.Collections;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Camera Script Variables")]

    [Tooltip("How fast the camera will move when panning")]
    [SerializeField] private float smoothSpeed = 5;        // Horizontal smooth speed when the rotation is moving towards its target Y Value
    [SerializeField] private float rotationSpeed = 50;     // Horizontal rotation speed 

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
    [SerializeField] private bool inBirdsEye; // True if birds eye is active
    [SerializeField] private bool birdsEyeLocked; // True if birds eye rotation is locked
    [SerializeField] private float birdsEyeAngle = 60f; // The X rotation of the birds eye
    [SerializeField] private float birdsEyeTime = .3f; // Time to transition into birds eye

    private Coroutine birdsEyeRoutine;

    #region Start
    void Start()
    {
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
        else if (posY <= VerticalDetectionU && inBirdsEye && !birdsEyeLocked)
        {
            TransitionToBirdsEye(false);
        }
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