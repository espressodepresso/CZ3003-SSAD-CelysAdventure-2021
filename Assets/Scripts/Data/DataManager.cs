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

    public CatList catList;
    private PlayerInfo playerData;
    private string quizCat;
    private MapCoordsList coordsList;

    private GameObject firebasesManager;
    public DatabaseReference DBreference;
    public String[] stageList = { "1_easy", "1_medium", "1_hard", "2_easy", "2_medium", "2_hard", "3_easy", "3_medium", "3_hard", "4_easy", "4_medium", "4_hard", "5_easy", "5_medium", "5_hard" };

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
    
    /**
     * Loads user data from firebase
     */
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
    /**
     * Saves user data to Firebase.
     */
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
    /**
     * Fetches current map player is in.
     * 
     * @return name of map
     */
    public string getcurStage()
    {
        return this.playerData.curStage;
    }
    /**
     * Sets map player is in.
     * 
     * @param stageName name of new map
     */
    public void SetCurStage(string stageName)
    {
        this.playerData.curStage = stageName;
    }
    /**
     * Sets difficulty of questions
     */
    public void LoadNewStage()
    {
        if (playerData.curMap == "Background_Gym")
        {
            playerData.curStage = playerData.nextMap[playerData.nextMap.Length - 1].ToString() + "_easy";
        }
    }
    /**
     * Fetches pokemon to be caught from random encounter quiz.
     * 
     * @return quiz pokemon
     */
    public string getQuizCat()
    {
        return playerData.quizCat;
    }
    /**
     * Fetches pokemon in player's inventory
     * 
     * @return List<string> of pokemon currently in inventory.
     */
    public List<string> getCurCats()
    {
        return this.playerData.pokemonHeld;
    }
    /**
     * Updates questions fetched for quiz.
     */
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

    
    /**
     * Fetches map player currently is in.
     * 
     * @return map player currently is in
     */
    public string getCurMap()
    {
        return this.playerData.curMap;
    }
    /**
     * Fetches player's next map.
     * 
     * @return player's next map
     */
    public string getNextMap()
    {
        return this.playerData.nextMap;
    }
    /**
     * Sets current map to new map.
     * 
     * @param curMap name of new map
     */
    public void SetCurMap(string curMap)
    {
        this.playerData.curMap = curMap;
    }
    /**
     * Sets next map to new map.
     * 
     * @param nextMap name of new map
     */
    public void SetNextMap(string nextMap)
    {
        this.playerData.nextMap = nextMap;
    }
    /**
     * Resets pokemons to only contain Cely.
     */
    public void ResetCats()
    {
        this.playerData.resetCats();
    }
    /**
     * Adds a new pokemon to inventory.
     * 
     * @param cat name of new cat
     */
    public void addCat(string cat)
    {
        playerData.addCat(cat);
    }
    /**
     * Updates current pokemon to new pokemon.
     * 
     * @param name of new pokemon
     */
    public void updateQuizCat(string newCat)
    {
        playerData.updateQuizCat(newCat);
    }
    /**
     * Replace pokemon in inventory with another pokemon.
     * 
     * @param oldCat name of pokemon to be replaced
     * @param newCat name of new pokemon
     */
    public void replaceCat(string oldCat, string newCat)
    {
        playerData.pokemonHeld.Remove(oldCat);
        playerData.pokemonHeld.Add(newCat);
    }
    /**
     * Updates curMap and nextMap based on where player is after teleporting.
     */
    public void UpdateMapStatus()
    {
        if (this.playerData.curMap == "Background_1")
        {
            this.SetCurMap("Background_Gym");
            this.SetNextMap("Background_2");
        }
        else if (this.playerData.curMap == "Background_2")
        {
            this.SetCurMap("Background_Gym");
            this.SetNextMap("Background_3");
        }
        else if (this.playerData.curMap == "Background_3")
        {
            this.SetCurMap("Background_Gym");
            this.SetNextMap("Background_4");
        }
        else if (this.playerData.curMap == "Background_Gym")
        {
            if (this.playerData.nextMap == "Background_2")
            {
                this.SetCurMap("Background_2");
                this.SetNextMap("Background_Gym");
            }
            else if (this.playerData.nextMap == "Background_3")
            {
                this.SetCurMap("Background_3");
                this.SetNextMap("Background_Gym");
            }
            else if (this.playerData.nextMap == "Background_4")
            {
                this.SetCurMap("Background_4");
                this.SetNextMap("Background_Gym");
            }
        }
    }

    //Map data
    /**
     * Returns exit coords of a map.
     * 
     * @param stageName name of map
     * @return float[] containing 2D coordinates of exit location.
     */
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
    /**
     * Returns entry coords of a map.
     * 
     * @param stageName name of map
     * @return float[] containing 2D coordinates of entry location
     */
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
    /**
     * Fetches a random pokemon.
     * 
     * @return random Cat
     */
    public Cat getRandomCat()
    {
        return catList.randomCat();
    }

    /**
     * Returns how many pokemons there are.
     * 
     * @return number of pokemons
     */
    public int CatListLength()
    {
        return catList.GetListLength();
    }
    /**
     * Fetches a pokemon given its name.
     * 
     * @param catName name of pokemon
     * @return desired Cat
     */
    public Cat GetCatFromName(string catName)
    {
        return catList.GetCatFromName(catName);
    }


}
