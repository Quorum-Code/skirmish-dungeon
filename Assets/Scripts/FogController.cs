using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    [SerializeField] private GameObject fogPlane;

    [SerializeField] private Texture2D fogPlaneAlpha;
    [SerializeField] private Texture2D fogPlaneBuffer;

    // Start is called before the first frame update
    void Start()
    {
        fogPlaneAlpha = new Texture2D(1024, 1024);
        fogPlaneBuffer = new Texture2D(1024, 1024);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ApplyAlpha() 
    {


    }
}
