using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MoleculeManager))]
public class LinkCreationManager : MonoBehaviour
{
    public Atom LeftGrabbed;
    public Atom RightGrabbed;

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
        if (GetDistance(LeftGrabbed, RightGrabbed) < _linkCreationThreshold)
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
            if (GetDistance(LeftGrabbed, RightGrabbed) > _linkCreationThreshold)
            {
                _createIsRunning = false;
                yield break;
            }
            // TODO maybe add an effect (visual, sound and haptics)
            yield return new WaitForEndOfFrame();
            dT += Time.deltaTime;
        }
        _moleculeManager.CreateLink(LeftGrabbed.id, RightGrabbed.id);
        _createIsRunning = false;
    }

    void TryDestroyLink()
    {
        if(GetDistance(LeftGrabbed,RightGrabbed) > _linkDestructionThreshold)
        {
            _moleculeManager.DestroyLink(LeftGrabbed.id, RightGrabbed.id);
            // TODO maybe add an effect (visual, sound and haptics)
        }
    }

    void Update()
    {
        if (LeftGrabbed != null && RightGrabbed != null)
        {
            if (_moleculeManager.AreLinked(LeftGrabbed, RightGrabbed))
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
        foreach (var grabbed in new[] { LeftGrabbed, RightGrabbed })
        {
            if (grabbed == null) continue;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(grabbed.transform.position, _linkCreationThreshold);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(grabbed.transform.position, _linkDestructionThreshold);
        }
    }
}
