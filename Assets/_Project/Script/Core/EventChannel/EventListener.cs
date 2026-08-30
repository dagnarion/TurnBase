using UnityEngine;
using UnityEngine.Events;

public class EventListener<T> : MonoBehaviour
{
    [SerializeField] private EventChannel<T> channel;
    [SerializeField] private UnityEvent<T> onEvent;
    public void Raise(T value)
    {
        onEvent?.Invoke(value);
    }

    public void OnEnable()
    {
        if(channel != null)
            channel.AddListener(this);
    }

    public void OnDisable()
    {
        if(channel != null)
            channel.RemoveListener(this);
    }
}
