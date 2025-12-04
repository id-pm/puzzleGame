using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParkingZone : MonoBehaviour
{
    [SerializeField] private float Distance;
    [SerializeField] private List<Transform> twoCellPrefab = new();
    [SerializeField] private List<Transform> threeCellPrefab = new();
    private BoxCollider boxCollider;
    private Vector3 UpLeft;
    private Vector3 UpRight;
    private Vector3 BottomLeft;
    private Vector3 BottomRight;
    [NonSerialized] public Vector3[] Angle;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    void Start()
    {
        Calculate();
        Angle = new Vector3[] { BottomLeft, UpRight, BottomRight, UpLeft };
    }
    private void OnDrawGizmosSelected()
    {
        Calculate();
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(BottomLeft, 0.1f);
        Gizmos.DrawSphere(BottomRight, 0.1f);
        Gizmos.DrawSphere(UpLeft, 0.1f);
        Gizmos.DrawSphere(UpRight, 0.1f);
    }
    private void OnDrawGizmos()
    {
        DrawLines();
    }
    public void ClearParkingZone()
    {
        foreach (Transform child in transform)
        {
            child.DOKill(true);
            Destroy(child.gameObject);
        }
    }
    private void DrawLines() 
    {
        boxCollider = GetComponent<BoxCollider>();
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size;
        float cellSize = size.x / 10f;
        for (int i = 1; i < 10; i++)
        {
            Vector3 start = new Vector3(center.x - size.x / 2 + i * cellSize, 0, center.z + size.z / 2);
            Vector3 end = new Vector3(center.x - size.x / 2 + i * cellSize, 0, center.z - size.z / 2);
            Gizmos.DrawLine(start, end);
            start = new Vector3(center.x - size.x / 2, 0, center.z + size.z / 2 - i * cellSize);
            end = new Vector3(center.x + size.x / 2, 0, center.z + size.z / 2 - i * cellSize);
            Gizmos.DrawLine(start, end);
        }
    }
    private void Calculate()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;
            UpLeft = boxCollider.transform.TransformPoint(
                new Vector3(
                    center.x - size.x / 2 - Distance,
                    0f,
                    center.z + size.z / 2 + Distance
                )
            );
            UpRight = boxCollider.transform.TransformPoint(
                new Vector3(
                    center.x + size.x / 2 + Distance,
                    0f,
                    center.z + size.z / 2 + Distance
                )
            );
            BottomLeft = boxCollider.transform.TransformPoint(
                new Vector3(
                    center.x - size.x / 2 - Distance,
                    0f,
                    center.z - size.z / 2 - Distance
                )
            );
            BottomRight = boxCollider.transform.TransformPoint(
                new Vector3(
                    center.x + size.x / 2 + Distance,
                    0f,
                    center.z - size.z / 2 - Distance
                )
            );
        }
    }

    public void SpawnCar(int x, int y, int rotate, int carSize, MyColor color)
    {
        Vector3 boardCenter = boxCollider.center;
        Vector3 boardSize = boxCollider.size;
        float cellSize = boardSize.x / 10f;

        Vector3 origin = new Vector3(boardCenter.x - boardSize.x / 2, 0, boardCenter.z + boardSize.z / 2);

        Vector3 backCellPos;
        switch (rotate % 4)
        {
            case 0:
                backCellPos = origin + new Vector3(cellSize * x + cellSize / 2, 0, -cellSize * (y + 1));
                break;
            case 1:
                backCellPos = origin + new Vector3(cellSize * x, 0, -(cellSize * y + cellSize / 2));
                break;
            case 2:
                backCellPos = origin + new Vector3(cellSize * x + cellSize / 2, 0, -cellSize * y);
                break;
            case 3:
                backCellPos = origin + new Vector3(cellSize * (x + 1), 0, -(cellSize * y + cellSize / 2));
                break;
            default:
                backCellPos = origin + new Vector3(cellSize * x + cellSize / 2, 0, -(cellSize * y + cellSize / 2));
                break;
        }

        Vector3 forward = Quaternion.Euler(0f, 90f * rotate, 0f) * Vector3.forward;

        float frontMargin = cellSize * 0.1f;
        Vector3 spawnPosition = backCellPos + forward * (cellSize - frontMargin);
        Transform car = Instantiate(GetPrefab(carSize == 2), spawnPosition, Quaternion.Euler(0f, 90f * rotate, 0f), transform);
        car.GetComponentInChildren<Car>().SetColor(color);

    }
    private Transform GetPrefab(bool isTwoCells)
    {
        if (isTwoCells)
        {
            int randomIndex = UnityEngine.Random.Range(0, twoCellPrefab.Count);
            return twoCellPrefab[randomIndex];
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, threeCellPrefab.Count);
            return threeCellPrefab[randomIndex];
        }
    }
}
