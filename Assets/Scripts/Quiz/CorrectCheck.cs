using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class CorrectCheck : MonoBehaviour
{
    string[] AnswerButtons = new string[] { "Answer1", "Answer2", "Answer3", "Answer4" };
    public bool isCorrect = false;

    public QuizManager quizManager;
    /**
     * Changes button colour of option to green or red depending on if the option on the button is right or wrong.
     */
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

    /**
     * Changes button colour of option to white.
     */
    void ChangeColorWhite()
    {
        Image img = this.gameObject.GetComponent<Image>();
        img.color = UnityEngine.Color.white;
        //Debug.Log("Color changed to white");
    }
    /**
     * Changes button colour of option to green.
     */
    IEnumerator ChangeColorGreen()
    {
        Image img = this.gameObject.GetComponent<Image>();
        img.color = UnityEngine.Color.green;
        //Debug.Log("Changed to green");
        yield return new WaitForSecondsRealtime(1);

        this.ChangeColorWhite();
        quizManager.Correct();
    }
    /**
     * Changes button colour of option to red.
     */
    IEnumerator ChangeColorRed()
    {
        Image img = this.gameObject.GetComponent<Image>();
        img.color = UnityEngine.Color.red;
        //Debug.Log("Changed to red");
        yield return new WaitForSecondsRealtime(0.5f);

        this.ChangeColorWhite();
        quizManager.Wrong();
    }
}
