using TMPro;
using UnityEngine;

[RequireComponent(typeof(Atom))]
public class RemainingLinks : MonoBehaviour
{
    [SerializeField] TMP_Text _label;
    Atom _atom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _atom = GetComponent<Atom>();
    }

    // Update is called once per frame
    void Update()
    {
        int remainingLinks = _atom.atomData.Connections - MoleculeGraph.Instance.BondsCount(_atom);
        _label.text = remainingLinks.ToString();
    }
}
