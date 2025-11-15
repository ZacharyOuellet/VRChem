using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using Oculus.Interaction;


// The int ini the event is the identifier of the interractor. 
// In our case, the interractor can be both a controller or a hand.
// We can now wich ones are selected because of it
[System.Serializable]
public class DirectedPointerEvent : UnityEvent<GameObject, PointerEvent> { }

public class GrabMediator : MonoBehaviour
{
    public static GrabMediator Instance { get; private set; }

    [Header("Events")]
    public DirectedPointerEvent DirectedPtrEvent;

    // Store delegates
    private readonly Dictionary<object, (Delegate grab, Delegate release)> _handlers =
        new Dictionary<object, (Delegate, Delegate)>();

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ------------------------- EVENT EMISSION -------------------------

    public void EmitGrab(GameObject obj, PointerEvent interractorId)
        => DirectedPtrEvent?.Invoke(obj.gameObject, interractorId);
}
