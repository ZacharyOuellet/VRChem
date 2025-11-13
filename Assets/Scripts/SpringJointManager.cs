using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpringJointManager : MonoBehaviour
{
    private readonly List<SpringJoint> _joints = new();
    private Rigidbody _rigidbody;

    [Header("Spring Settings")]
    [SerializeField] float spring = 100f;
    [SerializeField] float damper = 20f;
    [SerializeField] float minDistance = 3f;
    [SerializeField] float maxDistance = 3f;
    [SerializeField] float tolerance = 0.1f;
    [SerializeField] Vector3 anchor = Vector3.zero;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public SpringJoint AddJoint(Rigidbody target)
    {
        if (target == null)
        {
            Debug.LogWarning("Cannot create SpringJoint: target is null.");
            return null;
        }

        SpringJoint joint = gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = target;
        joint.autoConfigureConnectedAnchor = false;
        joint.spring = spring;
        joint.damper = damper;
        joint.minDistance = minDistance;
        joint.maxDistance = maxDistance;
        joint.anchor = anchor;
        joint.connectedAnchor = anchor;
        joint.tolerance = tolerance;

        _joints.Add(joint);
        return joint;
    }


    public void RemoveJoint(SpringJoint joint)
    {
        if (joint == null)
            return;

        _joints.Remove(joint);

        Destroy(joint);
    }

    public void ClearAllJoints()
    {
        foreach (var joint in _joints)
        {
            if (joint != null)
                Destroy(joint);
        }
        _joints.Clear();
    }

    /// Draws a line to the mid point of 2 nodes, if a full line is shown, it means that each node has a spring (dupplication)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var joint in _joints)
        {
            if (joint != null && joint.connectedBody != null)
            {

                Vector3 midPoint = (joint.connectedBody.position + transform.position)/2f;
                Gizmos.DrawLine(transform.position, midPoint);
            }
        }
    }
}