using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
/**
 * A class that handles all mastery report UI 
 */
public class MasteryReportUIManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject MasteryReportTable; /** A unity gameObject containing all mastery report UI */
    public GameObject QuestionBreakdown;/** A unity gameObject containing all question breakdown UI */
    public GameObject Menu_UI; /** A unity gameObject containg all menu UI */
    /**
     * this methods handles OnClick for view question breakdown button
     */
    public void clickViewQuestionBreakdown()
    {
        MasteryReportTable.SetActive(false);
        QuestionBreakdown.SetActive(true);
    }
    /**
     * this methods handles OnClick for back button in question breakdown page
     */
    public void clickBack()
    {
        MasteryReportTable.SetActive(true);
        QuestionBreakdown.SetActive(false);
    }
    /**
     * this methods handles OnClick for back to menu button 
     */
    public void clickBackToMenu()
    {
        MasteryReportTable.SetActive(false);
        Menu_UI.SetActive(true);
    }
    /**
     * this method handles OnClick for play button
     */
    public void PlayButton()
    {
        SceneManager.LoadScene("StageSelect");
    }
    /**
     * this method handles OnClick for custom stage button
     */
    public void CustomStageButton()
    {
        SceneManager.LoadScene("CustomStage");
    }
    /**
     * this method handles OnClick for mastery report button
     */
    public void MasteryReportButton()
    {
        SceneManager.LoadScene("MasteryReport");
    }
    /**
     * this method handles OnClick for leaderboard button
     */
    public void LeaderboardButton()
    {
        SceneManager.LoadScene("Leaderboard");
    }
    /**
     * this method handles OnClick for sign out button
     */
    public void SignOutButton()
    {
        GameObject firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        FirebaseAuth auth = firebasesManager.GetComponent<FirebaseManager>().auth;
        auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }
}
