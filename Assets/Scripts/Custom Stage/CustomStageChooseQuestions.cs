using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;

public class CustomStageChooseQuestions : MonoBehaviour
{
    private GameObject firebasesManager;
    public DatabaseReference DBreference;
   

    public GameObject questionPrefab;
    public GameObject container;
    

    public List<QuestionsAndAnswers> questionsList = new List<QuestionsAndAnswers>();
    private string[] levels = new string[] { "1_easy", "1_medium", "1_hard", "2_easy", "2_medium", "2_hard", "3_easy", "3_medium", "3_hard", "4_easy", "4_medium", "4_hard", "5_easy", "5_medium", "5_hard" };
    private string currentLevel;

    private List<string> chosenQuestionsList = new List<string>();
    private List<GameObject> InstantiatedQuestions = new List<GameObject>();

  

    public void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        Dropdown_IndexChanged(0);



    }
    public List<string> getChosenQuestionsList()
    {
        return chosenQuestionsList;
    }
    public void addToChosenQuestionsList(string questionId)
    {
        chosenQuestionsList.Add(questionId);
       
    }
    public void removeFromChosenQuestionsList(string questionId)
    {
        string name = questionId;
        if (chosenQuestionsList.Contains(name))
        {
            chosenQuestionsList.RemoveAll(pokemon => pokemon == name);
        }
       
    }
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
                tempItem.id = levelRequested+ "_"+ dbCount.ToString();
                dbCount++;

        
                questionsList.Add(tempItem);


            }
         
            int count = 0;
            foreach (QuestionsAndAnswers question in questionsList)
            {
                int y_change = count * (250);
              
                GameObject tempObject = Instantiate(questionPrefab, new Vector3(container.transform.position.x, container.transform.position.y + 1150-y_change), Quaternion.identity, container.transform);
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
