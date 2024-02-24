using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class FadeHelper : MonoBehaviour
{
    public TextMeshProUGUI textToFade;
    public MeshRenderer meshToFade;
    public Image imageToFade;

    [Range(0f, 1f)] public float fadeSpeed = 0.1f;
    [Range(0f, 1f)] public float maxFade = 1f;
    [Range(0f, 1f)] public float minFade;

    public bool fadeRecursively; /// if true then fades in and out constantly
    public bool adjustToPauseTime = true;
    public bool isFadingOpen;

    private bool destroyOnFadeAway;


    private void Start()
    {
        textToFade = textToFade != null ? textToFade : GetComponent<TextMeshProUGUI>();
        meshToFade = meshToFade != null ? meshToFade : GetComponent<MeshRenderer>();
        imageToFade = imageToFade != null ? imageToFade : GetComponent<Image>();
    }

    void Update()
    {
        if (textToFade != null)
            textToFade.color = FadeColor(textToFade.color);

        if (meshToFade != null)
        {
            for (int i = 0; i < meshToFade.materials.Count(); i++)
                meshToFade.materials[i].color = FadeColor(meshToFade.materials[i].color);
        }

        if (imageToFade != null)
            imageToFade.color = FadeColor(imageToFade.color);
    }


    public void Fade()
    {
        isFadingOpen = !isFadingOpen;
    }
    public void Fade(bool fadeOpen)
    {
        isFadingOpen = fadeOpen;
    }

    public void StartRecursiveFade()
    {
        fadeRecursively = true;
    }
    public void EndRecursiveFade(bool fadeOpen)
    {
        fadeRecursively = false;
        isFadingOpen = fadeOpen;
    }

    public void FadeAwayAndDestroy()
    {
        isFadingOpen = false;

        destroyOnFadeAway = true;
    }


    private Color FadeColor(Color color)
    {
        color.a += (isFadingOpen ? fadeSpeed : -fadeSpeed) * ( adjustToPauseTime ? Pause.adjTimeScale : Time.timeScale );
        color.a = Mathf.Clamp(color.a, minFade, maxFade);

        if(destroyOnFadeAway && color.a <= minFade)
        {
            Destroy(gameObject, 0f);
        }

        if (fadeRecursively && (color.a <= minFade || color.a >= maxFade))
        {
            isFadingOpen = !isFadingOpen;
        }

        return color;
    }
}
