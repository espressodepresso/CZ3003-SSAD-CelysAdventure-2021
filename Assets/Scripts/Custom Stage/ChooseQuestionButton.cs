using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseQuestionButton : MonoBehaviour
{
    private GameObject questionManager;
    private bool selectedState = false;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
