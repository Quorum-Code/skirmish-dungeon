using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    Camera mainCamera;

    private List<GameObject> pawnSprites = new List<GameObject>();
    private List<GameObject> staticObjects = new List<GameObject>();
    private List<SpriteRenderer> staticSprites = new List<SpriteRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        pawnSprites.AddRange(GameObject.FindGameObjectsWithTag("Pawn"));
        staticObjects.AddRange(GameObject.FindGameObjectsWithTag("StaticSprite"));

        Debug.Log(staticObjects[0].gameObject.name);

        for (int i = 0; i < staticObjects.Count; i++) 
        {
            SpriteRenderer sr = staticObjects[i].GetComponent<SpriteRenderer>();
            if (sr) 
            {
                staticSprites.Insert(i, sr);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePawns();
        UpdateStaticSprites();
    }

    public void AddPawn(GameObject pawn) 
    {
        pawnSprites.Add(pawn);
    }

    public void AddStaticSprite(GameObject staticObject, SpriteRenderer staticSprite) 
    {
        staticObjects.Add(staticObject);
        staticSprites.Add(staticSprite);
    }

    private void UpdatePawns() 
    {
        foreach (GameObject g in pawnSprites) 
        {
            g.transform.rotation = mainCamera.transform.rotation;
        }
    }

    private void UpdateStaticSprites()
    {
        Vector3 directionAB;
        float dotProduct;
        GameObject g;
        for (int i = 0; i < staticObjects.Count; i++)
        {
            g  = staticObjects[i];

            directionAB = (g.transform.position - mainCamera.transform.position).normalized;
            dotProduct = Vector3.Dot(directionAB, g.transform.forward);

            if (dotProduct < 0f) 
            {
                staticSprites[i].flipX = !staticSprites[i].flipX;
                g.transform.Rotate(new Vector3(0f, 180f, 0));
            }
        }
    }
}
