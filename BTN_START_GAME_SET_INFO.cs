using System;
using UnityEngine;

public class BTN_START_GAME_SET_INFO : MonoBehaviour
{
    private void OnClick()
    {
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHeroName = GameObject.Find("InputPlayerName").GetComponent<UIInput>().text;
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHero = GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection;
        if (GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection == "EREN")
        {
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHero = "MIKASA";
        }
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playersRegistered[0].name = GameObject.Find("InputPlayerName").GetComponent<UIInput>().text;
            if (GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection == "EREN")
            {
                if (GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().checkIfHasEren())
                {
                    if (GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playersRegistered[0].resourceId == "EREN")
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHero = GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playersRegistered[0].resourceId = "EREN";
                    }
                    else
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHero = GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playersRegistered[0].resourceId = "MIKASA";
                    }
                }
                else
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHero = GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playersRegistered[0].resourceId = "EREN";
                }
            }
            else
            {
                GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playersRegistered[0].resourceId = GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection;
            }
        }
        else
        {
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().setMyInfo(GameObject.Find("InputPlayerName").GetComponent<UIInput>().text, GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection);
        }
    }
}

