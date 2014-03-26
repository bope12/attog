using System;
using UnityEngine;

public class BTN_START_MULTI_SERVER : MonoBehaviour
{
    private void OnClick()
    {
        string text = GameObject.Find("InputServerName").GetComponent<UIInput>().label.text;
        int num = int.Parse(GameObject.Find("InputMaxPlayer").GetComponent<UIInput>().label.text);
        int num2 = int.Parse(GameObject.Find("InputMaxTime").GetComponent<UIInput>().label.text);
        int port = int.Parse(GameObject.Find("InputPort").GetComponent<UIInput>().label.text);
        string selection = GameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().StartAsServer(text, num, port, selection, !GameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked ? (!GameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked ? 0 : 2) : 1, num2 * 60);
        NGUITools.SetActive(base.transform.parent.gameObject, false);
        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiWait, true);
    }
}

