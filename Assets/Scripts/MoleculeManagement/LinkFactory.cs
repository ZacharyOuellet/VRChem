using UnityEngine;

public class LinkFactory : MonoBehaviour
{
    [SerializeField] GameObject _linkPrefab;

    public MoleculeLink CreateLinkObject(Transform a, Transform b, SpringJoint joint)
    {
        var obj = Instantiate(_linkPrefab, transform);
        var link = obj.GetComponent<MoleculeLink>();
        link.Init(a, b, joint);
        return link;
    }

    public void DestroyLinkObject(MoleculeLink link)
    {
        if (link != null)
            Destroy(link.gameObject);
    }
}