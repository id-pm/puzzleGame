using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private CarSettings settings;
    private Transform modelRoot;
    private Car car;
    private bool isMoving = false;
    private PickupSpot tempSpot;
    private Vector3[] pathArray;
    private Tweener pathTween;
    private Sequence driftSeq;
    private Sequence bounceSequence;
    private CarAudio carAudio;

    public void Initialize(CarSettings settings, Transform modelRoot, Car car)
    {
        this.settings = settings;
        this.modelRoot = modelRoot;
        this.car = car;
        bounceSequence = settings.CreateCarBounceAnimation(transform);
        carAudio = GetComponent<CarAudio>();
    }

    public void LeaveParking(PickupSpot fromSpot)
    {
        DOTween.Sequence()
            .OnStart(() =>
            {
                carAudio.StartEngine();
            })
            .Append(transform.DOLocalMoveZ(transform.localPosition.z - settings.backUpDistance, 0.5f))
            .Join(transform.DOLocalRotate(new Vector3(0f, -90f, 0f), 0.5f))
            .JoinCallback(() =>
            {
                carAudio.Drift();
            })
            .Append(transform.DOLocalMoveX(settings.exitOffset, 1f))
            .OnComplete(() =>
            {
                carAudio.StopEngine();
                fromSpot.Car = null;
                fromSpot.IsOccupied = false;
                Destroy(gameObject);
            })
            .Play();
    }

    public void StartMoving()
    {
        isMoving = true;
        Vector3 nextPoint = transform.position + transform.forward * settings.driveDistance;
        float duration = Vector3.Distance(transform.position, nextPoint) / settings.Speed;

        carAudio.StartEngine();

        transform.DOMove(nextPoint, duration)
            .OnComplete(() =>
            {
                LevelHendler.State = GameState.Idle;
                isMoving = false;
            })
            .Play();
    }

    private void OnWaypointChanged(int wpIndex)
    {
        if (wpIndex >= pathArray.Length) return;
        pathTween.Pause();

        Vector3 dir = (pathArray[wpIndex] - transform.position).normalized;
        float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float crossY = Vector3.Cross(transform.forward, dir).y;
        float sign = Mathf.Sign(crossY);
        float driftYaw = targetYaw + settings.driftAngle * sign;

        driftSeq?.Kill();
        settings.smokeObject.position = transform.position - transform.forward;
        settings.smokes[0].Play();

        Tweener rollTween = modelRoot.DOLocalRotate(
            new Vector3(0f, 0f, settings.rollAngle * sign),
            settings.rotateDuration
        ).SetEase(Ease.OutQuad).OnComplete(() => pathTween.Play());

        carAudio.Drift();

        driftSeq = DOTween.Sequence()
            .Join(transform.DORotate(new Vector3(0f, driftYaw, 0f), settings.rotateDuration).SetEase(Ease.Linear))
            .Join(rollTween)
            .Append(transform.DORotate(new Vector3(0f, targetYaw, 0f), settings.rotateDuration).SetEase(Ease.Linear))
            .Join(modelRoot.DOLocalRotate(Vector3.zero, settings.rotateDuration).SetEase(Ease.InQuad))
            .Play();
    }

    private void OnDriveComplete()
    {
        carAudio.StopEngine();
        LevelHendler.State = GameState.Idle;
        car.CurrentSpot = tempSpot;
        tempSpot.Car = car;
        settings._spotsController.CarArrived();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMoving || other.gameObject.layer != LayerMask.NameToLayer("Cars")) return;

        carAudio.Crash();
        settings.smokeObject.position = other.ClosestPoint(transform.position);
        settings.smokes[0].Play();

        transform.DOKill();

        other.transform
            .DOPunchPosition(transform.forward * settings.crashPunchPower, 0.3f, 10, 1)
            .SetRelative()
            .SetUpdate(true)
            .Restart();

        GameManager.Vibrate();
        isMoving = false;
        bounceSequence?.Restart();
    }
    void OnTriggerExit(Collider other)
    {
        if (!isMoving || other.gameObject.layer != LayerMask.NameToLayer("Walls")) return;

        tempSpot = settings._spotsController.GetFreeSpot();
        transform.DOKill();
        if (tempSpot == null)
        {
            bounceSequence?.Restart();
            car.NotifyNoSpotFound();
            return;
        }
        transform.SetParent(null);
        GetComponent<Collider>().enabled = false;
        int index = (int)(transform.rotation.eulerAngles.y / 90f);

        List<Vector3> pathPoints = new();
        if (transform.rotation.eulerAngles.y != 0f)
        {
            if (transform.rotation.eulerAngles.y == 180f && transform.position.x < 0f) index = 0;
            pathPoints.Add(settings._parkingZone.Angle[index]);
            index = index == 0 ? settings._parkingZone.Angle.Length : index;
            if (index == 2 || index == settings._parkingZone.Angle.Length)
            {
                pathPoints.Add(settings._parkingZone.Angle[--index]);
            }
        }

        pathPoints.Add(tempSpot.WorldBottomCenter);
        pathPoints.Add(tempSpot.WorldCenter);
        pathArray = pathPoints.ToArray();

        pathTween?.Kill();
        driftSeq?.Kill();

        pathTween = transform
            .DOPath(pathArray, settings.Speed, PathType.Linear, PathMode.Full3D)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .OnWaypointChange(OnWaypointChanged)
            .OnComplete(OnDriveComplete)
            .Play();
    }
}
