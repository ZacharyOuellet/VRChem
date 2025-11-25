using UnityEngine;
using System.Collections.Generic;



[RequireComponent(typeof(LinkFactory))]
public class MoleculeManager : MonoBehaviour
{
    [Header("Repulsion")]
    [SerializeField] float _repulsionStrength = 0.5f;
    [SerializeField] float _minDistance = 0.4f;
    [SerializeField] Transform _lookAtTarget;

    LinkFactory _linkFactory;

    void Awake()
    {
        Application.targetFrameRate = 120;
        if (_linkFactory == null)
            _linkFactory = GetComponent<LinkFactory>();
    }

    public void AddAtom(Atom atom)
    {
        MoleculeGraph.Instance.AddAtom(atom);
        atom.SetLookAtTarget(_lookAtTarget);
    }

    public void DestroyAtom(Atom atom)
    {
        List<int> atomIdList = new();
        //double foreach to prevent modifying array while iterating 
        foreach(var linked in atom.linkedAtoms)
        {
            atomIdList.Add(linked.id);
        }
        foreach(var linked in atomIdList)
        {
            DestroyLink(atom.id, linked);
        }
        MoleculeGraph.Instance.RemoveAtom(atom.id);
        atom.gameObject.SetActive(false);
        MonoBehaviour.Destroy(atom.gameObject, 0.01f);
    }


    private void FixedUpdate()
    {
        RepulsionSystem.Apply(MoleculeGraph.Instance.Molecules, _minDistance, _repulsionStrength);
    }

    public bool CreateLink(int id1, int id2)
    {
        if (!MoleculeGraph.Instance.Atoms.TryGetValue(id1, out Atom a) ||
            !MoleculeGraph.Instance.Atoms.TryGetValue(id2, out Atom b))
        {
            Debug.LogError($"Invalid atom ids: {id1}, {id2}");
            return false;
        }

        if (!CheckLinkValidity(a, b))
            return false;

        var joint = a.GetComponent<SpringJointManager>()
                        .AddJoint(b.GetComponent<Rigidbody>());

        var visual = _linkFactory.CreateLinkObject(a.transform, b.transform, joint);

        MoleculeGraph.Instance.AddLink(a, b, visual);
        return true;
    }

    public bool AddBound(int id1, int id2)
    {

        if (!MoleculeGraph.Instance.Atoms.TryGetValue(id1, out Atom a) ||
            !MoleculeGraph.Instance.Atoms.TryGetValue(id2, out Atom b))
        {
            Debug.LogError($"Invalid atom ids: {id1}, {id2}");
            return false;
        }
        if(!MoleculeGraph.Instance.AreLinked(a, b))
        {
            Debug.LogError("Can't add a bound if there is no link");
            return false;
        }
        if (MoleculeGraph.Instance.BondsCount(a) >= a.atomData.Connections || MoleculeGraph.Instance.BondsCount(b) >= b.atomData.Connections)
        {
            Debug.Log("Atom exceeded connection limit");
            return false;
        }
        if (!MoleculeGraph.Instance.Links.TryGetValue(new IdPair(id1, id2), out MoleculeLink link)) return false;
        MoleculeLink newLink = _linkFactory.UpdateLink(link, link.Bonds + 1);
        MoleculeGraph.Instance.ChangeLink(id1, id2, newLink);

        return true;
    }

    private bool CheckLinkValidity(Atom a, Atom b)
    {
        if (a == b)
        {
            Debug.Log("Atom can't link to itself");
            return false;
        }

        if (MoleculeGraph.Instance.BondsCount(a) >= a.atomData.Connections ||
            MoleculeGraph.Instance.BondsCount(b) >= b.atomData.Connections)
        {
            Debug.Log("Atom exceeded connection limit");
            return false;
        }

        if (MoleculeGraph.Instance.AreLinked(a, b))
        {
            Debug.Log("Link already exists");
            return false;
        }

        return true;
    }

    public bool AreLinked(Atom a, Atom b)
    {
        return MoleculeGraph.Instance.AreLinked(a, b);
    }

    public void DestroyLink(int id1, int id2)
    {
        if (!MoleculeGraph.Instance.Atoms.TryGetValue(id1, out Atom a) ||
            !MoleculeGraph.Instance.Atoms.TryGetValue(id2, out Atom b))
            return;

        if (MoleculeGraph.Instance.Links.TryGetValue(new IdPair(id1, id2), out MoleculeLink link))
        {
            a.GetComponent<SpringJointManager>().RemoveJoint(link.jointRef);
            b.GetComponent<SpringJointManager>().RemoveJoint(link.jointRef);
            a.rb.linearVelocity = Vector3.zero;
            b.rb.linearVelocity = Vector3.zero;
            _linkFactory.DestroyLinkObject(link);
        }
        MoleculeGraph.Instance.RemoveLink(a, b);
    }

    public void DestroyAllLinks()
    {
        foreach (var l in MoleculeGraph.Instance.Links.Values)
            _linkFactory.DestroyLinkObject(l);

        foreach (var atom in MoleculeGraph.Instance.Atoms.Values)
        {
            atom.GetComponent<SpringJointManager>().ClearAllJoints();
            atom.rb.linearVelocity = Vector3.zero;
        }

        MoleculeGraph.Instance.ClearLinks();
    }

    public void DestroyAllMolecules()
    {
        List<int> atomsId = new List<int>();
        foreach (var atom in MoleculeGraph.Instance.Atoms)
        {
            atomsId.Add(atom.Key);
        }
        foreach(int id in atomsId)
        {
            DestroyAtom(MoleculeGraph.Instance.Atoms[id]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var atom in MoleculeGraph.Instance.Atoms.Values)
            Gizmos.DrawWireSphere(atom.transform.position, _minDistance);
    }
}
