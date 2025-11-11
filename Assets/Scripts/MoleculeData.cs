using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewMolecule", menuName = "Molecule")]
public class MoleculeData : ScriptableObject
{
    [SerializeField] private string m_ID;
    public string ID => m_ID;
    public string Name;
    public string Description;
    public Sprite Sprite;
    public AtomeData[] Atomes;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_ID))
        {
            m_ID = Guid.NewGuid().ToString();
        }
    }
}
