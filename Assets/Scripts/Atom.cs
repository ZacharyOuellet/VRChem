using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Atom : MonoBehaviour
{
    [SerializeField] public AtomData atomData;
    public int id { get; private set; }
    public Rigidbody rb;

    private static int nextId = 0;
    void Awake()
    {
        id = ++nextId;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public List<Atom> linkedAtoms = new();
}
