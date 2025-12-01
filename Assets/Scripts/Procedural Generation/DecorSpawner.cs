using UnityEngine;

public class DecorSpawner : Decorator
{
    public DecorObjArray objArray;
    private static int floorMaterialIndex;

    public override void Decorate()
    {
        // pick random obj + instance
        GameObject newDecor = objArray.GetRandom();
        if (newDecor == null) { return; } // chance to not generate hit
        GameObject newObj = Instantiate(newDecor, transform.position, transform.rotation, transform.parent);

        // update the material

        // destroy self
#if !UNITY_EDITOR
        Destroy(gameObject);
#else 
        DestroyImmediate(gameObject);
#endif
    }

}
