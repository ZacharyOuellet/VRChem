using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoleculeManager))]
public class MoleculeEvaluator : MonoBehaviour
{
    [SerializeField] private MoleculeManager moleculeManager;
    [SerializeField] private MoleculeData[] ObjectiveMoleculeDatas;
    private int currentObjectiveMolecule;

    public String ObjectiveMoleculeName { get { return ObjectiveMoleculeDatas[currentObjectiveMolecule].Name; } }
    public String ObjectiveMoleculeDescription { get { return ObjectiveMoleculeDatas[currentObjectiveMolecule].Description; } }

    private void Start()
    {
        moleculeManager = GetComponent<MoleculeManager>();
    }

    private void EvaluateMolecule(HashSet<Atom> Molecule)
    {
        Dictionary<String, int> atoms = new Dictionary<String, int>();
        foreach (var atom in Molecule)
        {
            string id = atom.atomData.ID;
            if (atoms.ContainsKey(id))
            {
                atoms[id]++;
            }
            else
            {
                atoms.Add(id, 1);
            }
        }

        if (TestMolecule(atoms, ObjectiveMoleculeDatas[currentObjectiveMolecule]))
        {
            Debug.Log("Success");
            currentObjectiveMolecule++;
            currentObjectiveMolecule = Math.Min(currentObjectiveMolecule, ObjectiveMoleculeDatas.Length - 1);
            moleculeManager.DestroyAllMolecules();
        }

    }

    private void Update()
    {
        foreach (var molecule in MoleculeGraph.Instance.Molecules)
        {
            EvaluateMolecule(molecule);
        }
    }

    private bool TestMolecule(Dictionary<String, int> atomsTested, MoleculeData ObjectiveMolecule)
    {
        // Create objective dictionnary 
        Dictionary<String, int> objectiveAtomsData = new Dictionary<String, int>();
        foreach (var atomEntry in ObjectiveMolecule.Atoms)
        {
            objectiveAtomsData.Add(atomEntry.atomData.ID, atomEntry.count);
        }


        foreach (var atom in atomsTested) // Atoms : [(H, 2), (O, 1)]
        {
            string atomId = atom.Key;
            int atomsCount = atom.Value;

            if (objectiveAtomsData.ContainsKey(atomId))
            {
                objectiveAtomsData[atomId] -= atomsCount;
            }
            else
            {
                return false;
            }
        }

        // Test objectiveAtomsData => each entry should be 0
        foreach (var count in objectiveAtomsData.Values)
        {
            if (count != 0)
            {
                return false;
            }
        }

        return true;
    }

}