using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class COLOSSAL_TITAN : MonoBehaviour
{
    private string actionName;
    private string attackAnimation;
    private float attackCheckTime;
    private float attackCheckTimeA;
    private float attackCheckTimeB;
    private bool attackChkOnce;
    private int attackCount;
    private float attackEndWait;
    private int attackPattern = -1;
    public GameObject bottomObject;
    private Transform checkHitCapsuleEnd;
    private Vector3 checkHitCapsuleEndOld;
    private float checkHitCapsuleR;
    private Transform checkHitCapsuleStart;
    public GameObject door_broken;
    public GameObject door_closed;
    public bool hasDie;
    private bool isSteamNeed;
    public static float minusDistance = 99999f;
    public static GameObject minusDistanceEnemy;
    public float myDistance;
    public GameObject myHero;
    public int NapeArmor = 0x2710;
    public int NapeArmorTotal = 0x2710;
    public GameObject neckSteamObject;
    private string state = "idle";
    public GameObject sweepSmokeObject;
    private float tauntTime;
    private float waitTime = 2f;

    private void attack_sweep(string type = "")
    {
        this.callTitanHAHA();
        this.state = "attack_sweep";
        this.attackAnimation = "sweep" + type;
        this.attackCheckTimeA = 0.4f;
        this.attackCheckTimeB = 0.57f;
        this.checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
        this.checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        this.checkHitCapsuleR = 20f;
        this.crossFade("attack_" + this.attackAnimation, 0.1f);
        this.attackChkOnce = false;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().Play();
        if (Network.peerType == NetworkPeerType.Server)
        {
            base.networkView.RPC("startSweepSmoke", RPCMode.Others, new object[0]);
        }
    }

    private void Awake()
    {
        base.rigidbody.freezeRotation = true;
        base.rigidbody.useGravity = false;
    }

    public void beTauntedBy(GameObject target, float tauntTime)
    {
    }

    public void blowPlayer(GameObject player, Transform neck)
    {
        Vector3 vector = -(Vector3) ((neck.position + (base.transform.forward * 50f)) - player.transform.position);
        float num = 20f;
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            player.GetComponent<HERO>().blowAway((Vector3) ((vector.normalized * num) + (Vector3.up * 1f)));
        }
        else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            object[] args = new object[] { (Vector3) ((vector.normalized * num) + (Vector3.up * 1f)) };
            player.GetComponent<HERO>().networkView.RPC("blowAway", RPCMode.All, args);
        }
    }

    private void callTitan(bool special = false)
    {
        if (special || (GameObject.FindGameObjectsWithTag("titan").Length <= 6))
        {
            GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
            int index = UnityEngine.Random.Range(0, objArray.Length);
            GameObject obj2 = objArray[index];
            while (objArray[index] == null)
            {
                index = UnityEngine.Random.Range(0, objArray.Length);
                obj2 = objArray[index];
            }
            objArray[index] = null;
            string[] strArray = new string[] { "TITAN_NEW_1", "TITAN_NEW_2" };
            GameObject obj3 = (GameObject) Network.Instantiate(Resources.Load(strArray[UnityEngine.Random.Range(0, strArray.Length)]), obj2.transform.position, obj2.transform.rotation, 0);
            if (special)
            {
                GameObject[] objArray2 = GameObject.FindGameObjectsWithTag("route");
                obj3.GetComponent<TITAN>().setRoute(objArray2[UnityEngine.Random.Range(0, objArray2.Length)]);
                obj3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
                obj3.GetComponent<TITAN>().activeRad = 0;
                obj3.GetComponent<TITAN>().toCheckPoint((Vector3) obj3.GetComponent<TITAN>().checkPoints[0], 10f);
            }
            else
            {
                float num2 = 0.7f;
                float num3 = 0.7f;
                if (IN_GAME_MAIN_CAMERA.difficulty != 0)
                {
                    if (IN_GAME_MAIN_CAMERA.difficulty == 1)
                    {
                        num2 = 0.4f;
                        num3 = 0.7f;
                    }
                    else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
                    {
                        num2 = -1f;
                        num3 = 0.7f;
                    }
                }
                if (GameObject.FindGameObjectsWithTag("titan").Length == 5)
                {
                    obj3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                }
                else if (UnityEngine.Random.Range((float) 0f, (float) 1f) >= num2)
                {
                    if (UnityEngine.Random.Range((float) 0f, (float) 1f) < num3)
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
                    }
                    else
                    {
                        obj3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
                    }
                }
                obj3.GetComponent<TITAN>().activeRad = 200;
            }
            GameObject obj4 = (GameObject) Network.Instantiate(Resources.Load("FX/FXtitanSpawn"), obj3.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0);
            obj4.transform.localScale = obj3.transform.localScale;
        }
    }

    private void callTitanHAHA()
    {
        this.attackCount++;
        int num = 4;
        int num2 = 7;
        if (IN_GAME_MAIN_CAMERA.difficulty != 0)
        {
            if (IN_GAME_MAIN_CAMERA.difficulty == 1)
            {
                num = 4;
                num2 = 6;
            }
            else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
            {
                num = 3;
                num2 = 5;
            }
        }
        if ((this.attackCount % num) == 0)
        {
            this.callTitan(false);
        }
        if (this.NapeArmor < (this.NapeArmorTotal * 0.3))
        {
            if ((this.attackCount % ((int) (num2 * 0.5f))) == 0)
            {
                this.callTitan(true);
            }
        }
        else if ((this.attackCount % num2) == 0)
        {
            this.callTitan(true);
        }
    }

    [RPC]
    private void changeDoor()
    {
        this.door_broken.SetActiveRecursively(true);
        this.door_closed.SetActiveRecursively(false);
    }

    private RaycastHit[] checkHitCapsule(Vector3 start, Vector3 end, float r)
    {
        return Physics.SphereCastAll(start, r, end - start, Vector3.Distance(start, end));
    }

    private GameObject checkIfHitHand(Transform hand)
    {
        float num = 30f;
        foreach (Collider collider in Physics.OverlapSphere(hand.GetComponent<SphereCollider>().transform.position, num + 1f))
        {
            if (collider.transform.root.tag == "Player")
            {
                GameObject gameObject = collider.transform.root.gameObject;
                if (gameObject.GetComponent<TITAN_EREN>() != null)
                {
                    if (!gameObject.GetComponent<TITAN_EREN>().isHit)
                    {
                        gameObject.GetComponent<TITAN_EREN>().hitByTitan();
                    }
                    return gameObject;
                }
                if ((gameObject.GetComponent<HERO>() != null) && !gameObject.GetComponent<HERO>().isInvincible())
                {
                    return gameObject;
                }
            }
        }
        return null;
    }

    private void crossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            object[] args = new object[] { aniName, time };
            base.networkView.RPC("netCrossFade", RPCMode.Others, args);
        }
    }

    private void findNearestHero()
    {
        this.myHero = this.getNearestHero();
    }

    private GameObject getNearestHero()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("Player");
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        foreach (GameObject obj3 in objArray)
        {
            if (((obj3.GetComponent<HERO>() == null) || !obj3.GetComponent<HERO>().HasDied()) && ((obj3.GetComponent<TITAN_EREN>() == null) || !obj3.GetComponent<TITAN_EREN>().hasDied))
            {
                float num3 = Mathf.Sqrt(((obj3.transform.position.x - base.transform.position.x) * (obj3.transform.position.x - base.transform.position.x)) + ((obj3.transform.position.z - base.transform.position.z) * (obj3.transform.position.z - base.transform.position.z)));
                if (((obj3.transform.position.y - base.transform.position.y) < 450f) && (num3 < positiveInfinity))
                {
                    obj2 = obj3;
                    positiveInfinity = num3;
                }
            }
        }
        return obj2;
    }

    private void idle()
    {
        this.state = "idle";
        this.crossFade("idle", 0.2f);
    }

    private void kick()
    {
        this.state = "kick";
        this.actionName = "attack_kick_wall";
        this.attackCheckTime = 0.64f;
        this.attackChkOnce = false;
        this.crossFade(this.actionName, 0.1f);
    }

    private void killPlayer(GameObject hitHero)
    {
        if (hitHero != null)
        {
            Vector3 position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                if (!hitHero.GetComponent<HERO>().HasDied())
                {
                    hitHero.GetComponent<HERO>().die((Vector3) (((hitHero.transform.position - position) * 15f) * 4f), false);
                }
            }
            else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) && !hitHero.GetComponent<HERO>().HasDied())
            {
                hitHero.GetComponent<HERO>().markDie();
                object[] args = new object[] { (Vector3) (((hitHero.transform.position - position) * 15f) * 4f), false };
                hitHero.GetComponent<HERO>().networkView.RPC("netDie", RPCMode.All, args);
            }
        }
    }

    private void neckSteam()
    {
        this.neckSteamObject.GetComponent<ParticleSystem>().Stop();
        this.neckSteamObject.GetComponent<ParticleSystem>().Play();
        if (Network.peerType == NetworkPeerType.Server)
        {
            base.networkView.RPC("startNeckSteam", RPCMode.Others, new object[0]);
        }
        this.isSteamNeed = true;
        Transform neck = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
        float radius = 30f;
        foreach (Collider collider in Physics.OverlapSphere(neck.transform.position - ((Vector3) (base.transform.forward * 10f)), radius))
        {
            if (collider.transform.root.tag == "Player")
            {
                GameObject gameObject = collider.transform.root.gameObject;
                if ((gameObject.GetComponent<TITAN_EREN>() == null) && (gameObject.GetComponent<HERO>() != null))
                {
                    this.blowPlayer(gameObject, neck);
                }
            }
        }
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
    }

    [RPC]
    public void netDie()
    {
        if (!this.hasDie)
        {
            this.hasDie = true;
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().oneTitanDown();
        }
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        base.animation.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        base.animation.Play(aniName);
        base.animation[aniName].normalizedTime = normalizedTime;
    }

    private void playAnimation(string aniName)
    {
        base.animation.Play(aniName);
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            object[] args = new object[] { aniName };
            base.networkView.RPC("netPlayAnimation", RPCMode.Others, args);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        base.animation.Play(aniName);
        base.animation[aniName].normalizedTime = normalizedTime;
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            object[] args = new object[] { aniName, normalizedTime };
            base.networkView.RPC("netPlayAnimationAt", RPCMode.Others, args);
        }
    }

    private void playSound(string sndname)
    {
        this.playsoundRPC(sndname);
        if (Network.peerType == NetworkPeerType.Server)
        {
            object[] args = new object[] { sndname };
            base.networkView.RPC("playsoundRPC", RPCMode.Others, args);
        }
    }

    [RPC]
    private void playsoundRPC(string sndname)
    {
        base.transform.Find(sndname).GetComponent<AudioSource>().Play();
    }

    [RPC]
    private void removeMe()
    {
        Network.RemoveRPCs(base.networkView.viewID);
        UnityEngine.Object.Destroy(base.gameObject);
    }

    private void slap(string type)
    {
        this.callTitanHAHA();
        this.state = "slap";
        this.attackAnimation = type;
        if ((type == "r1") || (type == "r2"))
        {
            this.checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        }
        if ((type == "l1") || (type == "l2"))
        {
            this.checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
        }
        this.attackCheckTime = 0.57f;
        this.attackChkOnce = false;
        this.crossFade("attack_slap_" + this.attackAnimation, 0.1f);
    }

    private void Start()
    {
        if (this.myHero == null)
        {
            this.findNearestHero();
        }
        this.NapeArmor = 0x3e8;
        bool flag = false;
        if ((GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>() != null) && (LevelInfo.getInfo(GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().map).respawnMode == RespawnMode.NEVER))
        {
            flag = true;
        }
        if (IN_GAME_MAIN_CAMERA.difficulty == 0)
        {
            this.NapeArmor = !flag ? 0x1388 : 0x7d0;
        }
        else if (IN_GAME_MAIN_CAMERA.difficulty == 1)
        {
            this.NapeArmor = !flag ? 0x1f40 : 0xdac;
            foreach (AnimationState state1 in base.animation)
            {
                state1.speed = 1.02f;
            }
        }
        else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
        {
            this.NapeArmor = !flag ? 0x2ee0 : 0x1388;
            foreach (AnimationState state2 in base.animation)
            {
                state2.speed = 1.05f;
            }
        }
        this.NapeArmorTotal = this.NapeArmor;
        this.state = "wait";
        Transform transform = base.transform;
        transform.position += (Vector3) (-Vector3.up * 10000f);
    }

    [RPC]
    private void startNeckSteam()
    {
        this.neckSteamObject.GetComponent<ParticleSystem>().Stop();
        this.neckSteamObject.GetComponent<ParticleSystem>().Play();
    }

    [RPC]
    private void startSweepSmoke()
    {
        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().Play();
    }

    private void steam()
    {
        this.callTitanHAHA();
        this.state = "steam";
        this.actionName = "attack_steam";
        this.attackCheckTime = 0.45f;
        this.crossFade(this.actionName, 0.1f);
        this.attackChkOnce = false;
    }

    [RPC]
    private void stopSweepSmoke()
    {
        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
        this.sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
    }

    [RPC]
    public void titanGetHit(NetworkViewID player, int speed)
    {
        if (base.networkView.isMine)
        {
            Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
            Vector3 vector = NetworkView.Find(player).gameObject.transform.position - transform.transform.position;
            if (vector.magnitude < 30f)
            {
                this.NapeArmor -= speed;
                this.neckSteam();
                if (this.NapeArmor <= 0)
                {
                    this.NapeArmor = 0;
                    if (!this.hasDie)
                    {
                        base.networkView.RPC("netDie", RPCMode.OthersBuffered, new object[0]);
                        this.netDie();
                        if (player.isMine)
                        {
                            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().titanGetKillbyServer(speed, base.name);
                        }
                        else
                        {
                            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().titanGetKill(player, speed, base.name);
                        }
                    }
                }
                else if (player.isMine)
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().CTgetHurtbyServer(speed, base.name);
                }
                else
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().CTgetHurt(player, speed, base.name);
                }
            }
        }
    }

    private void Update()
    {
        if (this.state != "null")
        {
            if (this.state == "wait")
            {
                this.waitTime -= Time.deltaTime;
                if (this.waitTime <= 0f)
                {
                    base.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
                    UnityEngine.Object.Instantiate(Resources.Load("FX/ThunderCT"), base.transform.position + ((Vector3) (Vector3.up * 350f)), Quaternion.Euler(270f, 0f, 0f));
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    {
                        this.idle();
                    }
                    else if (base.networkView.isMine)
                    {
                        this.idle();
                    }
                    else
                    {
                        this.state = "null";
                    }
                }
            }
            else if (!(this.state == "idle"))
            {
                if (this.state == "attack_sweep")
                {
                    if ((this.attackCheckTimeA != 0f) && (((base.animation["attack_" + this.attackAnimation].normalizedTime >= this.attackCheckTimeA) && (base.animation["attack_" + this.attackAnimation].normalizedTime <= this.attackCheckTimeB)) || (!this.attackChkOnce && (base.animation["attack_" + this.attackAnimation].normalizedTime >= this.attackCheckTimeA))))
                    {
                        if (!this.attackChkOnce)
                        {
                            this.attackChkOnce = true;
                        }
                        foreach (RaycastHit hit in this.checkHitCapsule(this.checkHitCapsuleStart.position, this.checkHitCapsuleEnd.position, this.checkHitCapsuleR))
                        {
                            GameObject gameObject = hit.collider.gameObject;
                            if (gameObject.tag == "Player")
                            {
                                this.killPlayer(gameObject);
                            }
                            if (((gameObject.tag == "erenHitbox") && (this.attackAnimation == "combo_3")) && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER))
                            {
                                gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(3);
                            }
                        }
                        foreach (RaycastHit hit2 in this.checkHitCapsule(this.checkHitCapsuleEndOld, this.checkHitCapsuleEnd.position, this.checkHitCapsuleR))
                        {
                            GameObject hitHero = hit2.collider.gameObject;
                            if (hitHero.tag == "Player")
                            {
                                this.killPlayer(hitHero);
                            }
                        }
                        this.checkHitCapsuleEndOld = this.checkHitCapsuleEnd.position;
                    }
                    if (base.animation["attack_" + this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
                        this.sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
                        if (Network.peerType == NetworkPeerType.Server)
                        {
                            base.networkView.RPC("stopSweepSmoke", RPCMode.Others, new object[0]);
                        }
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == "kick")
                {
                    if (!this.attackChkOnce && (base.animation[this.actionName].normalizedTime >= this.attackCheckTime))
                    {
                        GameObject obj4;
                        this.attackChkOnce = true;
                        this.door_broken.SetActiveRecursively(true);
                        this.door_closed.SetActiveRecursively(false);
                        base.networkView.RPC("changeDoor", RPCMode.OthersBuffered, new object[0]);
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                        {
                            obj4 = (GameObject) Network.Instantiate(Resources.Load("FX/boom1_CT_KICK"), (Vector3) ((base.transform.position + (base.transform.forward * 120f)) + (base.transform.right * 30f)), Quaternion.Euler(270f, 0f, 0f), 0);
                            Network.Instantiate(Resources.Load("rock"), (Vector3) ((base.transform.position + (base.transform.forward * 120f)) + (base.transform.right * 30f)), Quaternion.Euler(0f, 0f, 0f), 0);
                        }
                        else
                        {
                            obj4 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/boom1_CT_KICK"), (Vector3) ((base.transform.position + (base.transform.forward * 120f)) + (base.transform.right * 30f)), Quaternion.Euler(270f, 0f, 0f));
                            UnityEngine.Object.Instantiate(Resources.Load("rock"), (Vector3) ((base.transform.position + (base.transform.forward * 120f)) + (base.transform.right * 30f)), Quaternion.Euler(0f, 0f, 0f));
                        }
                    }
                    if (base.animation[this.actionName].normalizedTime >= 1f)
                    {
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == "slap")
                {
                    if (!this.attackChkOnce && (base.animation["attack_slap_" + this.attackAnimation].normalizedTime >= this.attackCheckTime))
                    {
                        GameObject obj5;
                        this.attackChkOnce = true;
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                        {
                            obj5 = (GameObject) Network.Instantiate(Resources.Load("FX/boom1"), this.checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f), 0);
                        }
                        else
                        {
                            obj5 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/boom1"), this.checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f));
                        }
                        obj5.transform.localScale = new Vector3(5f, 5f, 5f);
                    }
                    if (base.animation["attack_slap_" + this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == "steam")
                {
                    if (!this.attackChkOnce && (base.animation[this.actionName].normalizedTime >= this.attackCheckTime))
                    {
                        GameObject obj6;
                        this.attackChkOnce = true;
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                        {
                            obj6 = (GameObject) Network.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + ((Vector3) (base.transform.up * 185f)), Quaternion.Euler(270f, 0f, 0f), 0);
                            obj6 = (GameObject) Network.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + ((Vector3) (base.transform.up * 303f)), Quaternion.Euler(270f, 0f, 0f), 0);
                            obj6 = (GameObject) Network.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + ((Vector3) (base.transform.up * 50f)), Quaternion.Euler(270f, 0f, 0f), 0);
                        }
                        else
                        {
                            obj6 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + ((Vector3) (base.transform.forward * 185f)), Quaternion.Euler(270f, 0f, 0f));
                            obj6 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + ((Vector3) (base.transform.forward * 303f)), Quaternion.Euler(270f, 0f, 0f));
                            obj6 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + ((Vector3) (base.transform.forward * 50f)), Quaternion.Euler(270f, 0f, 0f));
                        }
                    }
                    if (base.animation[this.actionName].normalizedTime >= 1f)
                    {
                        GameObject obj7;
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                        {
                            obj7 = (GameObject) Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + ((Vector3) (base.transform.up * 185f)), Quaternion.Euler(270f, 0f, 0f), 0);
                            obj7 = (GameObject) Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + ((Vector3) (base.transform.up * 303f)), Quaternion.Euler(270f, 0f, 0f), 0);
                            obj7 = (GameObject) Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + ((Vector3) (base.transform.up * 50f)), Quaternion.Euler(270f, 0f, 0f), 0);
                        }
                        else
                        {
                            obj7 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + ((Vector3) (base.transform.forward * 185f)), Quaternion.Euler(270f, 0f, 0f));
                            obj7 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + ((Vector3) (base.transform.forward * 303f)), Quaternion.Euler(270f, 0f, 0f));
                            obj7 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + ((Vector3) (base.transform.forward * 50f)), Quaternion.Euler(270f, 0f, 0f));
                        }
                        if (this.hasDie)
                        {
                            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                            {
                                UnityEngine.Object.Destroy(base.gameObject);
                            }
                            else if (base.networkView.isMine)
                            {
                                base.networkView.RPC("removeMe", RPCMode.AllBuffered, new object[0]);
                            }
                            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().gameWin();
                        }
                        this.findNearestHero();
                        this.idle();
                        this.playAnimation("idle");
                    }
                }
                else if (this.state == string.Empty)
                {
                }
            }
            else if (this.attackPattern == -1)
            {
                this.slap("r1");
                this.attackPattern++;
            }
            else if (this.attackPattern == 0)
            {
                this.attack_sweep(string.Empty);
                this.attackPattern++;
            }
            else if (this.attackPattern == 1)
            {
                this.steam();
                this.attackPattern++;
            }
            else if (this.attackPattern == 2)
            {
                this.kick();
                this.attackPattern++;
            }
            else if (this.isSteamNeed || this.hasDie)
            {
                this.steam();
                this.isSteamNeed = false;
            }
            else if (this.myHero == null)
            {
                this.findNearestHero();
            }
            else
            {
                Vector3 vector = this.myHero.transform.position - base.transform.position;
                float current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                float f = -Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
                this.myDistance = Mathf.Sqrt(((this.myHero.transform.position.x - base.transform.position.x) * (this.myHero.transform.position.x - base.transform.position.x)) + ((this.myHero.transform.position.z - base.transform.position.z) * (this.myHero.transform.position.z - base.transform.position.z)));
                float num3 = this.myHero.transform.position.y - base.transform.position.y;
                if ((this.myDistance < 85f) && (UnityEngine.Random.Range(0, 100) < 5))
                {
                    this.steam();
                }
                else
                {
                    if ((num3 > 310f) && (num3 < 350f))
                    {
                        if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("APL1").position) < 40f)
                        {
                            this.slap("l1");
                            return;
                        }
                        if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("APL2").position) < 40f)
                        {
                            this.slap("l2");
                            return;
                        }
                        if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("APR1").position) < 40f)
                        {
                            this.slap("r1");
                            return;
                        }
                        if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("APR2").position) < 40f)
                        {
                            this.slap("r2");
                            return;
                        }
                        if ((this.myDistance < 150f) && (Mathf.Abs(f) < 80f))
                        {
                            this.attack_sweep(string.Empty);
                            return;
                        }
                    }
                    if (((num3 < 300f) && (Mathf.Abs(f) < 80f)) && (this.myDistance < 85f))
                    {
                        this.attack_sweep("_vertical");
                    }
                    else
                    {
                        switch (UnityEngine.Random.Range(0, 7))
                        {
                            case 0:
                                this.slap("l1");
                                break;

                            case 1:
                                this.slap("l2");
                                break;

                            case 2:
                                this.slap("r1");
                                break;

                            case 3:
                                this.slap("r2");
                                break;

                            case 4:
                                this.attack_sweep(string.Empty);
                                break;

                            case 5:
                                this.attack_sweep("_vertical");
                                break;

                            case 6:
                                this.steam();
                                break;
                        }
                    }
                }
            }
        }
    }
}

