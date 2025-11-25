using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
public struct AtomEntry
{
    public AtomData atomData;
    public int count;
}

[CreateAssetMenu(fileName = "NewMolecule", menuName = "Molecule")]
public class MoleculeData : ScriptableObject
{
    [SerializeField] private string m_ID;
    public string ID => m_ID;
    public string Name;
    public string Description;
    public Sprite Sprite;
    public AtomEntry[] Atoms;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_ID))
        {
            m_ID = Guid.NewGuid().ToString();
        }
    }
}
