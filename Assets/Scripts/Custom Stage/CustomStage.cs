using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
 
//! A class containing Custom Stage data

public class CustomStage
{
    public List<string> pokemons; /** A list of pokemons chosen by user to be used in the custom stage */
    public string background; /** The background chosen by user to be used in the custom stage */
    public List<string> questions; /** The questions chosen by user to be used in the custom stage */
    /**
     * this method handles taking a string and forming a CustomStage object
     * @param jsonString - the json string from database
     */
    public static CustomStage CreateFromJSON(string jsonString)
    {
        return JsonConvert.DeserializeObject<CustomStage>(jsonString);
    }
    /**
     * this method handles taking a CustomStage Object and convert to json string 
     */
    public string SaveToJSON()
    {
        string jsonData = JsonConvert.SerializeObject(this);
        return jsonData;
    }
    /** 
     * A constructor for Custom Stage
     * @param pokemons - list of pokemons
     * @param questions - list of questions
     * @param background - background
     */
    public CustomStage(List<string> pokemons, List<string> questions, string background)
    {
        this.pokemons = pokemons;
        this.questions = questions;
        this.background = background;
    }
    /**
     * A constructor for Custom Stage
     */
    public CustomStage()
    {

    }
}
