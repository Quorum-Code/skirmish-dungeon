using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTest : MonoBehaviour
{
    [SerializeField] float alpha = 1.0f;
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = this.gameObject.GetComponent<Renderer>().material;

        if (material == null) 
        {
            Debug.Log("No material...");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
    }
}
