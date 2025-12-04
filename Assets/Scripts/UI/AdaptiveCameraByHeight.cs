using UnityEngine;

[ExecuteAlways]
public class AdaptiveCameraByHeight : MonoBehaviour
{
    public float safeAreaHeight = 10f;
    public Vector3 safeAreaCenter = new Vector3(0f, 0f, 2f);

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCameraPosition();
    }

    void Start()
    {
        AdjustCameraPosition();
    }

    void OnValidate()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        AdjustCameraPosition();
    }

    void AdjustCameraPosition()
    {
        if (cam == null)
            return;

        Vector3 bottomPoint = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 1f));
        Vector3 topPoint = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 1f));
        float heightAtUnitDistance = topPoint.y - bottomPoint.y;

        float t = safeAreaHeight / heightAtUnitDistance;

        cam.transform.position = safeAreaCenter - t * cam.transform.forward;
    }
}
