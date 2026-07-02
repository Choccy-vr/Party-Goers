using TMPro;
using UnityEngine;

public class PlayerGameStatsUI : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI roundText;

    private VRPartyPlayer player;

    void Start()
    {
        TryFindLocalPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            TryFindLocalPlayer();
        }

        if (TurnManager.Instance != null)
        {
            if (roundText != null)
            {
                roundText.text = TurnManager.Instance.amountRounds.Value + " Turns left";
            }

            if (TurnManager.Instance.currentTurnPlayerObj != null && turnText != null)
            {
                string username = TurnManager.Instance.currentTurnPlayerObj.playerData.username.ToString();
                turnText.text = (string.IsNullOrEmpty(username) ? "Unknown Player" : username) + "'s turn";
            }
            else if (turnText != null)
            {
                turnText.text = "Waiting for turn...";
            }
        }

        if (player != null && coinsText != null)
        {
            coinsText.text = "Coins: " + player.playerData.coins.ToString();
        }
    }

    private void TryFindLocalPlayer()
    {
        if (NetworkIdenity.Instance != null && NetworkIdenity.Instance.networkPlayerIdenity != null)
        {
            player = NetworkIdenity.Instance.networkPlayerIdenity.GetComponent<VRPartyPlayer>();
        }
    }
}