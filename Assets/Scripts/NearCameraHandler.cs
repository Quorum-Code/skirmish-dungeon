using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class NearCameraHandler : MonoBehaviour
{
    [SerializeField] string wallTag;
    [SerializeField] Collider nearCollider;
    [SerializeField] float alphaSpeed = 3f;

    List<Fadeable> nearFadeables = new List<Fadeable>();
    List<Fadeable> farFadeables = new List<Fadeable>();

    List<Fadeable> changingFadeables = new List<Fadeable>();

    // Start is called before the first frame update
    void Start()
    {
        nearCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        foreach (Fadeable f in changingFadeables) 
        {
            if (nearFadeables.Contains(f))
            {
                nearFadeables.Remove(f);
                farFadeables.Add(f);
            }
            else if (farFadeables.Contains(f))
            {
                farFadeables.Remove(f);
            }
        }
        changingFadeables.Clear();

        foreach (Fadeable f in nearFadeables) 
        {
            f.subAlpha(Time.deltaTime * alphaSpeed);
        }

        foreach (Fadeable f in farFadeables) 
        {
            f.addAlpha(Time.deltaTime * alphaSpeed);
            if (f.material.color.a >= 1f) 
            {
                changingFadeables.Add(f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == wallTag) 
        {
            nearFadeables.Add(new Fadeable(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == wallTag)
        {
            foreach (Fadeable f in nearFadeables) 
            {
                if (f.gameObject == other.gameObject) 
                {
                    changingFadeables.Add(f);
                    return;
                }
            }
        }
    }

    protected struct Fadeable 
    {
        public Fadeable(GameObject _gameObject) 
        {
            gameObject = _gameObject;
            material = gameObject.GetComponent<Renderer>().material;
        }

        public void setAlpha(float alpha) 
        {
            material.color = new Color(material.color.r, material.color.g, material.color.b, Mathf.Clamp(alpha, 0.1f, 1f));
        }

        public void addAlpha(float alpha) 
        {
            material.color = new Color(material.color.r, material.color.g, material.color.b, Mathf.Clamp(material.color.a + alpha, 0.1f, 1f));
        }

        public void subAlpha(float alpha) 
        {
            addAlpha(-alpha);
        }

        public GameObject gameObject { get; }
        public Material material { get; }
    }
}
