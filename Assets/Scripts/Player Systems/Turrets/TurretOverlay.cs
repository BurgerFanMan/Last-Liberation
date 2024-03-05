using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretOverlay : MonoBehaviour
{
    public GameObject rangeRendererPrefab;
    public float overlayOpenTime = 1f;
    public Vector3 overlayOffset = new Vector3(0f, 40f, 0f);
    public string turretTag = "Turret";

    private Turret _hoveredTurret = null;
    private Transform _canvas;
    private Transform _turretOverlay;
    private List<GameObject> _rangeRenderers = new List<GameObject>();

    private float _timeDone = 0f;
    private float _transparency;

    private bool _allOverlaysOpen = false;

    private void Start()
    {
        _canvas = FindObjectOfType<Canvas>().transform;
    }

    void Update()
    {
        _transparency = 40f;

        if (Input.GetKeyDown(InputManager.GetValue("turret_togglerangevisibility")))
        {
            if (_allOverlaysOpen)
                _allOverlaysOpen = false;
            else
            {
                _allOverlaysOpen = true;

                Turret[] turrets = FindObjectsOfType<Turret>();
                _transparency /= turrets.Length;

                for (int i = 0; i < turrets.Length; i++)
                {
                    GenerateOverlay(turrets[i]);
                }
            }
        }

        if (_allOverlaysOpen)
            return;

        if (_turretOverlay != null)
            _turretOverlay.position = Camera.main.WorldToScreenPoint(_hoveredTurret.transform.position) + overlayOffset;

        if (RayStore.hitInfo.transform == null || RayStore.hitInfo.transform.tag != turretTag)
        {
            RemoveOverlay();

            return;
        }
        
        Turret turret = RayStore.hitInfo.transform.GetComponent<Turret>();

        if (_hoveredTurret != turret)
        {
            _hoveredTurret = turret;

            RemoveOverlay();

            return;
        }

        //this runs only when the mouse is hovering over the same turret
        if(_timeDone < overlayOpenTime)
        {
            _timeDone += Time.deltaTime;

            return;
        }

        if(_turretOverlay == null)
            GenerateOverlay();
    }

    void RemoveOverlay()
    {
        _timeDone = 0f;

        if (_turretOverlay != null)
        {
            _turretOverlay.GetComponent<ScaleHelper>()?.ScaleClosedAndDestroy();
            _turretOverlay = null;
        }

        for(int i = 0; i < _rangeRenderers.Count; i++)
        {
            Destroy(_rangeRenderers[i]);
        }

        _rangeRenderers.Clear();
    }

    void GenerateOverlay()
    {
        _turretOverlay = Instantiate(_hoveredTurret.overlayIcon, 
            Camera.main.WorldToScreenPoint(_hoveredTurret.transform.position) + overlayOffset, 
            Quaternion.identity,
        _canvas).transform;

        GenerateOverlay(_hoveredTurret);
    }
    public void GenerateOverlay(Turret hoveredTurret)
    {
        for (int i = 0; i < hoveredTurret.subTurrets.Count; i++)
        {
            SubTurret subTurret = hoveredTurret.subTurrets[i];
            SubTurretClass subTurretClass = hoveredTurret.subTurretClasses[i];

            GenerateSectorMesh meshGenerator = Instantiate(rangeRendererPrefab, hoveredTurret.transform, false).GetComponent<GenerateSectorMesh>();

            meshGenerator.transform.forward = hoveredTurret.transform.right;
            meshGenerator.transform.localPosition = subTurret.turretBase.localPosition;
            meshGenerator.angle = hoveredTurret.angleRange * 2f;
            meshGenerator.radius = hoveredTurret.maxRange;
            meshGenerator.centerAngle = subTurretClass.startingAngle + 180f; //fix this because it depends on CURRENT turret rotation ;-;

            meshGenerator.RenderSector();

            MeshRenderer renderer = meshGenerator.GetComponent<MeshRenderer>();

            Color color = renderer.material.GetColor("_Color");
            color.a = _transparency / 100f;

            renderer.material.SetColor("_Color", color);

            _rangeRenderers.Add(meshGenerator.gameObject);
        }
    }
}
