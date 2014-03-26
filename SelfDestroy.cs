using System;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float CountDown = 5f;

    [RPC]
    private void removeMe()
    {
        if (base.networkView.isMine)
        {
            Network.RemoveRPCs(base.networkView.viewID);
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    private void Start()
    {
    }

    private void Update()
    {
        this.CountDown -= Time.deltaTime;
        if (this.CountDown <= 0f)
        {
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
            else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) || (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT))
            {
                if (base.networkView != null)
                {
                    if (base.networkView.isMine)
                    {
                        base.networkView.RPC("removeMe", RPCMode.AllBuffered, new object[0]);
                    }
                }
                else
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
            }
        }
    }
}

