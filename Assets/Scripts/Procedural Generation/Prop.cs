using System;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private MapColourHandler colourHandler;
    [SerializeField] private GameObject[] models;
    [SerializeField] private bool doChangeMaterial;

    private void Start()
    {
        if (doChangeMaterial && models.Length > 0)
        {
            foreach (GameObject model in models) { model.GetComponent<MeshRenderer>().material = colourHandler.GetFloorPropMaterial(); }
        }
    }
}
