using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class CatList
{
    public Cat[] catList;

    public static CatList CreateFromJSON(string jsonString){
        return JsonConvert.DeserializeObject<CatList>(jsonString);
    }
    public Cat randomCat()
    {
        return catList[(UnityEngine.Random.Range(0,catList.Length))];
    }
    public Cat GetCatFromName(string catName)
    {
        foreach (var cat in this.catList){
            if (cat.catName == catName){
                return cat;
            } 
        }

        return catList[0];
    }
    public int GetListLength()
    {
        return catList.Length;
    }

}