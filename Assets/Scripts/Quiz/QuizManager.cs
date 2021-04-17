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
    public Button NextButton;

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
    private bool customStageCheck;
    private bool isQuiz = false;
    private int previousErrors = 0;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;
    public List<QuestionsAndAnswers> questionsList = new List<QuestionsAndAnswers>();
    public GameObject playerDataManager;
    public GameObject GymQuizStuff;
    public GameObject playerCustomManager;
    public GameObject playerController;
    public TMPro.TextMeshProUGUI PokemonLeft;

    public GameObject resultsPanel;
    public TMPro.TextMeshProUGUI resultsCorrect;
    public TMPro.TextMeshProUGUI resultsWrong;


    void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        playerDataManager = GameObject.FindGameObjectWithTag("DataManager");
        playerCustomManager = GameObject.FindGameObjectWithTag("CustomStage");
        playerController = GameObject.FindGameObjectWithTag("PlayerController");
        this.LoadQuestions();
    }

    /**
     * Returns difficulty for the next question based on student's scores on previous questions. 
     * 
     * @return String of the next difficulty.
     */
    private string NextStageDifficulty()
    {
    string nextMap = playerDataManager.GetComponent<DataManager>().getNextMap();
        
        if (previousErrors > 3)
        {
            return nextMap[nextMap.Length - 1].ToString() + "_easy";
        }
        if (previousErrors > 0 && previousErrors < 4)
        {
            return nextMap[nextMap.Length - 1].ToString() + "_medium";
        }
        else
        {
            return nextMap[nextMap.Length - 1].ToString() + "_hard";
        }
    }
    /**
     * Loads questions from FIrebase Database. 
     * 
     */
    public void LoadQuestions()
    {
        //playerDataManager.GetComponent<DataManager>().LoadNewStage();
        if (playerDataManager.GetComponent<DataManager>().getCurMap() == "Background_Gym")
        {
            playerDataManager.GetComponent<DataManager>().SetCurStage(NextStageDifficulty());
        }
        if (playerCustomManager.GetComponent<PlayCustomStage>().customStageCheck)
        {
            var questions = playerCustomManager.GetComponent<PlayCustomStage>().getCustomQuestions();
            foreach (var qn in questions)
            {
                StartCoroutine(queryQuestion(qn));
            }
        }
        else
        {
            string curStage = playerDataManager.GetComponent<DataManager>().getcurStage();
            Debug.Log("current stage: " + curStage);
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
    }
    /**
     * Request quesions from Firebase database with the appropriate difficulty.
     * 
     * @param levelRequested difficulty for requested questions
     */ 
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
    /**
     * 
     */
    private IEnumerator queryQuestion(string questionId)
    {
        string[] questionIds = questionId.Split('_');
        string stage = questionIds[0] + "_" + questionIds[1];
        string questionNum = questionIds[2];

        var DBTask = DBreference.Child("questions").Child(stage).Child(questionNum).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            string question = snapshot.Child("question").Value.ToString();
            string a = snapshot.Child("answers").Child("a").Value.ToString();
            string b = snapshot.Child("answers").Child("b").Value.ToString();
            string c = snapshot.Child("answers").Child("c").Value.ToString();
            string d = snapshot.Child("answers").Child("d").Value.ToString();
            string[] choices = new string[] { a, b, c, d };
            string correct = snapshot.Child("answers").Child("correct").Value.ToString();
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
            questionsList.Add(tempItem);
        }

    }
    /**
     * If the correct option is selected, this function is carried out. Score is incremented and the next quesiton is fetched.
     */
    public void Correct()
    {
        inProgress = false;
        this.score++;
        //NextButton.interactable = true;
        playerDataManager.GetComponent<DataManager>().updateQns("1_hard", currerntQuestionID, true, isQuiz);
        if (playerDataManager.GetComponent<DataManager>().getCurMap() != "Background_Gym")
        {
            playerDataManager.GetComponent<DataManager>().updateQuizCat(curCat.catName);
        }
        StartCoroutine(playerDataManager.GetComponent<DataManager>().SaveUserDataToFirebase());
        GetQuestion(this.questionsList, this.targetScore);
    }

    /**
     * If the wrong option is selected, this function is carried out. Error is incremented and recorded.
     */
    public void Wrong()
    {
        playerDataManager.GetComponent<DataManager>().updateQns("1_hard", currerntQuestionID, false, isQuiz);
        this.error++;
        if (playerDataManager.GetComponent<DataManager>().getCurMap() == "Background_Gym")
        {
            StartCoroutine(playerDataManager.GetComponent<DataManager>().SaveUserDataToFirebase());
            //remove cat
            if (playerDataManager.GetComponent<DataManager>().CatListLength() == 0)
            {
                OnQuizEnd();
            }
            GetQuestion(this.questionsList, this.targetScore);
        }
    }

    /**
     * Result panel is set to inactive after 3 seconds, and score and error variables are reset to 0.
     */
    public IEnumerator GymQuizSuccess()
    {
        
        this.score = 0;
        this.error = 0;
        Score.text = score.ToString();
        yield return new WaitForSeconds(3);
        resultsPanel.SetActive(false);
        OnQuizEnd();
    }
    /**
     * Displays questions on UI Panel. Also checks for conditions of the quiz ending: namely too many errors or student has
     * answered the required number of questions correctly. If either conditions are fufilled, end of the quiz is triggered.
     * 
     * @param Questions a list of questions in the format of a list of QuestionsAndAnswers 
     * @param targetScore number of questions student needs to answer correctly for quiz to end
     */
    public void GetQuestion(List<QuestionsAndAnswers> Questions, int targetScore)
    {
        if (score == targetScore)
        {
            
            if (playerDataManager.GetComponent<DataManager>().getCurMap() == "Background_Gym")
            {
                //Debug.Log("is it coming here");
                resultsCorrect.text = score.ToString();
                resultsWrong.text = error.ToString();
                resultsPanel.SetActive(true);
                StartCoroutine(GymQuizSuccess());
            }
            else
            {
                this.score = 0;
                this.error = 0;
                Score.text = score.ToString();
                OnQuizEnd();
            }
            
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
        else if (this.error == 5)
        {
            Score.text = score.ToString();
            previousErrors = score;
            //QuestionTxt.text = "The Quiz has ended.";
            this.score = 0;
            this.error = 0;
            Score.text = score.ToString();
            OnQuizEnd();
            QuizFail();
        }
        else if (Questions.Count == 0)
        {
            Score.text = score.ToString();
            previousErrors = score;
            //QuestionTxt.text = "The Quiz has ended.";
            this.score = 0;
            this.error = 0;
            Score.text = score.ToString();
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

            if (playerDataManager.GetComponent<DataManager>().getCurMap() == "Background_Gym")
            {
                Questions.RemoveAt(currerntQuestion);
            }
            //Choose a random pokemon and add to the quiz
            //Make sure to check quiz flag
            if (!isQuiz)
            {
                if (playerCustomManager.GetComponent<PlayCustomStage>().customStageCheck)
                {
                    
                    string curCatName = playerCustomManager.GetComponent<PlayCustomStage>().getRandomCat();
                    curCat =  playerDataManager.GetComponent<DataManager>().GetCatFromName(curCatName);
                }
                else
                {
                    curCat = playerDataManager.GetComponent<DataManager>().getRandomCat();                   
                }
                var spritePath = "Pokemon_Sprites/" + curCat.spriteFilename;
                GameObject.Find("QuizPokemon").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
            }
            
            //Debug.Log(Questions.Count);
        }
    }
    /**
     * Displays the different options for the answer on the UI panel.
     * 
     * @param qna a QuestionsAndAnswers
     */
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

    /**
     * Resets boolean value checking if option is correct to false.
     */
    void ResetAnswersCheck()
    {
        Ans1.GetComponent<CorrectCheck>().isCorrect = false;
        Ans2.GetComponent<CorrectCheck>().isCorrect = false;
        Ans3.GetComponent<CorrectCheck>().isCorrect = false;
        Ans4.GetComponent<CorrectCheck>().isCorrect = false;
    }


    /**
     * Fetches current pokemon
     * 
     * @return name of the pokemon
     */
    public string getCurCat()
    {
        return curCat.catName;
    }

    /**
     * Based on location of the player, returns number of questions required to pass the quiz.
     * 
     * @return number of questions
     */
    private int NumberOfQuestions()
    {
        Debug.Log("current map: " + playerDataManager.GetComponent<DataManager>().getCurMap());
        Debug.Log("current stage: " + playerDataManager.GetComponent<DataManager>().getcurStage());
        if (playerDataManager.GetComponent<DataManager>().getCurMap() != "Background_Gym")
        {
            return 1;
        }
        else
        {
            return 5;
        }
    }

    /**
     * Teleports player to outside of gym. Meant to be called when student fails gym quiz.
     */
    private void QuizFail()
    {
        float previousCoords_x = playerController.GetComponent<PlayerController>().previousCoords_x;
        float previousCoords_y = playerController.GetComponent<PlayerController>().previousCoords_y;
        string previousLocation = playerController.GetComponent<PlayerController>().previousLocation;
        playerController.GetComponent<PlayerController>().ChangeStageQuizFail(new Vector3(previousCoords_x, previousCoords_y, 0), previousLocation);
    }
    /**
     * Runs in the background when quiz starts and fetches new questions if needed.
     */
    public void HandleUpdate()
    {
        while (!inProgress)
        {

            //this.QnA = Question();
            //Debug.Log("Current stage:" + playerDataManager.GetComponent<DataManager>().getcurStage());
            //if (playerDataManager.GetComponent<DataManager>().getCurMap() == "Background_Gym")
            //{
            //    GymQuizStuff.SetActive(true);
            //    //PokemonLeft.text = playerDataManager.GetComponent<DataManager>().CatListLength().ToString();
            //}
            //else
            //{
            //    GymQuizStuff.SetActive(false);
            //}

            int tScore = this.NumberOfQuestions();
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
