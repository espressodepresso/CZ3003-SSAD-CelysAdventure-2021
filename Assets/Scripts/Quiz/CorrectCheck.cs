using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class CorrectCheck : MonoBehaviour
{
    string[] AnswerButtons = new string[] { "Answer1", "Answer2", "Answer3", "Answer4" };
    public bool isCorrect = false;

    public QuizManager quizManager;
    public void AnswerButtonClicked()
    {
        if (isCorrect) {
            StartCoroutine(nameof(ChangeColorGreen));

            
            isCorrect = false;
        }
        else 
        {
            StartCoroutine(nameof(ChangeColorRed));
        }
    }

    //void ChangeColorGreen()
    //{
    //    Image img = this.gameObject.GetComponent<Image>();
    //    img.color = UnityEngine.Color.green;
    //    Debug.Log("Color changed to green");
    //}
    
    void ChangeColorWhite()
    {
        Image img = this.gameObject.GetComponent<Image>();
        img.color = UnityEngine.Color.white;
        //Debug.Log("Color changed to white");
    }
    IEnumerator ChangeColorGreen()
    {
        Image img = this.gameObject.GetComponent<Image>();
        img.color = UnityEngine.Color.green;
        Debug.Log("Changed to green");
        yield return new WaitForSecondsRealtime(1);

        this.ChangeColorWhite();
        quizManager.Correct();
    }

    IEnumerator ChangeColorRed()
    {
        Image img = this.gameObject.GetComponent<Image>();
        img.color = UnityEngine.Color.red;
        Debug.Log("Changed to red");
        yield return new WaitForSecondsRealtime(0.5f);

        this.ChangeColorWhite();
        quizManager.Wrong();
    }
}
