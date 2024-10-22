using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Unit_Place_Controller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject turretPrefab;
    [SerializeField] GameObject ghostTurretPrefab;
    [SerializeField] Transform ghostTurret;
    [SerializeField] GameObject ghostTurretObj;
    [SerializeField] Transform tower;
    private Vector3 towerPosition;
    [SerializeField] Camera mainCam;

    [Header("Control Variables")]
    [SerializeField] float placementDelay = .1f;
    private bool canCreateTurret;
    [SerializeField] LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        towerPosition = tower.position;
        ghostTurretObj = Instantiate(ghostTurretPrefab, new Vector3(), Quaternion.identity);
        ghostTurretObj.SetActive(false);
        ghostTurret = ghostTurretObj.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        InputListener();
    }

    void InputListener()
    {
        if (Input.GetMouseButton(1))
        {
            GhostTurretMovement();
            if (Input.GetMouseButtonDown(0) && canCreateTurret) {CreateTurret();}
        }
        else ghostTurretObj.SetActive(false);
    }

    private void GhostTurretMovement()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 100, layerMask);
        Debug.DrawRay(ray.origin , ray.direction * 100, Color.red);

        if (hit.collider != null && hit.point.y == .5)
        {
            Debug.Log(hit.point);
            Vector3 HP = hit.point;
            Vector3 newPos = new(HP.x, Mathf.Round(HP.y * 10) / 10, HP.z);

            ghostTurret.position = newPos;
            ghostTurretObj.SetActive(true);
        }
    }

    private void CreateTurret()
    {
        Debug.Log("Turret Created at " + ghostTurret.position);
        canCreateTurret = false;
        Instantiate(turretPrefab, ghostTurret.position, Quaternion.identity);
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(placementDelay);
        canCreateTurret = true;
        yield break;
    }
}
