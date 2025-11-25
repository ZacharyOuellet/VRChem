using System;
using UnityEngine;


[Serializable]
public struct DebugLink
{
    public int nBounds;
    public IdPair pair;
}

[CreateAssetMenu(fileName = "NewDebugMolecule", menuName = "DebugMolecule")]
public class DebugMolecule : ScriptableObject
{

    public GameObject[] atomPrefabs;
    public DebugLink[] bounds;
}
