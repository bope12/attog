using System;
using UnityEngine;

public class BTN_RefreshList : MonoBehaviour
{
    private void OnClick()
    {
        GameObject.Find("PanelMultiJoin").GetComponent<PanelMultiJoin>().refresh();
    }
}

