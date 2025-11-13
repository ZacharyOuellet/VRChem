using UnityEngine;


[RequireComponent(typeof(MoleculeManager))]
public class MoleculeManagerDebugger : MonoBehaviour
{
    MoleculeManager _manager;
    [Header("Start Setup")]
    [Tooltip("Sets a molecule that will appear when starting the scene")]
    [SerializeField] DebugMolecule _startingMolecule;

    [Header("Create Link")]
    [SerializeField] Atom _atomA;
    [SerializeField] Atom _atomB;
    [SerializeField] bool _createLink;

    [Header("Destroy Link")]
    [SerializeField] int _id1 = -1;
    [SerializeField] int _id2 = -1;
    [SerializeField] bool _destroyLink;

    [Header("Destroy All Links")]
    [SerializeField] bool destroyAll;

    private void createDebugMolecule()
    {
        if (_startingMolecule == null) return;

        int i = 0;
        int[] idsTranslation = new int[_startingMolecule.atomPrefabs.Length];
        foreach (GameObject atom in _startingMolecule.atomPrefabs)
        {
            GameObject newAtom = Instantiate(atom, transform);
            newAtom.transform.position += Random.insideUnitSphere;
            idsTranslation[i++] = newAtom.GetComponent<Atom>().id;
            _manager.AddAtom(newAtom.GetComponent<Atom>());
        }

        foreach (IdPair link in _startingMolecule.links)
        {
            _manager.CreateLink(idsTranslation[link.A], idsTranslation[link.B]);
        }
    }

    private void Start()
    {
        _manager = GetComponent<MoleculeManager>();
        createDebugMolecule();
    }

    private void Update()
    {
        if (_createLink)
        {
            _manager.AddAtom(_atomA);
            _manager.AddAtom(_atomB);
            _manager.CreateLink(_atomA.id, _atomB.id);
            _createLink = false;
        }

        if (_destroyLink)
        {
            _manager.DestroyLink(_id1, _id2);
            _destroyLink = false;
        }

        if (destroyAll)
        {
            _manager.DestroyAllLinks();
            destroyAll = false;
        }
    }
}
