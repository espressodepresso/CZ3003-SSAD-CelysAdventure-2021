using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Database;

public class QuestionBreakdownManagerTeacher : MonoBehaviour
{
    public Transform entryContainer;
    public GameObject entryTemplate;
    public GameObject title;
    public GameObject scrollBar;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;

    private PlayerInfo playerData;
    public List<QuestionsAndAnswers> questionsList = new List<QuestionsAndAnswers>();
    void Awake()
    {
        // get the firebase object
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");

        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;

    }
    private IEnumerator RequestForQuestions(string levelRequested, string quizId, System.Action callback)
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
                tempItem.id = levelRequested + "_" + dbCount.ToString();
                dbCount++;


                questionsList.Add(tempItem);


            }
            callback();

           





        }


    }

    // called when user clicks view question breakdown

    public void ShowQuestionBreakdown( string stage, string playerJson)
    {
        // load player data 
        playerData = PlayerInfo.CreateFromJSON(playerJson);

        // change the title to the stage 

        title.GetComponent<TextMeshProUGUI>().text = stage;

        // stage is  something like "1_easy exploration" so need to split
        string[] stages = stage.Split(' ');

        
  

        StartCoroutine(RequestForQuestions(stages[0], "gh123",()=>
        {
            // clear existing questions before displaying new ones
            foreach (Transform item in entryContainer)
            {
                Destroy(item.gameObject);
            }
            displayQuestionData(stages[0], stages[1]);
        }));
       
    }
    // displaying the questions on UI
    public void displayQuestionData(string stage,string typeStage)
    {
        int count = 0;
        AnsweredQns[] questionsArr;
        if (typeStage == "exploration")
        {
             questionsArr = playerData.exploreAnsweredQns;
        }
        else
        {
            questionsArr = playerData.quizAnsweredQns;
        }
        foreach (AnsweredQns answeredQn in  questionsArr)
        {
            if (answeredQn.stageName == stage)
            {
                break;
            }
            count++;
        }
        Dictionary<string, int> qnsCorrect = questionsArr[count].qnsCorrect;
        Dictionary<string, int> qnsWrong = questionsArr[count].qnsWrong;
        instantiateQuestions(qnsCorrect, true);
        instantiateQuestions(qnsWrong, false);

    }
    // Unity method to display the questions
    public void instantiateQuestions(Dictionary<string, int> questions, bool correct)
    {
        foreach (KeyValuePair<string, int> question in questions)
        {
        
            string[] questionNumber = question.Key.Split('_');
            int indexToAccess = int.Parse(questionNumber[0]);
            QuestionsAndAnswers questionToAccess = questionsList[indexToAccess];
            GameObject newEntry = Instantiate(entryTemplate, entryContainer);
            TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = questionToAccess.Question;
            texts[1].text = "a) " + questionToAccess.Answers[0];
            texts[2].text = "b) " + questionToAccess.Answers[1];
            texts[3].text = "c) " + questionToAccess.Answers[2];
            texts[4].text = "d) " + questionToAccess.Answers[3];
            int correctIndex = questionToAccess.CorrectAnswer;
            texts[correctIndex].color = Color.green;
            
            string correctStr = "";
            if (correct)
            {
                GameObject wrong = newEntry.transform.GetChild(7).gameObject;
                wrong.SetActive(false);
                correctStr = "Correct";     
            }
            else
            {
                GameObject tick = newEntry.transform.GetChild(6).gameObject;
                tick.SetActive(false);
                correctStr = "Wrong";
            }
            texts[5].text = "Number of Attempts: " + question.Value.ToString() + " | Score: "+ correctStr;

        }
    }
    /* this function does not work 
    public void resetScroll()
    {
        scrollBar.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }
   
    */
    // Update is called once per frame
    void Update()
    {
        
    }
}
