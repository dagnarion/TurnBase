using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventChannel", menuName = "EventChannel")]
public abstract class EventChannel<T> : ScriptableObject
{
    private readonly List<EventListener<T>> _eventListeners = new List<EventListener<T>>();
    public UnityEvent<T> onEventRaised;

    private bool _isRunning;
    
    public void EventRaised(T value)
    {
        if (_isRunning)
        {
            Debug.Log("RaiseEvent called while event is already running (reentrancy blocked).");
            return;
        }

        try
        {
            _isRunning = true;
            if (_eventListeners != null)
            {
                List<EventListener<T>> snapshot = new List<EventListener<T>>(_eventListeners);
                foreach (var listener in snapshot)
                {
                    try
                    {
                        listener?.Raise(value);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("RaiseEvent called but exception " + e.Message);
                    }
                }

                var hanlder = onEventRaised;
                if (hanlder != null)
                    hanlder.Invoke(value);
            }
        }
        finally
        {
            _isRunning = false;
        }
    }

    public void AddListener(EventListener<T> listener)
    {
        if (listener == null) return;
        if(!_eventListeners.Contains(listener))
            _eventListeners.Add(listener);
    }

    public void RemoveListener(EventListener<T> listener)
    {
        if (listener == null) return;
        if(_eventListeners.Contains(listener))
            _eventListeners.Remove(listener);
    }
}
