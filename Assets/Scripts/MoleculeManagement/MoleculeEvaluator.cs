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
    
    public ParticleSystem SuccessParticle;

    public String ObjectiveMoleculeName { get { return ObjectiveMoleculeDatas[currentObjectiveMolecule].Name; } }
    public String ObjectiveMoleculeDescription { get { return ObjectiveMoleculeDatas[currentObjectiveMolecule].Description; } }

    private void Start()
    {
        moleculeManager = GetComponent<MoleculeManager>();
        audioPlayer = GetComponent<AudioSource>();
    }

    private void EvaluateMolecule(HashSet<Atom> Molecule)
    {
        if (TestMolecule(Molecule, ObjectiveMoleculeDatas[currentObjectiveMolecule]))
        {
            Debug.Log("Success");
            currentObjectiveMolecule++;
            currentObjectiveMolecule = currentObjectiveMolecule % ObjectiveMoleculeDatas.Length;
            
            // OnSuccess
            if (SuccessClip)
            {
                audioPlayer.PlayOneShot(SuccessClip);
            }

            if (SuccessParticle)
            {
                SuccessParticle.Play();
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

    private bool TestMolecule(HashSet<Atom> Molecule, MoleculeData ObjectiveMolecule)
    {
        Dictionary<String, int> atomsDict = new Dictionary<String, int>();
        List<Atom> atomsList = new List<Atom>();
        foreach (var atom in Molecule)
        {
            string id = atom.atomData.ID;
            if (atomsDict.ContainsKey(id))
            {
                atomsDict[id]++;
            }
            else
            {
                atomsDict.Add(id, 1);
            }
            atomsList.Add(atom);
        }
        
        if (TestAtomsCount(atomsDict, ObjectiveMolecule))
        {
            if (TestAtomsBonds(atomsList))
            {
                return true;
            }
        }

        return false;
    }

    private bool TestAtomsCount(Dictionary<String, int> atomsTested, MoleculeData ObjectiveMolecule)
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

    private bool TestAtomsBonds(List<Atom> atoms)
    {
        foreach (Atom atom in atoms)
        {
            if (MoleculeGraph.Instance.BondsCount(atom) != atom.atomData.Connections)
            {
                return false;
            }
        }
        return true;
    }
    
}