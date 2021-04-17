using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class PlayerInfo 
{

    public string playerName;
    public string curMap;
    public string nextMap;
    public string curStage;
    public string quizCat;
    public int noPokemon;
    public List<string> pokemonHeld;
    public AnsweredQns[] exploreAnsweredQns;
    public AnsweredQns[] quizAnsweredQns;
    public int totalNoCorrect;
    public int totalNoAttempted;

    public PlayerInfo()
    {

    }
    public PlayerInfo(string name, string curmap, string nextmap, string curstage, string quizcat, int nopokemon, int totalcorrect, int totalattempt)
    {
        playerName = name;
        curMap = curmap;
        nextMap = nextmap;
        curStage = curstage;
        quizCat = quizcat;
        noPokemon = nopokemon;
        pokemonHeld = new List<string>(new string[] { "Cely" });
        AnsweredQns temp = new AnsweredQns("1_easy");
        exploreAnsweredQns = new AnsweredQns[] {temp};
        quizAnsweredQns = new AnsweredQns[] { temp };
        totalNoCorrect = totalcorrect;
        totalNoAttempted = totalattempt;


    }

    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonConvert.DeserializeObject<PlayerInfo>(jsonString);
    }
    public void SaveToJSON()
    {
        string jsonData = JsonConvert.SerializeObject(this);
        //Debug.Log(Application.persistentDataPath);
        //string jsonSavePath = @"C:\Users\myath\ssad\Assets\Scripts\Data\PlayerData.json";
        //string jsonSavePath = @"D:\Unity Projects\SSAD_Project_Xcellent\Assets\Scripts\Data\PlayerData.json";
        string jsonSavePath = @"C:\Users\Wai Leong\Desktop\Y3S2\CZ3003 - SSAD\asdasdasdasdasdas\SSAD_Project_Xcellent\SSAD-Project-Xcellent\SSAD_Project_Xcellent\Assets\Scripts\Unit Tests\Player_Test_Data.json";
        File.WriteAllText( jsonSavePath, jsonData );
    }

    public void resetCats()
    {
        this.pokemonHeld.Clear();
        this.pokemonHeld.Add("Cely");
        this.noPokemon=1;
    }

    public void addCat(string cat)
    {
        this.pokemonHeld.Add(cat);
        this.noPokemon += 1;
    }
    public void updateQuizCat(string newCat)
    {
        this.quizCat = newCat;
    }

    public void updateQuiz(string curStage, string qnID, bool correct){
        updateQns(quizAnsweredQns,curStage,qnID,correct);
    }
    public void updateExplore(string curStage, string qnID, bool correct){
        updateQns(exploreAnsweredQns,curStage,qnID,correct);
    }


    public void updateQns(AnsweredQns[] Qns,string curStage, string qnID, bool correct)
    {
        foreach (AnsweredQns qndata in Qns)
        {
            if (qndata.stageName == curStage)
            {
                totalNoAttempted+=1;

                if (correct){
                    totalNoCorrect+=1;

                    if (qndata.qnsCorrect.ContainsKey(qnID)){
                        var curScore = qndata.qnsCorrect[qnID];
                        qndata.qnsCorrect[qnID] = curScore+1;
                    }
                    else
                    {                        
                        qndata.qnsCorrect.Add(qnID,1);
                    }
                }
                else{
                    if (qndata.qnsWrong.ContainsKey(qnID)){
                        var curScore = qndata.qnsWrong[qnID];
                        qndata.qnsWrong[qnID] = curScore+1;
                    }
                    else
                    {
                        qndata.qnsWrong.Add(qnID,1);
                    }
                }
            }
        }
    }
    
}



