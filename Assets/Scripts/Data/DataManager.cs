using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;



public class DataManager : MonoBehaviour
{
    public TextAsset mapCoordsJson;
    public TextAsset catDataJson;

    private CatList catList;
    private PlayerInfo playerData;
    private string quizCat;
    private MapCoordsList coordsList;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;

    public void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");

        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        catList = CatList.CreateFromJSON(catDataJson.text);
        coordsList = MapCoordsList.CreateFromJSON(mapCoordsJson.text);

        StartCoroutine(LoadUserData(returnValue =>
        {
            playerData = PlayerInfo.CreateFromJSON(returnValue);
            DontDestroyOnLoad(this.gameObject);
        }));



    }
    

    private IEnumerator LoadUserData(System.Action<string> callback)
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


    //Player Data
    public string getcurStage()
    {
        return this.playerData.curStage;
    }

    public string getQuizCat()
    {
        return playerData.quizCat;
    }

    public List<string> getCurCats()
    {
        return this.playerData.pokemonHeld;
    }

    public void updateQns(string curStage, string qnID, bool correct, bool isQuiz)
    {
        if (isQuiz)
        {
            playerData.updateQuiz(curStage, qnID, correct);
        }
        else
        {
            playerData.updateExplore(curStage, qnID, correct);
        }
    }

    public string getCurMap()
    {
        return playerData.curMap;
    }
    public string getNextMap()
    {
        return playerData.nextMap;
    }

    public void addCat(string cat)
    {
        var playerCat = playerData.pokemonHeld;
        playerData.pokemonHeld.Add(cat);
        playerData.noPokemon += 1;
    }

    public void updateQuizCat(string newCat)
    {
        playerData.updateQuizCat(newCat);
    }

    public void replaceCat(string oldCat, string newCat)
    {
        playerData.pokemonHeld.Remove(oldCat);
        playerData.pokemonHeld.Add(newCat);
    }

    //Map data
    public float[] getExitCoords(string stageName)
    {
        foreach (MapCoords mapcoords in coordsList.mapscoordslist)
        {
            if (stageName == mapcoords.mapName)
            {
                return (mapcoords.exit);
            }
        }
        float[] extra = { 0.5f, 0.4f };
        return extra;
    }

    public float[] getEntryCoords(string stageName)
    {
        foreach (MapCoords mapcoords in coordsList.mapscoordslist)
        {
            if (stageName == mapcoords.mapName)
            {
                return (mapcoords.entry);
            }
        }
        float[] extra = { 0.5f, 0.4f };
        return extra;
    }

    //Pokemon Data
    public Cat getRandomCat()
    {
        return catList.randomCat();
    }
}
