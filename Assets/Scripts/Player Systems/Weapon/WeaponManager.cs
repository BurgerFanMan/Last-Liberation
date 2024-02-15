using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using TMPro;

public class WeaponManager : ICanBeUpgraded //UL(upgrade level) 0 is reload time, 1 is mag size, 2 is fire rate
{
    [Header("Main")]
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] float _reloadTime;
    [SerializeField] int _magazineSize;
    [SerializeField] float _fireRate = 1f;

    [SerializeField] float _turnSpeed;
    [SerializeField] float _rotationClamp = 90f;
    
    [SerializeField] Transform _reloadIcon;
    [SerializeField] List<Weapon> _snipers;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _reloadNumb;
    [SerializeField] TextMeshProUGUI _reloadCap;
    [Tooltip("Won't fire if mouse is hovering over UI of this tag")]
    [SerializeField] string _forbiddenUITag;

    
    private float timeSinceReload; 
    private int magazineCount;  
    private bool reloadFirstFrame = true;
    private bool readyToFire = true;
    private float timeDone;

    private Vector3 targetPos;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        foreach (Weapon sniper in _snipers)
        {
            sniper.SetValues(_bulletPrefab, _turnSpeed, _rotationClamp);
        }

        magazineCount = _magazineSize + (int)_upgradeLevel[1];
        _reloadIcon.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Pause.isPaused)
            return;

        _reloadNumb.text = $"{magazineCount}";
        _reloadCap.text = $"{_magazineSize + (int)_upgradeLevel[1]}";

        foreach (Weapon sniper in _snipers)
        {
            sniper.FacePosition(RayStore.GroundedHitPoint);
        }

        if (Input.GetKey(InputManager.GetValue("weapon_reload")))
        {
            magazineCount = 0;

            BeginReload();
        }
       

        if (magazineCount <= 0)
        {
            if (reloadFirstFrame)
            {
                BeginReload();
            }

            _reloadIcon.localScale = new Vector3(timeSinceReload / (_reloadTime * _upgradeLevel[0]), 1f, 1f);

            timeSinceReload += 1f * Pause.adjTimeScale;

            if (timeSinceReload >= _reloadTime * _upgradeLevel[0])
            {
                FinishReload();
            }
        }
        else if (!readyToFire)
        {
            timeDone += Pause.adjTimeScale;

            if(timeDone > 1f / (_fireRate * _upgradeLevel[2]))
            {
                readyToFire = true;
                timeDone = 0f;
            }
        }
        else if (Input.GetKey(InputManager.GetValue("weapon_fire")) && !OverUI() && !SharedVariables.inBuildMode)
        {
            Fire();
        }
    }

    void Fire()
    {
        if (audioSource != null)
            audioSource.Play();
        foreach (Weapon sniper in _snipers)
        {
            if (sniper.Shoot())
                break;
        }

        readyToFire = false;
        magazineCount--;
    }

    void BeginReload()
    {
        _reloadIcon.gameObject.SetActive(true);

        readyToFire = true;
        timeDone = 0f;

        reloadFirstFrame = false;
    }
    void FinishReload()
    {
        magazineCount = _magazineSize + (int)_upgradeLevel[1]; timeSinceReload = 0f;
        _reloadIcon.gameObject.SetActive(false);

        reloadFirstFrame = true;
    }

    //Utility functions
    bool OverUI()
    {
        return EventSystem.current.IsPointerOverGameObject() &&
                      EventSystem.current.currentSelectedGameObject != null &&
                      EventSystem.current.currentSelectedGameObject.CompareTag(_forbiddenUITag) ? true
                      : false;
    }
}
