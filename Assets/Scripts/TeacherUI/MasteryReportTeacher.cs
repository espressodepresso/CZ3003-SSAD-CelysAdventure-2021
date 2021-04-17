using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;
using System;

public class MasteryReportTeacher : MonoBehaviour
{
    public Transform entryContainer;
    public GameObject entryTemplate;

    private PlayerInfo playerData;
    private string playerJson;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;

    public GameObject[] leaderboardButtons;

    public void DisplayMastery(string userID)
    {
        // get the firebase object
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");

        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        StartCoroutine(LoadUserData(userID, returnValue =>
        {
            Debug.Log(returnValue);
            playerJson = returnValue;
            playerData = PlayerInfo.CreateFromJSON(returnValue);
            foreach (Transform item in entryContainer)
            {
                Destroy(item.gameObject);
            }
            DisplayUserData();
        }));
          
    }
    private void DisplayUserData()
    {
        GameObject currentStageTitle = GameObject.Find("CurrentStageTitle");
        currentStageTitle.GetComponent<TextMeshProUGUI>().text = "Current Stage: " + playerData.curStage;
        GameObject totalNumAttemptTitle = GameObject.Find("TotalNumberAttemptTitle");
        totalNumAttemptTitle.GetComponent<TextMeshProUGUI>().text = "Total Number of Attempts: " + playerData.totalNoAttempted.ToString();
        GameObject totalNumCorrectTitle = GameObject.Find("TotalNumberCorTitle");
        totalNumCorrectTitle.GetComponent<TextMeshProUGUI>().text = "Total Number Correct: " + playerData.totalNoCorrect.ToString();
        GameObject numberPokemonTitle = GameObject.Find("NumberPokemonTitle");
        numberPokemonTitle.GetComponent<TextMeshProUGUI>().text = "Total Number Pokemons Held: " + playerData.pokemonHeld.Count.ToString();
        
        int numExploreStage = playerData.exploreAnsweredQns.Length;
        // for exploration stages
        for (int i = 0; i < numExploreStage; i++)
        {
            // displaying the rows on the mastery report table 
            GameObject newEntry = Instantiate(entryTemplate, entryContainer);
            TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            AnsweredQns answeredQns = playerData.exploreAnsweredQns[i];
            string stageName = answeredQns.stageName + " exploration";
            // stage 
            texts[0].text = stageName;
            int correct = answeredQns.qnsCorrect.Count;
            int total = correct + answeredQns.qnsWrong.Count;
            // total score
            texts[1].text = correct.ToString() + "/" + total.ToString();
            newEntry.name = stageName;
            newEntry.GetComponent<ViewQuestionBreakdwonTeacher>().playerJson = playerJson;
        }
        
        int numQuizStage = playerData.quizAnsweredQns.Length;
        // for quiz stages
        for (int i = 0; i < numQuizStage; i++)
        {
            GameObject newEntry = Instantiate(entryTemplate, entryContainer);
            TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            AnsweredQns answeredQns = playerData.quizAnsweredQns[i];
            string stageName = answeredQns.stageName + " quiz";
            texts[0].text = stageName;
            int correct = answeredQns.qnsCorrect.Count;
            int total = correct + answeredQns.qnsWrong.Count;
            texts[1].text = correct.ToString() + "/" + total.ToString();
            newEntry.name = stageName;
            newEntry.GetComponent<ViewQuestionBreakdwonTeacher>().playerJson = playerJson;
        }
    }
    // load user data from firebase
    private IEnumerator LoadUserData(string userID,System.Action<string> callback)
    { 
        var DBTask = DBreference.Child("users").Child(userID).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            callback(snapshot.GetRawJsonValue());
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
