using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/** 
 * A class that handles UI functions for the login and menu 
 */
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject loginUI; /** An unity gameObject containing login UI objects */
    public GameObject registerUI; /** An unity gameObject containing register UI objects */
    public GameObject userDataUI; /** An unity gameObject containing user data UI objects */
    public GameObject scoreboardUI;  /** An unity gameObject containing scoreboard UI objects */
    public GameObject menuUI; /** An unity gameObject containing menu UI objects */

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

   
    /**
     * this method sets all screens to inactive, hence making a blank screen
     */
    public void ClearScreen() //Turn off all screens
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        scoreboardUI.SetActive(false);
    }
    /**
     * this method sets login screen to active, hence displaying login screen
     */
    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
    }
    /** 
     * this method sets register screen to active, hence displaying register screen
     */
    public void RegisterScreen() // Regester button
    {
        ClearScreen();
        registerUI.SetActive(true);
    }
    /**
     * this method sets user data screen to active, hence displaying user data screen
     */
    public void UserDataScreen() //Logged in
    {
        ClearScreen();
        userDataUI.SetActive(true);
    }
    /** 
     * this method sets menu screen to active, hence displaying menu screen
     */
    public void MenuScreen() //Logged in
    {
        ClearScreen();
        menuUI.SetActive(true);
    }
    /**
     * this method sets scoreboard screen to active, hence displaying scoreboard screen
     */
    public void ScoreboardScreen() //Scoreboard button
    {
        ClearScreen();
        menuUI.SetActive(false);
        scoreboardUI.SetActive(true);
    }
}
