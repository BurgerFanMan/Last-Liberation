using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{   
    public float minBuildRange;
    public float maxBuildRange;

    [SerializeField] GameObject _placeEffect;
    [SerializeField] Material _ghostMaterial;
    [SerializeField] Material _blockedGhostMaterial;

    [SerializeField] PauseManager _pauseManager;

    public List<TurretInfo> turretInfos;


    private TurretInfo _selectedTurret;
    private UIVisibilityManager _uIManager;

    private GameObject _ghost;

    private bool _placable;
    private bool _hasToExitBuildMode;

    private void Start()
    {
        SharedVariables.buildSys = this;

        _uIManager = FindObjectOfType<UIVisibilityManager>();
    }

    private void Update()
    {
        if(_hasToExitBuildMode && Input.GetKeyUp(InputManager.GetValue("turret_place")))
        {
            ExitBuildMode();
        }

        if (_selectedTurret == null || Pause.isPaused)
            return;

        //setting ghost position and rotation
        _ghost.transform.position = RayStore.GroundedHitPoint;

        float distanceSqr = _ghost.transform.position.sqrMagnitude;
        if (distanceSqr < minBuildRange * minBuildRange)
            _ghost.transform.position = _ghost.transform.position.normalized * minBuildRange;
        else if (distanceSqr > maxBuildRange * maxBuildRange)
            _ghost.transform.position = _ghost.transform.position.normalized * maxBuildRange;

        _ghost.transform.LookAt(Vector3.Normalize(-_ghost.transform.position) + _ghost.transform.position);

        if (_placable && Input.GetKeyDown(InputManager.GetValue("turret_place")))
        {
            if(PlaceTurret())
                _hasToExitBuildMode = true;
        }

        if (Input.GetKeyDown(InputManager.GetValue("turret_cancelplace")))
        {
            DeselectTurret();

            ExitBuildMode();
        }
    }

    public void SelectTurret(int turretInfoIndex)
    {
	    if(_selectedTurret != null)
		    DeselectTurret();

        _placable = true;
        SharedVariables.inBuildMode = true;
        
        _selectedTurret = turretInfos[turretInfoIndex];
        _ghost = Instantiate(_selectedTurret.turretGhost, RayStore.GroundedHitPoint, Quaternion.identity);

        Turret turret = _selectedTurret.turretPrefab.GetComponent<Turret>();

        _ghost.GetComponent<TurretGhost>().RenderTurretRange(turret);

        _uIManager.ChangeBuildMode(true);
        _pauseManager.SlowGame();
    }
    public void DeselectTurret()
    {
        Destroy(_ghost, 0f);
        
        _selectedTurret = null;
        _ghost = null;

        _uIManager.ChangeBuildMode(false);
        _pauseManager.UnslowGame();
    }

    public void UnblockPlacement()
    {
        ChangeGhostMaterials(_ghost, _ghostMaterial);

        _placable = true;
    }
    public void BlockPlacement()
    {
        ChangeGhostMaterials(_ghost, _blockedGhostMaterial);

        _placable = false;
    }

    bool PlaceTurret() //returns false if the user is building more turrets
    {
        Turret turret = Instantiate(_selectedTurret.turretPrefab, _ghost.transform.position, _ghost.transform.rotation).GetComponent<Turret>();    

        if(_placeEffect != null)
            Instantiate(_placeEffect, _ghost.transform.position, _ghost.transform.rotation);

        Money.money -= _selectedTurret.cost;

        TurretOverlay overlay = FindObjectOfType<TurretOverlay>();
        if (overlay != null)
            overlay.GenerateOverlay(turret);
        if (SharedVariables.cameraShake != null)
            SharedVariables.cameraShake.ShakeCamera(20f, 0.1f, 1);

        if (!(Input.GetKey(InputManager.GetValue("turret_placemultiple")) && Money.money >= _selectedTurret.cost))
        {
            DeselectTurret();

            return true;
        }

        return false;
    }

    void ChangeGhostMaterials(GameObject go, Material material)
    {
        TurretGhost ghost = go.GetComponent<TurretGhost>();

        foreach(MeshRenderer renderer in ghost.renderers)
        {
            renderer.material = material;
        }
    }
    void ExitBuildMode()
    {
        SharedVariables.inBuildMode = false;

        _hasToExitBuildMode = false;
    }
}