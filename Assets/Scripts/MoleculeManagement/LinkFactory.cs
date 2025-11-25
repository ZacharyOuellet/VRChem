using UnityEngine;

public class LinkFactory : MonoBehaviour
{
    [SerializeField] GameObject[] linkPrefabs;

    public MoleculeLink CreateLinkObject(Transform a, Transform b, SpringJoint joint, int bonds = 1)
    {
        var obj = Instantiate(linkPrefabs[bonds - 1], transform);
        var link = obj.GetComponent<MoleculeLink>();
        link.Init(a, b, joint);
        return link;
    }

    public MoleculeLink UpdateLink(MoleculeLink link, int bonds)
    {
        Debug.Log("LinkFactory UpdateLink called with " + bonds + " bonds");
        if(link == null) return null;
        if(link.Bonds == bonds) return link;
        if(bonds > linkPrefabs.Length)
        {
            Debug.LogError("Tried to create more links than there are prefabs ");
            return link;
        }
        if(bonds < 1)
        {
            Debug.LogError("Cant set negative or no bonds, destroy it instead");
            return link;
        }
        var obj = Instantiate(linkPrefabs[bonds - 1], transform);
        var newLink = obj.GetComponent<MoleculeLink>();
        newLink.Init(link);
        Destroy(link.gameObject);
        Debug.Log("LinkFactory UpdateLink finished");
        return newLink;
    }

    public void DestroyLinkObject(MoleculeLink link)
    {
        if (link != null)
            Destroy(link.gameObject);
    }
}