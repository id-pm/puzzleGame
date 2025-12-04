using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CarMovement), typeof(CarVisual), typeof(CarAudio))]
public class Car : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CarSettings carSettings;
    [SerializeField] private Transform modelRoot;
    public static event System.Action OnNoAvailableSpotFound;
    public MyColor CarColor;
    public int Seats;
    public int occupiedSeats { get; private set; } = 0;
    public PickupSpot CurrentSpot { get; set; }
    private bool hasLeftParking = false;
    private CarMovement carMovement;
    private CarVisual carVisual;
    private CarAudio carAudio;

    void Awake()
    {
        carMovement = GetComponent<CarMovement>();
        carVisual = GetComponent<CarVisual>();
        carAudio = GetComponent<CarAudio>();

        carMovement.Initialize(carSettings, modelRoot, this);
        carVisual.Initialize(carSettings);
        carAudio.Initialize(carSettings);
    }
    public void NotifyNoSpotFound()
    {
        OnNoAvailableSpotFound?.Invoke();
    }
    public bool AddPassenger()
    {
        if (hasLeftParking) return false;

        occupiedSeats++;
        if (occupiedSeats == Seats)
        {
            hasLeftParking = true;
            carMovement.LeaveParking(CurrentSpot);
            return true;
        }
        return false;
    }

    public void SetColor(MyColor color)
    {
        CarColor = color;
        carVisual.SetColor(color);
    }
    public void StartMove() => carMovement.StartMoving();
    public void VipArrived()
    {
        CurrentSpot = carSettings._spotsController.GetVIPSpot();
        CurrentSpot.Car = this;
        carSettings._spotsController.VIPArrived();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        LevelHendler.I.CarWasClicked(this);
    }
}
