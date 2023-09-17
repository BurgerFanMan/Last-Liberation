using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using TMPro;

public class SniperManager : ICanBeUpgraded
{
    [Header("Main")]
    public List<Sniper> _snipers;
    [SerializeField] GameObject _bulletPrefab;
    public float _reloadTime;
    public int _magazineSize;
    [SerializeField] float _turnSpeed;
    [SerializeField] float _rotationClamp = 90f;
    [SerializeField] List<Transform> _followMouse;
    [SerializeField] Transform _reloadIcon;
    [SerializeField] TextMeshProUGUI _reloadNumb;
    [SerializeField] TextMeshProUGUI _reloadCap;
    [Tooltip("Won't fire if mouse is hovering over UI of this tag")]
    [SerializeField] string _forbiddenUITag;

    private Vector3 targetPos; 
    float timeSinceFire; 
    int magazineCount;
    private AudioSource audioSource;
    private bool reloadFirstFrame = true;
    private RayStore rayStore;

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
        if (!_paused)
        {
            foreach(Transform tran in _followMouse)
            {
                tran.position = Input.mousePosition;
            }
            
            _reloadNumb.text = magazineCount.ToString();
            _reloadCap.text = (_magazineSize + upgradeLevel[1]).ToString();
        }

        if (magazineCount <= 0)
        {
            if (reloadFirstFrame)
            {
                _reloadIcon.gameObject.SetActive(true);
                reloadFirstFrame = false;
            }

            timeSinceFire += 1 * Time.deltaTime * _timeScale;
            _reloadIcon.localScale = new Vector3(timeSinceFire / (_reloadTime * upgradeLevel[0]), 1f, 1f);
            
            if(timeSinceFire >= _reloadTime * upgradeLevel[0])
            {
                magazineCount = _magazineSize + (int)upgradeLevel[1]; timeSinceFire = 0;
                _reloadIcon.gameObject.SetActive(false);

                reloadFirstFrame = true;
            }
        }

        targetPos = rayStore.RayHitPoint();
        targetPos.y = 0;

        foreach(Sniper sniper in _snipers)
        {
            sniper.FacePosition(targetPos);
        }
        //deal with this
        bool overUI = EventSystem.current.IsPointerOverGameObject() &&
                      EventSystem.current.currentSelectedGameObject != null &&
                      EventSystem.current.currentSelectedGameObject.CompareTag(_forbiddenUITag) ? true 
                      : false;

        if (Input.GetKeyDown(KeyCode.Mouse0) && magazineCount > 0 && !overUI && !_paused)
        {
            if(audioSource != null)
                audioSource.Play();
            foreach(Sniper sniper in _snipers)
            {
                if (sniper.Shoot())
                    break;
            }
            magazineCount--;
        }
    }
}
