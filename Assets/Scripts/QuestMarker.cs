using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMarker : MonoBehaviour
{
    public string questToMark;
    public bool markComplete;

    public bool markOnEnter;
    private bool canMark;

    public bool deactivateOnMarking;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canMark && Input.GetButtonDown("Fire1")) // ako misija nije oznacena i ako se pritisnula lijeva tipka misa
        {
            canMark = false;
            MarkQuest();
        }
    }

    public void MarkQuest() // oznacivanje misije da je gotova ili ne
    {
        if (markComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToMark); //oznaci da je gotova
        }
        else
        {
            QuestManager.instance.MarkQuestIncomplete(questToMark); //oznaci da nije gotova
        }

        gameObject.SetActive(!deactivateOnMarking); //zelimo deaktivirati
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if(markOnEnter == true)
            {
                MarkQuest();
            }
            else
            {
                canMark = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canMark = false;
        }
    }

}
