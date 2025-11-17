using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    private bool isMenuOpen = false;
    [SerializeField] GameObject menuObject = null;
    [SerializeField] List<GameObject> atoms = null;
    [SerializeField] float radius = 0.12f;
    [SerializeField] float height = 0.05f;
    [SerializeField] float spawnDistanceThreshold = 0.2f;
    [SerializeField] MoleculeManager manager = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuObject.SetActive(isMenuOpen);
        SetAtomPos();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void handleReleaseAtom(GameObject atom, PointerEvent interractorId)
    {
        if (atom.transform.parent == null) return;
        if (atom.transform.parent.name != "Menu") return;
        if (interractorId.Type != PointerEventType.Unselect) return;

        Vector3 releasedPos = atom.transform.position;

        setBackAtom(atom);

        float distanceFromMenu = Vector3.Distance(releasedPos, menuObject.transform.position);
        Debug.Log("distance from menu " + distanceFromMenu);

        if (distanceFromMenu > spawnDistanceThreshold)
        {
            spawnAtom(atom, releasedPos);
        }
    }

    private void SetAtomPos()
    {
        float evenSpacingRad = 2 * Mathf.PI / atoms.Count;
        float angle = 0;

        for (int i = 0; i < atoms.Count; i++)
        {
            GameObject atomInstance = Instantiate(atoms[i], transform);
            float xPos = radius * Mathf.Cos(angle);
            float zPos = radius * Mathf.Sin(angle);

            atomInstance.transform.localPosition = new Vector3(xPos, height, zPos);

            angle += evenSpacingRad;

            atoms[i] = atomInstance;
        }
    }

    private void setBackAtom(GameObject atom)
    {
        int atomIndex = atoms.FindIndex(arrAtom => arrAtom.name == atom.GetComponent<Atom>().atomData.name);
        Debug.Log(atomIndex);
        float evenSpacingRad = 2 * Mathf.PI / atoms.Count;
        float angle = evenSpacingRad * atomIndex;
        float xPos = radius * Mathf.Cos(angle);
        float zPos = radius * Mathf.Sin(angle);
        atom.transform.localPosition = new Vector3(xPos, height, zPos);
    }

    private void spawnAtom(GameObject atom, Vector3 worldPos)
    {
        var atomDataName = atom.GetComponent<Atom>().atomData.name;
        Debug.Log($"spawning atom {atomDataName} at {worldPos}");

        int atomIndex = atoms.FindIndex(a =>
            a.GetComponent<Atom>().atomData.name == atomDataName);

        GameObject newAtom = Instantiate(atoms[atomIndex], worldPos, Quaternion.identity);
        manager.AddAtom(newAtom.GetComponent<Atom>());
    }

    public void toggle()
    {
        isMenuOpen = !isMenuOpen;
        menuObject.SetActive(isMenuOpen);
    }
}
