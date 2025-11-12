using Meta.WitAi;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MoleculeManager : MonoBehaviour
{
    [SerializeField] GameObject linkPrefab;

    private Dictionary<int, Atom> atoms = new();
    private Dictionary<IdPair, MoleculeLink> links = new();

    [Header("Repulsion")]
    [SerializeField] float repulsionStrength = 0.5f;
    [SerializeField] float minDistance = 0.4f;


    #region Debugging
    // DEBUGGING SECTION
    [Space(5)]
    [Header("Debugging")]
    [SerializeField] DebugMolecule debugMolecule = null;

    [Header("Adding a link")]
    [SerializeField] Atom debugAtom1 = null;
    [SerializeField] Atom debugAtom2 = null;
    [SerializeField] bool createLink;

    [Header("Destroying a link")]
    [SerializeField] int deleteId1 = -1;
    [SerializeField] int deleteId2 = -1;
    [SerializeField] bool destroyLink;

    [SerializeField] bool destroyAllLinks;

    private void createDebugMolecule()
    {
        if (debugMolecule == null) return;

        int i = 0;
        int[] idsTranslation = new int[debugMolecule.atomPrefabs.Count()];
        foreach (GameObject atom in debugMolecule.atomPrefabs)
        {
            GameObject newAtom = Instantiate(atom, transform);
            newAtom.transform.position += Random.insideUnitSphere;
            idsTranslation[i++] = newAtom.GetComponent<Atom>().id;
            AddAtom(newAtom.GetComponent<Atom>());
        }

        foreach(IdPair link in debugMolecule.links)
        {
            CreateLink(idsTranslation[link.A], idsTranslation[link.B]);
        }
    }

    private void debugCreateLink()
    {
        if(debugAtom1 == null || debugAtom2 == null) return;

        AddAtom(debugAtom1);
        AddAtom(debugAtom2);
        CreateLink(debugAtom1.id, debugAtom2.id);
        debugAtom1 = null;
        debugAtom2 = null;
    }

    private void debugDestroyLink()
    {
        if(deleteId1 >= 0 && deleteId2 >= 0)
        {
            DestroyLink(deleteId1, deleteId2);
        }
    }
    // END OF DEBUGGING SECTION
    #endregion Debugging

    private void Start()
    {
        createDebugMolecule();
    }

    private void Update()
    {
        ApplyRepulsion();

        // The rest is for debugging and testing
        if (createLink)
        {
            debugCreateLink();
            createLink = false;
        }
        if (destroyLink)
        {
            debugDestroyLink();
            destroyLink = false;
        }
        if (destroyAllLinks)
        {
            DestroyAllLinks();
            destroyAllLinks = false;
        }
    }

    public void AddAtom(Atom atom)
    {
        if (atoms.ContainsKey(atom.id)) return;
        atoms.Add(atom.id, atom);
    }

    public bool CheckLinkValidity(Atom atom1, Atom atom2)
    {
        if (atom1.linkedAtoms.Count >= atom1.atomData.Connections )
        {
            Debug.LogWarning("Not able to create link because of atom" + atom1.id + "limits");
            // Maybe emit event for player feedback
            return false;
        }
        if (atom2.linkedAtoms.Count >= atom2.atomData.Connections)
        {
            Debug.LogWarning("Not able to create link because of atom" + atom2.id + "limits");
            // Maybe emit event for player feedback
            return false;
        }
        if (links.TryGetValue(new IdPair(atom1.id, atom2.id), out MoleculeLink _))
        {
            Debug.LogWarning("Not able to create link because link already exists");
            // Maybe emit event for player feedback
            return false;
        }
        // maybe more stuff to do down he line
        return true;
    }


    public void CreateLink(int id1, int id2)
    {
        if (id1 == id2)
        {
            Debug.LogError("Atom cant link to itself");
            return;
        }
        if(!atoms.ContainsKey(id1) || !atoms.ContainsKey(id2))
        {
            Debug.LogError("One of those atom id does not exist when creating a link:" + id1 + " - " + id2 );
            return;
        }
        Atom atom1 = atoms[id1];
        Atom atom2 = atoms[id2];

        if(!CheckLinkValidity(atom1, atom2))
        {
            return;
        }
        GameObject linkObj = Instantiate(linkPrefab, transform);
        MoleculeLink link = linkObj.GetComponent<MoleculeLink>();
        Debug.Assert(link != null, "The link prefab is not set to a object that contains the MoleculeLink");

        atom1.linkedAtoms.Add(atom2);
        atom2.linkedAtoms.Add(atom1);

        SpringJoint joint = atom1.GetComponent<SpringJointManager>().AddJoint(atom2.GetComponent<Rigidbody>());

        link.Init(atom1.transform, atom2.transform, joint);
        links[new IdPair(atom1.id, atom2.id)] = link ;
    }

    public void DestroyLink(int id1, int id2)
    {
        if (!atoms.ContainsKey(id1) || !atoms.ContainsKey(id2))
        {
            Debug.LogError("One of those atom id does not exist when destroying a link:" + id1 + " - " + id2);
            return;
        }
        Atom atom1 = atoms[id1];
        Atom atom2 = atoms[id2];

        atom1.linkedAtoms.Remove(atom2);
        atom2.linkedAtoms.Remove(atom1);

        if(links.TryGetValue(new IdPair(id1, id2), out MoleculeLink link))
        {
            // must do on both because we dont know the one that has the spring joint
            atom1.GetComponent<SpringJointManager>().RemoveJoint(link.jointRef);
            atom2.GetComponent<SpringJointManager>().RemoveJoint(link.jointRef);
            Destroy(link.gameObject);
        }
        links.Remove(new IdPair(id1, id2));
    }

    public void DestroyAllLinks()
    {
        foreach (Atom atom in atoms.Values) 
        {
            atom.linkedAtoms.Clear();
            atom.GetComponent<SpringJointManager>().ClearAllJoints();
        }
        foreach ( MoleculeLink link in links.Values)
        {
            Destroy(link.gameObject);
        }
        links.Clear();
    }

    public bool AreLinked(Atom atom1, Atom atom2)
    {
        return links.ContainsKey(new IdPair(atom1.id, atom2.id));
    }

    private void ApplyRepulsion()
    {
        foreach(Atom atom in atoms.Values)
        {
            foreach (Atom otherAtom in atoms.Values)
            {
                if (atom.Equals(otherAtom)) continue;
                if (AreLinked(atom, otherAtom)) continue;
                Vector3 delta = otherAtom.transform.position - atom.transform.position;
                float dist = delta.magnitude;
                if (dist < minDistance && dist > 0.001f)
                {
                    Vector3 force = delta.normalized * (repulsionStrength / (dist * dist));
                    Debug.DrawRay(atom.transform.position, -force, Color.red);
                    Debug.DrawRay(otherAtom.transform.position, force, Color.cyan);
                    atom.rb.AddForce(-force);
                    otherAtom.rb.AddForce(force);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // show the radius of atoms
        foreach (Atom atom in atoms.Values)
        {
            Gizmos.DrawWireSphere(atom.transform.position, minDistance);
        }
    }
}
