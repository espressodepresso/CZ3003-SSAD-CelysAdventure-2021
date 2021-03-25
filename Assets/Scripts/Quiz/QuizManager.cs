using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class QuizManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI QuestionTxt;
    public TMPro.TextMeshProUGUI Score;
    public Button Ans1;
    public Button Ans2;
    public Button Ans3;
    public Button Ans4;

    public event Action OnQuizEnd;
    public event Action OnReplace;

    private int targetScore = 2;
    private string currerntQuestionID;
    private int currerntQuestion;
    private int score = 0;
    private int error = 0;
    private List<QuestionsAndAnswers> QnA;
    public Cat curCat;
    private bool inProgress;
    private bool isQuiz = false;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;
    public List<QuestionsAndAnswers> questionsList = new List<QuestionsAndAnswers>();
    public GameObject playerDataManager;
    void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");

        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;

        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");
        string curStage = playerDataManager.GetComponent<DataManager>().getcurStage();
        StartCoroutine(RequestForQuestions(curStage, "gh123", returnValue =>
             {

                 returnValue.ForEach(item =>
                 {
                    //Debug.Log(item.Question);
                    //Debug.Log(item.CorrectAnswer);

                });
             }
            ));



    }
    private IEnumerator RequestForQuestions(string levelRequested, string quizId, System.Action<List<QuestionsAndAnswers>> callback)
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


                //Debug.Log(childSnapshot.Child("question").Value.ToString());
                //Debug.Log(childSnapshot.Child("answers").Child("d").Value.ToString());
                //Debug.Log(childSnapshot.Child("answers").Child("correct").Value.ToString());
                //Debug.Log("-------------------------");
                questionsList.Add(tempItem);
            }
            //questionsList.ForEach(item => Debug.Log(item.question));
            callback(questionsList);
        }
    }
    public void Correct()
    {
        inProgress = false;
        this.score++;
        playerDataManager.GetComponent<DataManager>().updateQns("1_hard", currerntQuestionID, true, isQuiz);
        playerDataManager.GetComponent<DataManager>().updateQuizCat(curCat.catName);
        StartCoroutine(playerDataManager.GetComponent<DataManager>().SaveUserDataToFirebase());
        GetQuestion(this.questionsList, this.targetScore);
    }
    public void Wrong()
    {
        playerDataManager.GetComponent<DataManager>().updateQns("1_hard", currerntQuestionID, false, isQuiz);
        this.error++;
    }

    public void GetQuestion(List<QuestionsAndAnswers> Questions, int targetScore)
    {
        if (score == targetScore)
        {
            this.score = 0;
            this.error = 0;
            Score.text = score.ToString();
            OnQuizEnd();
            if (!isQuiz)
            {
                if (playerDataManager.GetComponent<DataManager>().getCurCats().Count == 6)
                {
                    OnReplace();
                }
                else
                {
                    playerDataManager.GetComponent<DataManager>().addCat(curCat.catName);
                }
            }

        }
        else if (Questions.Count == 0)
        {
            Score.text = score.ToString();
            QuestionTxt.text = "The Quiz has ended.";
            OnQuizEnd();
        }
        else
        {
            inProgress = true;
            ResetAnswersCheck();
            Score.text = score.ToString();
            currerntQuestion = UnityEngine.Random.Range(0, Questions.Count);
            currerntQuestionID = currerntQuestion.ToString() + "_question";
            QuestionTxt.text = Questions[currerntQuestion].Question;
            SetAnswers(Questions[currerntQuestion]);

            //Choose a random pokemon and add to the quiz
            //Make sure to check quiz flag
            if (!isQuiz)
            {
                curCat = playerDataManager.GetComponent<DataManager>().getRandomCat();
                var spritePath = "Pokemon_Sprites/" + curCat.spriteFilename;
                GameObject.Find("QuizPokemon").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
            }

            //Questions.RemoveAt(currerntQuestion);
            //Debug.Log(Questions.Count);
        }
    }

    public void SetAnswers(QuestionsAndAnswers qna)
    {

        Ans1.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = qna.Answers[0];
        Ans2.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = qna.Answers[1];
        Ans3.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = qna.Answers[2];
        Ans4.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = qna.Answers[3];

        switch (qna.CorrectAnswer)
        {
            case 1:
                Ans1.GetComponent<CorrectCheck>().isCorrect = true;
                break;
            case 2:
                Ans2.GetComponent<CorrectCheck>().isCorrect = true;
                break;
            case 3:
                Ans3.GetComponent<CorrectCheck>().isCorrect = true;
                break;
            case 4:
                Ans4.GetComponent<CorrectCheck>().isCorrect = true;
                break;
        }
    }

    void ResetAnswersCheck()
    {
        Ans1.GetComponent<CorrectCheck>().isCorrect = false;
        Ans2.GetComponent<CorrectCheck>().isCorrect = false;
        Ans3.GetComponent<CorrectCheck>().isCorrect = false;
        Ans4.GetComponent<CorrectCheck>().isCorrect = false;
    }

    public string getCurCat()
    {
        return curCat.catName;
    }

    public void HandleUpdate(int tScore)
    {
        while (!inProgress)
        {

            //this.QnA = Question();
            this.targetScore = tScore;
            GetQuestion(this.questionsList, this.targetScore);
        }

        // for debugging remove when done
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnQuizEnd();
        }
    }
}
