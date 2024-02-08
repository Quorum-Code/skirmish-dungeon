using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [SerializeField] GameObject selectorObject;
    private GameObject selectedObject;
    private GameObject hoveredObject;

    [SerializeField] GameObject debugSelectedObject;

    private IEnumerator smoothMoveSelector;

    [SerializeField] LayerMask selectables;

    // Start is called before the first frame update
    void Start()
    {
        selectorObject = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            CheckCursorHover();
        }

        //if (debugSelectedObject != null) 
        //{
            //MoveSelector(debugSelectedObject.transform.position);
            //debugSelectedObject = null;
        //}
    }

    public void CheckCursorHover() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, selectables)) 
        {
            Debug.Log(selectables.value);
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            MoveSelector(hit.transform.position);
        }
    }

    public void MoveSelector(Vector3 moveTo)
    {
        if (smoothMoveSelector != null) 
        {
            StopCoroutine(smoothMoveSelector);
        }

        smoothMoveSelector = SmoothMoveSelector(moveTo);
        StartCoroutine(smoothMoveSelector);
    }

    public void DisableSelector() 
    {

    }

    private IEnumerator SmoothMoveSelector(Vector3 moveTo) 
    {
        Vector3 from = this.transform.position;

        float timer = 0f;
        float speed = 10f;
        while (timer < 1f) 
        {
            timer += (1 - 1.5f * Mathf.Pow(1.8f * timer - .9f, 8)) * Time.deltaTime * speed;
            this.transform.position = Vector3.Lerp(from, moveTo, timer);
            
            yield return null;
        }

        this.transform.position = moveTo;
    }
}
