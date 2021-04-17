using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
 
//! A class for question object created by the user in custom stage

public class DatabaseQuestion
{
    public string question; /** The question title */
    public Dictionary<string, string> answers = new Dictionary<string, string>(); /** A dictionary containg option name and its data */

    /** 
     * A constructor
     */
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
