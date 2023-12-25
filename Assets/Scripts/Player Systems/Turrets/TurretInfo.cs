using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "/Last Liberation/Turret Info")]
public class TurretInfo : ScriptableObject
{
    public GameObject turretPrefab;
    public GameObject turretGhost;
    public GameObject turretWarningGhost;
}
