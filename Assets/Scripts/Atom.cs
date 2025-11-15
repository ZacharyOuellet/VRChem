using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Atom : MonoBehaviour
{
    [SerializeField] public AtomData atomData;
    public int id { get; private set; }
    [HideInInspector]public Rigidbody rb;

    Grabbable _grabbable;
    GrabMediator _grabMediator;


    private static int nextId = 0;
    void Awake()
    {
        id = ++nextId;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _grabbable = GetComponent<Grabbable>();
        _grabMediator = GrabMediator.Instance;
        if (_grabbable != null) _grabbable.WhenPointerEventRaised += Grabbable_WhenPointerEventRaised;
    }

    private void Grabbable_WhenPointerEventRaised(PointerEvent ptrEvent)
    {
        _grabMediator.EmitGrab(gameObject, ptrEvent);
    }

    public List<Atom> linkedAtoms = new();
}
