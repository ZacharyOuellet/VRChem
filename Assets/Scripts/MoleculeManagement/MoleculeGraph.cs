using System.Collections.Generic;
using UnityEngine;

public class MoleculeGraph
{
    private readonly Dictionary<int, Atom> _atoms = new();
    private readonly Dictionary<IdPair, MoleculeLink> _links = new();

    private List<HashSet<Atom>> _cachedMolecules = null;
    private bool _dirty = true;

    private static MoleculeGraph _Instance;

    public static MoleculeGraph Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new MoleculeGraph();
            }
            return _Instance;
        }
    }

    public IReadOnlyList<HashSet<Atom>> Molecules
    {
        get
        {
            if (_dirty || _cachedMolecules == null)
                _cachedMolecules = ComputeConnectedComponents();
            return _cachedMolecules;
        }
    }

    public IReadOnlyDictionary<int, Atom> Atoms => _atoms;
    public IReadOnlyDictionary<IdPair, MoleculeLink> Links => _links;

    public void AddAtom(Atom atom)
    {
        if (!_atoms.ContainsKey(atom.id))
        {
            _atoms.Add(atom.id, atom);
            _dirty = true;
        }
    }

    public void RemoveAtom(int id)
    {
        if (!_atoms.ContainsKey(id))
        {
            return;
        }
        Atom atom = _atoms[id];
        foreach (var linkedAtom in atom.linkedAtoms)
        {
            RemoveLink(atom, linkedAtom);
        }
        _atoms.Remove(id);
        _dirty = true;
    }

    public void AddLink(Atom a, Atom b, MoleculeLink link)
    {
        _links[new IdPair(a.id, b.id)] = link;

        a.linkedAtoms.Add(b);
        b.linkedAtoms.Add(a);

        _dirty = true;
    }

    public void RemoveLink(Atom a, Atom b)
    {
        a.linkedAtoms.Remove(b);
        b.linkedAtoms.Remove(a);

        _links.Remove(new IdPair(a.id, b.id));
        _dirty = true;
    }

    public bool AreLinked(Atom a, Atom b)
    {
        return _links.ContainsKey(new IdPair(a.id, b.id));
    }

    public int BondsCount(Atom a)
    {
        int count = 0;
        foreach (Atom b in a.linkedAtoms)
        {
            MoleculeLink link = _links[new IdPair(a.id, b.id)];
            if (link != null) count += link.Bonds;
        }
        return count;
    }

    public void ChangeLink(int idA, int idB, MoleculeLink newLink)
    {
        _links[new IdPair(idA, idB)] = newLink;
        _dirty = true;
    }

    public void ClearLinks()
    {
        foreach (var a in _atoms.Values)
            a.linkedAtoms.Clear();

        _links.Clear();
        _dirty = true;
    }

    private List<HashSet<Atom>> ComputeConnectedComponents()
    {
        HashSet<Atom> visited = new();
        List<HashSet<Atom>> comps = new();

        foreach (var atom in _atoms.Values)
        {
            if (!visited.Contains(atom))
            {
                var comp = DFS(atom, visited);
                comps.Add(comp);
            }
        }

        return comps;
    }

    private HashSet<Atom> DFS(Atom start, HashSet<Atom> visited)
    {
        Stack<Atom> stack = new();
        HashSet<Atom> comp = new();

        stack.Push(start);

        while (stack.Count > 0)
        {
            Atom a = stack.Pop();

            if (!visited.Add(a))
                continue;

            comp.Add(a);

            foreach (var n in a.linkedAtoms)
                if (!visited.Contains(n))
                    stack.Push(n);
        }

        return comp;
    }
}