using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PartyProgressScreenHelper : MonoBehaviour
{
    [Header("Round")]
    [SerializeField] TextMeshProUGUI roundText;
    [Header("Player Standings")]
    [SerializeField] PlayerStandingHelper player1Standing;
    [SerializeField] PlayerStandingHelper player2Standing;
    [SerializeField] PlayerStandingHelper player3Standing;
    [SerializeField] PlayerStandingHelper player4Standing;

    List<PlayerSessionData> players;
    int round;
    float nextUpdateTime;

    void Update()
    {
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + 0.5f;

        if (!CompareNetworkListWithList(GameSessionManager.Instance.activePlayers, players))
        {
            players = new List<PlayerSessionData>();
            for (int i = 0; i < GameSessionManager.Instance.activePlayers.Count; i++)
            {
                players.Add(GameSessionManager.Instance.activePlayers[i]);
                if (i == 0)
                {
                    player1Standing.player = GameSessionManager.Instance.activePlayers[i];
                }
                else if (i == 1)
                {
                    player2Standing.player = GameSessionManager.Instance.activePlayers[i];
                }
                else if (i == 2)
                {
                    player3Standing.player = GameSessionManager.Instance.activePlayers[i];
                }
                else
                {
                    player4Standing.player = GameSessionManager.Instance.activePlayers[i];
                }
            }

        }
        if (round != TurnManager.Instance.currentRound)
        {
            round = TurnManager.Instance.currentRound;
            roundText.text = $"{round}/{TurnManager.Instance.amountRounds}";
        }
    }
    bool CompareNetworkListWithList<T>(NetworkList<T> networkList, List<T> standardList) where T : unmanaged, System.IEquatable<T>
    {
        if (networkList == null || standardList == null) return false;

        if (networkList.Count != standardList.Count) return false;

        for (int i = 0; i < networkList.Count; i++)
        {
            if (!networkList[i].Equals(standardList[i]))
            {
                return false;
            }
        }

        return true;
    }
}
