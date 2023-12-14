using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using TMPro;

public class SniperManager : ICanBeUpgraded
{
    [Header("Main")]
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] float _reloadTime;
    [SerializeField] int _magazineSize;
    [SerializeField] float _fireRate = 1f;

    [SerializeField] float _turnSpeed;
    [SerializeField] float _rotationClamp = 90f;
    
    [SerializeField] Transform _reloadIcon;
    [SerializeField] List<Transform> _followMouse;
    [SerializeField] List<Sniper> _snipers;

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

    private RayStore rayStore;
    private AudioSource audioSource;

    protected override void Awake()
    {
        rayStore = FindObjectOfType<RayStore>();
        audioSource = GetComponent<AudioSource>();

        foreach (Sniper sniper in _snipers)
        {
            sniper.SetValues(_bulletPrefab, _turnSpeed, _rotationClamp);
        }

        magazineCount = _magazineSize;
        _reloadIcon.gameObject.SetActive(false);
    }
    void Update()
    {
        if (_paused)
            return;

        bool overUI = EventSystem.current.IsPointerOverGameObject() &&
                      EventSystem.current.currentSelectedGameObject != null &&
                      EventSystem.current.currentSelectedGameObject.CompareTag(_forbiddenUITag) ? true
                      : false;

        targetPos = rayStore.RayHitPoint();
        targetPos.y = 0;


        _reloadNumb.text = magazineCount.ToString();
        _reloadCap.text = (_magazineSize + upgradeLevel[1]).ToString();

        foreach (Transform trans in _followMouse)
        {
            trans.position = Input.mousePosition;
        }
        foreach (Sniper sniper in _snipers)
        {
            sniper.FacePosition(targetPos);
        }

        if (magazineCount <= 0)
        {
            if (reloadFirstFrame)
            {
                _reloadIcon.gameObject.SetActive(true);   

                readyToFire = true;
                timeDone = 0f;

                reloadFirstFrame = false;
            }

            timeSinceReload += 1 * Time.deltaTime * _timeScale;
            _reloadIcon.localScale = new Vector3(timeSinceReload / (_reloadTime * upgradeLevel[0]), 1f, 1f);

            if (timeSinceReload >= _reloadTime * upgradeLevel[0])
            {
                magazineCount = _magazineSize + (int)upgradeLevel[1]; timeSinceReload = 0f;
                _reloadIcon.gameObject.SetActive(false);

                reloadFirstFrame = true;
            }
        }
        else if (!readyToFire)
        {
            timeDone += Time.deltaTime;

            if(timeDone > 1f / (_fireRate * upgradeLevel[2]))
            {
                readyToFire = true;
                timeDone = 0f;
            }
        }
        else if (Input.GetKey(KeyCode.Mouse0) && !overUI)
        {
            if(audioSource != null)
                audioSource.Play();
            foreach(Sniper sniper in _snipers)
            {
                if (sniper.Shoot())
                    break;
            }

            readyToFire = false;
            magazineCount--;
        }
    }
}
