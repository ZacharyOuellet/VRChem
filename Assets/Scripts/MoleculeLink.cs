using UnityEngine;

public class MoleculeLink : MonoBehaviour
{

    [SerializeField] Transform _atom1;
    [SerializeField] Transform _atom2;
    [SerializeField] float _diameter;
    [SerializeField] float _stretchFactor;
    [HideInInspector] public SpringJoint jointRef { get; private set; } = null;

    public void Init(Transform atomA, Transform atomB, SpringJoint joint)
    {
        (_atom1, _atom2, jointRef) = (atomA, atomB, joint);
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
        transform.localScale = new Vector3(_diameter*_stretchFactor / distance, distance / 2f, _diameter*_stretchFactor/distance);
    }
}
