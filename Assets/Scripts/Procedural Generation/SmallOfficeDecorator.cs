using System.Collections.Generic;
using UnityEngine;

public class SmallOfficeDecorator : MonoBehaviour, Decorator
{
    GenObj generator;

    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject WallObject;

    public void Decorate(GenObj genObj)
    {
        generator = genObj;
        if (generator.connections.Count != 1) { return; } // requires one connection

        List<Vector2Int> remainingDirections = new List<Vector2Int> { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        Vector2Int hallDirection = generator.connections[0];
        remainingDirections.Remove(hallDirection);

        GameObject newHall = Instantiate(doorObject, transform.position, Quaternion.identity);
        newHall.transform.parent = transform;
        newHall.transform.LookAt(transform.position + new Vector3(-hallDirection.y, 0, hallDirection.x));
        

        foreach (Vector2Int wallDirection in remainingDirections)
        {
            GameObject newWall = Instantiate(WallObject, transform.position, Quaternion.identity);
            newWall.transform.parent = transform;
            newWall.transform.LookAt(transform.position + new Vector3(-wallDirection.y, 0, wallDirection.x));
        }
    }
}
