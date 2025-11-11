using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{
    [SerializeField] public AtomData atomData;
    public int id { get; private set; }

    private static int nextId = 0;
    void Awake()
    {
        id = ++nextId;
    }

    public List<Atom> linkedAtoms = new();
}
