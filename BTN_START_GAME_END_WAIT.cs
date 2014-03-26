using System;
using UnityEngine;

public class BTN_START_GAME_END_WAIT : MonoBehaviour
{
    private void OnClick()
    {
        if (Network.isServer)
        {
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().startGameEndWait();
        }
    }
}

