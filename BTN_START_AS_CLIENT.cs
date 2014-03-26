using System;
using UnityEngine;

public class BTN_START_AS_CLIENT : MonoBehaviour
{
    private void OnClick()
    {
        int result = 0;
        if (!int.TryParse(GameObject.Find("InputPort").GetComponent<UIInput>().label.text, out result))
        {
            result = 0x40b28;
            GameObject.Find("InputPort").GetComponent<UIInput>().label.text = "265000";
        }
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().StartAsClient(GameObject.Find("InputIP").GetComponent<UIInput>().label.text, result);
        GameObject.Find("LabelJoinInfo").GetComponent<UILabel>().text = "Connecting...";
        base.GetComponent<UIButton>().isEnabled = false;
        base.transform.parent.Find("ButtonBACK").GetComponent<UIButton>().isEnabled = false;
        PlayerPrefs.SetString("lastIP", GameObject.Find("InputIP").GetComponent<UIInput>().label.text);
        PlayerPrefs.SetString("lastPort", GameObject.Find("InputPort").GetComponent<UIInput>().label.text);
    }
}

