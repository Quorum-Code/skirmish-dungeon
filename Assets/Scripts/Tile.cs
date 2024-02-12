using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] protected Renderer render;
    [SerializeField] protected Collider boxCollider;
    [SerializeField] protected Material known;
    [SerializeField] protected Material unknown;
    [SerializeField] protected Material known_faded;
    [SerializeField] protected Material unknown_faded;

    public bool isSelectable = false;
    public bool isKnown { get; private set; } = false;
    public bool isFaded { get; private set; } = false;
    private IEnumerator fadeCoroutine;

    private void Start()
    {
        if (render == null || known == null || unknown == null) 
        {
            Debug.LogError("Missing variable...");
            Destroy(this);
        }


    }

    public void ChangeState(bool known, bool fade) 
    {
        Debug.Log("Called changeState");

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = ChangeMaterial(known, fade);
        StartCoroutine(fadeCoroutine);
    }
    
    private IEnumerator ChangeMaterial(bool _isKnown, bool _isFaded) 
    {
        // as nth of a second
        float fadeSpeed = 3f;
        Material prev;
        Material target;

        if (isKnown && isFaded) prev = known_faded;
        else if (!isKnown && isFaded) prev = unknown_faded;
        else if (isKnown && !isFaded) prev = known;
        else prev = unknown;

        if (_isKnown && _isFaded) target = known_faded;
        else if (!_isKnown && _isFaded) target = unknown_faded;
        else if (_isKnown && !_isFaded) target = known;
        else target = unknown;

        float timer = 0f;
        while (timer < 1f) 
        {
            render.material.Lerp(prev, target, timer);

            // 1/3rd of a second timer
            timer += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        isKnown = _isKnown;
        isFaded = _isFaded;

        if (isKnown && isFaded) render.material = known_faded;
        else if (!isKnown && isFaded) render.material = unknown_faded;
        else if (isKnown && !isFaded) render.material = known;
        else if (!isKnown && !isFaded) render.material = unknown;
    }
}
