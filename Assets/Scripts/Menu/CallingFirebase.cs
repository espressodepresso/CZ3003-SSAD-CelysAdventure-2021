using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Collections.Generic;

/**
 *  A test class. This class is used to purely testing purpose to read/write from the database.
 */
public class CallingFirebase : MonoBehaviour
{
    private GameObject firebasesManager; /** A unity gameObject that contains the firebase init data */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */
    public List<QuestionsAndAnswers> questionsList = new List<QuestionsAndAnswers>();  /** List that contains the questions data from the database */

    void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        //Debug.Log("Heredeath");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        //StartCoroutine(UpdateUserData());
        //Get the currently logged in user data
        //StartCoroutine(LoadUserData());
        /*
        StartCoroutine(RequestForQuestions("1_hard", "gh123", 2, returnValue =>
        {

            returnValue.ForEach(item => Debug.Log(item.question));
        }
        ));*/




    }
    /**
     * this method loads user data from firebase database based on the current logged in user.
     */
    private IEnumerator LoadUserData()
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

            Debug.Log(snapshot.Child("deaths").Value.ToString());
            Debug.Log(snapshot.Child("kills").Value.ToString());
            Debug.Log(snapshot.Child("username").Value.ToString());
        }
    }
    /**
     * this method gets questions data from the database
     * @param levelRequested - the level of the questions
     * @param quizId - the id of the current quiz
     * @param numQuestions - the number of questions to be taken from database
     * @param callback - the callback function to be executed with the returned list of questions
     * @return a list of questions from the database
    */
    private IEnumerator RequestForQuestions(string levelRequested,string quizId,int numQuestions, System.Action<List<QuestionsAndAnswers>> callback)
    {
    
        
        var DBTask = DBreference.Child("questions").Child(levelRequested).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshots = DBTask.Result;
            int count = 0;
            
            //long childrenCount = snapshots.ChildrenCount;
            foreach (var childSnapshot in snapshots.Children)
            {
                count++;
                string question = childSnapshot.Child("question").Value.ToString();
                string a = childSnapshot.Child("answers").Child("a").Value.ToString();
                string b = childSnapshot.Child("answers").Child("b").Value.ToString();
                string c = childSnapshot.Child("answers").Child("c").Value.ToString();
                string d = childSnapshot.Child("answers").Child("d").Value.ToString();
                string[] choices = new string[]{a,b,c,d};
                string correct = childSnapshot.Child("answers").Child("correct").Value.ToString();
                int correctIndex;
                if(correct == "a"){
                    correctIndex = 0;
                }else if(correct == "b"){
                    correctIndex = 1;

                }else if(correct == "c"){
                    correctIndex = 2;
                }else{
                    correctIndex = 3;
                }
                QuestionsAndAnswers tempItem = new QuestionsAndAnswers(question,choices,correctIndex);

                
                //Debug.Log(childSnapshot.Child("question").Value.ToString());
                //Debug.Log(childSnapshot.Child("answers").Child("d").Value.ToString());
                //Debug.Log(childSnapshot.Child("answers").Child("correct").Value.ToString());
                //Debug.Log("-------------------------");
                questionsList.Add(tempItem);
                if(count == numQuestions)
                {
                    break;
                }

            }
            //questionsList.ForEach(item => Debug.Log(item.question));
            callback(questionsList);



        }
        

    }
    /**
     * this method updates user data of currently logged in user to database
    */

    private IEnumerator UpdateUserData()
    {
        /*{
            "stageName": "1_hard",
      "qnsCorrect": { "0": 1 },
      "qnsWrong": { "0": 1 }
        }*/
        User user = new User("myat", "myat005@gmail.com");
        string json = JsonUtility.ToJson(user);
        var DBTask = DBreference.Child("users").Child(firebasesManager.GetComponent<FirebaseManager>().userID).SetRawJsonValueAsync(json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
            Debug.Log("Its updated");
        }
    }

}
/**
 *  A class made to write user email, name data. 
 */
public class User
{
    
    public string email;
    public string user_name;

    public User()
    {
    }

    public User(string username, string email)
    {
        this.user_name = username;
        this.email = email;
    }
}
