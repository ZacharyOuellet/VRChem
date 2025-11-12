using System;
// This is used in the MoleculeManager to create a map of all links 
using UnityEngine;

[Serializable]
public struct IdPair : IEquatable<IdPair>
{
    public int A;
    public int B;

    public IdPair(int a, int b)
    {
        if (a < b) { A = a; B = b; }
        else { A = b; B = a; }
    }

    public bool Equals(IdPair other) => A == other.A && B == other.B;
    public override bool Equals(object obj) => obj is IdPair other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(A, B);
}