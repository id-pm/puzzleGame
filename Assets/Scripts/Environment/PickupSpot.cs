using UnityEngine;

public class PickupSpot : MonoBehaviour
{
    [SerializeField] private SpotSettings _parkSet;
    [SerializeField] public bool IsActive = false;
    [SerializeField] public bool IsVIP = false;
    public Car Car = null;
    public bool IsOccupied = false;

    private BoxCollider boxCollider;
    public Vector3 WorldBottomCenter {
        get;
        private set;
    }
    public Vector3 WorldCenter {
        get;
        private set;
    }
    
    void Start()
    {
        WorldBottomCenter = GetBottomCenterCoord();
        WorldCenter = GetParkingCoord();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetBottomCenterCoord(), 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetParkingCoord(), 0.1f);
    }
    private Vector3 GetBottomCenterCoord()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            Vector3 localBottomRight = new Vector3(
                center.x,
                center.y,
                center.z - size.z / 2
            );

            return boxCollider.transform.TransformPoint(localBottomRight);
        }
        return Vector3.zero;
    }
    private Vector3 GetParkingCoord()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            Vector3 localParkingCenter = new Vector3(
                center.x,
                center.y,
                center.z + size.z / 2 - _parkSet._parkingFreeSpace
            );

            return boxCollider.transform.TransformPoint(localParkingCenter);
        }
        return Vector3.zero;
    }
}
