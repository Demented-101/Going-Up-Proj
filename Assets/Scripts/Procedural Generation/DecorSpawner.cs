using UnityEngine;

public class DecorSpawner : Decorator
{
    public GameObject[] availableDecorObjects;

    public override void Decorate()
    {
        // pick random obj + instance
        GameObject newDecor = availableDecorObjects[Random.Range(0, availableDecorObjects.Length)];
        GameObject newObj = Instantiate(newDecor, transform.position, transform.rotation, transform.parent);

        // update the material
    }

}
