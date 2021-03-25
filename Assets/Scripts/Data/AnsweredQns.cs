using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnsweredQns
{
    public string stageName;
    public Dictionary<string,int> qnsCorrect;
    public Dictionary<string,int> qnsWrong;
}
