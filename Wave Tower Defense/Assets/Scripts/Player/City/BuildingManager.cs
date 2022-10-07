using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [Header("Building Management")]
    public List<GameObject> _buildings = new List<GameObject>();
    [SerializeField] float _totalPopulationThousands = 1000f;
    [SerializeField] float _buildingHealth = 10f;

    [Header("Graphics and Effects")]
    [SerializeField] GameObject _buildingDestroyPrefab;
    [SerializeField] Text popDisplay;
    [SerializeField] bool popPercentage = false;
    [SerializeField] List<Mesh> _meshes = new List<Mesh>();
    [SerializeField] bool _randomRotation = true;

    [Header("Building Weapons")]
    [SerializeField] bool _useWeapons = true;
    [SerializeField] int _maxConcurrentSmallArms = 10;
    [SerializeField] Vector3 _smallArmsOffset;

    private List<Building> buildClasses = new List<Building>();
    private SmallArms _smallArms;

    private float population;
    void Awake()
    {
        _buildings = arrayToList(GameObject.FindGameObjectsWithTag("Building"));
        foreach(GameObject building in _buildings)
        {
            AssignBuildingValues(building);
        }

        _smallArms = GetComponent<SmallArms>();
    }

    private void Start()
    {
        if(_smallArms != null)
        {
            List <GameObject> buildingsToAdd = new List<GameObject>(_buildings);
            for(int i = _maxConcurrentSmallArms; i > 0; i--)
            {
                Transform build = GetFarthestObject(buildingsToAdd, transform.position);
                buildingsToAdd.Remove(build.gameObject);
                _smallArms.ChangeValues(build.position + _smallArmsOffset);
            }
        }
    }

    void Update()
    {
        population = 0f;
        for(int i = 0; i < buildClasses.Count; i++)
        {
            Building build = buildClasses[i];
            if(build._health <= 0f)
            {
                DestroyBuilding(_buildings[buildClasses.IndexOf(build)]);
            }
            else
            {
                population += build.currentPopulation;
            }
        }
        if(popDisplay != null)
        popDisplay.text = popPercentage ? Mathf.Round(population / _totalPopulationThousands * 100f).ToString()
                + "%": Mathf.Round(population).ToString() + "K";

        if(_buildings.Count <= 0)
        {
            Lose();
        }
    }

    void AssignBuildingValues(GameObject building)
    {
        MeshFilter meshFilter = building.GetComponent<MeshFilter>();
        meshFilter.mesh = _meshes[Random.Range(0, _meshes.Count - 1)];
        if (_randomRotation)
            meshFilter.transform.rotation = Quaternion.Euler(new Vector3(-90f, Random.Range(0f, 360f), 0f));

        Building build = new Building();
        buildClasses.Add(build);

        build.maxPopulation = _totalPopulationThousands / _buildings.Count;
        build._maxHealth = _buildingHealth;

        build.StartCustom();
    }
    public List<GameObject> arrayToList(GameObject[] array)
    {
        List<GameObject> b = new List<GameObject>();
        for(int i = array.Length; i > 0; i--)
        {
            b.Add(array[i - 1]);
        }
        return b;
    }

    public float GetPopulation()
    {
        return population;
    }

    public void DamageBuilding(GameObject buildingToDamage, float _damage)
    {
        Building build = buildClasses[_buildings.IndexOf(buildingToDamage)];
        if(build != null)
        {
            build._health -= _damage;
        }
    }
    public void DestroyBuilding(GameObject buildingToDestroy)
    {
        if (_buildings.Contains(buildingToDestroy))
        {
            buildClasses.Remove(buildClasses[_buildings.IndexOf(buildingToDestroy)]);
            _buildings.Remove(buildingToDestroy);

            if(_buildingDestroyPrefab != null)
                Instantiate(_buildingDestroyPrefab, buildingToDestroy.transform.position, buildingToDestroy.transform.rotation);
            Destroy(buildingToDestroy.gameObject);

            List<GameObject> buildingsToAdd = new List<GameObject>(_buildings);
            _smallArms.ResetValue();
            for (int i = _maxConcurrentSmallArms; i > 0; i--)
            {
                Transform build = GetFarthestObject(buildingsToAdd, transform.position);
                buildingsToAdd.Remove(build.gameObject);
                _smallArms.ChangeValues(build.position + _smallArmsOffset);
            }
        }
    }

    void Lose()
    {
        Object.FindObjectOfType<MajorEvents>().Lose();
    }

    Transform GetFarthestObject(List<GameObject> gos, Vector3 currentPos)
    {
        Transform closest = null;
        float dist = 0f;
        foreach (GameObject go in gos)
        {
            if (go != null)
            {
                float thisDist = Vector3.Distance(go.transform.position, currentPos);
                if (thisDist > dist)
                {
                    dist = thisDist;
                    closest = go.transform;
                }
            }
        }
        return closest;
    }
}

public class Building
{
    public float _maxHealth;
    public float _health;
    public float maxPopulation;
    public float currentPopulation;

    public void StartCustom()
    {
        currentPopulation = maxPopulation;
        _health = _maxHealth;
    }
}
