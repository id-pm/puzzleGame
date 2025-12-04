using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SpotsController : MonoBehaviour
{
    private PickupSpot[] parkingSpots;
    public event Action OnCarArrived;
    public event Action OnVIPArrived;
    [SerializeField] private Image imageLock;
    [SerializeField] private int countVip = 1;
    [SerializeField] private int countLock = 2;

    void Awake()
    {
    }
    public void ClearSpots()
    {
        Initialize();
    }
    public List<PickupSpot> GetOccupiedSpots()
    {
        return parkingSpots.Where(x => x.IsOccupied).ToList();
    }
    public PickupSpot GetFreeSpot()
    {
        PickupSpot p = parkingSpots.Where(x => !x.IsOccupied && x.IsActive && x.IsVIP == false).FirstOrDefault();
        if (p != null) p.IsOccupied = true;
        return p;
    }
    public void Initialize()
    {
        parkingSpots = transform.GetComponentsInChildren<PickupSpot>();
        foreach (var spot in parkingSpots)
        {
            spot.IsOccupied = false;
            spot.IsActive = true;
            spot.IsVIP = false;
            if(spot.Car != null)
            {
                Destroy(spot.Car.gameObject);
            }
        }
        for(int i = 1; i <= countLock; i++)
        {
            var spot = parkingSpots[^i];
            spot.IsActive = false;
            var text = spot.GetComponentInChildren<TextMeshProUGUI>();
            var border = spot.GetComponentInChildren<Image>().color;
            border = Color.gray;
            text.color = Color.grey;
            text.text = "UNLOCK";
            var button = spot.GetComponent<Button>();
            var image = Instantiate(imageLock, spot.GetComponentInChildren<Canvas>().transform);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Destroy(image);
                border = Color.white;
                text.text = "";
                spot.IsActive = true;
            });
        }
        for (int i = 0; i < countVip; i++)
        {
            var spot = parkingSpots[i];
            spot.IsVIP = true;
            var text = spot.GetComponentInChildren<TextMeshProUGUI>();
            spot.GetComponentInChildren<Image>().color = Color.yellow;
            text.color = Color.yellow;
            text.text = "VIP";
        }
    }
    public PickupSpot GetVIPSpot()
    {
        Debug.Log($"GetVIPSpot: {parkingSpots.Length} spots found");
        PickupSpot p = parkingSpots.Where(x => x.IsVIP).FirstOrDefault();
        Debug.Log($"VIP spot found: {p.name}");
        return p;
    }
    public void CarArrived()
    {
        OnCarArrived?.Invoke();
    }
    public void VIPArrived()
    {
        OnVIPArrived?.Invoke();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Correctness", "UNT0008:Null propagation on Unity objects", Justification = "<Pending>")]
    public PickupSpot FindCarByColor(MyColor color)
    {
        PickupSpot p = parkingSpots.Where(x => x.Car?.CarColor == color && x.Car?.occupiedSeats < x.Car?.Seats).FirstOrDefault();
        return p;
    }
}
