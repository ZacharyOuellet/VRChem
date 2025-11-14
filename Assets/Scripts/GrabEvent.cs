using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

[System.Serializable]
public class GrabEvent : UnityEvent<GameObject, Hand> { }
