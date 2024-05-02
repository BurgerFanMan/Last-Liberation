using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuItems
{
    static string turretPath = "Assets/Prefabs/Turrets";
    [MenuItem("Assets/Generate Turret Data")]
    private static void GenerateTurretData()
    {
        //generating turret ghost by stripping components, and adding necessary ones
        GameObject turretGhost = Object.Instantiate((GameObject)Selection.activeObject);

        Object.DestroyImmediate(turretGhost.GetComponent<Turret>());
        Object.DestroyImmediate(turretGhost.GetComponent<AudioSource>());
        Object.DestroyImmediate(turretGhost.GetComponent<AdjustSoundVolume>());
        Object.DestroyImmediate(turretGhost.GetComponent<AdjustSoundSpeed>());

        turretGhost.AddComponent<TurretGhost>();
        turretGhost.AddComponent<Rigidbody>().isKinematic = true;

        turretGhost.tag = "Untagged";

        GameObject turretGhostPrefab = PrefabUtility.SaveAsPrefabAsset(turretGhost, $"{turretPath}/Ghosts/{Selection.activeObject.name} Ghost.prefab", out bool turretGhostSuccess);

        Object.DestroyImmediate(turretGhost);

        if (turretGhostSuccess)
            Debug.Log("Turret ghost successfully created. Remember to assign the Range Renderer Prefab!");

        //generating turret info
        TurretInfo turretInfo = (TurretInfo)ScriptableObject.CreateInstance("TurretInfo");

        turretInfo.turretPrefab = (GameObject)Selection.activeObject;
        turretInfo.turretGhost = turretGhostPrefab;

        AssetDatabase.CreateAsset(turretInfo, $"{turretPath}/Infos/{Selection.activeObject.name} Info.asset");

        Debug.Log("Turret info created successfully.");

        //assigning turret info to build system
        BuildSystem buildSys = Object.FindObjectOfType<BuildSystem>();

        if (buildSys == null) 
        {
            Debug.Log("No build system exists in scene. Ensure you are in the Game scene before retrying generation.");

            return;
        }

        buildSys.turretInfos.Add(turretInfo);
    }

    [MenuItem("Assets/Generate Turret Data", true)]
    private static bool GenerateTurretDataValidation()
    {
        if (Selection.activeObject.GetType() != typeof(GameObject))
            return false;

        GameObject gameObject = (GameObject)Selection.activeObject;

        if (gameObject.GetComponent<Turret>() == null)
            return false;

        return true;
    }
}
