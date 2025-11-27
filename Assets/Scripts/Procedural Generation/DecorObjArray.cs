using UnityEngine;

[CreateAssetMenu(fileName = "DecorObjArray", menuName = "Scriptable Objects/DecorObjArray")]
public class DecorObjArray : ScriptableObject
{
    [SerializeField] private GameObject[] DecorObjs;
    [SerializeField][Range(0,100)] private int generationChance = 100;

    public GameObject GetRandom()
    {
        if (generationChance != 100) { if (Random.Range(0, 100) > generationChance) return null; }
        return DecorObjs[Random.Range(0, DecorObjs.Length)];
    }
}
