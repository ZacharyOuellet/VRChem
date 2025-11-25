using UnityEngine;

public class MoleculeLink : MonoBehaviour
{
    public int Bonds
    {
        get { return _bonds; }
        private set { _bonds = value; }
    }

    [SerializeField] private int _bonds;
    [SerializeField] Transform _atom1;
    [SerializeField] Transform _atom2;
    [SerializeField] float _diameter;
    [SerializeField] float _stretchFactor;
    [SerializeField] float _maxDiameter = 1f;
    [HideInInspector] public SpringJoint jointRef { get; private set; } = null;

    public void Init(Transform atomA, Transform atomB, SpringJoint joint)
    {
        (_atom1, _atom2, jointRef) = (atomA, atomB, joint);
    }

    public void Init(MoleculeLink link)
    {
        (_atom1, _atom2, jointRef) = (link._atom1, link._atom2, link.jointRef);
    }

    void Update()
    {
        if (_atom1 == null || _atom2 == null)
            return;

        Vector3 pos1 = _atom1.position;
        Vector3 pos2 = _atom2.position;
        Vector3 mid = (pos1 + pos2) / 2f;

        transform.position = mid;

        Vector3 dir = pos2 - pos1;
        transform.up = dir.normalized;


        float distance = dir.magnitude;
        float diameter = Mathf.Min(_diameter * _stretchFactor / distance, _maxDiameter);
        for (int i =0; i< transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.localScale = new Vector3(diameter, distance/2f , diameter);
        };
    }
}
