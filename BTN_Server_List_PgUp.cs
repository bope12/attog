using System;
using UnityEngine;

public class BTN_Server_List_PgUp : MonoBehaviour
{
    private void OnClick()
    {
        GameObject.Find("PanelMultiJoin").GetComponent<PanelMultiJoin>().pageUp();
    }
}

