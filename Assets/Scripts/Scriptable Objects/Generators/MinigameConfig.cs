using UnityEngine;
[CreateAssetMenu(fileName = "Minigame Config", menuName = "Scriptable Objects/Minigame Config")]
public class MinigameConfig : ScriptableObject
{
    [Header("Scene")]
    public string sceneName;
    [Header("Player")]
    public PlayerConfig playerConfig;
    [Header("Minigame")]
    public MinigameType minigameType;
}
public enum MinigameType
{
    fourPlayer,
    oneVSthree,
    oneVSone,
    twoVStwo

}
