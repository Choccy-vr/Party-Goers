using UnityEngine;
[CreateAssetMenu(fileName = "Map Config", menuName = "Scriptable Objects/Map Config")]
public class MapConfig : ScriptableObject
{
    public string mapName;
    public string sceneName;
    public PlayerConfig playerConfig;
}
