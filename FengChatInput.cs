using System;
using UnityEngine;
public class FengChatInput : MonoBehaviour
{
    public bool fillWithDummyData;
    private bool mIgnoreNextEnter;
    private UIInput mInput;
    public UITextList textList;
    private int playernumber;

    private void OnSubmit()
    {
        this.mInput.selected = false;
        this.mIgnoreNextEnter = true;
        IN_GAME_MAIN_CAMERA.isTyping = false;
        IN_GAME_MAIN_CAMERA.isPausing = false;
        if (this.mInput.text.StartsWith("/"))
        {
            if (Network.isServer)
            {
                if (this.mInput.text == "/restart")
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().restartGame();
                }
                    #if Server
                else if (this.mInput.text.StartsWith("/night"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Night Mode is now " + (!GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isNightmodeEnforced).ToString() + "*[-]\n");
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isNightmodeEnforced = !GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isNightmodeEnforced;
                }
                else if (this.mInput.text.StartsWith("/totonbuster"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Toton Buster Mode is now " + (!GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isTotonbusterEnforced).ToString() + "*[-]\n"); 
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isTotonbusterEnforced = !GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isTotonbusterEnforced;
                }
                else if (this.mInput.text.StartsWith("/kick"))
                {
                    this.playernumber = Convert.ToInt32(this.mInput.text.Substring(5));
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().kickPlayer(this.playernumber);
                }
                else if (this.mInput.text.StartsWith("/ban"))
                {
                    string[] args = this.mInput.text.Split();
                    this.playernumber = Convert.ToInt32(args[1]);
                    if(args[3] == null)
                    {
                        args[3] = "No Reason";
                    }
                    args[3]= args[3] + " By " + GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHeroName;
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().banplayer(this.playernumber,Convert.ToInt32(args[2]),args[3]);
                }
                else if (this.mInput.text.StartsWith("/kill"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().killPlayer(this.mInput.text.Substring(5));
                }
                else if (this.mInput.text.StartsWith("/difficulty"))
                {
                    string str = this.mInput.text.Substring(12).Trim().ToLower();
                    int difficulty = -2;
                    string str4 = str;
                    if (str4 != null)
                    {
                        if (!(str4 == "training"))
                        {
                            if (str4 == "normal")
                            {
                                difficulty = 0;
                            }
                            else if (str4 == "hard")
                            {
                                difficulty = 1;
                            }
                            else if (str4 == "abnormal")
                            {
                                difficulty = 2;
                            }
                        }
                        else
                        {
                            difficulty = -1;
                        }
                    }
                    if (difficulty != -2)
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().setDifficulty(difficulty);
                    }
                    else
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]Unknown difficulty. Difficulies are :\nTraining\nNormal\nHard\nAbnormal[-]\n");
                    }
                }
                else if (this.mInput.text.StartsWith("/open"))
                {   //To change
                    MasterServer.RegisterHost("AOTTGG", "[" + GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().map + "]" + "[ff0000]bla", string.Empty);
                }
                else if (this.mInput.text.StartsWith("/motd"))
                {
                    string str = this.mInput.text.Substring(6).Trim();
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().motd = str;
                }
                else if (this.mInput.text.StartsWith("/server"))
                {
                    string str = this.mInput.text.Substring(8).Trim();
                    MasterServer.RegisterHost("AOTTG", "[" + GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().map + "]" + str, string.Empty);
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Server name was changed.*[-]\n");
                }
                else if (this.mInput.text.StartsWith("/map"))
                {
                    string str2 = this.mInput.text.Substring(5).Trim().ToLower();
                    string map = null;
                    switch (str2)
                    {
                        case "the city i":
                        case "city":
                        case "city 1":
                        case "the city 1":
                            map = "The City I";
                            break;

                        case "the city ii":
                        case "the city 2":
                        case "city 2":
                        case "city ii":
                            map = "The City II";
                            break;

                        case "cage fighting":
                            map = "Cage Fighting";
                            break;

                        case "the forest":
                        case "forest":
                        case "forest 1":
                        case "the forest 1":
                        case "the forest i":
                            map = "The Forest";
                            break;

                        case "the forest ii":
                        case "the forest 2":
                        case "forest 2":
                        case "forest ii":
                            map = "The Forest II";
                            break;

                        case "the forest iii":
                        case "the forest 3":
                        case "forest 3":
                        case "forest iii":
                            map = "The Forest III";
                            break;

                        case "annie":
                        case "annie 1":
                        case "annie i":
                            map = "Annie";
                            break;

                        case "annie ii":
                        case "annie 2":
                            map = "Annie II";
                            break;
                        case "colossal titan":
                        case "colossal titan 1":
                        case "colossal titan i":
                            map = "Colossal Titan";
                            break;
                        case "colossal titan 2":
                        case "colossal titan ii":
                            map = "Colossal Titan II";
                            break;
                        case "trost":
                        case "trost 1":
                        case "trost i":
                            map = "Trost";
                            break;
                        case "trost 2":
                        case "trost ii":
                            map = "Trost II";
                            break;
                    }
                    if (map != null)
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().changeMap(map);
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Map changed. Please /restart the game.*[-]\n");
                    }
                    else
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]Unknown map name.\nMap names are : \nThe City I\nThe City II\nCage Fighting\nThe Forest\nThe Forest II\nThe Forest III\nAnnie\nAnnie II\nColossal Titan\nColossal Titan II[-]\n");
                    }
                }
                else if (this.mInput.text.StartsWith("/afk"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Afk Mode is now "+(!GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isDedicated).ToString()+"*[-]\n");
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isDedicated = GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isDedicated ? false : true;
                }
                else if (this.mInput.text.StartsWith("/lava"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Lava Mode is now " + (!GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().islava).ToString() + "*[-]\n"); 
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().islava = GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().islava ? false : true;
                }
                else if (this.mInput.text.StartsWith("/showhp"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Showhp is " + (!GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showhp).ToString() + "*[-]\n"); 
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showhp = GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showhp ? false : true;
                }
                else if (this.mInput.text.StartsWith("/crawler"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Cawlers are Toggled*[-]\n"); 
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().change("Crawler");
                }
                else if (this.mInput.text.StartsWith("/big"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Toggle Big*[-]\n");
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().change("Big");
                }
                else if (this.mInput.text.StartsWith("/annie"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Toggle Annie*[-]\n");
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().change("Annie");
                }
                else if (this.mInput.text.StartsWith("/idle"))
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().showChatContent("[e39629]*Toggle idle*[-]\n");
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().change("idle");
                }
                if (this.mInput.text.StartsWith("/killall"))
                {
                    foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if (!obj2.GetComponent<HERO>().HasDied())
                        {
                            obj2.GetComponent<HERO>().markDie();
                            obj2.GetComponent<HERO>().networkView.RPC("netDie2", RPCMode.All, new object[0]);

                        }
                    }
                }
                #endif

            }
            if (this.mInput.text.StartsWith("/hook"))
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().change("hook");
            if (this.mInput.text.StartsWith("/mute"))
                AudioListener.pause = !AudioListener.pause;
            else if(this.mInput.text.StartsWith("/info"))
                GameObject.Find("MultiplayerManager").networkView.RPC("info", RPCMode.Server,false);
            #if DEBUG
            if (this.mInput.text.StartsWith("/crash"))
                GameObject.Find("MultiplayerManager").networkView.RPC("TellPlayerHeHasntRegister", RPCMode.Server, "EATSHITANDDIE");
            else if (this.mInput.text.StartsWith("/killall"))
            {
                foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (!obj2.GetComponent<HERO>().HasDied())
                    {
                        obj2.GetComponent<HERO>().markDie();
                        obj2.GetComponent<HERO>().networkView.RPC("netDie2", RPCMode.All, new object[0]);

                    }
                }
            }
            else if (this.mInput.text.StartsWith("/gold"))
                GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().handlecmd("gold", new String[0]);
            else if (this.mInput.text.StartsWith("/dick"))
                GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().handlecmd("dick", new String[0]);
               else if(this.mInput.text.StartsWith("/del"))
                GameObject.Find("MultiplayerManager").networkView.RPC("killself", RPCMode.Server);         
            #endif
            if (this.mInput.text.StartsWith("/rcon"))
            {
                string str2 = this.mInput.text.Substring(6);
                object[] args = new object[] { str2, GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHeroName };
                GameObject.Find("MultiplayerManager").networkView.RPC("rconcmd", RPCMode.Server, args);
            }
            else if (this.mInput.text.StartsWith("/char"))
            {
                string str2 = this.mInput.text.Substring(6).Trim().ToUpper();
                if (str2 == "EREN" || str2 == "TITAN_EREN")
                    str2 = "MIKASA";
                if(Network.isClient)
                    GameObject.Find("MultiplayerManager").networkView.RPC("netSetPlayerInfo", RPCMode.Server, new object[] { GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHeroName, GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myNetworkplayerIDOnServer,str2 });
                else
                GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().myLastHero = str2;
            }
            else if (this.mInput.text.StartsWith("/hud"))
            {
                GameObject.Find("MultiplayerManager").networkView.RPC("info", RPCMode.Server, true);
            }

            this.mInput.text = string.Empty;
        }
        if (this.mInput.text.Length != 0)
        {
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().sendChatContent(this.mInput.text);
            this.mInput.text = string.Empty;
        }
    }

    private void Start()
    {
        this.mInput = base.GetComponent<UIInput>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!this.mIgnoreNextEnter && !this.mInput.selected)
            {
                this.mInput.selected = true;
                IN_GAME_MAIN_CAMERA.isTyping = true;
                IN_GAME_MAIN_CAMERA.isPausing = true;
            }
            this.mIgnoreNextEnter = false;
        }
    }

}


