using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * A class that handles viewing question breakdown click logic
 */
public class ViewQuestionBreakdwon : MonoBehaviour
{
    
    public string playerJson; /** json string that contains player data */

    /**
     * this method is a wrapper function that handles OnClick for view question breakdown button
     */
    public void clickQuestionBreakdown()
    {
       
        GameObject.FindGameObjectWithTag("MasteryReportUI").GetComponent<MasteryReportUIManager>().clickViewQuestionBreakdown();
        GameObject.FindGameObjectWithTag("QuestionBreakdown").GetComponent<QuestionBreakdownManager>().ShowQuestionBreakdown(this.name,playerJson);
    }
}
