using Oculus.Haptics;
using Oculus.Interaction;
using Oculus.Interaction.Feedback;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MoleculeManager))]
public class LinkCreationManager : MonoBehaviour
{
    public Atom[] _grabbedPair = new Atom[2] { null, null };

    

    [Header("Distances")]
    [SerializeField] float _linkCreationThreshold = 0.2f;
    [SerializeField] float _linkAdditionThreshold = 0.07f;
    [SerializeField] float _linkDestructionThreshold = 1.0f;

    [Header("Times")]
    [SerializeField] float _linkCreationTime = 1f;

    [Header("Haptics")]
    [SerializeField] HapticClip _linkCreationHaptic;
    [SerializeField] HapticClip _linkDestructionHaptic;

    [Header("Audio")]
    [SerializeField] AudioClip _linkCreationAudio;
    [SerializeField] AudioClip _linkDestructionAudio;

    MoleculeManager _moleculeManager;
    private bool _createIsRunning = false;
    private bool _addIsRunning = false;
    private HapticClipPlayer _hapticPlayer;
    private AudioSource _audioPlayer;

    void Start()
    {
        _moleculeManager = GetComponent<MoleculeManager>();
        _hapticPlayer = new HapticClipPlayer();
        _audioPlayer = GetComponent<AudioSource>();
    }

    float GetDistance(Atom a, Atom b)
    {
        if (a == null || b == null)
        {
            Debug.LogError("a or b in null");
            return Mathf.Infinity;
        }
        Vector3 delta = b.transform.position - a.transform.position;
        return delta.magnitude;
    }

    void TryCreateLink()
    {
        if (_createIsRunning || _addIsRunning) return;
        if (GetDistance(_grabbedPair[0], _grabbedPair[1]) < _linkCreationThreshold)
        {
            _createIsRunning = true;
            StartCoroutine(CreateLink());
        }
    }

    IEnumerator CreateLink()
    {
        Debug.LogWarning("CREATELINK COROUTINE STARTED");
        float dT = 0;
        while (dT < _linkCreationTime)
        {
            if (GetDistance(_grabbedPair[0], _grabbedPair[1]) > _linkCreationThreshold)
            {
                _createIsRunning = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            dT += Time.deltaTime;
        }
        if(!_moleculeManager.CreateLink(_grabbedPair[0].id, _grabbedPair[1].id))
        {
            // TODO maybe add an effect (visual, sound and haptics) for when link cant be created
            _createIsRunning = false;
            yield break;
        }
        _audioPlayer.clip = _linkCreationAudio;
        _audioPlayer.Play();
        _hapticPlayer.clip = _linkCreationHaptic;
        _hapticPlayer.Play(Controller.Both);
        Debug.LogWarning("CREATELINK WAITING FOR DISTANCE >");

        // prevent adding a link before wanting to
        while (GetDistance(_grabbedPair[0], _grabbedPair[1]) < _linkCreationThreshold)
        {
            yield return new WaitForEndOfFrame();
        }

        _createIsRunning = false;
        Debug.LogWarning("CREATELINK COROUTINE ENDED GRACEFULLY");

    }

    void TryAddLink()
    {
        if (_createIsRunning || _addIsRunning) return;
        Debug.Log("TRY ADD LINK");
        if (GetDistance(_grabbedPair[0], _grabbedPair[1]) < _linkAdditionThreshold)
        {
            _addIsRunning = true;
            StartCoroutine(AddLink());
        }
    }

    IEnumerator AddLink()
    {
        Debug.LogWarning("ADDLINK COROUTINE STARTED");

        float dT = 0;
        while (dT < _linkCreationTime)
        {
            if (GetDistance(_grabbedPair[0], _grabbedPair[1]) > _linkAdditionThreshold)
            {
                _addIsRunning = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            dT += Time.deltaTime;
        }
        if (!_moleculeManager.AddBound(_grabbedPair[0].id, _grabbedPair[1].id))
        {
            // TODO maybe add an effect (visual, sound and haptics) for when bound cant be added
            _addIsRunning = false;
            yield break;
        }
        _audioPlayer.clip = _linkCreationAudio;
        _audioPlayer.Play();
        _hapticPlayer.clip = _linkCreationHaptic;
        _hapticPlayer.Play(Controller.Both);
        Debug.LogWarning("ADDLINK WAITING FOR DISTANCE >");

        // prevent adding a link before wanting to
        while (GetDistance(_grabbedPair[0], _grabbedPair[1]) < _linkCreationThreshold)
        {
            yield return new WaitForEndOfFrame();
        }
        _addIsRunning = false;
        Debug.LogWarning("ADDLINK COROUTINE ENDED GRACEFULLY");
    }

    void TryDestroyLink()
    {
        if(GetDistance(_grabbedPair[0], _grabbedPair[1]) > _linkDestructionThreshold)
        {
            _moleculeManager.DestroyLink(_grabbedPair[0].id, _grabbedPair[1].id);
            // TODO maybe add an effect (visual, sound and haptics)
            _audioPlayer.clip = _linkDestructionAudio;
            _audioPlayer.Play();
            _hapticPlayer.clip = _linkDestructionHaptic;
            _hapticPlayer.Play(Controller.Both);
        }
    }

    void Update()
    {
        if (_grabbedPair[0] != null && _grabbedPair[1] != null)
        {
            if (_moleculeManager.AreLinked(_grabbedPair[0], _grabbedPair[1]))
            {
                TryAddLink();
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
