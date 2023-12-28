using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{   //ok so now I have to hide the UI and disable shooting and such when opening the build menu

    [SerializeField] KeyCode _placeKey = KeyCode.Mouse0;
    [SerializeField] KeyCode _cancelKey = KeyCode.Escape;

    public float minBuildRange;
    public float maxBuildRange;
    
    [SerializeField] Material _ghostMaterial;
    [SerializeField] Material _blockedGhostMaterial;

    [SerializeField] PauseManager _pauseManager;

    [SerializeField] List<TurretInfo> _turretInfos;

    private TurretInfo _selectedTurret;
    private UIVisibilityManager _uIManager;

    private GameObject _ghost;

    private bool _placable;

    private void Start()
    {
        SharedVariables.buildSys = this;

        _uIManager = FindObjectOfType<UIVisibilityManager>();
    }

    private void Update()
    {
        if (_selectedTurret == null || Pause.isPaused)
            return;

        _ghost.transform.position = RayStore.GroundedHitPoint;
        _ghost.transform.LookAt(Vector3.Normalize(-_ghost.transform.position) + _ghost.transform.position);

        if (_placable && Input.GetKeyDown(_placeKey))
        {
            PlaceTurret();
        }

        if (Input.GetKeyDown(_cancelKey))
        {
            DeselectTurret();
        }
    }

    public void SelectTurret(int turretInfoIndex)
    {
        _placable = true;
        SharedVariables.inBuildMode = true;
        
        _selectedTurret = _turretInfos[turretInfoIndex];
        _ghost = Instantiate(_selectedTurret.turretGhost, RayStore.GroundedHitPoint, Quaternion.identity);

        Turret turret = _selectedTurret.turretPrefab.GetComponent<Turret>();

        _ghost.GetComponent<TurretGhost>().RenderTurretRange(turret.maxRange, turret.angleRange * 2f);

        _uIManager.ChangeBuildMode(true);
        _pauseManager.SlowGame();
    }
    public void DeselectTurret()
    {
        Destroy(_ghost, 0f);

        Invoke("ExitBuildMode", 0.1f);
        
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

    void PlaceTurret()
    {
        Instantiate(_selectedTurret.turretPrefab, RayStore.GroundedHitPoint, _ghost.transform.rotation);

        DeselectTurret();
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
    }
}