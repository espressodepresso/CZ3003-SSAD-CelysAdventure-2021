using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnsweredQns
{
    public string stageName;
    public Dictionary<string,int> qnsCorrect;
    public Dictionary<string,int> qnsWrong;
    
    public AnsweredQns()
    {

    }
    public AnsweredQns(string stagename)
    {
        stageName = stagename;
        qnsCorrect = new Dictionary<string, int>
        {
            {"0_question",0}
        };
        qnsWrong = new Dictionary<string, int>
        {
            {"0_question",0}
        };
    }
}
