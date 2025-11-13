using UnityEngine;

public class LinkFactory : MonoBehaviour
{
    [SerializeField] GameObject linkPrefab;

    public MoleculeLink CreateLinkObject(Transform a, Transform b, SpringJoint joint)
    {
        var obj = Instantiate(linkPrefab, transform);
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