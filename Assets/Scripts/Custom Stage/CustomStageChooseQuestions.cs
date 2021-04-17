using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;

//! A class that handles choosing Questions - Questions Manager

public class CustomStageChooseQuestions : MonoBehaviour
{
    private GameObject firebasesManager; /** A firebase class that contains the script to FirebaseManager */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */


    public Transform entryContainer; /** A unity Transform object that contains container to display all the questions */
    public GameObject entryTemplate; /** A unity Gameobject that contains container to dispaly one question */

    public List<QuestionsAndAnswers> questionsList = new List<QuestionsAndAnswers>(); /** A list of questions to display */
    private string[] levels = new string[] { "1_easy", "1_medium", "1_hard", "2_easy", "2_medium", "2_hard", "3_easy", "3_medium", "3_hard", "4_easy", "4_medium", "4_hard", "5_easy", "5_medium", "5_hard" }; /** the levels in the game */
    private string currentLevel; /** the current level chosen by user */

    private List<string> chosenQuestionsList = new List<string>(); /** A list of chosen questions */
    private List<GameObject> InstantiatedQuestions = new List<GameObject>(); /** A list of questions Game Objects */

  

    public void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        Dropdown_IndexChanged(0);



    }
    /**
     * this method returns the chosen question list
     */
    public List<string> getChosenQuestionsList()
    {
        return chosenQuestionsList;
    }
    /** 
     * this method handles adding to the chosen question list
     * @param questionId - questionId of question to be added
     */
    public void addToChosenQuestionsList(string questionId)
    {
        chosenQuestionsList.Add(questionId);
       
    }
    /** 
     * this method handles removing from the chosen question list
     * @param questionId - questionId of question to be removed
     */
    public void removeFromChosenQuestionsList(string questionId)
    {
        string name = questionId;
        if (chosenQuestionsList.Contains(name))
        {
            chosenQuestionsList.RemoveAll(pokemon => pokemon == name);
        }
       
    }
    /** 
     * this method handles OnChange for the level dropdown index
     * @param index - current index of dropdown chosen by user
     */
    public void Dropdown_IndexChanged(int index)
    {
        questionsList.Clear();
        currentLevel = levels[index];
        foreach(GameObject instantiatedQuestion in InstantiatedQuestions)
        {
            Destroy(instantiatedQuestion);
        }
        StartCoroutine(RequestForQuestions(levels[index], "gh123"));
    }
    /**
     * this method handles getting questions from the database for a particular level
     * @param levelRequested - the level chosen 
     * @param quizId - can put any value
     */
    private IEnumerator RequestForQuestions(string levelRequested, string quizId)
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

            int dbCount = 0;
            //long childrenCount = snapshots.ChildrenCount;
            foreach (var childSnapshot in snapshots.Children)
            {

                string question = childSnapshot.Child("question").Value.ToString();
                string a = childSnapshot.Child("answers").Child("a").Value.ToString();
                string b = childSnapshot.Child("answers").Child("b").Value.ToString();
                string c = childSnapshot.Child("answers").Child("c").Value.ToString();
                string d = childSnapshot.Child("answers").Child("d").Value.ToString();
                string[] choices = new string[] { a, b, c, d };
                string correct = childSnapshot.Child("answers").Child("correct").Value.ToString();
                int correctIndex;
                if (correct == "a")
                {
                    correctIndex = 1;
                }
                else if (correct == "b")
                {
                    correctIndex = 2;

                }
                else if (correct == "c")
                {
                    correctIndex = 3;
                }
                else
                {
                    correctIndex = 4;
                }
                QuestionsAndAnswers tempItem = new QuestionsAndAnswers(question, choices, correctIndex);
                tempItem.id = levelRequested+ "_"+ dbCount.ToString()+"_question";
                dbCount++;

        
                questionsList.Add(tempItem);


            }
         
            int count = 0;
            foreach (QuestionsAndAnswers question in questionsList)
            {
                int y_change = count * (250);
              
                GameObject tempObject = Instantiate(entryTemplate, entryContainer);
                InstantiatedQuestions.Add(tempObject);
                tempObject.name = question.id;
                tempObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = question.Question;
                tempObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "a) "+question.Answers[0];
                tempObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "b) "+question.Answers[1];
                tempObject.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = "c) " + question.Answers[2];
                tempObject.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = "d) " + question.Answers[3];
                tempObject.GetComponent<ChooseQuestionButton>().StartButton();
                count++;
                
            }

       




        }


    }

}
