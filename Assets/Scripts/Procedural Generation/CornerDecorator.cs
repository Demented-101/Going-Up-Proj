using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CornerDecorator : Decorator
{
    [SerializeField] private GameObject hallObject;
    [SerializeField] private GameObject wallObject;
    [SerializeField] private GameObject[] hallSideRoomObjects;

    public override void Decorate()
    {
        if (generator.connections.Count < 2) { return; } // requires at least two connections

        List<Vector2Int> remainingDirections = new List<Vector2Int> { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        int spawnOfficeDirection = Random.Range(0, generator.connections.Count + 1);

        for (int i = 0; i < generator.connections.Count; i++)
        {
            Vector2Int hallDirection = generator.connections[i];
            remainingDirections.Remove(hallDirection);

            // select hall object to spawn
            GameObject selectedHall = hallObject;
            if(spawnOfficeDirection == i)
            {
                selectedHall = hallSideRoomObjects[Random.Range(0, hallSideRoomObjects.Length)];
            }

            // spawn new hall
            GameObject newHall = Instantiate(selectedHall, transform.position, Quaternion.identity);
            newHall.transform.parent = transform;
            newHall.transform.LookAt(transform.position + new Vector3(hallDirection.y, 0, -hallDirection.x));
        }

        foreach (Vector2Int wallDirection in remainingDirections)
        {
            // spawn wall
            GameObject newWall = Instantiate(wallObject, transform.position, Quaternion.identity);
            newWall.transform.parent = transform;
            newWall.transform.LookAt(transform.position + new Vector3(-wallDirection.y, 0, wallDirection.x));
        }
    }
}
