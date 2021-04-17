using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/** 
 * A class that handles choosing a question for custom stage creation
 */
public class ChooseQuestionButton : MonoBehaviour
{
    private GameObject questionManager; /** A unity gameObject that contains script to questionManager */
    private bool selectedState = false; /** A state of whether this question is selected or not */

    /** 
     * this method displays the selected state of a question when UI reloads 
     */
    public void StartButton()
    {
        questionManager = GameObject.Find("QuestionsManager");
        List<string> chosenQuestionList = questionManager.GetComponent<CustomStageChooseQuestions>().getChosenQuestionsList();
        if (chosenQuestionList.Contains(this.name))
        {
            selectedState = true;
            GetComponent<Image>().color = Color.yellow;

        }

    }
    /**
     * this method handles toggling selection state of this question and adds or removes from questionManager accordingly.
     */
    public void clickChooseQuestion()
    {
        
        selectedState = !selectedState;
        if (selectedState)
        {
            questionManager.GetComponent<CustomStageChooseQuestions>().addToChosenQuestionsList(this.name);
            GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            questionManager.GetComponent<CustomStageChooseQuestions>().removeFromChosenQuestionsList(this.name);
            GetComponent<Image>().color = Color.white;
        }
        
    }
}
