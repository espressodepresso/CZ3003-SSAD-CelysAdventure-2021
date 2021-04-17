using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Database;

public class LogoutButton : MonoBehaviour
{
    private GameObject firebasesManager;
    public DatabaseReference DBreference;
    // Start is called before the first frame update
    /**
     * Logs user out from the game.
     */
    public void SignOutButton()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        FirebaseAuth auth = firebasesManager.GetComponent<FirebaseManager>().auth;
        auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }
}
