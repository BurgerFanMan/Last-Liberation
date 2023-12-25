using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    [Header("Building Management")]
    public List<GameObject> _buildings = new List<GameObject>();
    [SerializeField] float _buildingHealth = 100f;

    [Header("Graphics and Effects")]
    [SerializeField] List<Mesh> _meshes = new List<Mesh>();

    private List<Building> buildClasses = new List<Building>(); 

    void Awake()
    {
        _buildings = ArrayToList(GameObject.FindGameObjectsWithTag("Building"));
        foreach(GameObject building in _buildings)
        {
            AssignBuildingValues(building);
        }

        void AssignBuildingValues(GameObject building)
        {
            int index = Random.Range(0, _meshes.Count - 1);

            MeshFilter meshFilter = building.GetComponent<MeshFilter>();
            meshFilter.mesh = _meshes[index];

            Building build = new Building();
            build.modelIndex = index;
            buildClasses.Add(build);

            build.Start(_buildingHealth);
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

        if (_buildings.Count <= 0)
        {
            Lose();
        }
    }
    public void DestroyBuilding(GameObject buildingToDestroy)
    {
        int index = _buildings.IndexOf(buildingToDestroy);

        buildClasses.Remove(buildClasses[index]);
        _buildings.Remove(buildingToDestroy);

        Destroy(buildingToDestroy);
    }

    void Lose()
    {
        FindObjectOfType<MajorEvents>().Lose();
    }

    //Utility functions
    public List<GameObject> ArrayToList(GameObject[] array)
    {
        List<GameObject> b = new List<GameObject>();
        for (int i = array.Length; i > 0; i--)
        {
            b.Add(array[i - 1]);
        }
        return b;
    }
}

public class Building
{
    public float _maxHealth;
    public float _health;
    public int modelIndex;

    public void Start(float maxHealth)
    {
        _maxHealth = maxHealth;

        _health = maxHealth;
    }
}
