using System.Collections;
using TMPro;
using UnityEngine;

public class WhiteBoard : MonoBehaviour
{
    [SerializeField] private MoleculeEvaluator Evaluator;
    [SerializeField] private TMP_Text ObjectiveTextName;
    [SerializeField] private TMP_Text ObjectiveTextDescription;
    [SerializeField] private GameObject ImageGO;
    public bool ShouldDisplayText = false;
    public int StartSequenceDuration = 3;

    private void Update()
    {
        if (ShouldDisplayText)
        {
            ObjectiveTextName.text = Evaluator.ObjectiveMoleculeName;
            ObjectiveTextDescription.text = Evaluator.ObjectiveMoleculeDescription;
            ImageGO.SetActive(false);
        }
        else
        {
            ObjectiveTextName.text = "";
            ObjectiveTextDescription.text = "";
            ImageGO.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        ShouldDisplayText = false;
        yield return new WaitForSeconds(StartSequenceDuration);
        ShouldDisplayText = true;
    }
}
