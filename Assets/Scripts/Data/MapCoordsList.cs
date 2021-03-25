using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapCoordsList
{
    public MapCoords[] mapscoordslist;
    

    public static MapCoordsList CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MapCoordsList>(jsonString);
    }

}
