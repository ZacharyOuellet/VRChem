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
        Application.targetFrameRate = 120;
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

    public bool AddBound(int id1, int id2)
    {

        if (!_graph.Atoms.TryGetValue(id1, out Atom a) ||
            !_graph.Atoms.TryGetValue(id2, out Atom b))
        {
            Debug.LogError($"Invalid atom ids: {id1}, {id2}");
            return false;
        }
        if(!_graph.AreLinked(a, b))
        {
            Debug.LogError("Can't add a bound if there is no link");
            return false;
        }
        if (_graph.BondsCount(a) >= a.atomData.Connections || _graph.BondsCount(b) >= b.atomData.Connections)
        {
            Debug.Log("Atom exceeded connection limit");
            return false;
        }
        if (!_graph.Links.TryGetValue(new IdPair(id1, id2), out MoleculeLink link)) return false;
        MoleculeLink newLink = _linkFactory.UpdateLink(link, link.Bonds + 1);
        _graph.ChangeLink(id1, id2, newLink);

        return true;
    }

    private bool CheckLinkValidity(Atom a, Atom b)
    {
        if (a == b)
        {
            Debug.Log("Atom can't link to itself");
            return false;
        }

        if (_graph.BondsCount(a) >= a.atomData.Connections ||
            _graph.BondsCount(b) >= b.atomData.Connections)
        {
            Debug.Log("Atom exceeded connection limit");
            return false;
        }

        if (_graph.AreLinked(a, b))
        {
            Debug.Log("Link already exists");
            return false;
        }

        return true;
    }

    public bool AreLinked(Atom a, Atom b)
    {
        return _graph.AreLinked(a, b);
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
            a.rb.linearVelocity = Vector3.zero;
            b.rb.linearVelocity = Vector3.zero;
            _linkFactory.DestroyLinkObject(link);
        }
        _graph.RemoveLink(a, b);
    }

    public void DestroyAllLinks()
    {
        foreach (var l in _graph.Links.Values)
            _linkFactory.DestroyLinkObject(l);

        foreach (var atom in _graph.Atoms.Values)
        {
            atom.GetComponent<SpringJointManager>().ClearAllJoints();
            atom.rb.linearVelocity = Vector3.zero;
        }

        _graph.ClearLinks();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var atom in _graph.Atoms.Values)
            Gizmos.DrawWireSphere(atom.transform.position, _minDistance);
    }
}
