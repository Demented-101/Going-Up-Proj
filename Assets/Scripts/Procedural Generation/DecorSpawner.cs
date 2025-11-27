using UnityEngine;

public class DecorSpawner : Decorator
{
    public DecorObjArray objArray;

    public override void Decorate()
    {
        // pick random obj + instance
        GameObject newDecor = objArray.GetRandom();
        if (newDecor == null) { return; } // chance to not generate hit
        GameObject newObj = Instantiate(newDecor, transform.position, transform.rotation, transform.parent);

        // update the material
    }

}
