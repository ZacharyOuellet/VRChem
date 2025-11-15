using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Oculus.Interaction;

[RequireComponent(typeof(MoleculeManager))]
public class LinkCreationManager : MonoBehaviour
{
    public Atom[] _grabbedPair = new Atom[2] { null, null };

    

    [Header("Distances")]
    [SerializeField] float _linkCreationThreshold = 0.2f;
    [SerializeField] float _linkDestructionThreshold = 1.0f;

    [Header("Times")]
    [SerializeField] float _linkCreationTime = 1f;

    MoleculeManager _moleculeManager;
    private bool _createIsRunning = false;

    void Start()
    {
        _moleculeManager = GetComponent<MoleculeManager>();
    }

    float GetDistance(Atom a, Atom b)
    {
        Vector3 delta = b.transform.position - a.transform.position;
        return delta.magnitude;
    }

    void TryCreateLink()
    {
        if (_createIsRunning) return;
        if (GetDistance(_grabbedPair[0], _grabbedPair[1]) < _linkCreationThreshold)
        {
            _createIsRunning = true;
            StartCoroutine(CreateLink());
        }
    }

    IEnumerator CreateLink()
    {
        float dT = 0;
        while (dT < _linkCreationTime)
        {
            if (GetDistance(_grabbedPair[0], _grabbedPair[1]) > _linkCreationThreshold)
            {
                _createIsRunning = false;
                yield break;
            }
            // TODO maybe add an effect (visual, sound and haptics)
            yield return new WaitForEndOfFrame();
            dT += Time.deltaTime;
        }
        _moleculeManager.CreateLink(_grabbedPair[0].id, _grabbedPair[1].id);
        _createIsRunning = false;
    }

    void TryDestroyLink()
    {
        if(GetDistance(_grabbedPair[0], _grabbedPair[1]) > _linkDestructionThreshold)
        {
            _moleculeManager.DestroyLink(_grabbedPair[0].id, _grabbedPair[1].id);
            // TODO maybe add an effect (visual, sound and haptics)
        }
    }

    void Update()
    {
        if (_grabbedPair[0] != null && _grabbedPair[1] != null)
        {
            if (_moleculeManager.AreLinked(_grabbedPair[0], _grabbedPair[1]))
            {
                TryDestroyLink();
            }
            else
            {
                TryCreateLink();
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var grabbed in _grabbedPair)
        {
            if (grabbed == null) continue;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(grabbed.transform.position, _linkCreationThreshold);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(grabbed.transform.position, _linkDestructionThreshold);
        }
    }

    public void OnPtrEvent(GameObject obj, PointerEvent pointerEvent)
    {
        Atom grabbedAtom;
        if (!obj.TryGetComponent<Atom>(out grabbedAtom))
            grabbedAtom = obj.GetComponentInParent<Atom>();

        if (grabbedAtom == null) return;

        switch(pointerEvent.Type)
        {
            case PointerEventType.Select:
                if (_grabbedPair[0] == null)
                {
                    _grabbedPair[0] = grabbedAtom;
                }
                else
                {
                    _grabbedPair[1] = grabbedAtom;
                }
                break;
            case PointerEventType.Unselect:
                if (_grabbedPair[0] == grabbedAtom)
                {
                    _grabbedPair[0] = null;
                }
                else if (_grabbedPair[1] == grabbedAtom)
                {
                    _grabbedPair[1] = null;
                }
                else
                {
                    Debug.LogWarning("An atom was dropped but was not considered picked up before, there is probably an error somewhere");
                }
                break;
            default: break;
        }
    }
}
