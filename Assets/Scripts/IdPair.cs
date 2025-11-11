using System;
// This is used in the MoleculeManager to create a map of all links 
public readonly struct IdPair : IEquatable<IdPair>
{
    public readonly int A;
    public readonly int B;

    public IdPair(int a, int b)
    {
        if (a < b) { A = a; B = b; }
        else { A = b; B = a; }
    }

    public bool Equals(IdPair other) => A == other.A && B == other.B;
    public override bool Equals(object obj) => obj is IdPair other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(A, B);
}