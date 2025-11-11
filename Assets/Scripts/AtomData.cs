using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewAtome", menuName = "Atome")]
public class AtomData : ScriptableObject
{
    [SerializeField] private string m_ID;
    public string ID => m_ID;
    public string Name;
    public string Representation;
    public int Connections;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_ID))
        {
            m_ID = Guid.NewGuid().ToString();
        }
    }
}
