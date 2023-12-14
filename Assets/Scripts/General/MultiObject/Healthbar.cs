using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Healthbar : MonoBehaviour
{
    [SerializeField] ICanTakeDamage _healthSource;

    [SerializeField] Transform _healthScale;
    [SerializeField] Transform _healthRotate;

    [SerializeField] bool _rotateHealthbar = true;

    [Header("Color")]
    [SerializeField] bool _changeColour;
    [SerializeField] Gradient _gradient;
    [SerializeField] SpriteRenderer _colourToChange;

    [Header("Fade")]
    [SerializeField] bool _fade = true;
    [SerializeField] float _timeTillFade = 4f;
    [Tooltip("Determines whether the healthbar fades or instantly disappears after Time Till Fade")]
    [SerializeField] bool _instantFade = false;
    [SerializeField] float _fadePerSecond = 0.3f;
    [SerializeField] List<SpriteRenderer> _fadeRenderers = new List<SpriteRenderer>();

    private float healthTemp;
    private Transform cam;

    private float scaleY, scaleZ;
    private float fadeTime = 0f;
    private bool showing, fading;

    private void Start()
    {
        healthTemp = _healthSource._health;
        cam = Camera.main.transform;

        scaleZ = _healthScale.localScale.z;
        scaleY = _healthScale.localScale.y;

        if (_changeColour)
        {
            _colourToChange.color = ColorFromGradient(1);
        }

        if (_fade)
        {
            foreach (SpriteRenderer spriteRenderer in _fadeRenderers)
            {
                Color color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
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
        _healthScale.localScale = new Vector3(scale, scaleY, scaleZ);

        if (_changeColour)
        {
            _colourToChange.color = ColorFromGradient(scale);
        }

        if (_fade)
        {
            foreach (SpriteRenderer spriteRenderer in _fadeRenderers)
            {
                Color color = spriteRenderer.color;

                spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
            }
            showing = true;
        }
    }

    void HealthbarRotate()
    {
        _healthRotate.transform.LookAt(cam);
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
            foreach (SpriteRenderer spriteRenderer in _fadeRenderers)
            {
                Color color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
            }
        }
        else
        {
            foreach (SpriteRenderer spriteRenderer in _fadeRenderers)
            {
                Color color = spriteRenderer.color;

                spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
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
                fadeTime += 1f * Pause.adjTimeScale;
            }
        }
        else if (fading)
        {
            if(_fadeRenderers[0].color.a <= 0f)
            {
                fading = false;
            }
            else foreach (SpriteRenderer spriteRenderer in _fadeRenderers)
            {
                Color color = spriteRenderer.color;

                float a = color.a - _fadePerSecond * Pause.adjTimeScale;
                if (a <= 0f)
                    a = 0f;
                spriteRenderer.color = new Color(color.r, color.g, color.b, a);
            }
        }
    }
}
