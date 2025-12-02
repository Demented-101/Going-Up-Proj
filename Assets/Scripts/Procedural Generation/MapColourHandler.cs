using UnityEngine;

[CreateAssetMenu(fileName = "MapColourHandler", menuName = "Scriptable Objects/MapColourHandler")]
public class MapColourHandler : ScriptableObject
{
    [SerializeField] private Material[] propMaterials;
    private int floorMaterial;

    public Material GetFloorPropMaterial()
    {
        return propMaterials[floorMaterial];
    }

    public void ReRoll()
    {
        floorMaterial = Random.Range(0, propMaterials.Length);
    }
}
