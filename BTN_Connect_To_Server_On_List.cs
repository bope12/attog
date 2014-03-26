using System;
using UnityEngine;

public class BTN_Connect_To_Server_On_List : MonoBehaviour
{
    public int index;

    private void OnClick()
    {
        base.transform.parent.parent.GetComponent<PanelMultiJoin>().connectToIndex(this.index);
    }
}

