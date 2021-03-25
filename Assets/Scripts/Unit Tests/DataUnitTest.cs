using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUnitTest : MonoBehaviour
{
    private PlayerInfo playerDataTest;
    
    public TextAsset playerTestJson;

    public void Start(){
        playerDataTest = PlayerInfo.CreateFromJSON(playerTestJson.text);
        testLoading();
    }

    public void testLoading(){
        if (playerDataTest.curMap == "Background_1"){
            print("curMap loaded correctly");
        } else{ print("curMap not loaded"); }
        
        if (playerDataTest.nextMap == "Background_Gym"){
            print("nextMap loaded correctly");
        } else{ print("nextMap not loaded"); }
        
        if (playerDataTest.curStage == "1_easy"){
            print("curStage loaded correctly");
        } else{ print("curStage not loaded"); }

        if (playerDataTest.playerName == "TestPlayer"){
            print("playerName loaded correctly");
        } else{ print("playerName not loaded"); }

        if (playerDataTest.quizCat == "Cat Cat"){
            print("quizCat loaded correctly");
        } else{ print("quizCat not loaded"); }

        if (playerDataTest.pokemonHeld[0] == "Catto"){
            print("pokemonHeld loaded correctly");
        } else{ print("pokemonHeld not loaded"); }

        if (playerDataTest.noPokemon == 4){
            print("noPokemon loaded correctly");
        } else{ print("noPokemon not loaded"); }

        if (playerDataTest.totalNoAttempted == 123){
            print("totalNoAttempted loaded correctly");
        } else{ print("totalNoAttempted not loaded"); }

        if (playerDataTest.totalNoCorrect == 100){
            print("totalNoCorrect loaded correctly");
        } else{ print("totalNoCorrect not loaded"); }
        
        if (playerDataTest.exploreAnsweredQns[0].qnsCorrect.ContainsKey("0_question")){
            print("exploreAnsweredQns loaded correctly");
        } else{ print("exploreAnsweredQns not loaded"); }
    }
    
    
}
