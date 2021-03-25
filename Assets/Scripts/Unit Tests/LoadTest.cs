using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class LoadTest : MonoBehaviour
{

    private PlayerInfo playerData;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;

    public void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");

        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;

        StartCoroutine(LoadUserDataTesting(returnValue =>
        {

            Debug.Log("Testingggggggg");
            playerData = PlayerInfo.CreateFromJSON(returnValue);
            DontDestroyOnLoad(this.gameObject);
        }));



    }

    private IEnumerator LoadUserDataTesting(System.Action<string> callback)
    {
        Debug.Log("Calculation for 1000 iteration is starting");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i=0; i<1000;i++)
        {
            // Debug.Log(i);
            // DBTask = DBreference.Child("users").Child(firebasesManager.GetComponent<FirebaseManager>().userID).GetValueAsync();
            DBreference.Child("users").Child(firebasesManager.GetComponent<FirebaseManager>().userID).GetValueAsync();
        }
        watch.Stop();
        Debug.Log("Time taken for 1000 iteration: " +watch.ElapsedMilliseconds +  "ms");

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
            callback(snapshot.GetRawJsonValue());
        }
    }

    public IEnumerator SaveUserDataToFirebase()
    {


        string player_json = JsonConvert.SerializeObject(this.playerData);
        var DBTask = DBreference.Child("users").Child(firebasesManager.GetComponent<FirebaseManager>().userID).SetRawJsonValueAsync(player_json);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Debug.Log("its updated");
        }
    }
}
