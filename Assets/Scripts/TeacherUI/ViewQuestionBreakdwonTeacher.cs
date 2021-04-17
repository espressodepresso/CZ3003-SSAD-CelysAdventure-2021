using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewQuestionBreakdwonTeacher : MonoBehaviour
{
    // json string that contains player data
    public string playerJson;
   
    // called when user clicks view question breakdown
    public void clickQuestionBreakdown()
    {
        GameObject.FindGameObjectWithTag("TeacherUI").GetComponent<TeacherUIManager>().questionBreakdownScreen();
        GameObject.FindGameObjectWithTag("QuestionBreakdown").GetComponent<QuestionBreakdownManagerTeacher>().ShowQuestionBreakdown(this.name,playerJson);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
