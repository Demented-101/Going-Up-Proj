using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class LargeOfficeDecorator : Decorator
{
    public List<Vector2Int> officeConnections { get; private set; } = new List<Vector2Int> { };
    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject wallObject;
    [SerializeField] private GameObject cubicleSet;

    private bool isCenterOffice = false;
    private Vector3 cubicleDir;

    public override void Decorate()
    {
        if (generator != null && generator.handler != null)
        {
            isCenterOffice = true;
            propegate();
            GenerateRoom();
        }
        
    }

    public void GenerateRoom(Vector3 cubDir)
    {
        List<Vector2Int> remainingDirections = new List<Vector2Int> { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        if (cubicleDir.magnitude <= 0) { cubicleDir = cubDir; }

        if (generator != null && generator.connections != null)
        {
            foreach (Vector2Int hallDirection in generator.connections) // generate doors
            {
                remainingDirections.Remove(hallDirection);

                GameObject newHall = Instantiate(doorObject, transform.position, Quaternion.identity);
                newHall.transform.parent = transform;
                newHall.transform.LookAt(transform.position + new Vector3(-hallDirection.y, 0, hallDirection.x));
            }
        }

        foreach (Vector2Int gapDirection in officeConnections) // skip gaps - connections between offices
        {
            remainingDirections.Remove(gapDirection);

            // add cubicles if center room
            if (isCenterOffice)
            {
                Vector3 offset = new Vector3(gapDirection.x * 15, 0, gapDirection.y * 15);
                GenerateCubicleRow(transform.position + offset);
            }
        }

        foreach (Vector2Int wallDirection in remainingDirections) // generate final walls
        {
            GameObject newWall = Instantiate(wallObject, transform.position, Quaternion.identity);
            newWall.transform.parent = transform;
            newWall.transform.LookAt(transform.position + new Vector3(-wallDirection.y, 0, wallDirection.x));
        }

        // generate centeral cubicles
        GenerateCubicleRow(transform.position);
        GenerateCubicleRow(transform.position + cubicleDir.normalized * 30/4);
        GenerateCubicleRow(transform.position - cubicleDir.normalized * 30/4);
    }

    public void GenerateRoom()
    {
        GenerateRoom(cubicleDir);
    }


    private void propegate()
    {
        Vector2Int direction = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down }[Random.Range(0, 4)];
        Vector2Int usedDirection = Vector2Int.zero;

        for (int i = 0; i < 4; i++)
        {
            direction = new Vector2Int(-direction.y, direction.x);

            Vector2Int nextPosition = generator.gridPosition + direction;
            if (usedDirection != Vector2Int.zero && direction != -usedDirection) continue;
            if (generator.handler.IsGridPositionUsed(nextPosition)) continue;

            usedDirection = direction;
            MakeSegment(nextPosition, direction);
            generator.handler.AddGridPosition(nextPosition);
        }
        if (usedDirection == Vector2Int.zero) { usedDirection = direction; }
        cubicleDir = new Vector3(usedDirection.x, 0, usedDirection.y);
    }

    private void MakeSegment(Vector2Int gridPos, Vector2Int gridDir)
    {
        Vector3 nextWorldPos = new Vector3(gridPos.x * Utils.gridSize, 0, gridPos.y * Utils.gridSize);

        GameObject newSegment = Instantiate(gameObject, nextWorldPos, Quaternion.identity, transform.parent);

        officeConnections.Add(gridDir);
        LargeOfficeDecorator nextDecorator = newSegment.GetComponent<LargeOfficeDecorator>();
        if (nextDecorator != null)
        {
            nextDecorator.officeConnections = new List<Vector2Int> { -gridDir };
            nextDecorator.GenerateRoom(new Vector3(gridDir.x, 0, gridDir.y));
        }
    }

    private void GenerateCubicleRow(Vector3 position)
    {
        GameObject newRowA = Instantiate(cubicleSet, transform);
        GameObject newRowB = Instantiate(cubicleSet, transform);

        newRowA.transform.position = position + Vector3.Cross(cubicleDir, Vector3.up).normalized * 7;
        newRowA.transform.LookAt(cubicleDir + newRowA.transform.position);
        newRowB.transform.position = position - Vector3.Cross(cubicleDir, Vector3.up).normalized * 7;
        newRowB.transform.LookAt(cubicleDir + newRowB.transform.position);
    }
}
