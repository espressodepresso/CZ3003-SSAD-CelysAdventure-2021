using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;

public class CreateOwnQuestion : MonoBehaviour
{
    public TMP_InputField questionTitle;
    public TMP_InputField A;
    public TMP_InputField B;
    public TMP_InputField C;
    public TMP_InputField D;

    public GameObject UIManager;
    private GameObject firebasesManager;
    public DatabaseReference DBreference;

    private string[] levels = new string[] { "1_easy", "1_medium", "1_hard", "2_easy", "2_medium", "2_hard", "3_easy", "3_medium", "3_hard", "4_easy", "4_medium", "4_hard", "5_easy", "5_medium", "5_hard" };
    private string[] answerOptions = new string[] { "a", "b", "c", "d" };

    private string level = "1_easy";
    private string correctAns="a";
    private long childCount = 0;

    private void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
    }

    public void ClickOKButton()
    {
     
        StartCoroutine(RequestForLevelChildCount(updateToDB));
        
    }
    public void LevelDropdown_IndexChanged(int index)
    {
        level = levels[index];
    }
    public void CorrectAnsDropdown_IndexChanged(int index)
    {
        correctAns = answerOptions[index];
    }
   
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
    
    public void updateToDB()
    {
       
        DatabaseQuestion question = new DatabaseQuestion(questionTitle.text, A.text, B.text, C.text, correctAns, D.text);
        StartCoroutine(AddNewQuestionToFirebase(question));
    }

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
