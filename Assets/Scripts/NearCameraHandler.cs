using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class NearCameraHandler : MonoBehaviour
{
    [SerializeField] string wallTag;
    [SerializeField] float alphaSpeed = 3f;

    private void OnTriggerEnter(Collider other)
    {
        Tile tile = other.GetComponent<Tile>();
        if (tile) 
        {
            tile.ChangeState(tile.isKnown, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Tile tile = other.GetComponent<Tile>();
        if (tile)
        {
            tile.ChangeState(tile.isKnown, false);
        }
    }
}
