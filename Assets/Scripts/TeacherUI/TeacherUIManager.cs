using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;


public class TeacherUIManager : MonoBehaviour
{
    //Screen object variables
    public GameObject masteryReportTable;
    public GameObject questionBreakdown;
    public GameObject menu_UI;
    public GameObject studentStats;
    public GameObject registerScreen;


    //Functions to change the login screen UI
    public void Start()
    {
        ClearScreen();
        menu_UI.SetActive(true);
    }

    public void ClearScreen() //Turn off all screens
    {
        masteryReportTable.SetActive(false);
        questionBreakdown.SetActive(false);
        menu_UI.SetActive(false);
        studentStats.SetActive(false);
        registerScreen.SetActive(false);
    }

    public void MasteryScreen()
    {
        ClearScreen();
        masteryReportTable.SetActive(true);
    }
    public void questionBreakdownScreen()
    {
        ClearScreen();
        questionBreakdown.SetActive(true);
    }
    public void MenuScreen()
    {
        ClearScreen();
        menu_UI.SetActive(true);
    }
    public void StudentStatsScreen()
    {
        ClearScreen();
        studentStats.SetActive(true);
    }
    public void RegisterScreen()
    {
        ClearScreen();
        registerScreen.SetActive(true);

    }
    public void ToCustomStage()
    {
        SceneManager.LoadScene("TeacherCustomStage");
    }
    public void SignOutButton()
    {
        GameObject firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        FirebaseAuth auth = firebasesManager.GetComponent<FirebaseManager>().auth;
        auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }



    public void testButton()
    {
        Debug.Log("button clicked");
    }
}
