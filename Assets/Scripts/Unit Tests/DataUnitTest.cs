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
        testResetCats();
        testupdateQuizCat();
        testAddCat();
        testUpdateQuiz();
        testUpdateExplore();
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
    
    public void testResetCats()
    {
        this.playerDataTest.resetCats();
        if (playerDataTest.noPokemon==1 && playerDataTest.pokemonHeld[0]=="Cely")
        {
            print("ResetCat works");
        }else {print("ResetCat Failed");}
    }
    public void testupdateQuizCat()
    {
        this.playerDataTest.updateQuizCat("Cely");
        if (playerDataTest.quizCat == "Cely"){
            print("UpdateQuizCat works");
        }else {print("UpdateQuizCat Failed");}
    }

    public void testAddCat()
    {
        playerDataTest.addCat("Cely");
        if (playerDataTest.noPokemon == 2 && playerDataTest.pokemonHeld[1]=="Cely")
        {
            print("Add cat works");
        }
        else {print("AddCat Failed");}
    }

    public void testUpdateQuiz()
    {
        bool flag = true;
        playerDataTest.updateQuiz("1_easy","2_question",true);
        if (playerDataTest.quizAnsweredQns[0].qnsCorrect.ContainsKey("2_question")){
            if (playerDataTest.quizAnsweredQns[0].qnsCorrect["2_question"] != 1)
            {
                flag=false;
            }
        }
        else {flag=false;}
        playerDataTest.updateQuiz("1_easy","2_question",false);
        if (playerDataTest.quizAnsweredQns[0].qnsWrong.ContainsKey("2_question")){
            if (playerDataTest.quizAnsweredQns[0].qnsWrong["2_question"] != 1)
            {
                flag=false;
            }
        }
        else {flag=false;}

        if (flag==true){
            print("UpdateQuiz works");
        } else {print("UpdateQuiz Failed");}
    }

    public void testUpdateExplore()
    {
        bool flag = true;
        playerDataTest.updateExplore("1_easy","2_question",true);
        if (playerDataTest.exploreAnsweredQns[0].qnsCorrect.ContainsKey("2_question")){
            if (playerDataTest.exploreAnsweredQns[0].qnsCorrect["2_question"] != 1)
            {
                flag=false;
            }
        }
        else {flag=false;}
        playerDataTest.updateExplore("1_easy","2_question",false);
        if (playerDataTest.exploreAnsweredQns[0].qnsWrong.ContainsKey("2_question")){
            if (playerDataTest.exploreAnsweredQns[0].qnsWrong["2_question"] != 1)
            {
                flag=false;
            }
        }
        else {flag=false;}

        if (flag==true){
            print("UpdateExplore works");
        } else {print("UpdateExplore Failed");}
    }


}
