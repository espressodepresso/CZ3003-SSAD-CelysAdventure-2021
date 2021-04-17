using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
/**
 * A class that handles leaderboard UI interactions 
 */
public class LeaderboardUIManager : MonoBehaviour
{

    public GameObject LeaderboardTable; /** A unity gameObject containing all leaderboard UI */
    public GameObject Menu_UI; /** A unity gameObject containg all menu UI */
    /**
     * this method handles OnClick for click back to menu button
     */
    public void clickBackToMenu()
    {
        LeaderboardTable.SetActive(false);
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
