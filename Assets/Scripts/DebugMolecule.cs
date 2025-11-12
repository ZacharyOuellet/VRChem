using UnityEngine;

[CreateAssetMenu(fileName = "NewDebugMolecule", menuName = "DebugMolecule")]
public class DebugMolecule : ScriptableObject
{
    public GameObject[] atomPrefabs;
    public IdPair[] links;
}
