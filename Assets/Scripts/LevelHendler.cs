using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ParkingLevelGenerator;

public enum GameState { Idle, CraneReady, MoveCar}

public class LevelHendler : MonoBehaviour
{
    [SerializeField] public PassengersController _passengerController;
    [SerializeField] public ParkingZone _parkingZoneController;
    [SerializeField] private Transform smoke;
    [SerializeField] private CraneAbility crane;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private UIController UIController;

    [Header("Set count car in random mode")]
    [SerializeField] [Range(10, 20)] private int minCountCar = 20;
    [SerializeField] [Range(20, 50)] private int maxCountCar = 50;

    public static GameState State = GameState.Idle;
    private const int levelSize = 10;
    private MyColor[] colors;
    private LevelsConfig levelsConfig;
    private List<CarPlacement> _currentVehiclesLayout;

    #region Singleton
    public static LevelHendler I { get; private set; }
    private LevelHendler() { }
   
    void Awake() => I = this;
    #endregion

    public void Start()
    {
        colors = ((MyColor[])Enum.GetValues(typeof(MyColor)))
           .Where(c => c != MyColor.None && c != MyColor.Special)
           .ToArray();

        SettingsManager.ChangeSound(backgroundMusic);
        DOTween.SetTweensCapacity(500, 50);
        CarSettings carSettings = Resources.Load<CarSettings>("CarSettings");
        levelsConfig = Resources.Load<LevelsConfig>("LevelsConfig");

        _currentVehiclesLayout = GetVehiclesLayout();
        
        ApplyVehicleLayoutAndSpawnPassengers(_currentVehiclesLayout);

        carSettings.SetSmoke(smoke);
        carSettings._parkingZone = _parkingZoneController;
        carSettings._spotsController = _passengerController._spotsController;
        
        PassengersController.OnAllPassengersDelivered += OnVictory;
        Car.OnNoAvailableSpotFound += OnDefeat;
    }
    private List<CarPlacement> GetVehiclesLayout()
    {
        List<CarPlacement> vehiclesLayout = new();
        if (GameManager.currentLevel == -1)
        {
            vehiclesLayout = GenerateRandomLevel(UnityEngine.Random.Range(minCountCar, maxCountCar), levelSize, levelSize);
        }
        else
        {
            vehiclesLayout = levelsConfig.levels[GameManager.currentLevel].values;
        }
        return vehiclesLayout;
    }
    private void ApplyVehicleLayoutAndSpawnPassengers(List<CarPlacement> vehiclesLayout)
    {
        
        _parkingZoneController.ClearParkingZone();
        _passengerController.ClearPassengersList();
        _passengerController._spotsController.ClearSpots();
        int index = 0;
        foreach (var placement in vehiclesLayout)
        {
            _parkingZoneController.SpawnCar(placement.X, placement.Y, placement.Rotate, placement.Size, colors[index]);
            index = (index + 1) % colors.Length;
        }
        Debug.Log("Count " + _parkingZoneController.transform.childCount);
        _passengerController.GeneratePassengersList();
    }
    public void LoadNextLevel()
    {
        if(GameManager.currentLevel != -1)
        {
            GameManager.currentLevel++;
            if (GameManager.currentLevel >= levelsConfig.levels.Count)
            {
                return;
            }
        }
        ApplyVehicleLayoutAndSpawnPassengers(GetVehiclesLayout());
        UIController.HideResult();
    }
    public void RestartLevel()
    {
        ApplyVehicleLayoutAndSpawnPassengers(_currentVehiclesLayout);
        UIController.HideResult();
    }
    public void OpenMenu()
    {
        GameManager.LoadMenu();
    }
    public void OnVictory()
    {
        Debug.Log("Victory!");
        GameManager.UnlockLevel();
        UIController.ShowResult(true);
    }

    public void OnDefeat()
    {
        UIController.ShowResult(false);
        Debug.Log("Defeat! No available parking spots for the car.");
    }

    public void CarWasClicked(Car car)
    {
        switch(State)
        {
            case GameState.CraneReady:
                crane.MoveTargetToDestination(car.transform);
                break;
            case GameState.Idle:
                State = GameState.MoveCar;
                car.StartMove();
                break;
        }
    }
}
