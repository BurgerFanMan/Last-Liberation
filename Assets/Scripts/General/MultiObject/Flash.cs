using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Flash : ICanBePaused
{
    [SerializeField] float _flashDuration;
    [SerializeField] MeshRenderer _meshRend;
    [SerializeField] List<int> _matIndex = new List<int>();
    [SerializeField] Color _newColor;

    private List<Material> materials = new List<Material>();
    private List<Color> oldColors = new List<Color>();
    private bool waitingForMat;
    private float timeDone;
    private void Start()
    {
        foreach(int ind in _matIndex)
        {
            materials.Add(_meshRend.materials[ind]);
        }
        foreach(Material mat in materials)
        {
            oldColors.Add(mat.color);
        }
    }

    private void Update()
    {
        if (waitingForMat)
        {
            timeDone += Time.deltaTime * _timeScale;
            if(timeDone >= _flashDuration)
            {
                FlashEnd();

                timeDone = 0f;
                waitingForMat = false;
            }
        }
    }

    public void FlashStart()
    {
        foreach(Material mat in materials)
        {
            mat.color = _newColor;
        }
        waitingForMat = true;
        timeDone = 0f;
    }

    void FlashEnd()
    {
        foreach (Material mat in materials)
        {
            mat.color = oldColors[materials.IndexOf(mat)];
        }
    }
}
