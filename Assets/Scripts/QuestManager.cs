using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string[] questMarkerNames;
    public bool[] questMarkersComplete;

    public static QuestManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(instance);
        }

        questMarkersComplete = new bool[questMarkerNames.Length]; //iste duzine
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetQuestNumber (string questToFind) //vrati broj misije
    {
        for (int i=0;i<questMarkerNames.Length; i++) //prolazi kroz sve misije
        {
            if (questMarkerNames[i] == questToFind) //ako smo nasli trazenu misiju
            {
                return i; //broj misije
            }
        }

        Debug.LogError("Quest" + questToFind + "does not exist!");
        return 0; // nema misije
    }

    public bool CheckIfComplete(string questToCheck) // provjeri je li misija gotova
    {
        if (GetQuestNumber(questToCheck) != 0) // vraca broj misije
        {
            return questMarkersComplete[GetQuestNumber(questToCheck)]; //ako je misija gotova
        }

        return false; //ako misija nije gotova
    }

    public void MarkQuestComplete(string questToMark) //oznaci da je misija gotova
    {
        questMarkersComplete[GetQuestNumber(questToMark)] = true; 
        UpdateLocalQuestObjects(); //azuriranje quest objekata (pomicanje kamena)
    }

    public void MarkQuestIncomplete(string questToMark) //oznaci da misije nije gotova
    {
        questMarkersComplete[GetQuestNumber(questToMark)] = false;
        UpdateLocalQuestObjects(); //azuriranje quest objekata
    }

    public void UpdateLocalQuestObjects() //azuriranje quest objekata
    {
        QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>(); //pronadi quest objekte

        if (questObjects.Length > 0) 
        {
            for (int i = 0; i < questObjects.Length; i++)
            {
                questObjects[i].CheckCompletion();
            }
        }
    }

    public void SaveQuestData()
    {
        for (int i = 0; i< questMarkerNames.Length; i++)
        {
            if (questMarkersComplete[i]) { 
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 1);
            }
            else
            {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 0); //oznaci sa 0 ako quest nije gotov?
            }
        }
    }

    public void LoadQuestData()
    {
        for (int i=0; i< questMarkerNames.Length; i++)
        {
            int valueToSet = 0;
            if (PlayerPrefs.HasKey("QuestMarker_" + questMarkerNames[i]))
            {
                valueToSet = PlayerPrefs.GetInt("QuestMarker_" + questMarkerNames[i]); //dobivamo vrijednost 0 ili 1
            }

            if (valueToSet == 0)
            {
                questMarkersComplete[i] = false;
            }
            else
            {
                questMarkersComplete[i] = true;
            }
        }
    }
}
