namespace Bleckers;

public class AutoProp<T> {
    public event EventHandler<T>? ValueChanged;

    public AutoProp(T initialValue) {
        _value = initialValue;
    }

    private T _value;
    public T Value {
        get { return _value; }
        set {
            _value = value;
            ValueChanged?.Invoke(this, Value);
        }
    }
}
