using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoleculeManager))]
public class MoleculeEvaluator : MonoBehaviour
{
    [SerializeField] private MoleculeData[] ObjectiveMoleculeDatas;
    private MoleculeManager moleculeManager;
    private int currentObjectiveMolecule;

    public float DelayAfterSuccess = 5.0f;
    
    public AudioClip SuccessClip;
    private AudioSource audioPlayer;

    public String ObjectiveMoleculeName { get { return ObjectiveMoleculeDatas[currentObjectiveMolecule].Name; } }
    public String ObjectiveMoleculeDescription { get { return ObjectiveMoleculeDatas[currentObjectiveMolecule].Description; } }

    private void Start()
    {
        moleculeManager = GetComponent<MoleculeManager>();
        audioPlayer = GetComponent<AudioSource>();
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
            
            // OnSuccess
            if (SuccessClip)
            {
                audioPlayer.PlayOneShot(SuccessClip);
            }
            StartCoroutine(SuccessSequence());
        }
    }

    private void Update()
    {
        foreach (var molecule in MoleculeGraph.Instance.Molecules)
        {
            EvaluateMolecule(molecule);
        }
    }

    IEnumerator SuccessSequence()
    {
        yield return new WaitForSeconds(DelayAfterSuccess);
        moleculeManager.DestroyAllMolecules();
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