using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
 
//! A class for handling creating own question for custom stage creation

public class CreateOwnQuestion : MonoBehaviour
{
    public TMP_InputField questionTitle; /** A TMPro class for question title input */
    public TMP_InputField A; /** A TMPro class for a option input */
    public TMP_InputField B; /** A TMPro class for b option input */
    public TMP_InputField C; /** A TMPro class for c option input */
    public TMP_InputField D; /** A TMPro class for d option input */

    public GameObject UIManager; /** A unity gameObject that contains script to UIManager */
    private GameObject firebasesManager; /** A firebase class that contains the script to FirebaseManager */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */

    private string[] levels = new string[] { "1_easy", "1_medium", "1_hard", "2_easy", "2_medium", "2_hard", "3_easy", "3_medium", "3_hard", "4_easy", "4_medium", "4_hard", "5_easy", "5_medium", "5_hard" }; /** all the possible levels */
    private string[] answerOptions = new string[] { "a", "b", "c", "d" }; /** all the possible options for a question */

    private string level = "1_easy"; /** The current level that question is created for and intitialise current level as 1_easy */
    private string correctAns="a"; /** The correct answer of that question and initialise as a */
    private long childCount = 0; /** The number of questions in the current level that are already in the database */

    private void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
    }
    /**
     * this method handles OnClick for the OK button 
     */
    public void ClickOKButton()
    {
     
        StartCoroutine(RequestForLevelChildCount(updateToDB));
        
    }
    /** 
     * this method handles OnChange for the dropdown index for level choice of the question
     * @param index - chosen index by user
     */
    public void LevelDropdown_IndexChanged(int index)
    {
        level = levels[index];
    }
    /**
     * this method handles OnChange for the dropdown index for correct ans choice of the question
     * @param index - chosen index by user
     */
    public void CorrectAnsDropdown_IndexChanged(int index)
    {
        correctAns = answerOptions[index];
    }
    /**
     * this method handles adding a new question created by user to the database 
     * @param questionObj - a DatabaseQuestion object containing question created by user
     */
    private IEnumerator AddNewQuestionToFirebase(DatabaseQuestion questionObj)
    {
        string json = JsonConvert.SerializeObject(questionObj);
        var DBTask = DBreference.Child("questions").Child(level).Child(childCount.ToString()).SetRawJsonValueAsync(json);

        //var DBTask = DBreference.Child("questions").Child(level).Child("4").SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("successful update to DB");
            UIManager.GetComponent<CustomStageUIManager>().clickOKInAddYourOwnQuestionsUI();
        }
    }
    /**
     * this method handles forming the Database Question Object and calling AddNewQuestionToFirebase method
     */
    public void updateToDB()
    {
       
        DatabaseQuestion question = new DatabaseQuestion(questionTitle.text, A.text, B.text, C.text, correctAns, D.text);
        StartCoroutine(AddNewQuestionToFirebase(question));
    }
    /**
     * this method handles requesting for the number of questions in the current level that are already in the database
     * @param callback - callback function to be executed when this function finishes
     */
    private IEnumerator RequestForLevelChildCount(System.Action callback)
    {
        var DBTask = DBreference.Child("questions").Child(level).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshots = DBTask.Result;
            childCount = snapshots.ChildrenCount;
            callback();
        }
    }

}
