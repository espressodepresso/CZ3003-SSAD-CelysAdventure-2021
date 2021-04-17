using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Database;
using UnityEngine.SceneManagement;
/** 
 * A class for adding Custom Stage data to gameplay so that it can be played
 */
public class PlayCustomStage : MonoBehaviour
{
    public TMP_InputField customStageInput; /** A TMPro class containg customStage input - the id */

    public GameObject errorMessage; /** A unity gameobject text displayed when an invalid customStage id is inputed */

    private GameObject firebasesManager; /** A firebase class that contains the script to FirebaseManager */
    public DatabaseReference DBreference; /** A firebase class that contains the reference to the database */

    public CustomStage customStage; /** A Custom Stage object */
    public bool customStageCheck = false; /** A check for the game logic to determine if a custom stage is being played */


    void Awake()
    {
        firebasesManager = GameObject.FindGameObjectWithTag("firebase");
        DBreference = firebasesManager.GetComponent<FirebaseManager>().DBreference;
        DontDestroyOnLoad(this.gameObject);
        
        
    }
    /**
    * this method gets the custom stage data from database
    * @param id - id of the custom stage requested
    */
    private IEnumerator getCustomStageFromFirebase(string id)
    {
        var DBTask = DBreference.Child("customStage").Child(id).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if(snapshot.Value == null){
                if(errorMessage != null)
                    errorMessage.SetActive(true);
            } else{
                if (errorMessage != null)
                    errorMessage.SetActive(false);
                string json = snapshot.GetRawJsonValue();
                GameObject dataManager = GameObject.FindGameObjectWithTag("DataManager");
                customStage = CustomStage.CreateFromJSON(json);
                customStageCheck = true;
                Debug.Log(customStage.questions[0]);
                SceneManager.LoadScene("test");
            }
            
        }

    }
    /**
     * this method handles OnClick for play custom stage on stage select page 
     */
    public void onClick()
    {
        StartCoroutine(getCustomStageFromFirebase(customStageInput.text));
        
    }
    /**
     * this method returns the custom stage's background
     * @return string of background 
     */
    public string getcustomBackground()
    {
        return customStage.background;
    }
    /**
     * this method returns the custom stage's pokemons
     * @return list of pokemon names
     */
    public List<string> getCustomPokemons()
    {
        return customStage.pokemons;
    }
    /**
     * this method returns the custom stage's questions
     * @return list of question ids
     */
    public List<string> getCustomQuestions()
    {
        return customStage.questions;
    }
    /**
     * this method returns a random cat pokemon from the custom stage's pokemons
     * @return cat pokemon name
     */
    public string getRandomCat()
    {
        return customStage.pokemons[(UnityEngine.Random.Range(0,customStage.pokemons.Count))];
    }
    
}
