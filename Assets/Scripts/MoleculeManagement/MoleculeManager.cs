using UnityEngine;


[RequireComponent(typeof(LinkFactory))]
public class MoleculeManager : MonoBehaviour
{
    [Header("Repulsion")]
    [SerializeField] float _repulsionStrength = 0.5f;
    [SerializeField] float _minDistance = 0.4f;

    MoleculeGraph _graph = new();
    LinkFactory _linkFactory;

    void Awake()
    {
        if (_linkFactory == null)
            _linkFactory = GetComponent<LinkFactory>();
    }

    public void AddAtom(Atom atom)
    {
        _graph.AddAtom(atom);
    }

    private void FixedUpdate()
    {
        RepulsionSystem.Apply(_graph.Molecules, _minDistance, _repulsionStrength);
    }

    public bool CreateLink(int id1, int id2)
    {
        if (!_graph.Atoms.TryGetValue(id1, out Atom a) ||
            !_graph.Atoms.TryGetValue(id2, out Atom b))
        {
            Debug.LogError($"Invalid atom ids: {id1}, {id2}");
            return false;
        }

        if (!CheckLinkValidity(a, b))
            return false;

        var joint = a.GetComponent<SpringJointManager>()
                        .AddJoint(b.GetComponent<Rigidbody>());

        var visual = _linkFactory.CreateLinkObject(a.transform, b.transform, joint);

        _graph.AddLink(a, b, visual);
        return true;
    }

    private bool CheckLinkValidity(Atom a, Atom b)
    {
        if (a == b)
        {
            Debug.LogWarning("Atom can't link to itself");
            return false;
        }

        if (a.linkedAtoms.Count >= a.atomData.Connections ||
            b.linkedAtoms.Count >= b.atomData.Connections)
        {
            Debug.LogWarning("Atom exceeded connection limit");
            return false;
        }

        if (_graph.AreLinked(a, b))
        {
            Debug.LogWarning("Link already exists");
            return false;
        }

        return true;
    }

    public void DestroyLink(int id1, int id2)
    {
        if (!_graph.Atoms.TryGetValue(id1, out Atom a) ||
            !_graph.Atoms.TryGetValue(id2, out Atom b))
            return;

        if (_graph.Links.TryGetValue(new IdPair(id1, id2), out MoleculeLink link))
        {
            a.GetComponent<SpringJointManager>().RemoveJoint(link.jointRef);
            b.GetComponent<SpringJointManager>().RemoveJoint(link.jointRef);

            _linkFactory.DestroyLinkObject(link);
        }

        _graph.RemoveLink(a, b);
    }

    public void DestroyAllLinks()
    {
        foreach (var l in _graph.Links.Values)
            _linkFactory.DestroyLinkObject(l);

        foreach (var atom in _graph.Atoms.Values)
            atom.GetComponent<SpringJointManager>().ClearAllJoints();

        _graph.ClearLinks();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var atom in _graph.Atoms.Values)
            Gizmos.DrawWireSphere(atom.transform.position, _minDistance);
    }
}
