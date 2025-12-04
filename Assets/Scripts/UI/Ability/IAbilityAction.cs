using UnityEngine;

public interface IAbilityAction
{
    event System.Action OnActionCompleted;
    event System.Action OnActionStarted;
    void OnAbilityStart();
    void OnAbilityCancel();
    public string Name { get;}
    public string Description { get;}
    public Sprite Icon { get;}
}

