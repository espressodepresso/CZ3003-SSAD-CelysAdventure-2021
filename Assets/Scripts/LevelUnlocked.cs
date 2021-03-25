using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;

public class LevelUnlocked : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firebasesManager;
    public DatabaseReference DBreference;
    private string level;
    public Sprite Image1;
    public Sprite GreyImage1;
    public Button[] buttons;


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
            Debug.Log("level unlocked:");
            Debug.Log(levelUnlocked);
            for (int i = 0; i < levelUnlocked; i++)
            {
                buttons[i].interactable = true;
                buttons[i].GetComponent<Image>().sprite = Image1;
            }


        }
    }
    public void PlayButton()
    {

        //GameObject.Find("Main Camera").transform.position = new Vector3(0, 20, 0);
        SceneManager.LoadScene("test");
        //Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //camera.transform.position = new Vector3(0, 20, 0);
    }


        // Update is called once per frame
        void Update()
        {

        }

    }
