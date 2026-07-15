using TMPro;
using UnityEngine;

public class PlayerStandingHelper : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerUsernameText;
    [SerializeField] TextMeshProUGUI playerCoinText;
    [SerializeField] TextMeshProUGUI playerStarText;

    public PlayerSessionData player;

    public void refreshPlayerStanding(PlayerSessionData updatedPlayer)
    {
        player = updatedPlayer;
        playerUsernameText.text = player.username.ToString();
        playerCoinText.text = "Coins: " + player.coins;
        playerStarText.text = "Stars: " + player.stars;
    }

}
