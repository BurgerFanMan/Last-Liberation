using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Weapon : IFireBullets
{
    [Header("Weapon Settings")]
    public int magazineSize;
    public float reloadTime;
    public float fireRate;
    public float cameraShakeIntensity = 4f;
    public AudioSource audioSource;

    [Header("Turrets")]
    public float turretTurnSpeed;
    public float turretGunTurnSpeed;
    public float turretRotationClamp;
    public List<WeaponTurretStruct> turretStructs = new List<WeaponTurretStruct>(); //I kind of just need this so you can assign values from Unity's editor, and it's then fed into classes instead

    [Header("UI")]
    public GameObject reloadIcon;
    public TextMeshProUGUI magazineCountDisplay;
    public TextMeshProUGUI magazineSizeDisplay;
    public string forbiddenUITag = "UINoShoot"; //won't fire if hovering over UI with this tag

    private int magazineCount;
    private float timeSinceReload;
    private float timeSinceFire;

    private List<WeaponTurret> turrets = new List<WeaponTurret>();

    // Start is called before the first frame update
    void Start()
    {
        audioSource = audioSource == null ? GetComponent<AudioSource>() : audioSource;

        magazineCount = magazineSize;
        reloadIcon.transform.localScale = new Vector3(0f, 1f, 1f);

        for (int i = 0; i < turretStructs.Count; i++)
        {
            WeaponTurretStruct turret = turretStructs[i];
            turrets.Add(new WeaponTurret
            {
                turretBody = turret.turretBody,
                turretGun = turret.turretGun,
                firePoint = turret.firePoint,

                lineRenderer = turret.lineRenderer,

                startingForward = turret.turretBody.forward,
                startingAngle = turret.turretBody.eulerAngles.y
            });
        }  
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause.isPaused)
            return;

        //cycling through turrets to set line renderer position and rotate them
        for (int i = 0; i < turrets.Count; i++)
        {
            WeaponTurret turret = turrets[i];

            //actual rotation stuff
            Vector3 direction = turret.turretBody.position - RayStore.GroundedHitPoint;
            Vector3 gunDirection = turret.turretGun.position - RayStore.GroundedHitPoint;

            float angleToTarget = Vector3.Angle(direction.normalized, -turret.startingForward);

            Vector3 bodyRotation;
            Vector3 gunRotation;

            if (angleToTarget < turretRotationClamp)
            {
                bodyRotation = Quaternion.LookRotation(-direction).eulerAngles;

                gunRotation = Quaternion.LookRotation(-gunDirection).eulerAngles;


                //setting line renderer position
                Vector3 hitPoint;

                if (Physics.Raycast(turret.firePoint.position, turret.turretGun.forward, out RaycastHit hitInfo, 100f, ~(1 << 9)))
                    hitPoint = hitInfo.point;
                else
                    hitPoint = turret.turretGun.forward * 100f;

                turret.lineRenderer.SetPosition(0, turret.firePoint.position);
                turret.lineRenderer.SetPosition(1, hitPoint);
            }
            else
            {
                bodyRotation = Quaternion.LookRotation(turret.startingForward).eulerAngles;

                gunRotation = Quaternion.LookRotation(turret.startingForward).eulerAngles;

                turret.lineRenderer.SetPosition(0, Vector3.zero);
                turret.lineRenderer.SetPosition(1, Vector3.zero);
            }

            bodyRotation = MoveTowardsWrapAround(turret.turretBody.eulerAngles, bodyRotation, turretTurnSpeed * Pause.adjTimeScale);
            gunRotation = MoveTowardsWrapAround(turret.turretGun.eulerAngles, gunRotation, turretGunTurnSpeed * Pause.adjTimeScale);

            turret.turretBody.rotation = Quaternion.Euler(turret.turretBody.rotation.x, bodyRotation.y, 0f);
            turret.turretGun.localRotation = Quaternion.Euler(gunRotation.x, 0f, 0f);
        }

        UpdateDisplays();


        CheckReload();

        if (magazineCount == 0)
        {
            if(timeSinceReload > reloadTime)
            {
                magazineCount = magazineSize;

                timeSinceReload = 0f;
            }
            else
            {
                timeSinceReload += Pause.adjTimeScale;

                return;
            }
        }

        if (!(timeSinceFire > 1f / fireRate))
        {
            timeSinceFire += Pause.adjTimeScale;

            return;
        }

        CheckFire();
    }


    void CheckReload()
    {
        if (Input.GetKey(InputManager.GetValue("weapon_reload")))
        {
            magazineCount = 0;
        }
    }
    void CheckFire()
    {
        if (Input.GetKey(InputManager.GetValue("weapon_fire")) && !OverUI() && !SharedVariables.inBuildMode)
        {
            if (!GetClosestTurret(turrets, out WeaponTurret turret))
                return;

            float randomRange = 1f;
            Vector3 randomness = new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
            SpawnBullet(turret.firePoint.position, Quaternion.Euler(turret.firePoint.eulerAngles + randomness));

            timeSinceFire = 0f;
            magazineCount -= 1;

            if (audioSource != null)
                audioSource.Play();

            if (SharedVariables.cameraShake != null)
                SharedVariables.cameraShake.ShakeCamera(cameraShakeIntensity, 0.1f, 0);
        }
    }


    void UpdateDisplays()
    {
        if (magazineCount == 0)
            reloadIcon.transform.localScale = new Vector3(timeSinceReload / reloadTime, 1f, 1f);           
        else
            reloadIcon.transform.localScale = new Vector3(0f, 1f, 1f);

        magazineCountDisplay.text = $"{magazineCount}";
        magazineSizeDisplay.text = $"{magazineSize}";
    }

    //Utility functions
    Vector3 MoveTowardsWrapAround(Vector3 current, Vector3 target, float maxDistanceDelta)
    {
        // avoid vector ops because current scripting backends are terrible at inlining
        float toVector_x = target.x - current.x;
        float toVector_y = target.y - current.y;
        float toVector_z = target.z - current.z;

        if ((target.x >= 270f && current.x <= 90f) || (target.x <= 90f && current.x >= 270f))
            toVector_x *= -1f;
        if ((target.y >= 270f && current.y <= 90f) || (target.y <= 90f && current.y >= 270f))
            toVector_y *= -1f;
        if ((target.z >= 270f && current.z <= 90f) || (target.z <= 90f && current.z >= 270f))
            toVector_z *= -1f;

        float sqdist = toVector_x * toVector_x + toVector_y * toVector_y + toVector_z * toVector_z;

        if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
            return target;
        var dist = (float)Mathf.Sqrt(sqdist);

        return new Vector3(current.x + toVector_x / dist * maxDistanceDelta,
            current.y + toVector_y / dist * maxDistanceDelta,
            current.z + toVector_z / dist * maxDistanceDelta);
    }
    bool GetClosestTurret(List<WeaponTurret> turrets, out WeaponTurret turret)
    {
        turret = null;
        float angleDistance = Mathf.Infinity;

        for(int i = 0; i < turrets.Count; i++)
        {
            Vector3 direction = turrets[i].turretBody.position - RayStore.GroundedHitPoint;
            float thisAngleDistance = Vector3.Angle(-direction, turrets[i].startingForward);

            if(thisAngleDistance < angleDistance && thisAngleDistance < turretRotationClamp)
            {
                turret = turrets[i];

                angleDistance = thisAngleDistance;
            }
        }

        return turret != null;
    }
    bool OverUI()
    {
        return EventSystem.current.IsPointerOverGameObject() &&
                      EventSystem.current.currentSelectedGameObject != null &&
                      EventSystem.current.currentSelectedGameObject.CompareTag(forbiddenUITag) ? true
                      : false;
    }
}

[System.Serializable]
public struct WeaponTurretStruct
{
    public Transform turretBody;
    public Transform turretGun;

    public Transform firePoint;

    public LineRenderer lineRenderer;
}

public class WeaponTurret
{
    public Transform turretBody;
    public Transform turretGun;

    public Transform firePoint;

    public LineRenderer lineRenderer;

    public Vector3 startingForward;
    public float startingAngle;
}
