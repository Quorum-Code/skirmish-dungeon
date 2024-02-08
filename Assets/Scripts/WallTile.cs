using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : MonoBehaviour
{
    [SerializeField] private GameObject spriteHost;
    private Collider wallCollider;
    private Material material;
    private IEnumerator fadeCoroutine;

    [SerializeField] private float maxAlpha = 1.0f;
    [SerializeField] private float minAlpha = 0.1f;
    [SerializeField] private float fadeSpeed = 3f;

    public void Start() 
    {
        wallCollider = gameObject.GetComponent<Collider>();
        material = spriteHost.GetComponent<Renderer>().material;
    }

    public void FadeOut() 
    {
        StartFade(minAlpha);
    }

    public void FadeIn() 
    {
        StartFade(maxAlpha);
    }

    private void StartFade(float targetAlpha) 
    {
        if (fadeCoroutine != null) 
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = FadeAlpha(targetAlpha);
        StartCoroutine(fadeCoroutine);
    }

    private IEnumerator FadeAlpha(float targetAlpha) 
    {
        gameObject.layer = 10;

        float sign = 1f;
        if (targetAlpha == minAlpha) { sign = -1f; }
        Color fadeColor = new Color(material.color.r, material.color.g, material.color.b, material.color.a);

        while (material.color.a >= minAlpha && material.color.a <= maxAlpha) 
        {
            fadeColor.a += Time.deltaTime * fadeSpeed * sign;

            material.color = fadeColor;
            yield return null;
        }

        fadeColor.a = targetAlpha;
        material.color = fadeColor;

        if (targetAlpha == maxAlpha) 
        {
            gameObject.layer = 9;
        }

        fadeCoroutine = null;
    }
}
