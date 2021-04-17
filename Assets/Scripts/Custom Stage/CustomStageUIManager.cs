using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

//! A class that handles UI functions for custom stage

public class CustomStageUIManager : MonoBehaviour
{
    List<string> pokemonSelected; /** A list of selected pokemons for use within this class */
    string background; /** The selected background */
    public string quizId; /** The custom stage ID */
    public GameObject SelectPokemonUI; /** An unity gameObject containing selection of pokemon UI objects */
    public GameObject SelectQuestionUI; /** An unity gameObject containing selection of questions UI objects */
    public GameObject AddYourOwnQuestionsUI; /** An unity gameObject containing create own question UI objects */
    public GameObject Menu_UI; /** An unity gameObject containing menu UI objects */
    public GameObject ShareCustomStage_UI; /** An unity gameObject containing share custom stage UI objects */

    private GameObject firebasesManager; /** A firebase class that contains the script to FirebaseManager */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */
    void Awake()
    {
        pokemonSelected = new List<string>();
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
    }
    /**
     * this method handles OnClick for sign out 
     */
    public void SignOutButton()
    {
        FirebaseAuth auth = firebasesManager.GetComponent<FirebaseManager>().auth;
        auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }
    /**
     * this method handles OnClick for pokemon button 
     */
    public void SelectPokemon(string name)
    {
        if (pokemonSelected.Contains(name))
        {
            pokemonSelected.RemoveAll(pokemon => pokemon == name);
        } else
        {
            pokemonSelected.Add(name);
        }
        
    }
    /** 
     * this method handles OnClick for background button
     */
    public void SelectBackground(string name)
    {
        background = name;
       
    }
    /**
     * this method handles OnClick for OK in create your own question UI
     */
    public void clickOKInAddYourOwnQuestionsUI()
    {
        AddYourOwnQuestionsUI.SetActive(false);
        SelectQuestionUI.SetActive(true);
        GameObject.Find("QuestionsManager").GetComponent<CustomStageChooseQuestions>().Dropdown_IndexChanged(0);
       
    }
    /**
     * this method handles OnClick for add your own question in select question UI
     */
    public void clickAddYourOwnQuestionsButton()
    {
        SelectQuestionUI.SetActive(false);
        AddYourOwnQuestionsUI.SetActive(true);
    }
    /**
     * this method handles OnClick for next button in select pokemon UI
     */
    public void clickNextButton()
    {
        SelectPokemonUI.SetActive(false);
        SelectQuestionUI.SetActive(true);
    }
    /**
     * this method handles OnClick for finish button in select question UI
     */
    public void clickFinishUIButton()
    {
        SelectQuestionUI.SetActive(false);
        ShareCustomStage_UI.SetActive(true);
    }
    /**
     * this method handles OnClick for back to menu button in share custom stage UI
     */
    public void clickBackToMenuButton()
    {
        ShareCustomStage_UI.SetActive(false);
        Menu_UI.SetActive(true);
    }
    /**
     * this method handles OnClick for finish button in select question UI
     */
    public void clickFinishButton()
    {
        List<string> chosenQuestionsList = GameObject.Find("QuestionsManager").GetComponent<CustomStageChooseQuestions>().getChosenQuestionsList();
        foreach(string chosenQuestion in chosenQuestionsList)
        {
            Debug.Log(chosenQuestion);
        }
        foreach(string pokemon in pokemonSelected)
        {
            Debug.Log(pokemon);
        }
        Debug.Log(background);
        CustomStage customStage = new CustomStage(pokemonSelected, chosenQuestionsList, background);
        string json = customStage.SaveToJSON();
        StartCoroutine(writeCustomStageToFirebase(json));
        //StartCoroutine(queryQuestion("1_hard_5_question"));
        //StartCoroutine(getCustomStageFromFirebase("12345"));
        
        
        
        
    }
    /**
     * this method handles writing the newly created data to the database
     * @param json - json string of custom stage data
     */
    private IEnumerator writeCustomStageToFirebase(string json)
    {

        quizId = DBreference.Child("customStage").Push().Key;
        var pokemonTask = DBreference.Child("customStage").Child(quizId).SetRawJsonValueAsync(json);
  
        yield return new WaitUntil(predicate: () => pokemonTask.IsCompleted);
        if (pokemonTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {pokemonTask.Exception}");
        }
        else
        {
            Debug.Log("successful update custom stage to DB");
        }

    }
    /**
     * this method gets the custom stage data from database - this method is for testing only
     * @param id - id of the custom stage requested
     */
    private IEnumerator getCustomStageFromFirebase(string id)
    {
        var DBTask = DBreference.Child("customStage").Child(id).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            string json = snapshot.GetRawJsonValue();
            CustomStage customStage = CustomStage.CreateFromJSON(json);
            Debug.Log(customStage.questions[0]);

        }

    }
    /**
     * this method handles OnClick for play button in menu
     */
    public void PlayButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
    /**
     * this method handles OnClick for custom stage button in menu
     */
    public void CustomStageButton()
    {
        SceneManager.LoadScene("CustomStage");
    }
    /**
     * this method handles OnClick for mastery report button in menu
     */
    public void MasteryReportButton()
    {
        SceneManager.LoadScene("MasteryReport");
    }
    /**
     * this method handles OnClick for leaderboard button in menu
     */
    public void LeaderboardButton()
    {
        SceneManager.LoadScene("Leaderboard");
    }



}
