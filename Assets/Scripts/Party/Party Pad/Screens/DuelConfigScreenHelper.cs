using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DuelConfigScreenHelper : MonoBehaviour
{
    [SerializeField] DuelButtonHelper player1Button;
    [SerializeField] DuelButtonHelper player2Button;
    [SerializeField] DuelButtonHelper player3Button;

    void OnEnable()
    {
        List<VRPartyPlayer> otherPlayers = PlayerManager.Instance.getAllOtherPlayers();
        for (int i = 0; i < otherPlayers.Count; i++)
        {
            if (i == 0)
            {
                player1Button.GetComponentInChildren<TextMeshProUGUI>().text = otherPlayers[i].playerData.username.ToString();
                player1Button.player = otherPlayers[i];
            }
            else if (i == 1)
            {
                player2Button.GetComponentInChildren<TextMeshProUGUI>().text = otherPlayers[i].playerData.username.ToString();
                player2Button.player = otherPlayers[i];
            }
            else
            {
                player3Button.GetComponentInChildren<TextMeshProUGUI>().text = otherPlayers[i].playerData.username.ToString();
                player3Button.player = otherPlayers[i];
            }
        }
    }
}
