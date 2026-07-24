using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Minigame Config", menuName = "Scriptable Objects/Create New Minigame Config")]
public class MinigameConfig : ScriptableObject
{
    public string minigameID;
    public string sceneName;
    public PlayerConfig playerConfig;
    public MinigameType minigameType;
    public GameObject managerPrefab;
}
public enum MinigameType
{
    fourPlayer,
    oneVSthree,
    oneVSone,
    twoVStwo

}
