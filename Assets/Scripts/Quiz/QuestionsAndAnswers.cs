[System.Serializable]

public class QuestionsAndAnswers
{
    public string Question;
    public string[] Answers;
    public int CorrectAnswer;
    public string id = "random";
    
    public QuestionsAndAnswers(string inQuestion, string[] inAnswers, int inCorrect){
        Question = inQuestion;
        Answers = inAnswers;
        CorrectAnswer = inCorrect;
    }
}
