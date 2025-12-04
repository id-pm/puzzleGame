using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings", menuName = "GameSettings/Car")]
public class CarSettings : ScriptableObject
{
    [SerializeField] public float exitOffset = -40f;
    [SerializeField] public float backUpDistance = 2f;
    [SerializeField] public float crashPunchPower = 0.3f;
    [SerializeField] public float driveDistance = 100f;
    [SerializeField] public AudioClip EngineClip;
    [SerializeField] public AudioClip DriftClip;
    [SerializeField] public AudioClip CrashClip;

    public float Speed = 1.0f;
    public float DurationRotate = 1.0f;
    public float DurationCompress = 0.7f;
    public float DurationDecompress = .3f;
    public float CompressionFactor = 0.7f;
    public float DriftEffect = 0.15f;
    public float driftAngle = 15f;       
    public float rollAngle = 30f;        
    public float rotateDuration = 0.3f;
    public List<ColorMaterialPair> colorMaterialPairs = new();
    public SpotsController _spotsController;
    public ParkingZone _parkingZone;
    public ParticleSystem[] smokes;
    public Transform smokeObject;


    public Sequence CreateCarBounceAnimation(Transform carTransform, System.Action onComplete = null)
    {
        Sequence sequence = DOTween.Sequence()
            .Join(carTransform.DOScaleX(carTransform.localScale.x + CompressionFactor, DurationCompress))
            .Join(carTransform.DOScaleZ(carTransform.localScale.z * CompressionFactor, DurationCompress))
            .Join(carTransform.DORotate(new Vector3(23f, carTransform.localRotation.eulerAngles.y), DurationCompress))
            .Append(carTransform.DOMove(carTransform.position, 1 / Speed))
            .Join(carTransform.DORotate(new Vector3(0f, carTransform.localRotation.eulerAngles.y), DurationCompress))
            .Join(carTransform.DOScaleX(carTransform.localScale.x - CompressionFactor / 4, DurationCompress))
            .Join(carTransform.DOScaleZ(carTransform.localScale.z / (CompressionFactor * 1.5f), DurationCompress))
            .Append(carTransform.DOScale(1f, DurationDecompress / 100f))
            .OnComplete(() =>
            {
                LevelHendler.State = GameState.Idle;
                onComplete?.Invoke();
            })
            .SetAutoKill(false);

        return sequence;
    }
    public void SetSmoke(Transform smoke)
    {
        smokeObject = smoke;
        smokes = smoke.GetComponentsInChildren<ParticleSystem>();
    }
}


