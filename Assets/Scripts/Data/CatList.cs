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

}