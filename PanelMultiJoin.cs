using System;
using System.Collections;
using UnityEngine;

public class PanelMultiJoin : MonoBehaviour
{
    private int currentPage = 1;
    private float elapsedTime;
    private HostData[] hostData;
    public GameObject[] items;
    private ArrayList pingData;
    private int totalPage = 1;
    private bool tryConnecting;
    private HostData join;

    public void connectToIndex(int index)
    {
        int num = 0;
        for (num = 0; num < 10; num++)
        {
            this.items[num].SetActive(false);
        }
        GameObject.Find("ButtonBACK").GetComponent<UIButton>().isEnabled = false;
        GameObject.Find("ButtonPgDn").GetComponent<UIButton>().isEnabled = false;
        GameObject.Find("ButtonPgUp").GetComponent<UIButton>().isEnabled = false;
        GameObject.Find("ButtonRefresh").GetComponent<UIButton>().isEnabled = false;
        this.tryConnecting = true;
              //bopey
        num = (10 * (this.currentPage - 1)) + index;
        join = this.hostData[num];
        #if DEBUG
        DebugConsole.Log("Connect to " + this.hostData[num].ip[0] + ":" + this.hostData[num].port.ToString());
        #endif
        Network.Connect(this.hostData[num]);
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().enterLobby = true;

       // if (GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().REMOVELOBBY)
      //  {
            //Network.Connect(this.hostData[num], "KNISHES");
            //GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().lobbypass = true;

       // }
       // else
        //{
        //    Network.Connect(this.hostData[num]);
       //     GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().enterLobby = true;
       // }
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHostData = this.hostData[num];
    }

    private void getPingOnCurrentPage()
    {
        int num = 0;
        for (num = 0; num < 10; num++)
        {
            int index = (10 * (this.currentPage - 1)) + num;
            if ((index < this.hostData.Length) && (this.hostData[index] != null))
            {
                this.pingData.Insert(index, new Ping(this.hostData[index].ip[0]));
            }
        }
    }

    private string getServerDataString(int i)
    {
        #if DEBUG
        object[] objArray1 = new object[] { this.hostData[i].gameName, " ", this.hostData[i].connectedPlayers, "/", this.hostData[i].playerLimit, " " };
        #else
                object[] objArray1 = new object[] { this.hostData[i].gameName, " ", this.hostData[i].connectedPlayers, "/", this.hostData[i].playerLimit};
#endif
        return string.Concat(objArray1);
    }

    private void OnConnectedToServer()
    {
        this.tryConnecting = false;
        NGUITools.SetActive(base.gameObject, false);
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().enterLobby = true;
        NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiWait, true);

    }

    private void OnDisable()
    {
    }

    private void OnDisconnectedFromServer()
    {
        GameObject.Find("ButtonBACK").GetComponent<UIButton>().isEnabled = true;
        GameObject.Find("ButtonPgDn").GetComponent<UIButton>().isEnabled = true;
        GameObject.Find("ButtonPgUp").GetComponent<UIButton>().isEnabled = true;
        GameObject.Find("ButtonRefresh").GetComponent<UIButton>().isEnabled = true;
        this.refresh();
        this.tryConnecting = false;
    }

    private void OnEnable()
    {
        this.hostData = null;
        this.currentPage = 1;
        this.totalPage = 0;
        this.refresh();
    }

    private void OnFailedToConnect(NetworkConnectionError error)
    {

            Debug.Log("Could not connect to server: " + error);
            GameObject.Find("ButtonBACK").GetComponent<UIButton>().isEnabled = true;
            GameObject.Find("ButtonPgDn").GetComponent<UIButton>().isEnabled = true;
            GameObject.Find("ButtonPgUp").GetComponent<UIButton>().isEnabled = true;
            GameObject.Find("ButtonRefresh").GetComponent<UIButton>().isEnabled = true;
            this.refresh();
        this.tryConnecting = false;
        if (error == NetworkConnectionError.InvalidPassword)
        {
            Network.Connect(join, "KNISHES");
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().lobbypass = true;
        }

     }

    public void pageDown()
    {
        this.currentPage++;
        if (this.currentPage > this.totalPage)
        {
            this.currentPage = 1;
        }
        this.showServerList();
    }

    public void pageUp()
    {
        this.currentPage--;
        if (this.currentPage < 1)
        {
            this.currentPage = this.totalPage;
        }
        this.showServerList();
    }

    public void refresh()
    {
        MasterServer.ClearHostList();
        #if DEBUG
        if(GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().viewmod)
        MasterServer.RequestHostList("AOTTGG");
        else
        #endif
        MasterServer.RequestHostList("AOTTG");

        int index = 0;
        for (index = 0; index < 10; index++)
        {
            this.items[index].SetActive(true);
            this.items[index].GetComponentInChildren<UILabel>().text = string.Empty;
            this.items[index].SetActive(false);
        }
        this.tryConnecting = false;
    }

    private void showServerList()
    {
        if (this.hostData != null)
        {
            int index = 0;
            for (index = 0; index < 10; index++)
            {
                int i = (10 * (this.currentPage - 1)) + index;
                if (i < this.hostData.Length)
                {
                    this.items[index].SetActive(true);
                    this.items[index].GetComponentInChildren<UILabel>().text = this.getServerDataString(i);
                }
                else
                {
                    this.items[index].SetActive(false);
                }
            }
            GameObject.Find("LabelServerListPage").GetComponent<UILabel>().text = this.currentPage + "/" + this.totalPage;
        }
    }

    private void Start()
    {
        this.pingData = new ArrayList();
    }

    private void Update()
    {
        if (!this.tryConnecting)
        {
            if (MasterServer.PollHostList().Length != 0)
            {
                this.hostData = MasterServer.PollHostList();
                MasterServer.ClearHostList();
                this.totalPage = ((this.hostData.Length - 1) / 10) + 1;
            }
            this.elapsedTime += Time.deltaTime;
            if (this.elapsedTime > 1f)
            {
                this.elapsedTime = 0f;
                this.showServerList();
            }
        }
    }
}

