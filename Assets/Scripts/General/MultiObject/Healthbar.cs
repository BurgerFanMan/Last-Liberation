using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Healthbar : ICanBePaused
{
    [SerializeField] ICanTakeDamage _healthSource;

    [SerializeField] Transform _healthbarHealth;
    [SerializeField] Transform _healthbar;

    [SerializeField] bool _rotateHealthbar = true;

    [Header("Color")]
    [SerializeField] bool _changeColour;
    [SerializeField] Gradient _gradient;
    [SerializeField] MeshRenderer _colourToChange;

    [Header("Fade")]
    [SerializeField] bool _fade = true;
    [SerializeField] float _timeTillFade = 4f;
    [Tooltip("Determines whether the healthbar fades or instantly disappears after Time Till Fade")]
    [SerializeField] bool _instantFade = false;
    [SerializeField] float _fadePerSecond = 0.3f;
    [SerializeField] List<MeshRenderer> _fadeMeshes = new List<MeshRenderer>();

    private float healthTemp;
    private Transform cam;

    private float scaleY, scaleX;
    private float fadeTime = 0f;
    private bool showing, fading;

    private List<Material> fadeMaterials = new List<Material>();

    private void Start()
    {
        healthTemp = _healthSource._health;
        cam = Camera.main.transform;

        scaleX = _healthbar.localScale.x;
        scaleY = _healthbar.localScale.y;

        if (_changeColour)
        {
            _colourToChange.materials[0].color = ColorFromGradient(1);
        }

        if (_fade)
        {
            foreach (MeshRenderer mesh in _fadeMeshes)
            {
                foreach (Material mat in mesh.materials)
                {
                    fadeMaterials.Add(mat);
                    Color color = mat.color;

                    mat.color = new Color(color.r, color.g, color.b, 0);
                }
            }
        }
    }

    private void Update()
    {
        if (_rotateHealthbar)
            HealthbarRotate();

        if (healthTemp != _healthSource._health)
        {
            HealthbarScale();
        }

        healthTemp = _healthSource._health;

        if (_fade)
            FadeUpdate();
    }

    void HealthbarScale()
    {
        float scale = _healthSource._health / _healthSource._maxHealth;
        _healthbarHealth.localScale = new Vector3(scaleX, scaleY, scale);

        if (_changeColour)
        {
            _colourToChange.materials[0].color = ColorFromGradient(scale);
        }

        if (_fade)
        {
            foreach (Material mat in fadeMaterials)
            {
                Color color = mat.color;

                mat.color = new Color(color.r, color.g, color.b, 1);
            }
            showing = true;
        }
    }

    void HealthbarRotate()
    {
        _healthbar.transform.LookAt(cam);
    }

    Color ColorFromGradient(float value)  // Range of 0-1
    {
        return _gradient.Evaluate(value);
    }

    void Fade()
    {
        showing = false;
        fadeTime = 0f;

        if (_instantFade)
        {
            foreach (Material mat in fadeMaterials)
            {
                Color color = mat.color;

                mat.color = new Color(color.r, color.g, color.b, 0);
            }
        }
        else
        {
            foreach (Material mat in fadeMaterials)
            {
                Color color = mat.color;

                mat.color = new Color(color.r, color.g, color.b, 1);
            }
            fading = true;
        }
    }

    void FadeUpdate()
    {
        if (showing)
        {
            if (fadeTime >= _timeTillFade)
            {
                Fade();
            }
            else
            {
                fadeTime += 1f * Time.deltaTime * _timeScale;
            }
        }
        else if (fading)
        {
            if(fadeMaterials[0].color.a <= 0f)
            {
                fading = false;
            }
            else foreach (Material mat in fadeMaterials)
            {
                Color color = mat.color;

                float a = color.a - _fadePerSecond * _timeScale * Time.deltaTime;
                if (a <= 0f)
                    a = 0f;
                mat.color = new Color(color.r, color.g, color.b, a);
            }
        }
    }
}
