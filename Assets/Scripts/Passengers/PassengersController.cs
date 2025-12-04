using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassengersController : MonoBehaviour
{
    [SerializeField] private float _distance = 0.2f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float durationP = 0.5f;
    [SerializeField] private Transform _wayObject;
    [SerializeField] public SpotsController _spotsController;
    [SerializeField] private List<ColorMaterialPair> colorMaterialPairs = new();
    
    public Passenger PassengerPrefab;
    public static MyColor CurrentColor { get; private set;}
    private static List<MyColor> PassengersColorList = new();
    private static List<Vector3> wayPoints = new();
    private List<Passenger> activePassengers = new();
    private Stack<Passenger> passengerPool = new();
    private List<Vector3> positions = new();
    private Vector3 pos;
    private bool wait = false;

    #region PassengerCount
    public static event Action OnAllPassengersDelivered;
    public static event Action<int> OnChangePassengersCount;
    public static event Action<Material> OnChangeCurrentColor;
    private ObservableProperty<int> passengerCount = new();
    private void OnPassengerCountChanged(int count)
    {
        OnChangePassengersCount?.Invoke(count);
        if (count == 0)
            OnAllPassengersDelivered?.Invoke();
    }
    #endregion

    private static class IsMoving
    {
        public static bool Value { get; private set; }
        public static bool Set(bool value) => Value = value;
    }

    private void Awake()
    {
        for (int i = 0; i < _wayObject.childCount; i++)
            wayPoints.Add(_wayObject.GetChild(i).position);

        pos = wayPoints[^1];
        wait = true;
    }

    private void Start()
    {
        _spotsController.OnVIPArrived += VipArrived;
    }

    void Update()
    {
        if (activePassengers.Count == 0) return;

        float distance = Vector3.Distance(activePassengers[0].Position, positions[0]);
        if (distance > _distance)
        {
            if (PassengersColorList.Count > 0)
                AddPassengerToLine();

            Vector3 direction = (activePassengers[0].Position - positions[0]).normalized;
            positions.Insert(0, positions[0] + direction * _distance);
            positions.RemoveAt(positions.Count - 1);
        }

        if (activePassengers.Count < 2) return;
        for (int i = 1; i < positions.Count; i++)
        {
            activePassengers[i].SetPosition(Vector3.Lerp(positions[i], positions[i - 1], distance / _distance));
            activePassengers[i].LookAtDirection(positions[i - 1] - positions[i]);
        }
    }

    public void GeneratePassengersList()
    {
        Transform parking = LevelHendler.I._parkingZoneController.transform;
        for (int i = 0; i < parking.childCount; i++)
        {
            Car car = parking.GetChild(i).GetComponentInChildren<Car>();
            for (int j = 0; j < car.Seats; j++)
                PassengersColorList.Add(car.CarColor);
            //Debug.Log(car.Seats + " seats for " + car.CarColor + " color");
        }
        passengerCount = new(PassengersColorList.Count);
        passengerCount.OnValueChanged += OnPassengerCountChanged;
        passengerCount.ForceNotify();
        BeginPassengerMovement();
    }

    private void BeginPassengerMovement()
    {
        AnimatePassengers(true);
        AddPassengerToLine();
        SetCurrentColor();
        _spotsController.OnCarArrived += WaitCar;

        activePassengers[0].MoveAlongPath(wayPoints, duration)
            .OnComplete(() =>
            {
                AnimatePassengers(false);
                wait = false;
                WaitCar();
            })
            .Play();
    }

    private void WaitCar()
    {
        if (wait) return;

        PickupSpot spot = _spotsController.FindCarByColor(CurrentColor);
        if (spot == null) return;

        wait = true;
        Passenger firstPassenger = activePassengers[0];
        RemovePassenger(firstPassenger);

        DOTween.Sequence()
            .Append(firstPassenger.MoveTo(spot.WorldCenter, durationP))
            .Insert(durationP / 2f, firstPassenger.LocalJump(spot.WorldCenter, 0.5f, 1, durationP / 2f))
            .OnComplete(() =>
            {
                spot.Car.AddPassenger();
                ReturnToPool(firstPassenger);
                wait = false;
                WaitCar();
            })
            .Play();
    }
    public void SkipTheLine(List<PickupSpot> spots, Action actionComplete)
    {
        wait = true;
        Sequence sequence = DOTween.Sequence()
           .OnComplete(() => 
           {
               actionComplete?.Invoke();
               wait = false;
           });
        spots.ForEach(spot =>
        {
            if (spot.Car != null)
            {
                sequence.Append(SkipTheLineSequence(spot));
            }
        });
        sequence.Play();
    }
    private Sequence SkipTheLineSequence(PickupSpot spot)
    {
        MyColor carColor = spot.Car.CarColor;
        Material mat = GetMaterialForColor(carColor);
        int freeSeats = spot.Car.Seats - spot.Car.occupiedSeats;

        List<Passenger> matchingPassengers = activePassengers
            .Where(p => p.IsMaterialEqual(mat))
            .Take(freeSeats)
            .ToList();

        int missing = freeSeats - matchingPassengers.Count;
        for (int i = 0; i < missing; i++)
            matchingPassengers.Add(AddPassengerToLine(carColor, false));

        Sequence seq = DOTween.Sequence();
        foreach (var passenger in matchingPassengers)
        {
            seq.Append(passenger.MoveTo(spot.WorldCenter, durationP)
                .OnStart(() =>
                {
                    RemovePassenger(passenger, false);
                    if (PassengersColorList.Count > 0) AddPassengerToLine(needNewPos: false);
                    else positions.RemoveAt(positions.Count - 1);
                })
                .OnComplete(() =>
                {
                    AnimatePassengers(false);
                    spot.Car.AddPassenger();
                    ReturnToPool(passenger);

                }));
        }
        return seq;
    }
    public void VipArrived()
    {
        wait = true;
        PickupSpot spot = _spotsController.GetVIPSpot();
        if (spot == null) return;
        SkipTheLineSequence(spot)
            .AppendCallback(() =>
            {
                LevelHendler.State = GameState.Idle;
                wait = false;
                WaitCar();
            })
            .Play();
    }

    private Passenger AddPassengerToLine(MyColor color = MyColor.None, bool needNewPos = true)
    {
        Passenger passenger = passengerPool.Count > 0
            ? passengerPool.Pop()
            : Instantiate(PassengerPrefab, wayPoints[0], Quaternion.identity, transform);

        passenger.gameObject.SetActive(true);
        passenger.transform.position = wayPoints[0];
        passenger.Init();

        MyColor actualColor = (color == MyColor.None)
            ? PassengersColorList[^1]
            : color;

        if (color == MyColor.None)
            PassengersColorList.RemoveAt(PassengersColorList.Count - 1);
        else
            PassengersColorList.Remove(PassengersColorList.FindLast(x => x == color));

        Material mat = GetMaterialForColor(actualColor);
        passenger.SetMaterial(mat);
        passenger.SetColor(actualColor);
        passenger.SetMoving(IsMoving.Value);

        activePassengers.Add(passenger);
        if (needNewPos) positions.Add(passenger.Position);
        return passenger;
    }

    private void AnimatePassengers(bool value)
    {
        IsMoving.Set(value);
        foreach (var p in activePassengers)
            p.SetMoving(value);
    }

    private void SetCurrentColor()
    {
        if (activePassengers.Count == 0) return;

        Material mat = activePassengers[0].GetMaterial();
        CurrentColor = colorMaterialPairs.FirstOrDefault(p => p.material == mat).color;
        OnChangeCurrentColor?.Invoke(mat);
    }

    private void RemovePassenger(Passenger p, bool removeWithPos = true)
    {
        wait = true;
        activePassengers.Remove(p);

        if (removeWithPos) positions.RemoveAt(0);

        SetCurrentColor();
        passengerCount.Value--;

        if (activePassengers.Count == 0) return;
        else AnimatePassengers(true);

        activePassengers[0].MoveTo(pos, durationP / 2f)
            .OnComplete(() => {
                //GameManager.Vibrate();
                AnimatePassengers(false);
                if(activePassengers.Count == 0)                     
                    OnAllPassengersDelivered?.Invoke();
                })
            .Play();
    }
    private void ReturnToPool(Passenger p)
    {
        p.gameObject.SetActive(false);
        passengerPool.Push(p);
    }

    private Material GetMaterialForColor(MyColor color)
    {
        return colorMaterialPairs.FirstOrDefault(pair => pair.color == color).material;
    }

    internal void ClearPassengersList()
    {
        AnimatePassengers(false);
        IsMoving.Set(false);
        wait = true;

        foreach (var passenger in activePassengers)
        {
            passenger.transform.DOKill();
            ReturnToPool(passenger);
        }
        activePassengers.Clear();
        positions.Clear();
        PassengersColorList.Clear();
        if (passengerCount != null)
            passengerCount.OnValueChanged -= OnPassengerCountChanged;
        passengerCount = new(0);

        _spotsController.OnCarArrived -= WaitCar;

    }
}
