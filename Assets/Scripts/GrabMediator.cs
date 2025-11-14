using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class GrabMediator : MonoBehaviour
{
    [Header("Interactors")]
    [SerializeField] private HandGrabInteractor _leftHandInteractor;
    [SerializeField] private GrabInteractor _leftControllerInteractor;
    [Space(5)]
    [SerializeField] private HandGrabInteractor _rightHandInteractor;
    [SerializeField] private GrabInteractor _rightControllerInteractor;

    [Header("Events")]
    public GrabEvent OnGrab;
    public GrabEvent OnRelease;

    private void OnEnable()
    {
        Register(_leftHandInteractor, OnLeftGrab, OnLeftRelease);
        Register(_leftControllerInteractor, OnLeftGrab, OnLeftRelease);

        Register(_rightHandInteractor, OnRightGrab, OnRightRelease);
        Register(_rightControllerInteractor, OnRightGrab, OnRightRelease);
    }

    private void OnDisable()
    {
        Unregister(_leftHandInteractor, OnLeftGrab, OnLeftRelease);
        Unregister(_leftControllerInteractor, OnLeftGrab, OnLeftRelease);

        Unregister(_rightHandInteractor, OnRightGrab, OnRightRelease);
        Unregister(_rightControllerInteractor, OnRightGrab, OnRightRelease);
    }

    private void Register(GrabInteractor interactor,
                          System.Action<GrabInteractable> grab,
                          System.Action<GrabInteractable> release)
    {
        if (interactor == null) return;

        interactor.WhenInteractableSelected.Action += grab;
        interactor.WhenInteractableUnselected.Action += release;
    }

    private void Register(HandGrabInteractor interactor,
                      System.Action<HandGrabInteractable> grab,
                      System.Action<HandGrabInteractable> release)
    {
        if (interactor == null) return;

        interactor.WhenInteractableSelected.Action += grab;
        interactor.WhenInteractableUnselected.Action += release;
    }

    private void Unregister(GrabInteractor interactor,
                            System.Action<GrabInteractable> grab,
                            System.Action<GrabInteractable> release)
    {
        if (interactor == null) return;

        interactor.WhenInteractableSelected.Action -= grab;
        interactor.WhenInteractableUnselected.Action -= release;
    }


    private void Unregister(HandGrabInteractor interactor,
                            System.Action<HandGrabInteractable> grab,
                            System.Action<HandGrabInteractable> release)
    {
        if (interactor == null) return;

        interactor.WhenInteractableSelected.Action -= grab;
        interactor.WhenInteractableUnselected.Action -= release;
    }


    private void OnLeftGrab(GrabInteractable obj) => OnGrab?.Invoke(obj.gameObject, Hand.Left);
    private void OnRightGrab(GrabInteractable obj) => OnGrab?.Invoke(obj.gameObject, Hand.Right);

    private void OnLeftRelease(GrabInteractable obj) => OnRelease?.Invoke(obj.gameObject, Hand.Left);
    private void OnRightRelease(GrabInteractable obj) => OnRelease?.Invoke(obj.gameObject, Hand.Right);

    private void OnLeftGrab(HandGrabInteractable obj) => OnGrab?.Invoke(obj.gameObject, Hand.Left);
    private void OnRightGrab(HandGrabInteractable obj) => OnGrab?.Invoke(obj.gameObject, Hand.Right);

    private void OnLeftRelease(HandGrabInteractable obj) => OnRelease?.Invoke(obj.gameObject, Hand.Left);
    private void OnRightRelease(HandGrabInteractable obj) => OnRelease?.Invoke(obj.gameObject, Hand.Right);
}
