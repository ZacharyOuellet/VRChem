using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] bool startStatus = true;
    [SerializeField] Vector3 _offset = new Vector3(0,0,0.3f);
    public bool Status { get; private set; }
    bool lastIsUsingHand;

    private void Start()
    {
        Status = !startStatus;
        Toggle(false);
    }

    public void Toggle(bool isUsingHand)
    {
        Status = !Status;
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(Status);
            if(lastIsUsingHand != isUsingHand)
            {
                lastIsUsingHand = isUsingHand;
                if(isUsingHand )
                {

                    transform.Translate(_offset);
                }
                else
                {
                    transform.Translate(-_offset);
                }
            }
        }
    }
}
