using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DatabaseQuestion
{
    public string question;
    public Dictionary<string, string> answers = new Dictionary<string, string>();
    
    
    public DatabaseQuestion(string questionInput, string aInput, string bInput, string cInput, string correctInput, string dInput)
    {
        question = questionInput;
        answers.Add("a", aInput);
        answers.Add("b", bInput);
        answers.Add("c", cInput);
        answers.Add("d", dInput);
        answers.Add("correct", correctInput);
    }

}
