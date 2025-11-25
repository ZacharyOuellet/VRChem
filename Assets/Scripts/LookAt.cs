using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] Transform _target;
    void Update()
    {
        transform.LookAt( _target );
    }
}
