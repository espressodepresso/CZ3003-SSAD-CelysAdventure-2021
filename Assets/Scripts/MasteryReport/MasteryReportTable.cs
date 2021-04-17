using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;
using System;
/**
 * A class that handles Mastery Report logic
 */
public class MasteryReportTable : MonoBehaviour
{
    public Transform entryContainer; /** A unity game object transform container that will contain all rows of the mastery report */
    public GameObject entryTemplate; /** A unity game object container that will contain one row of the mastery report */

    private PlayerInfo playerData; /** PlayerInfo object of the player */
    private string playerJson; /** PlayerInfo serialised in json string */

    private GameObject firebasesManager; /** A firebase class that contains the script to FirebaseManager */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */

    void Awake()
    {
        // get the firebase object
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");

        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        StartCoroutine(LoadUserData(returnValue =>
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
    /**
     * this method handles displaying all the mastery report rows from the playerData
     */
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
            newEntry.GetComponent<ViewQuestionBreakdwon>().playerJson = playerJson;


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
            newEntry.GetComponent<ViewQuestionBreakdwon>().playerJson = playerJson;




        }




    }
    /**
     * this method handles getting current user data from the database
     * @param callback - the callback function to be executed when data fetch is done
     */
    private IEnumerator LoadUserData(System.Action<string> callback)
    {
        var DBTask = DBreference.Child("users").Child(firebasesManager.GetComponent<FirebaseManager>().userID).GetValueAsync();

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

}
