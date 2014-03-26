using System;
using UnityEngine;

public class BTN_START_SINGLE_GAMEPLAY : MonoBehaviour
{
    private void OnClick()
    {
        string selection = GameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
        string str2 = GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection;
        int num = !GameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked ? (!GameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked ? 0 : 2) : 1;
        IN_GAME_MAIN_CAMERA.difficulty = num;
        IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.SINGLE;
        IN_GAME_MAIN_CAMERA.singleCharacter = str2.ToUpper();
        Application.LoadLevel(selection);
        CAMERA_TYPE tPS = CAMERA_TYPE.TPS;
        if (base.transform.parent.Find("GroupMode").Find("CheckboxDefault").GetComponent<UICheckbox>().isChecked)
        {
            tPS = CAMERA_TYPE.ORIGINAL;
        }
        if (base.transform.parent.Find("GroupMode").Find("CheckboxWOW").GetComponent<UICheckbox>().isChecked)
        {
            tPS = CAMERA_TYPE.WOW;
        }
        if (base.transform.parent.Find("GroupMode").Find("CheckboxTPS").GetComponent<UICheckbox>().isChecked)
        {
            tPS = CAMERA_TYPE.TPS;
            Screen.lockCursor = true;
        }
        IN_GAME_MAIN_CAMERA.cameraMode = tPS;
        Screen.showCursor = false;
        if (selection == "old_level3")
        {
            IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.COLLECT;
        }
        else
        {
            IN_GAME_MAIN_CAMERA.gamemode = GAMEMODE.KILL_TITAN;
        }
        if (selection == "trainning_0")
        {
            IN_GAME_MAIN_CAMERA.difficulty = -1;
        }
    }
}

