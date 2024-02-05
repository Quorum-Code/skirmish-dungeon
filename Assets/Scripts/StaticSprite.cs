using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSprite : MonoBehaviour
{
    /*
    [SerializeField] GameObject mainCamera;
    SpriteRenderer sr;
    bool isFlipped = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.gameObject;
        sr = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 directionAB = (this.transform.position - mainCamera.transform.position).normalized;

        float dotProduct = Vector3.Dot(directionAB, this.transform.forward);

        if (dotProduct <= 0f) 
        {
            Debug.Log("Object a in front");
            flip();
        }
    }

    private void flip() 
    {
        sr.flipX = !sr.flipX;
        this.transform.Rotate(new Vector3(0f, 180f, 0f));
    }*/
}
