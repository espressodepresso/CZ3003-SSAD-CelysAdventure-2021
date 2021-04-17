using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;
/**
 *  This class is responsible for stage selection in the game. 
 */
public class LevelUnlocked : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firebasesManager; /** A unity gameObject that contains the firebase init data */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */
    private string level; /** current level of user */
    public Sprite Image1; /** Sprite for a valid level */
    public Sprite GreyImage1; /** Sprite for an invalid level */
    public Button[] buttons; /** Array of buttons in the UI */


    void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        //Get the currently logged in user data
        StartCoroutine(LoadUserLevel());
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            buttons[i].GetComponent<Image>().sprite = GreyImage1;
        }
    }
    /**
     * this method handles loading the user's current level from the database 
     */
    private IEnumerator LoadUserLevel()
    {
        var DBTask = DBreference.Child("users").Child(firebasesManager.GetComponent<FirebaseManager>().userID).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            //Debug.Log("Level:");
            level = snapshot.Child("curStage").Value.ToString();
            //Debug.Log(snapshot.Child("level").Value.ToString());
            int levelUnlocked = System.Convert.ToInt32(level.Substring(0, 1));
            //Debug.Log("level unlocked:");
            //Debug.Log(levelUnlocked);
            for (int i = 0; i < levelUnlocked; i++)
            {
                buttons[i].interactable = true;
                buttons[i].GetComponent<Image>().sprite = Image1;
            }


        }
    }
    /**
     * this method handles OnClick for play button
     */
    public void PlayButton()
    {
        SceneManager.LoadScene("test");
    }

    }
