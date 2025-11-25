using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MenuManager : MonoBehaviour
{

    private bool isMenuOpen = false;
    [SerializeField] GameObject menuObject = null;
    [SerializeField] List<GameObject> atoms = null;
    [SerializeField] float radius = 0.12f;
    [SerializeField] float height = 0.05f;
    [SerializeField] float spawnDistanceThreshold = 0.2f;
    [SerializeField] float handMenuShift = 0.2f;
    [SerializeField] MoleculeManager manager = null;
    [SerializeField] private LayerMask menuLayer;
    [SerializeField] private LayerMask atomLayer;
    [SerializeField] Transform _lookAtTarget;

    [SerializeField] Trash _trash;
    private bool _usingHand = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuObject.SetActive(isMenuOpen);
        initAtom();
        SetAtomPos();
    }

    // Update is called once per frame
    void Update() {}

    public void handleReleaseAtom(GameObject atom, PointerEvent interractorId)
    {
        if (atom.transform.parent == null) return;
        if (atom.transform.parent.name != "Menu") return;
        if (interractorId.Type != PointerEventType.Unselect) return;

        Vector3 releasedPos = atom.transform.position;
        setBackAtom(atom);

        Vector3 menuPosition = menuObject.transform.position;
        if (_usingHand) 
        {
            menuPosition.x += handMenuShift;
            menuPosition.z += 0.1f;
        };
        float distanceFromMenu = Vector3.Distance(releasedPos, menuPosition);


        if (distanceFromMenu > spawnDistanceThreshold)
        {
            spawnAtom(atom, releasedPos);
        }
    }

    private void initAtom()
    {
        for (int i = 0; i < atoms.Count; i++)
        {
            GameObject atomInstance = Instantiate(atoms[i], transform);

            foreach (Transform trans in atomInstance.GetComponentsInChildren<Transform>(true))
            {
                int layerNumber = Mathf.RoundToInt(Mathf.Log(menuLayer.value, 2));
                trans.gameObject.layer = layerNumber;
            }
            atoms[i] = atomInstance;
        }

    }

    private void SetAtomPos()
    {
        float evenSpacingRad = 2 * Mathf.PI / atoms.Count;
        float angle = 0;

        for (int i = 0; i < atoms.Count; i++)
        {
            GameObject atomInstance = atoms[i];
            float xPos = radius * Mathf.Cos(angle);
            float zPos = radius * Mathf.Sin(angle);
            if (_usingHand)
            {
                xPos += handMenuShift;
                zPos += 0.1f;
            };

            atomInstance.transform.localPosition = new Vector3(xPos, height, zPos);
            atomInstance.GetComponent<Atom>().SetLookAtTarget(_lookAtTarget);
            angle += evenSpacingRad;
        }
    }

    private void setBackAtom(GameObject atom)
    {
        int atomIndex = atoms.FindIndex(arrAtom => arrAtom.GetComponent<Atom>().atomData.name == atom.GetComponent<Atom>().atomData.name);
        float evenSpacingRad = 2 * Mathf.PI / atoms.Count;
        float angle = evenSpacingRad * atomIndex;
        float xPos = radius * Mathf.Cos(angle);
        float zPos = radius * Mathf.Sin(angle);
        if (_usingHand)
        {
            xPos += handMenuShift;
            zPos += 0.1f;
        };
        atom.transform.localPosition = new Vector3(xPos, height, zPos);
    }

    private void spawnAtom(GameObject atom, Vector3 worldPos)
    {
        var atomDataName = atom.GetComponent<Atom>().atomData.name;
        int atomIndex = atoms.FindIndex(a =>
            a.GetComponent<Atom>().atomData.name == atomDataName);

        GameObject newAtom = Instantiate(atoms[atomIndex], manager.transform);
        newAtom.transform.position = new Vector3(worldPos.x, worldPos.y, worldPos.z);
        foreach (Transform trans in newAtom.GetComponentsInChildren<Transform>(true))
        {
            int layerNumber = Mathf.RoundToInt(Mathf.Log(atomLayer.value, 2));
            trans.gameObject.layer = layerNumber;
        }
        manager.AddAtom(newAtom.GetComponent<Atom>());
    }

    public void toggle()
    {
        _usingHand = false;
        isMenuOpen = !isMenuOpen;
        if(isMenuOpen && _trash.Status)
        {
            _trash.Toggle(_usingHand);
        }
        menuObject.SetActive(isMenuOpen);
        SetAtomPos();
    }
    public void toggleHand()
    {
        _usingHand = true;
        isMenuOpen = !isMenuOpen;
        if (isMenuOpen && _trash.Status)
        {
            _trash.Toggle(_usingHand);
        }
        menuObject.SetActive(isMenuOpen);
        SetAtomPos();
    }

    public void toggleTrash()
    {
        _usingHand = false;
        _trash.Toggle(_usingHand);
        if (isMenuOpen && _trash.Status)
        {
            isMenuOpen = false;
            menuObject.SetActive(isMenuOpen);
            SetAtomPos();
        }
    }

    public void toggleTrashHand()
    {
        _usingHand = true;
        _trash.Toggle(_usingHand);
        if (isMenuOpen && _trash.Status)
        {
            isMenuOpen = !isMenuOpen;
            menuObject.SetActive(isMenuOpen);
            SetAtomPos();
        }
    }
}
