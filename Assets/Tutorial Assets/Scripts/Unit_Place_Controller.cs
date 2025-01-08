using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Place_Controller : MonoBehaviour
{
    [Header("Unit Prefabs")]
    [SerializeField] GameObject turretPrefab; // Player Turret Prefab to create
    [SerializeField] GameObject wallPrefab; // Player Wall Prefab to create

    [Header("Components")]
    [SerializeField] Transform tower; // Player Tower Transform Component
    private Vector3 towerPosition; // Saved Tower Vector 3 coords
    [SerializeField] Camera mainCam; // Main Camera component
    [SerializeField] BoxCollider detectionBox; // Collider to check if there's space available
    [SerializeField] Transform detectionBoxT; // Transform for detection box

    [Header("Control Variables")]
    [SerializeField] float placementDelay = .1f; // Timed delay between each Player Turret placement
    // [SerializeField] float grid = 1.2f;
    // [SerializeField] bool gridPlacement;
    [SerializeField] bool canCreateTurret; // Check if the player can create a new Turret
    [SerializeField] LayerMask layerMask; // Ghost Movement Layermask
    private enum SelectedUnit { Turret, Wall }
    [SerializeField] SelectedUnit selectedUnit;

    [Header("Ghost Unit Variables")]
    [SerializeField] GameObject ghostUnit;
    MeshFilter ghostMeshFilter;
    MeshRenderer ghostMeshRenderer;
    Material ghostMaterial;
    [SerializeField] Color trueColour = Color.green;
    [SerializeField] Color falseColour = Color.red;

    [SerializeField] float ghostTransparency = 0.5f;

    [System.Serializable] public class UnitsInfo
    {
        [Tooltip("Name of the Unit.")]
        public string unitName;
        [Tooltip("The Mesh of the unit to display.")]
        public Mesh unitMesh;
        [Tooltip("The material of the mesh.")]
        public Material unitMaterial;
        [Tooltip("The x, y and z scale of the collision box of the Unit.")]
        public Vector3 detectBoxScale = new (1,2,1);
    }
    [SerializeField] public List<UnitsInfo> ghostUnitsInfo = new();

    private Quaternion newRotation;

    #region Start
    void Start()
    {
        towerPosition = tower.position;

        ghostUnit = Instantiate(ghostUnit, new Vector3(), Quaternion.identity); // Create Ghost in scene
        ghostMeshFilter = ghostUnit.GetComponent<MeshFilter>();
        MeshRenderer mr = ghostUnit.GetComponent<MeshRenderer>();
        ghostMeshRenderer = mr;
        ghostMaterial = mr.material;
        ghostUnit.SetActive(false);

        falseColour.a = ghostTransparency;
        trueColour.a = ghostTransparency;
        UpdateGhostUnitComponents(0);
    }
    #endregion

    #region Update
    void Update() => InputListener();
    #endregion

    #region Input Listener
    void InputListener()
    {
        if (Input.GetMouseButton(1))
        {
            GhostTurretMovement();
            if (Input.GetMouseButtonDown(0) && canCreateTurret && SufficentMoney()) {CreateTurret();}
        }
        else if (Input.GetMouseButtonUp(1))
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
                Vector3 newPos;
                // Round y to the nearest tenth
                // if (gridPlacement)
                // {
                //     newPos = new(hitPosition.x, Mathf.Round(hitPosition.y * 10) / 10, hitPosition.z);
                //     newPos /= grid;
                //     newPos = new Vector3 (Mathf.Round(newPos.x), newPos.y, Mathf.Round(newPos.z));
                //     newPos *= grid;
                // }
                // else  newPos = new(hitPosition.x, Mathf.Round(hitPosition.y * 10) / 10, hitPosition.z);
                newPos = new(hitPosition.x, Mathf.Round(hitPosition.y * 10) / 10, hitPosition.z);

                // Calulate roation to face away from the Tower
                float rotationY = Quaternion.LookRotation(ghostUnit.transform.position - towerPosition).eulerAngles.y;
                newRotation = Quaternion.Euler(0, rotationY, 0);

                // Set positions and rotations
                ghostUnit.transform.SetPositionAndRotation(newPos, newRotation);
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
        ghostUnit.SetActive(state); // Changed
        detectionBox.enabled = state;
    }
    #endregion

    #region Create Turret
    private void CreateTurret()
    {
        Debug.Log("Turret Created at " + ghostUnit.transform.position);
        canCreateTurret = false;
        GameObject unit = selectedUnit switch
        {
            SelectedUnit.Turret => turretPrefab,
            SelectedUnit.Wall => wallPrefab,
            _ => null,
        };
        Vector3 spawnPosition = ghostUnit.transform.position;
        // if (gridPlacement)
        // {
        //     spawnPosition /= grid;
        //     spawnPosition = new Vector3 (Mathf.Round(spawnPosition.x), spawnPosition.y, Mathf.Round(spawnPosition.z));
        //     spawnPosition *= grid;
        // }
        Instantiate(unit, spawnPosition, newRotation);
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
    public void ColliderTrigger() // Stop the player from placing a unit and enable the 'can't place turret' colour.
    {
        canCreateTurret = false;
        ghostMeshRenderer.material.SetColor("_BaseColor", falseColour);
    }

    public void ColliderTriggerExit() // Allow the player to place a unit and enable the 'can place turret' colour.
    {
        canCreateTurret = true;
        ghostMeshRenderer.material.SetColor("_BaseColor", trueColour);
    }
    #endregion

    public void SelectUnit(int unitNum) // Select Player Unit
    {
        switch (unitNum)
        {
            case 0:
                selectedUnit = SelectedUnit.Turret;
                UpdateGhostUnitComponents(unitNum);
                break;
            case 1:
                selectedUnit = SelectedUnit.Wall;
                UpdateGhostUnitComponents(unitNum);
                break;
        }
    }

    void UpdateGhostUnitComponents(int unitNum)
    {
        ghostMeshFilter.mesh = ghostUnitsInfo[unitNum].unitMesh;
        // Material mat = new (ghostUnitsInfo[unitNum].material);
        // Color col = mat.GetColor("_BaseColor");
        // col.a = ghostTransparency;
        // mat.SetColor("_BaseColor", col);
        // // mat.SetColor("_BaseColor", trueColour);

        ghostMaterial = new (ghostUnitsInfo[unitNum].unitMaterial);
        ghostMaterial.SetColor("_BaseColor", trueColour);
        ghostMeshRenderer.material = ghostMaterial;
        detectionBox.size = ghostUnitsInfo[unitNum].detectBoxScale;
    }
}