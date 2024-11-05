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
    [SerializeField] Material ghostMaterialW; // Ghost wall material    // New
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
        // Removed
    }
    #endregion

    #region Create Turret
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
        //ghostMaterial.SetColor("_Color", gMatF); // Changed
        ghostMaterialW.color = gMatF; // New
        ghostMaterialT.color = gMatF; // New
    }

    public void ColliderTriggerExit()
    {
        canCreateTurret = true;
        //ghostMaterial.SetColor("_Color", gMatT); // Changed
        ghostMaterialT.color = gMatT; // New
        ghostMaterialW.color = gMatW; // New
    }
    #endregion

    // New
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
}