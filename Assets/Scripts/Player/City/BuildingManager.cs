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
    [SerializeField] Text popDisplay;
    [SerializeField] bool popPercentage = false;
    [SerializeField] List<Mesh> _meshes = new List<Mesh>();

    [Header("Building Weapons")]
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

        void AssignBuildingValues(GameObject building)
        {
            int index = Random.Range(0, _meshes.Count - 1);

            MeshFilter meshFilter = building.GetComponent<MeshFilter>();
            meshFilter.mesh = _meshes[index];

            Building build = new Building();
            build.modelIndex = index;
            buildClasses.Add(build);

            build.Start(_buildingHealth, _totalPopulationThousands / _buildings.Count);
        }
    }

    private void Start()
    {
        UpdateSmallArms();
    }

    void Update()
    {
        population = 0f;
        for(int i = 0; i < buildClasses.Count; i++)
        {
            population += buildClasses[i]._currentPopulation;
        }
        if(popDisplay != null)
        popDisplay.text = popPercentage ? Mathf.Round(population / _totalPopulationThousands * 100f).ToString()
                + "%": Mathf.Round(population).ToString() + "K";

        if(_buildings.Count <= 0)
        {
            Lose();
        }
    }

    public void DamageBuilding(GameObject buildingToDamage, float _damage)
    {
        Building build = buildClasses[_buildings.IndexOf(buildingToDamage)];

        build._health -= _damage;

        if (build._health <= 0f)
        {
            DestroyBuilding(_buildings[buildClasses.IndexOf(build)]);
        }
    }
    public void DestroyBuilding(GameObject buildingToDestroy)
    {
        int index = _buildings.IndexOf(buildingToDestroy);

        buildClasses.Remove(buildClasses[index]);
        _buildings.Remove(buildingToDestroy);

        Destroy(buildingToDestroy);
        
        UpdateSmallArms();
    }

    void Lose()
    {
        FindObjectOfType<MajorEvents>().Lose();
    }

    void UpdateSmallArms()
    {
        if (_smallArms != null)
        {
            List<GameObject> buildingsToAdd = new List<GameObject>(_buildings);
            _smallArms.ResetValue();
            for (int i = _maxConcurrentSmallArms; i > 0; i--)
            {
                Transform build = GetFarthestObject(buildingsToAdd, transform.position);
                if (build == null)
                    break;
                buildingsToAdd.Remove(build.gameObject);
                _smallArms.ChangeValues(build.position + _smallArmsOffset);
            }
        }
    }

    //Utility functions
    public List<GameObject> arrayToList(GameObject[] array)
    {
        List<GameObject> b = new List<GameObject>();
        for (int i = array.Length; i > 0; i--)
        {
            b.Add(array[i - 1]);
        }
        return b;
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

    public float GetPopulation()
    {
        return population;
    }
}

public class Building
{
    public float _maxHealth;
    public float _health;
    public float _maxPopulation;
    public float _currentPopulation;
    public int modelIndex;

    public void Start(float maxHealth, float maxPop)
    {
        _maxHealth = maxHealth; _maxPopulation = maxPop;

        _health = maxHealth; _currentPopulation = maxPop;
    }
}
