using System;
using UnityEngine;

public class BTN_END_WAIT_BACK_TO_MAIN : MonoBehaviour
{
    private void OnClick()
    {
        if (Network.isClient)
        {
            Network.Disconnect();
        }
        else if (Network.isServer)
        {
            Network.Disconnect();
            if (GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().usingMasterServer)
            {
                MasterServer.UnregisterHost();
            }
        }
        IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
        NGUITools.SetActive(base.transform.parent.gameObject, false);
        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMain, true);
        GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
    }
}

