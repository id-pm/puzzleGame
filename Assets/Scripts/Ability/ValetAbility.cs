using UnityEngine;

public class ValetAbility : MonoBehaviour, IAbilityAction
{
    [SerializeField] private Sprite valetIcon; 
    private PassengersController _passengersController;
    private SpotsController _spotController;
    #region IAbilityAction
    public event System.Action OnActionStarted;
    public event System.Action OnActionCompleted;
    public void OnAbilityStart()
    {
        CleanSpots();
    }
    public void OnAbilityCancel()
    {
        
    }
    public string Name { get;} = "Valet";
    public string Description { get;} = "Use the valet to clean the spots.";
    public Sprite Icon => valetIcon;
    #endregion
    private void Awake()
    {
        _passengersController = FindAnyObjectByType<PassengersController>();
        _spotController = FindAnyObjectByType<SpotsController>();
        OnActionCompleted += Completed;
    }

    public void CleanSpots()
    {
        var occupiedSpots = _spotController.GetOccupiedSpots();
        if (occupiedSpots.Count == 0) return;
        OnActionStarted?.Invoke();
        _passengersController.SkipTheLine(occupiedSpots, RaiseActionCompleted);
        OnActionCompleted?.Invoke();
    }
    public void Completed()
    {
        Debug.Log("Doorman ability completed");
    }
    public void RaiseActionCompleted()
    {
        OnActionCompleted?.Invoke();
    }
}
