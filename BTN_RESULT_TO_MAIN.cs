using System;
using UnityEngine;

public class BTN_RESULT_TO_MAIN : MonoBehaviour
{
    private void OnClick()
    {
        Time.timeScale = 1f;
        IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
        GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
        UnityEngine.Object.Destroy(GameObject.Find("MultiplayerManager"));
        Application.LoadLevel("menu");
    }
}

