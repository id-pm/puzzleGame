using DG.Tweening;
using UnityEngine;

public class CraneAbility : MonoBehaviour, IAbilityAction
{
    [Header("Сrane elements")]
    [SerializeField] private Transform craneBase;
    [SerializeField] private Transform craneRolley;
    [SerializeField] private Transform craneHook;
    [SerializeField] private Transform craneCables;
    [SerializeField] private Transform craneMagnet;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceRotate;
    [SerializeField] private AudioSource audioSourceLifting;
    [SerializeField] private AudioClip acMove;
    [SerializeField] private AudioClip acFinishedMove;
    [SerializeField] private AudioClip acLifting;
    [SerializeField] private AudioClip acFinishedLifting;
    [SerializeField] private AudioClip acMagnet;
    [Header("Parameters")]
    [SerializeField] private float speedUnitsPerSecond;
    [SerializeField] private Sprite craneIcon;

    private Sequence _cableSequence;
    private Sequence _onStartPosition;
    private Transform _lastTarget;
    private PickupSpot _pickupSpot;
    private float magnetBottomY;

    #region IAbilityAction
    public event System.Action OnActionStarted;
    public event System.Action OnActionCompleted;
    public void OnAbilityStart()
    {
        LevelHendler.State = GameState.CraneReady;
    }
    public void OnAbilityCancel()
    {
        if (LevelHendler.State == GameState.CraneReady)
        {
            LevelHendler.State = GameState.Idle;
        }
    }
    public string Name { get;} = "Crane";
    public string Description { get;} = "Select a car to transport it to the VIP spot.";

    public Sprite Icon => craneIcon;
    #endregion

    private void Start()
    {
        _onStartPosition = DOTween.Sequence()
            .SetAutoKill(false)
            .AppendCallback(() => 
            {
                audioSourceRotate.PlayOneShot(acMove);
            })
            .Append(craneBase.transform.DOLocalRotate(Vector3.zero, 1f))
            .Join(craneRolley.DOLocalRotate(Vector3.zero, 1f))
            .Join(craneRolley.DOLocalMoveX(0f, 1f))
            .Join(craneHook.DOLocalMove(Vector3.zero, 1f))
            .Join(craneCables.DOScaleY(1f, 1f))
            .OnComplete(() => 
            {
                audioSourceRotate.Stop();
                audioSourceRotate.PlayOneShot(acFinishedMove);
            });
        magnetBottomY = craneMagnet.position.y - craneMagnet.GetComponent<BoxCollider>().bounds.extents.y;
        _pickupSpot = FindAnyObjectByType<SpotsController>().GetVIPSpot();
    }

    private Sequence RotateCraneAndRollSequence(Vector3 targetPosition, Sequence sec = null)
    {
        float desiredLocalX = 0f;
        Vector3 direction = targetPosition - craneBase.transform.position;
        direction.y = 0f;

        Quaternion lookRot = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion targetRot = lookRot * Quaternion.Euler(0f, 90f, 0f);

        Sequence sequence = DOTween.Sequence()
        .JoinCallback(() => 
        {
            audioSourceRotate.PlayOneShot(acMove);
        })
        .Append(
            craneBase.transform.DORotateQuaternion(targetRot, 1f)
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                Vector3 localPoint = craneBase.transform.InverseTransformPoint(targetPosition);
                desiredLocalX = localPoint.x;
                craneRolley.DOLocalMoveX(desiredLocalX, 1f)
                     .SetEase(Ease.Linear)
                     .Play();
                audioSourceRotate.Stop();
                audioSourceRotate.PlayOneShot(acFinishedMove);
            }))
        .AppendInterval(1f);
        return sequence;
    }
    private Sequence CableMoveSequence(float heightDifference, float duration)
    {
        return DOTween.Sequence()
            .OnStart(() => {
                audioSourceLifting.PlayOneShot(acLifting);
            })
            .SetUpdate(UpdateType.Fixed)
            .SetAutoKill(false)
            .Join(craneCables
                .DOScaleY(heightDifference, duration)
                .SetRelative()
                .SetEase(Ease.Linear))
            .Join(craneHook
                .DOMoveY(-heightDifference, duration)
                .SetRelative()
                .SetEase(Ease.Linear))
            .OnComplete(() => {
                audioSourceLifting.Stop();
                audioSourceLifting.PlayOneShot(acFinishedLifting);
                //AudioSource.PlayClipAtPoint(acFinishedLifting, Vector3.zero, audioSourceLifting.volume);
            });
    }
    public void MoveTargetToDestination(Transform target)
    {
        OnActionStarted?.Invoke();
        LevelHendler.State = GameState.MoveCar;
        _lastTarget = target;

        Vector3 targetPosition = target.GetComponent<BoxCollider>().bounds.center;

        var targetCol = target.GetComponent<BoxCollider>();

        float targetTopY = targetCol.bounds.center.y + targetCol.bounds.extents.y;

        float heightDifference = magnetBottomY - targetTopY;

        float angleY = 0f;
        Debug.Log($"Target position: {targetPosition}");
        DOTween.Sequence()
            .Join(RotateCraneAndRollSequence(targetPosition))
            .Join(CableMoveSequence(heightDifference, speedUnitsPerSecond))
            .AppendCallback(() => _lastTarget.SetParent(craneMagnet))
            .JoinCallback(() => {
                AudioSource.PlayClipAtPoint(acMagnet, Vector3.zero, audioSourceLifting.volume);
                GameManager.Vibrate();
            })
            .Append(CableMoveSequence(-2f, 0.5f))
            .Append(RotateCraneAndRollSequence(_pickupSpot.transform.position))
            .AppendCallback(() => {
                Vector3 dir = _pickupSpot.WorldCenter - _pickupSpot.WorldBottomCenter;
                angleY = Mathf.DeltaAngle(_lastTarget.rotation.eulerAngles.y, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
                craneRolley
                .DOLocalRotate(new Vector3(0f, angleY, 0f), 1f)
                .SetEase(Ease.Linear)
                .Play();
            })
            .Append(CableMoveSequence(2f, 1f))
            .AppendCallback(() => {
                _lastTarget.SetParent(null);
                _lastTarget.GetComponent<Car>().VipArrived();
                _onStartPosition.Restart();
            })
            .OnComplete(() =>
            {
                LevelHendler.State = GameState.Idle;
                OnActionCompleted?.Invoke();
            })
            .Play();
    }
}
