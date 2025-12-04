using System;

[Serializable]
public class ObservableProperty<T>
{
    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    public event Action<T> OnValueChanged;

    public ObservableProperty() { }
    public ObservableProperty(T initialValue) => _value = initialValue;

    public void ForceNotify() => OnValueChanged?.Invoke(_value);

    public override string ToString() => _value?.ToString();
    public static implicit operator T(ObservableProperty<T> prop) => prop.Value;
}
