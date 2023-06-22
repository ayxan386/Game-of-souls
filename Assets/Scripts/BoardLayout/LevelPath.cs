using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 1, menuName = "Data/Level Path", fileName = "Level Path")]
public class LevelPath : ScriptableObject
{
    public List<PathData> Path;
}

[Serializable]
public class PathData
{
    public string key;
    public List<string> connectedTileKeys;
}