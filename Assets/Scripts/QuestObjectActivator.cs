using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectActivator : MonoBehaviour
{
    public GameObject objectToActivate;
    public string questToCheck;
    public bool activeIfComplete;

    private bool initialCheckDone;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialCheckDone)
        {
            initialCheckDone = true;

            CheckCompletion();
        }
    }

    public void CheckCompletion() //provjerava je li misija gotova za quest object
    {
        if (QuestManager.instance.CheckIfComplete(questToCheck))
        {
            objectToActivate.SetActive(activeIfComplete);  //aktiviranje objekta ako je misija gotova (pomicanje kamena)
        }
    }
}
