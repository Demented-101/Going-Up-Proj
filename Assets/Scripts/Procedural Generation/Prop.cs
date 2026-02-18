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
    [SerializeField] private AudioClip breakSFX = null;
    [SerializeField] private float breakSFXVolume;
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

    public bool Break(int currentMach)
    {
        bool wouldBreak = currentMach >= breakStrength;
        if (!canBreak || !wouldBreak) { return false; }

        gameObject.SetActive(false);
        gameStatus.AddScore(breakPoints);
        if (breakSFX != null) { AudioSource.PlayClipAtPoint(breakSFX, transform.position, breakSFXVolume); }
        return true;
    }
}
