using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera mainCamera;

    [Header("Camera Script Variables")]

    [Tooltip("How fast the camera will move when panning")]
    [SerializeField] private float panSpeed = 10;        // How fast the camera will move when panning

    [Tooltip("Right distance detection")]
    [SerializeField] private float HorizontalDetectionR; // Right distance detection
    [Tooltip("Left distance detection")]
    [SerializeField] private float HorizontalDetectionL; // Left distance detection
    [Tooltip("Upwards distance detection")]
    [SerializeField] private float VerticalDetectionU;   // Upwards distance detection
    
    private float screenSizeX;
    private float screenSizeY;

    #region Start
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;                                  // Save the Main Camera Component
        screenSizeX = Screen.width;                                // Set the float Width value of the users screen resolution
        screenSizeY = Screen.height;                               // Set the float Height value of the users screen resolution
        HorizontalDetectionR = screenSizeX - HorizontalDetectionR; // Set the Right Horizontal Detection
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
