using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CornerDecorator : MonoBehaviour, Decorator
{
    GenObj generator;

    [SerializeField] private GameObject hallObject;
    [SerializeField] private GameObject wallObject;

    public void Decorate(GenObj genObj)
    {
        generator = genObj;
        List<Vector2Int> remainingDirections = new List<Vector2Int> { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        foreach (Vector2Int hallDirection in generator.connections)
        {
            remainingDirections.Remove(hallDirection);

            GameObject newHall = Instantiate(hallObject, transform.position, Quaternion.identity);
            newHall.transform.parent = transform;
            newHall.transform.LookAt(transform.position + new Vector3(hallDirection.y, 0, -hallDirection.x));
        }

        foreach (Vector2Int wallDirection in remainingDirections)
        {
            GameObject newWall = Instantiate(wallObject, transform.position, Quaternion.identity);
            newWall.transform.parent = transform;
            newWall.transform.LookAt(transform.position + new Vector3(-wallDirection.y, 0, wallDirection.x));
        }
    }
}
