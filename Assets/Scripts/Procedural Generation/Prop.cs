using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private MapColourHandler colourHandler;
    [SerializeField] private GameObject[] models;
    [SerializeField] private bool doChangeMaterial;
    [SerializeField][Range(0f, 360f)] private float allowedSpawnRotation = 0;
    [SerializeField] private bool canBreak = true;
    [SerializeField] private int breakStrength;
    [SerializeField] private int breakPoints;
    [SerializeField] private GameStatus gameStatus;

    public void Generate()
    {
        if (doChangeMaterial && models.Length > 0)
        {
            foreach (GameObject model in models) { model.GetComponent<MeshRenderer>().material = colourHandler.GetFloorPropMaterial(); }
        }

        float rotation = Random.Range(0, allowedSpawnRotation) - (allowedSpawnRotation / 2);
        transform.Rotate(0, rotation, 0);
    }

    public void Break(int currentMach)
    {
        if (!canBreak || currentMach < breakStrength) { return; }

        Debug.Log("BROKEN");
        gameObject.SetActive(false);
        gameStatus.AddScore(breakPoints);
    }
}
