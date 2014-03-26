using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class TITAN : MonoBehaviour
{
    [CompilerGenerated]
    private static Dictionary<string, int> SWITCHSMAP4;
    [CompilerGenerated]
    private static Dictionary<string, int> SWITCHSMAP5;
    [CompilerGenerated]
    private static Dictionary<string, int> SWITCHSMAP6;
    [CompilerGenerated]
    private static Dictionary<string, int> SWITCHSMAP7;
    private Vector3 abnorma_jump_bite_horizon_v;
    public AbnormalType abnormalType;
    public int activeRad = 0x7fffffff;
    private float angle;
    private string attackAnimation;
    private float attackCheckTime;
    private float attackCheckTimeA;
    private float attackCheckTimeB;
    private int attackCount;
    public float attackDistance = 13f;
    private bool attacked;
    private float attackEndWait;
    public float attackWait = 1f;
    private float between2;
    public float chaseDistance = 80f;
    public ArrayList checkPoints = new ArrayList();
    public GameObject currentCamera;
    private Transform currentGrabHand;
    private float desDeg;
    private float dieTime;
    private string fxName;
    private Vector3 fxPosition;
    private Quaternion fxRotation;
    private GameObject grabbedTarget;
    public GameObject grabTF;
    private float gravity = 120f;
    private bool grounded;
    public bool hasDie;
    private bool hasDieSteam;
    private string hitAnimation;
    private float hitPause;
    public bool isAlarm;
    private bool isAttackMoveByCore;
    private bool isGrabHandLeft;
    public float maxVelocityChange = 10f;
    public static float minusDistance = 99999f;
    public static GameObject minusDistanceEnemy;
    public int myDifficulty;
    public float myDistance;
    public GameObject myHero;
    public float myLevel = 1f;
    private bool needFreshCorePosition;
    private string nextAttackAnimation;
    private Vector3 oldCorePosition;
    private float sbtime;
    private Vector3 spawnPt;
    public float speed = 7f;
    private string state = "idle";
    private int stepSoundPhase = 2;
    private Vector3 targetCheckPt;
    private float targetR;
    private float tauntTime;
    private string turnAnimation;
    private float turnDeg;
    private GameObject whoHasTauntMe;
    //custom
    private int maxhealth = 0;
    private int health = 0;

    private void attack(string type)
    {
        this.state = "attack";
        this.attacked = false;
        this.isAlarm = true;
        if (this.attackAnimation == type)
        {
            this.attackAnimation = type;
            this.playAnimationAt("attack_" + type, 0f);
        }
        else
        {
            this.attackAnimation = type;
            this.playAnimationAt("attack_" + type, 0f);
        }
        this.nextAttackAnimation = null;
        this.fxName = null;
        this.isAttackMoveByCore = false;
        this.attackCheckTime = 0f;
        this.attackCheckTimeA = 0f;
        this.attackCheckTimeB = 0f;
        this.attackEndWait = 0f;
        this.fxRotation = Quaternion.Euler(270f, 0f, 0f);
        string key = type;
        if (key != null)
        {
            int num;
            if (SWITCHSMAP6 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
                dictionary.Add("abnormal_getup", 0);
                dictionary.Add("abnormal_jump", 1);
                dictionary.Add("combo", 2);
                dictionary.Add("front_ground", 3);
                dictionary.Add("kick", 4);
                dictionary.Add("slap_back", 5);
                dictionary.Add("slap_face", 6);
                dictionary.Add("stomp", 7);
                dictionary.Add("bite", 8);
                dictionary.Add("bite_l", 9);
                dictionary.Add("bite_r", 10);
                dictionary.Add("abnormal_jump_bite0", 11);
                dictionary.Add("cralwer_jump_0", 12);
                SWITCHSMAP6 = dictionary;
            }
            if (SWITCHSMAP6.TryGetValue(key, out num))
            {
                switch (num)
                {
                    case 0:
                        this.attackCheckTime = 0f;
                        this.fxName = string.Empty;
                        break;

                    case 1:
                        this.nextAttackAnimation = "abnormal_getup";
                        this.attackEndWait = (this.myDifficulty <= 0) ? UnityEngine.Random.Range((float) 1f, (float) 4f) : UnityEngine.Random.Range((float) 0f, (float) 1f);
                        this.attackCheckTime = 0.75f;
                        this.fxName = "boom4";
                        this.fxRotation = Quaternion.Euler(270f, base.transform.rotation.eulerAngles.y, 0f);
                        break;

                    case 2:
                        this.isAttackMoveByCore = true;
                        this.attackCheckTime = 0.65f;
                        this.fxName = "boom1";
                        break;

                    case 3:
                        this.fxName = "boom1";
                        this.attackCheckTime = 0.45f;
                        break;

                    case 4:
                        this.fxName = "boom5";
                        this.fxRotation = base.transform.rotation;
                        this.attackCheckTime = 0.43f;
                        break;

                    case 5:
                        this.fxName = "boom3";
                        this.attackCheckTime = 0.66f;
                        break;

                    case 6:
                        this.fxName = "boom3";
                        this.attackCheckTime = 0.655f;
                        break;

                    case 7:
                        this.fxName = "boom2";
                        this.attackCheckTime = 0.42f;
                        break;

                    case 8:
                        this.fxName = "bite";
                        this.attackCheckTime = 0.6f;
                        break;

                    case 9:
                        this.fxName = "bite";
                        this.attackCheckTime = 0.4f;
                        break;

                    case 10:
                        this.fxName = "bite";
                        this.attackCheckTime = 0.4f;
                        break;

                    case 11:
                        this.abnorma_jump_bite_horizon_v = Vector3.zero;
                        break;

                    case 12:
                        this.abnorma_jump_bite_horizon_v = Vector3.zero;
                        break;
                }
            }
        }
        this.needFreshCorePosition = true;
    }

    private void Awake()
    {
        base.rigidbody.freezeRotation = true;
        base.rigidbody.useGravity = false;
    }

    public void beLaughAttacked()
    {
        if ((!this.hasDie && (this.abnormalType != AbnormalType.TYPE_CRAWLER)) && (((this.state == "idle") || (this.state == "turn")) || (this.state == "chase")))
        {
            this.laugh(0f);
        }
    }

    public void beTauntedBy(GameObject target, float tauntTime)
    {
        this.whoHasTauntMe = target;
        this.tauntTime = tauntTime;
        this.isAlarm = true;
    }

    private void chase()
    {
        this.state = "chase";
        this.isAlarm = true;
        if (this.abnormalType == AbnormalType.NORMAL)
        {
            this.crossFade("run_walk", 0.5f);
        }
        else if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            this.crossFade("crawler_run", 0.5f);
        }
        else
        {
            this.crossFade("run_abnormal", 0.5f);
        }
    }

    private GameObject checkIfHitCrawlerMouth(Transform head, float rad)
    {
        float num = rad * this.myLevel;
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
        {
            if ((obj2.GetComponent<TITAN_EREN>() == null) && ((obj2.GetComponent<HERO>() == null) || !obj2.GetComponent<HERO>().isInvincible()))
            {
                float num3 = obj2.GetComponent<CapsuleCollider>().height * 0.5f;
                if (Vector3.Distance(obj2.transform.position + ((Vector3) (Vector3.up * num3)), head.transform.position - ((Vector3) ((Vector3.up * 1.5f) * this.myLevel))) < (num + num3))
                {
                    return obj2;
                }
            }
        }
        return null;
    }

    private GameObject checkIfHitHand(Transform hand)
    {
        float num = 2.4f * this.myLevel;
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
                }
                else if ((gameObject.GetComponent<HERO>() != null) && !gameObject.GetComponent<HERO>().isInvincible())
                {
                    return gameObject;
                }
            }
        }
        return null;
    }

    private GameObject checkIfHitHead(Transform head, float rad)
    {
        float num = rad * this.myLevel;
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
        {
            if ((obj2.GetComponent<TITAN_EREN>() == null) && ((obj2.GetComponent<HERO>() == null) || !obj2.GetComponent<HERO>().isInvincible()))
            {
                float num3 = obj2.GetComponent<CapsuleCollider>().height * 0.5f;
                if (Vector3.Distance(obj2.transform.position + ((Vector3) (Vector3.up * num3)), head.transform.position + ((Vector3) ((Vector3.up * 1.5f) * this.myLevel))) < (num + num3))
                {
                    return obj2;
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

    public bool die()
    {
        if (this.hasDie)
        {
            return false;
        }
        IN_GAME_MAIN_CAMERA component = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>();
        component.titanNum--;
        this.hasDie = true;
        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().oneTitanDown();
        if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            this.crossFade("crawler_die", 0.2f);
        }
        else if (this.abnormalType == AbnormalType.NORMAL)
        {
            this.crossFade("die_front", 0.05f);
        }
        else if ((this.abnormalType == AbnormalType.TYPE_I) || (this.abnormalType == AbnormalType.TYPE_JUMPER))
        {
            if (((base.animation.IsPlaying("attack_abnormal_jump") && (base.animation["attack_abnormal_jump"].normalizedTime > 0.7f)) || (base.animation.IsPlaying("attack_abnormal_getup") && (base.animation["attack_abnormal_getup"].normalizedTime < 0.7f))) || base.animation.IsPlaying("tired"))
            {
                this.crossFade("die_ground", 0.2f);
            }
            else
            {
                this.crossFade("die_back", 0.05f);
            }
        }
        return true;
    }

    public void dieBlow(Vector3 attacker, float hitPauseTime)
    {
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            this.dieBlowFunc(attacker, hitPauseTime);
            if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
            {
                GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            }
        }
        else
        {
            object[] args = new object[] { attacker, hitPauseTime };
            base.networkView.RPC("dieBlowRPC", RPCMode.All, args);
        }
    }

    public void dieBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (!this.hasDie)
        {
            base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
            this.hasDie = true;
            this.hitAnimation = "die_blow";
            this.hitPause = hitPauseTime;
            this.playAnimation(this.hitAnimation);
            base.animation[this.hitAnimation].time = 0f;
            base.animation[this.hitAnimation].speed = 0f;
            this.needFreshCorePosition = true;
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().oneTitanDown();
            if ((Network.peerType == NetworkPeerType.Server) && (this.grabbedTarget != null))
            {
                this.grabbedTarget.networkView.RPC("netUngrabbed", RPCMode.All, new object[0]);
            }
        }
    }

    [RPC]
    private void dieBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.networkView.isMine)
        {
            Vector3 vector = attacker - base.transform.position;
            if (vector.magnitude < 80f)
            {
                this.dieBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    public void dieHeadBlow(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType != AbnormalType.TYPE_CRAWLER)
        {
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                this.dieHeadBlowFunc(attacker, hitPauseTime);
                if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
                {
                    GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                }
            }
            else
            {
                object[] args = new object[] { attacker, hitPauseTime };
                base.networkView.RPC("dieHeadBlowRPC", RPCMode.All, args);
            }
        }
    }

    public void dieHeadBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (!this.hasDie)
        {
            GameObject obj2;
            this.playSound("snd_titan_head_blow");
            base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
            this.hasDie = true;
            this.hitAnimation = "die_headOff";
            this.hitPause = hitPauseTime;
            this.playAnimation(this.hitAnimation);
            base.animation[this.hitAnimation].time = 0f;
            base.animation[this.hitAnimation].speed = 0f;
            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().oneTitanDown();
            this.needFreshCorePosition = true;
            Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
            Transform transform2 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
            {
                obj2 = (GameObject) Network.Instantiate(Resources.Load("bloodExplore"), transform.position + ((Vector3) ((Vector3.up * 1f) * this.myLevel)), Quaternion.Euler(270f, 0f, 0f), 0);
            }
            else
            {
                obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("bloodExplore"), transform.position + ((Vector3) ((Vector3.up * 1f) * this.myLevel)), Quaternion.Euler(270f, 0f, 0f));
            }
            obj2.transform.localScale = base.transform.localScale;
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
            {
                obj2 = (GameObject) Network.Instantiate(Resources.Load("bloodsplatter"), transform.position, Quaternion.Euler(270f + transform2.rotation.eulerAngles.x, transform2.rotation.eulerAngles.y, transform2.rotation.eulerAngles.z), 0);
            }
            else
            {
                obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("bloodsplatter"), transform.position, Quaternion.Euler(270f + transform2.rotation.eulerAngles.x, transform2.rotation.eulerAngles.y, transform2.rotation.eulerAngles.z));
            }
            obj2.transform.localScale = base.transform.localScale;
            obj2.transform.parent = transform2;
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
            {
                obj2 = (GameObject) Network.Instantiate(Resources.Load("FX/justSmoke"), transform2.position, Quaternion.Euler(270f, 0f, 0f), 0);
            }
            else
            {
                obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/justSmoke"), transform2.position, Quaternion.Euler(270f, 0f, 0f));
            }
            obj2.transform.parent = transform2;
            if ((Network.peerType == NetworkPeerType.Server) && (this.grabbedTarget != null))
            {
                this.grabbedTarget.networkView.RPC("netUngrabbed", RPCMode.All, new object[0]);
            }
        }
    }

    [RPC]
    private void dieHeadBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.networkView.isMine)
        {
            Vector3 vector = attacker - base.transform.position;
            if (vector.magnitude < 80f)
            {
                this.dieHeadBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    private void eat()
    {
        this.state = "eat";
        this.attacked = false;
        if (this.isGrabHandLeft)
        {
            this.attackAnimation = "eat_l";
            this.crossFade("eat_l", 0.1f);
        }
        else
        {
            this.attackAnimation = "eat_r";
            this.crossFade("eat_r", 0.1f);
        }
    }

    private void eatSet(GameObject grabTarget)
    {
        if (!grabTarget.GetComponent<HERO>().isGrabbed)
        {
            this.grabToRight();
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
            {
                object[] args = new object[] { base.networkView.viewID, false };
                grabTarget.networkView.RPC("netGrabbed", RPCMode.All, args);
                object[] objArray2 = new object[] { "grabbed" };
                grabTarget.networkView.RPC("netPlayAnimation", RPCMode.All, objArray2);
                base.networkView.RPC("grabToRight", RPCMode.Others, new object[0]);
            }
            else
            {
                grabTarget.GetComponent<HERO>().grabbed(base.gameObject, false);
                grabTarget.GetComponent<HERO>().animation.Play("grabbed");
            }
        }
    }

    private void eatSetL(GameObject grabTarget)
    {
        if (!grabTarget.GetComponent<HERO>().isGrabbed)
        {
            this.grabToLeft();
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
            {
                object[] args = new object[] { base.networkView.viewID, true };
                grabTarget.networkView.RPC("netGrabbed", RPCMode.All, args);
                object[] objArray2 = new object[] { "grabbed" };
                grabTarget.networkView.RPC("netPlayAnimation", RPCMode.All, objArray2);
                base.networkView.RPC("grabToLeft", RPCMode.Others, new object[0]);
            }
            else
            {
                grabTarget.GetComponent<HERO>().grabbed(base.gameObject, true);
                grabTarget.GetComponent<HERO>().animation.Play("grabbed");
            }
        }
    }

    private void findNearestFacingHero()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("Player");
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        Vector3 position = base.transform.position;
        float current = 0f;
        float num3 = (this.abnormalType != AbnormalType.NORMAL) ? 180f : 100f;
        float f = 0f;
        foreach (GameObject obj3 in objArray)
        {
            if (obj3.networkView.owner.ToString() == "0" && GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isDedicated)
            {
            }
            else
            {
                Vector3 vector2 = obj3.transform.position - position;
                float sqrMagnitude = vector2.sqrMagnitude;
                if (sqrMagnitude < positiveInfinity)
                {
                    Vector3 vector3 = obj3.transform.position - base.transform.position;
                    current = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
                    f = -Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
                    if (Mathf.Abs(f) < num3)
                    {
                        obj2 = obj3;
                        positiveInfinity = sqrMagnitude;
                    }
                }
            }
        }
        if (obj2 != null)
        {
            this.myHero = obj2;
            this.tauntTime = 5f;
        }
    }

    private void findNearestHero()
    {
        this.myHero = this.getNearestHero();
    }

    private void FixedUpdate()
    {
        if ((!IN_GAME_MAIN_CAMERA.isPausing || (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)) && ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine))
        {
            base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
            if (this.needFreshCorePosition)
            {
                this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
                this.needFreshCorePosition = false;
            }
            if (this.hasDie)
            {
                if ((this.hitPause <= 0f) && base.animation.IsPlaying("die_headOff"))
                {
                    Vector3 vector = (base.transform.position - base.transform.Find("Amarture/Core").position) - this.oldCorePosition;
                    base.rigidbody.velocity = (Vector3) ((vector / Time.deltaTime) + (Vector3.up * base.rigidbody.velocity.y));
                }
                this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
            }
            else if (((this.state == "attack") && this.isAttackMoveByCore) || (this.state == "hit"))
            {
                Vector3 vector2 = (base.transform.position - base.transform.Find("Amarture/Core").position) - this.oldCorePosition;
                base.rigidbody.velocity = (Vector3) ((vector2 / Time.deltaTime) + (Vector3.up * base.rigidbody.velocity.y));
                this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
            }
            if (this.hasDie)
            {
                if (this.hitPause > 0f)
                {
                    this.hitPause -= Time.deltaTime;
                    if (this.hitPause <= 0f)
                    {
                        base.animation[this.hitAnimation].speed = 1f;
                        this.hitPause = 0f;
                    }
                }
                else if (base.animation.IsPlaying("die_blow"))
                {
                    if (base.animation["die_blow"].normalizedTime < 0.55f)
                    {
                        base.rigidbody.velocity = (Vector3) ((-base.transform.forward * 300f) + (Vector3.up * base.rigidbody.velocity.y));
                    }
                    else if (base.animation["die_blow"].normalizedTime < 0.83f)
                    {
                        base.rigidbody.velocity = (Vector3) ((-base.transform.forward * 100f) + (Vector3.up * base.rigidbody.velocity.y));
                    }
                    else
                    {
                        base.rigidbody.velocity = (Vector3) (Vector3.up * base.rigidbody.velocity.y);
                    }
                }
            }
            else if (this.myHero != null)
            {
                if (((this.state == "chase") || (this.state == "wander")) || (this.state == "toCheckPoint"))
                {
                    Vector3 vector3;
                    if (this.state == "wander")
                    {
                        vector3 = (Vector3) ((base.transform.forward * this.speed) * 0.8f);
                    }
                    else
                    {
                        vector3 = (Vector3) (base.transform.forward * this.speed);
                    }
                    Vector3 velocity = base.rigidbody.velocity;
                    Vector3 force = vector3 - velocity;
                    force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                    force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                    force.y = 0f;
                    base.rigidbody.AddForce(force, ForceMode.VelocityChange);
                    float current = 0f;
                    if (this.state == "wander")
                    {
                        current = base.transform.rotation.eulerAngles.y - 90f;
                    }
                    else if (this.state == "toCheckPoint")
                    {
                        Vector3 vector6 = this.targetCheckPt - base.transform.position;
                        current = -Mathf.Atan2(vector6.z, vector6.x) * 57.29578f;
                    }
                    else
                    {
                        Vector3 vector7 = this.myHero.transform.position - base.transform.position;
                        current = -Mathf.Atan2(vector7.z, vector7.x) * 57.29578f;
                    }
                    float num2 = -Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
                    if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
                    {
                        base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num2, 0f), ((this.speed * 0.3f) * Time.deltaTime) / this.myLevel);
                    }
                    else
                    {
                        base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num2, 0f), ((this.speed * 0.5f) * Time.deltaTime) / this.myLevel);
                    }
                }
                if (((this.abnormalType == AbnormalType.TYPE_I) || (this.abnormalType == AbnormalType.TYPE_JUMPER)) && ((this.state == "attack") && (this.attackAnimation == "abnormal_jump")))
                {
                    Vector3 zero = (Vector3) (((base.transform.forward * this.speed) * this.myLevel) * 0.5f);
                    Vector3 vector9 = base.rigidbody.velocity;
                    if ((base.animation["attack_abnormal_jump"].normalizedTime <= 0.28f) || (base.animation["attack_abnormal_jump"].normalizedTime >= 0.8f))
                    {
                        zero = Vector3.zero;
                    }
                    Vector3 vector10 = zero - vector9;
                    vector10.x = Mathf.Clamp(vector10.x, -this.maxVelocityChange, this.maxVelocityChange);
                    vector10.z = Mathf.Clamp(vector10.z, -this.maxVelocityChange, this.maxVelocityChange);
                    vector10.y = 0f;
                    base.rigidbody.AddForce(vector10, ForceMode.VelocityChange);
                }
            }
        }
    }

    private string[] GetAttackStrategy()
    {
        Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        Transform transform2 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
        string[] strArray = null;
        int num = 0;
        if (this.isAlarm || ((this.myHero.transform.position.y + 3f) <= (transform2.position.y + (10f * this.myLevel))))
        {
            if (this.myHero.transform.position.y > (transform2.position.y - (3f * this.myLevel)))
            {
                if (this.myDistance < (this.attackDistance * 0.5f))
                {
                    if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("chkOverHead").position) < (3.6f * this.myLevel))
                    {
                        if (this.between2 > 0f)
                        {
                            strArray = new string[] { "grab_head_front_r" };
                        }
                        else
                        {
                            strArray = new string[] { "grab_head_front_l" };
                        }
                    }
                    else if (Mathf.Abs(this.between2) < 90f)
                    {
                        if (Mathf.Abs(this.between2) < 30f)
                        {
                            if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("chkFront").position) < (2.5f * this.myLevel))
                            {
                                strArray = new string[] { "attack_bite", "attack_bite", "attack_slap_face" };
                            }
                        }
                        else if (this.between2 > 0f)
                        {
                            if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("chkFrontRight").position) < (2.5f * this.myLevel))
                            {
                                strArray = new string[] { "attack_bite_r" };
                            }
                        }
                        else if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("chkFrontLeft").position) < (2.5f * this.myLevel))
                        {
                            strArray = new string[] { "attack_bite_l" };
                        }
                    }
                    else if (this.between2 > 0f)
                    {
                        if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("chkBackRight").position) < (2.8f * this.myLevel))
                        {
                            strArray = new string[] { "grab_head_back_r", "grab_head_back_r", "attack_slap_back" };
                        }
                    }
                    else if (Vector3.Distance(this.myHero.transform.position, base.transform.Find("chkBackLeft").position) < (2.8f * this.myLevel))
                    {
                        strArray = new string[] { "grab_head_back_l", "grab_head_back_l", "attack_slap_back" };
                    }
                }
                if (strArray != null)
                {
                    return strArray;
                }
                if (this.abnormalType == AbnormalType.NORMAL)
                {
                    if ((this.myDifficulty <= 0) && (UnityEngine.Random.Range(0, 0x3e8) >= 3))
                    {
                        return strArray;
                    }
                    if (Mathf.Abs(this.between2) >= 60f)
                    {
                        return strArray;
                    }
                    return new string[] { "attack_combo" };
                }
                if ((this.abnormalType != AbnormalType.TYPE_I) && (this.abnormalType != AbnormalType.TYPE_JUMPER))
                {
                    return strArray;
                }
                if ((this.myDifficulty <= 0) && (UnityEngine.Random.Range(0, 100) >= 50))
                {
                    return strArray;
                }
                return new string[] { "attack_abnormal_jump" };
            }
            if (Mathf.Abs(this.between2) < 90f)
            {
                if (this.between2 > 0f)
                {
                    num = 1;
                }
                else
                {
                    num = 2;
                }
            }
            else if (this.between2 > 0f)
            {
                num = 4;
            }
            else
            {
                num = 3;
            }
            switch (num)
            {
                case 1:
                    if (this.myDistance >= (this.attackDistance * 0.25f))
                    {
                        if (this.myDistance < (this.attackDistance * 0.5f))
                        {
                            if (this.abnormalType == AbnormalType.NORMAL)
                            {
                                return new string[] { "grab_ground_front_r", "grab_ground_front_r", "attack_stomp" };
                            }
                            return new string[] { "grab_ground_front_r", "grab_ground_front_r", "attack_abnormal_jump" };
                        }
                        if (this.abnormalType == AbnormalType.NORMAL)
                        {
                            if (this.myDifficulty > 0)
                            {
                                return new string[] { "attack_front_ground", "attack_combo", "attack_combo" };
                            }
                            return new string[] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" };
                        }
                        return new string[] { "attack_abnormal_jump" };
                    }
                    if (this.abnormalType != AbnormalType.NORMAL)
                    {
                        return new string[] { "attack_kick" };
                    }
                    return new string[] { "attack_front_ground", "attack_stomp" };

                case 2:
                    if (this.myDistance >= (this.attackDistance * 0.25f))
                    {
                        if (this.myDistance < (this.attackDistance * 0.5f))
                        {
                            if (this.abnormalType == AbnormalType.NORMAL)
                            {
                                return new string[] { "grab_ground_front_l", "grab_ground_front_l", "attack_stomp" };
                            }
                            return new string[] { "grab_ground_front_l", "grab_ground_front_l", "attack_abnormal_jump" };
                        }
                        if (this.abnormalType == AbnormalType.NORMAL)
                        {
                            if (this.myDifficulty > 0)
                            {
                                return new string[] { "attack_front_ground", "attack_combo", "attack_combo" };
                            }
                            return new string[] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" };
                        }
                        return new string[] { "attack_abnormal_jump" };
                    }
                    if (this.abnormalType != AbnormalType.NORMAL)
                    {
                        return new string[] { "attack_kick" };
                    }
                    return new string[] { "attack_front_ground", "attack_stomp" };

                case 3:
                    if (this.myDistance >= (this.attackDistance * 0.5f))
                    {
                        return strArray;
                    }
                    if (this.abnormalType != AbnormalType.NORMAL)
                    {
                        return new string[] { "grab_ground_back_l" };
                    }
                    return new string[] { "grab_ground_back_l" };

                case 4:
                    if (this.myDistance >= (this.attackDistance * 0.5f))
                    {
                        return strArray;
                    }
                    if (this.abnormalType != AbnormalType.NORMAL)
                    {
                        return new string[] { "grab_ground_back_r" };
                    }
                    return new string[] { "grab_ground_back_r" };
            }
        }
        return strArray;
    }

    private GameObject getNearestHero()
    {
        GameObject[] objArray = GameObject.FindGameObjectsWithTag("Player");
        GameObject obj2 = null;
        float positiveInfinity = float.PositiveInfinity;
        Vector3 position = base.transform.position;
        foreach (GameObject obj3 in objArray)
        {
            if (obj3.networkView.owner.ToString() == "0" && GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().isDedicated)
            {
            }
            else
            {
                Vector3 vector2 = obj3.transform.position - position;
                float sqrMagnitude = vector2.sqrMagnitude;
                if (sqrMagnitude < positiveInfinity)
                {
                    obj2 = obj3;
                    positiveInfinity = sqrMagnitude;
                }
            }
        }
        return obj2;
    }

    private void grab(string type)
    {
        this.state = "grab";
        this.attacked = false;
        this.isAlarm = true;
        this.attackAnimation = type;
        this.crossFade("grab_" + type, 0.1f);
        this.isGrabHandLeft = true;
        this.grabbedTarget = null;
        string key = type;
        if (key != null)
        {
            int num;
            if (SWITCHSMAP7 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
                dictionary.Add("ground_back_l", 0);
                dictionary.Add("ground_back_r", 1);
                dictionary.Add("ground_front_l", 2);
                dictionary.Add("ground_front_r", 3);
                dictionary.Add("head_back_l", 4);
                dictionary.Add("head_back_r", 5);
                dictionary.Add("head_front_l", 6);
                dictionary.Add("head_front_r", 7);
                SWITCHSMAP7 = dictionary;
            }
            if (SWITCHSMAP7.TryGetValue(key, out num))
            {
                switch (num)
                {
                    case 0:
                        this.attackCheckTimeA = 0.34f;
                        this.attackCheckTimeB = 0.49f;
                        break;

                    case 1:
                        this.attackCheckTimeA = 0.34f;
                        this.attackCheckTimeB = 0.49f;
                        this.isGrabHandLeft = false;
                        break;

                    case 2:
                        this.attackCheckTimeA = 0.37f;
                        this.attackCheckTimeB = 0.6f;
                        break;

                    case 3:
                        this.attackCheckTimeA = 0.37f;
                        this.attackCheckTimeB = 0.6f;
                        this.isGrabHandLeft = false;
                        break;

                    case 4:
                        this.attackCheckTimeA = 0.45f;
                        this.attackCheckTimeB = 0.5f;
                        this.isGrabHandLeft = false;
                        break;

                    case 5:
                        this.attackCheckTimeA = 0.45f;
                        this.attackCheckTimeB = 0.5f;
                        break;

                    case 6:
                        this.attackCheckTimeA = 0.38f;
                        this.attackCheckTimeB = 0.55f;
                        break;

                    case 7:
                        this.attackCheckTimeA = 0.38f;
                        this.attackCheckTimeB = 0.55f;
                        this.isGrabHandLeft = false;
                        break;
                }
            }
        }
        if (this.isGrabHandLeft)
        {
            this.currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
        }
        else
        {
            this.currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        }
    }

    [RPC]
    public void grabbedTargetEscape()
    {
        Debug.Log(this.grabbedTarget + "is free by him/her-self!!!");
        this.grabbedTarget = null;
    }

    [RPC]
    public void grabToLeft()
    {
        Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
        this.grabTF.transform.parent = transform;
        this.grabTF.transform.parent = transform;
        this.grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
        this.grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
        Transform transform1 = this.grabTF.transform;
        transform1.localPosition -= (Vector3) ((Vector3.right * transform.GetComponent<SphereCollider>().radius) * 0.3f);
        Transform transform2 = this.grabTF.transform;
        transform2.localPosition -= (Vector3) ((Vector3.up * transform.GetComponent<SphereCollider>().radius) * 0.51f);
        Transform transform3 = this.grabTF.transform;
        transform3.localPosition -= (Vector3) ((Vector3.forward * transform.GetComponent<SphereCollider>().radius) * 0.3f);
        this.grabTF.transform.localRotation = Quaternion.Euler(this.grabTF.transform.localRotation.eulerAngles.x, this.grabTF.transform.localRotation.eulerAngles.y + 180f, this.grabTF.transform.localRotation.eulerAngles.z + 180f);
    }

    [RPC]
    public void grabToRight()
    {
        Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        this.grabTF.transform.parent = transform;
        this.grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
        this.grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
        Transform transform1 = this.grabTF.transform;
        transform1.localPosition -= (Vector3) ((Vector3.right * transform.GetComponent<SphereCollider>().radius) * 0.3f);
        Transform transform2 = this.grabTF.transform;
        transform2.localPosition += (Vector3) ((Vector3.up * transform.GetComponent<SphereCollider>().radius) * 0.51f);
        Transform transform3 = this.grabTF.transform;
        transform3.localPosition -= (Vector3) ((Vector3.forward * transform.GetComponent<SphereCollider>().radius) * 0.3f);
        this.grabTF.transform.localRotation = Quaternion.Euler(this.grabTF.transform.localRotation.eulerAngles.x, this.grabTF.transform.localRotation.eulerAngles.y + 180f, this.grabTF.transform.localRotation.eulerAngles.z);
    }

    private void hit(string animationName, Vector3 attacker, float hitPauseTime)
    {
        this.state = "hit";
        this.hitAnimation = animationName;
        this.hitPause = hitPauseTime;
        this.playAnimation(this.hitAnimation);
        base.animation[this.hitAnimation].time = 0f;
        base.animation[this.hitAnimation].speed = 0f;
        base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
        this.needFreshCorePosition = true;
        if ((Network.peerType == NetworkPeerType.Server) && (this.grabbedTarget != null))
        {
            this.grabbedTarget.networkView.RPC("netUngrabbed", RPCMode.All, new object[0]);
        }
    }

    public void hitEye()
    {
        if (!this.hasDie)
        {
            this.justHitEye();
        }
    }

    [RPC]
    public void hitEyeRPC(NetworkViewID player)
    {
        if (!this.hasDie)
        {
            if ((Network.peerType == NetworkPeerType.Server) && (this.grabbedTarget != null))
            {
                this.grabbedTarget.networkView.RPC("netUngrabbed", RPCMode.All, new object[0]);
            }
            Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
            Vector3 vector = NetworkView.Find(player).gameObject.transform.position - transform.transform.position;
            if ((vector.magnitude < 10f) && !this.hasDie)
            {
                this.justHitEye();
            }
        }
    }

    public void hitL(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType != AbnormalType.TYPE_CRAWLER)
        {
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                this.hit("hit_eren_L", attacker, hitPauseTime);
            }
            else
            {
                object[] args = new object[] { attacker, hitPauseTime };
                base.networkView.RPC("hitLRPC", RPCMode.All, args);
            }
        }
    }

    [RPC]
    private void hitLRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.networkView.isMine)
        {
            Vector3 vector = attacker - base.transform.position;
            if (vector.magnitude < 80f)
            {
                this.hit("hit_eren_L", attacker, hitPauseTime);
            }
        }
    }

    public void hitR(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType != AbnormalType.TYPE_CRAWLER)
        {
            if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
            {
                this.hit("hit_eren_R", attacker, hitPauseTime);
            }
            else
            {
                object[] args = new object[] { attacker, hitPauseTime };
                base.networkView.RPC("hitRRPC", RPCMode.All, args);
            }
        }
    }

    [RPC]
    private void hitRRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.networkView.isMine && !this.hasDie)
        {
            Vector3 vector = attacker - base.transform.position;
            if (vector.magnitude < 80f)
            {
                this.hit("hit_eren_R", attacker, hitPauseTime);
            }
        }
    }

    private void idle(float sbtime = 0f)
    {
        this.sbtime = sbtime;
        if ((this.myDifficulty == 2) && ((this.abnormalType == AbnormalType.TYPE_JUMPER) || (this.abnormalType == AbnormalType.TYPE_I)))
        {
            this.sbtime = UnityEngine.Random.Range((float) 0f, (float) 1.5f);
        }
        else if (this.myDifficulty >= 1)
        {
            this.sbtime = 0f;
        }
        this.sbtime = Mathf.Max(0.5f, this.sbtime);
        this.state = "idle";
        if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            this.crossFade("crawler_idle", 0.2f);
        }
        else
        {
            this.crossFade("idle", 0.2f);
        }
    }

    public bool IsGrounded()
    {
        LayerMask mask = ((int) 1) << LayerMask.NameToLayer("Ground");
        LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("EnemyAABB");
        LayerMask mask3 = mask2 | mask;
        return Physics.Raycast(base.gameObject.transform.position + ((Vector3) (Vector3.up * 0.1f)), -Vector3.up, (float) 0.3f, mask3.value);
    }

    private void justEatHero(GameObject target, Transform hand)
    {
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
        {
            if (!target.GetComponent<HERO>().HasDied())
            {
                target.GetComponent<HERO>().markDie();
                target.GetComponent<HERO>().networkView.RPC("netDie2", RPCMode.All, new object[0]);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            target.GetComponent<HERO>().die2(hand);
        }
    }

    private void justHitEye()
    {
        this.state = "hit_eye";
        this.playAnimation("hit_eye");
    }

    private void LateUpdate()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing || (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE))
        {
            if (base.animation.IsPlaying("run_walk"))
            {
                if ((((base.animation["run_walk"].normalizedTime % 1f) > 0.1f) && ((base.animation["run_walk"].normalizedTime % 1f) < 0.6f)) && (this.stepSoundPhase == 2))
                {
                    this.stepSoundPhase = 1;
                    Transform transform = base.transform.Find("snd_titan_foot");
                    transform.GetComponent<AudioSource>().Stop();
                    transform.GetComponent<AudioSource>().Play();
                }
                if (((base.animation["run_walk"].normalizedTime % 1f) > 0.6f) && (this.stepSoundPhase == 1))
                {
                    this.stepSoundPhase = 2;
                    Transform transform2 = base.transform.Find("snd_titan_foot");
                    transform2.GetComponent<AudioSource>().Stop();
                    transform2.GetComponent<AudioSource>().Play();
                }
            }
            if (base.animation.IsPlaying("crawler_run"))
            {
                if ((((base.animation["crawler_run"].normalizedTime % 1f) > 0.1f) && ((base.animation["crawler_run"].normalizedTime % 1f) < 0.56f)) && (this.stepSoundPhase == 2))
                {
                    this.stepSoundPhase = 1;
                    Transform transform3 = base.transform.Find("snd_titan_foot");
                    transform3.GetComponent<AudioSource>().Stop();
                    transform3.GetComponent<AudioSource>().Play();
                }
                if (((base.animation["crawler_run"].normalizedTime % 1f) > 0.56f) && (this.stepSoundPhase == 1))
                {
                    this.stepSoundPhase = 2;
                    Transform transform4 = base.transform.Find("snd_titan_foot");
                    transform4.GetComponent<AudioSource>().Stop();
                    transform4.GetComponent<AudioSource>().Play();
                }
            }
            if (base.animation.IsPlaying("run_abnormal"))
            {
                if ((((base.animation["run_abnormal"].normalizedTime % 1f) > 0.47f) && ((base.animation["run_abnormal"].normalizedTime % 1f) < 0.95f)) && (this.stepSoundPhase == 2))
                {
                    this.stepSoundPhase = 1;
                    Transform transform5 = base.transform.Find("snd_titan_foot");
                    transform5.GetComponent<AudioSource>().Stop();
                    transform5.GetComponent<AudioSource>().Play();
                }
                if ((((base.animation["run_abnormal"].normalizedTime % 1f) > 0.95f) || ((base.animation["run_abnormal"].normalizedTime % 1f) < 0.47f)) && (this.stepSoundPhase == 1))
                {
                    this.stepSoundPhase = 2;
                    Transform transform6 = base.transform.Find("snd_titan_foot");
                    transform6.GetComponent<AudioSource>().Stop();
                    transform6.GetComponent<AudioSource>().Play();
                }
            }
            if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine)
            {
                this.grounded = false;
            }
        }
    }

    private void laugh(float sbtime = 0f)
    {
        this.sbtime = sbtime;
        this.state = "laugh";
        this.crossFade("laugh", 0.2f);
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
    }

    [RPC]
    private void netDie()
    {
        if (!this.hasDie)
        {
            this.hasDie = true;
            if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
            {
                this.crossFade("crawler_die", 0.2f);
            }
            else if (this.abnormalType == AbnormalType.NORMAL)
            {
                this.crossFade("die_front", 0.05f);
            }
            else if ((this.abnormalType == AbnormalType.TYPE_I) || (this.abnormalType == AbnormalType.TYPE_JUMPER))
            {
                if (((base.animation.IsPlaying("attack_abnormal_jump") && (base.animation["attack_abnormal_jump"].normalizedTime > 0.7f)) || (base.animation.IsPlaying("attack_abnormal_getup") && (base.animation["attack_abnormal_getup"].normalizedTime < 0.7f))) || base.animation.IsPlaying("tired"))
                {
                    this.crossFade("die_ground", 0.2f);
                }
                else
                {
                    this.crossFade("die_back", 0.05f);
                }
            }
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

    [RPC]
    public void netSetAbnormalType(int type)
    {
        if (type == 0)
        {
            this.abnormalType = AbnormalType.NORMAL;
        }
        else if (type == 1)
        {
            this.abnormalType = AbnormalType.TYPE_I;
        }
        else if (type == 2)
        {
            this.abnormalType = AbnormalType.TYPE_JUMPER;
        }
        else if (type == 3)
        {
            this.abnormalType = AbnormalType.TYPE_CRAWLER;
        }
        if ((this.abnormalType == AbnormalType.TYPE_I) || (this.abnormalType == AbnormalType.TYPE_JUMPER))
        {
            this.speed = 18f;
            if (this.myLevel > 1f)
            {
                this.speed *= Mathf.Sqrt(this.myLevel);
            }
            if (this.myDifficulty == 1)
            {
                this.speed *= 1.4f;
            }
            if (this.myDifficulty == 2)
            {
                this.speed *= 1.6f;
            }
            base.animation["turnaround1"].speed = 2f;
            base.animation["turnaround2"].speed = 2f;
        }
        if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            this.chaseDistance += 50f;
            this.speed = 25f;
            if (this.myLevel > 1f)
            {
                this.speed *= Mathf.Sqrt(this.myLevel);
            }
            if (this.myDifficulty == 1)
            {
                this.speed *= 2f;
            }
            if (this.myDifficulty == 2)
            {
                this.speed *= 2.2f;
            }
            base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().height = 10f;
            base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().radius = 5f;
            base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0f, 5.05f, 0f);
            base.transform.localScale = new Vector3(this.myLevel, this.myLevel, this.myLevel);
        }
        this.idle(0f);
    }

    [RPC]
    private void netSetLevel(float level, int AI)
    {
        this.setLevel(level, AI);
    }

    private void OnCollisionStay()
    {
        this.grounded = true;
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

    private IEnumerator regenerateHealth()
    {
        while (!this.hasDie)
        {
            if (this.health < this.maxhealth)
            {
                this.health += 150;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    [RPC]
    private void removeMe()
    {
        Network.RemoveRPCs(base.networkView.viewID);
        UnityEngine.Object.Destroy(base.gameObject);
    }

    public void setAbnormalType(AbnormalType type)
    {
        int num = 0;
        if (type == AbnormalType.NORMAL)
        {
            num = 0;
        }
        else if (type == AbnormalType.TYPE_I)
        {
            num = 1;
        }
        else if (type == AbnormalType.TYPE_JUMPER)
        {
            num = 2;
        }
        else if (type == AbnormalType.TYPE_CRAWLER)
        {
            num = 3;
        }
        if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
        {
            object[] args = new object[] { num };
            base.networkView.RPC("netSetAbnormalType", RPCMode.AllBuffered, args);
        }
        else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            if (IN_GAME_MAIN_CAMERA.difficulty == 2)
            {
                num = (UnityEngine.Random.Range((float) 0f, (float) 1f) <= 0.15f) ? 3 : 2;
            }
            this.netSetAbnormalType(num);
        }
    }

    private void setLevel(float level, int AI)
    {
        this.myLevel = level;
        this.myLevel = Mathf.Clamp(this.myLevel, 0.7f, 3f);
        this.attackWait += UnityEngine.Random.Range((float) 0f, (float) 2f);
        this.chaseDistance += this.myLevel * 10f;
        base.transform.localScale = new Vector3(this.myLevel, this.myLevel, this.myLevel);
        float num = 1.4f - ((this.myLevel - 0.7f) * 0.15f);
        num = Mathf.Clamp(num, 0.9f, 1.5f);
        foreach (AnimationState state in base.animation)
        {
            state.speed = num;
        }
        Rigidbody rigidbody = base.rigidbody;
        rigidbody.mass *= this.myLevel;
        base.rigidbody.rotation = Quaternion.Euler(0f, (float) UnityEngine.Random.Range(0, 360), 0f);
        if (this.myLevel > 1f)
        {
            this.speed *= Mathf.Sqrt(this.myLevel);
        }
        this.myDifficulty = AI;
        if ((this.myDifficulty == 1) || (this.myDifficulty == 2))
        {
            foreach (AnimationState state2 in base.animation)
            {
                state2.speed = num * 1.05f;
            }
            this.speed *= 1.4f;
            this.chaseDistance *= 1.15f;
        }
        if (this.myDifficulty == 2)
        {
            foreach (AnimationState state3 in base.animation)
            {
                state3.speed = num * 1.05f;
            }
            this.speed *= 1.6f;
            this.chaseDistance *= 1.3f;
        }
        if ((IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN) || (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE))
        {
            this.chaseDistance = 999999f;
        }
        this.attackDistance = Vector3.Distance(base.transform.position, base.transform.Find("ap_front_ground").position) * 1.65f;
    }

    private void setmyLevel()
    {
        base.animation.cullingType = AnimationCullingType.BasedOnRenderers;
        if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
        {
            object[] args = new object[] { this.myLevel, GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().difficulty };
            base.networkView.RPC("netSetLevel", RPCMode.AllBuffered, args);
            base.animation.cullingType = AnimationCullingType.AlwaysAnimate;
        }
        else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            this.setLevel(this.myLevel, IN_GAME_MAIN_CAMERA.difficulty);
        }
    }

    public void setRoute(GameObject route)
    {
        this.checkPoints = new ArrayList();
        for (int i = 1; i <= 10; i++)
        {
            this.checkPoints.Add(route.transform.Find("r" + i).position);
            MonoBehaviour.print(this.checkPoints[0]);
        }
        this.checkPoints.Add("end");
    }

    private void Start()
    {
        if (FengMultiplayerScript.isBigTotons)
        {
            this.myLevel = UnityEngine.Random.Range((float)0.5f, (float)6f);
        }
        else
        {
            this.myLevel = UnityEngine.Random.Range((float)0.5f, (float)3.5f);
        }
        this.grabTF = new GameObject();
        this.grabTF.name = "titansTmpGrabTF";
        this.currentCamera = GameObject.Find("MainCamera");
        this.setmyLevel();
        if (FengMultiplayerScript.isTitanHpEnabled)
        {
            float num = (this.myLevel <= 1f) ? 0f : this.myLevel;
            this.health = ((int)((60f * num) * num)) + 1;
            this.maxhealth = this.health;
            if (FengMultiplayerScript.isTitanRegenEnabled)
            {
                base.StartCoroutine(this.regenerateHealth());
            }
        }
        this.setAbnormalType(this.abnormalType);
        if (this.myHero == null)
        {
            this.findNearestHero();
        }
        this.spawnPt = base.transform.position;
    }

    [RPC]
    public void titanGetHit(NetworkViewID player, int speed)
    {
        bool flag = true;
        if (base.networkView.isMine)
        {
            if (FengMultiplayerScript.isTitanHpEnabled)
            {
                flag = false;
                this.health -= speed;
                if (this.health > 0)
                {
                    if (player.isMine)
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().netShowDamage(speed);
                    }
                    else
                    {
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().networkView.RPC("netShowDamage", player.owner, new object[] { speed });
                    }
                }
                else
                {
                    flag = true;
                }
            }
            if (flag)
            {
                Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                Vector3 vector = NetworkView.Find(player).gameObject.transform.position - transform.transform.position;
                if ((vector.magnitude < 10f) && !this.hasDie)
                {
                    base.networkView.RPC("netDie", RPCMode.OthersBuffered, new object[0]);
                    if (this.grabbedTarget != null)
                    {
                        this.grabbedTarget.networkView.RPC("netUngrabbed", RPCMode.All, new object[0]);
                    }
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
        }
    }

    public void toCheckPoint(Vector3 targetPt, float r)
    {
        this.state = "toCheckPoint";
        this.targetCheckPt = targetPt;
        this.targetR = r;
        if (this.abnormalType == AbnormalType.NORMAL)
        {
            this.crossFade("run_walk", 0.5f);
        }
        else if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            this.crossFade("crawler_run", 0.5f);
        }
        else
        {
            this.crossFade("run_abnormal", 0.5f);
        }
    }

    private void turn(float d)
    {
        if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            if (d > 0f)
            {
                this.turnAnimation = "crawler_turnaround_R";
            }
            else
            {
                this.turnAnimation = "crawler_turnaround_L";
            }
        }
        else if (d > 0f)
        {
            this.turnAnimation = "turnaround2";
        }
        else
        {
            this.turnAnimation = "turnaround1";
        }
        this.playAnimation(this.turnAnimation);
        base.animation[this.turnAnimation].time = 0f;
        d = Mathf.Clamp(d, -120f, 120f);
        this.turnDeg = d;
        this.desDeg = base.gameObject.transform.rotation.eulerAngles.y + this.turnDeg;
        this.state = "turn";
    }

    private void Update()
    {
        if (((!IN_GAME_MAIN_CAMERA.isPausing || (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)) && (this.myDifficulty >= 0)) && ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine))
        {
            if ((this.activeRad < 0x7fffffff) && (((this.state == "idle") || (this.state == "wander")) || (this.state == "chase")))
            {
                if (this.checkPoints.Count > 1)
                {
                    if (Vector3.Distance((Vector3) this.checkPoints[0], base.transform.position) > this.activeRad)
                    {
                        this.toCheckPoint((Vector3) this.checkPoints[0], 10f);
                    }
                }
                else if (Vector3.Distance(this.spawnPt, base.transform.position) > this.activeRad)
                {
                    this.toCheckPoint(this.spawnPt, 10f);
                }
            }
            if (this.whoHasTauntMe != null)
            {
                this.tauntTime -= Time.deltaTime;
                if (this.tauntTime <= 0f)
                {
                    this.whoHasTauntMe = null;
                }
                this.myHero = this.whoHasTauntMe;
            }
            if (this.hasDie)
            {
                this.dieTime += Time.deltaTime;
                if ((this.dieTime > 2f) && !this.hasDieSteam)
                {
                    this.hasDieSteam = true;
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    {
                        GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie1"));
                        obj2.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
                        obj2.transform.localScale = base.transform.localScale;
                    }
                    else if (base.networkView.isMine)
                    {
                        GameObject obj3 = (GameObject) Network.Instantiate(Resources.Load("FX/FXtitanDie1"), base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0);
                        obj3.transform.localScale = base.transform.localScale;
                    }
                }
                if (this.dieTime > 5f)
                {
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    {
                        GameObject obj4 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie"));
                        obj4.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
                        obj4.transform.localScale = base.transform.localScale;
                        UnityEngine.Object.Destroy(base.gameObject);
                    }
                    else if (base.networkView.isMine)
                    {
                        GameObject obj5 = (GameObject) Network.Instantiate(Resources.Load("FX/FXtitanDie"), base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0);
                        obj5.transform.localScale = base.transform.localScale;
                        base.networkView.RPC("removeMe", RPCMode.AllBuffered, new object[0]);
                        this.myDifficulty = -1;
                    }
                }
            }
            else
            {
                if (this.state == "hit")
                {
                    if (this.hitPause > 0f)
                    {
                        this.hitPause -= Time.deltaTime;
                        if (this.hitPause <= 0f)
                        {
                            base.animation[this.hitAnimation].speed = 1f;
                            this.hitPause = 0f;
                        }
                    }
                    if (base.animation[this.hitAnimation].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                }
                if (this.myHero == null)
                {
                    this.findNearestHero();
                }
                else
                {
                    if ((((this.state == "idle") || (this.state == "chase")) || (this.state == "wander")) && ((this.whoHasTauntMe == null) && (UnityEngine.Random.Range(0, 100) < 10)))
                    {
                        this.findNearestFacingHero();
                    }
                    this.myDistance = Mathf.Sqrt(((this.myHero.transform.position.x - base.transform.position.x) * (this.myHero.transform.position.x - base.transform.position.x)) + ((this.myHero.transform.position.z - base.transform.position.z) * (this.myHero.transform.position.z - base.transform.position.z)));
                    if (this.state == "laugh")
                    {
                        if (base.animation["laugh"].normalizedTime >= 1f)
                        {
                            this.idle(2f);
                        }
                    }
                    else
                    {
                        string str3;
                        Dictionary<string, int> dictionary;
                        int num19;
                        if (this.state == "idle")
                        {
                            if (this.sbtime > 0f)
                            {
                                this.sbtime -= Time.deltaTime;
                            }
                            else
                            {
                                if (!this.isAlarm)
                                {
                                    if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.02f)
                                    {
                                        this.wander(0f);
                                        return;
                                    }
                                    if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.01f)
                                    {
                                        this.turn((float) UnityEngine.Random.Range(30, 120));
                                        return;
                                    }
                                    if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.01f)
                                    {
                                        this.turn((float) UnityEngine.Random.Range(-30, -120));
                                        return;
                                    }
                                }
                                this.angle = 0f;
                                this.between2 = 0f;
                                if ((this.myDistance < this.chaseDistance) || (this.whoHasTauntMe != null))
                                {
                                    Vector3 vector = this.myHero.transform.position - base.transform.position;
                                    this.angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                                    this.between2 = -Mathf.DeltaAngle(this.angle, base.gameObject.transform.rotation.eulerAngles.y - 90f);
                                    if (this.myDistance >= this.attackDistance)
                                    {
                                        if (this.isAlarm || (Mathf.Abs(this.between2) < 90f))
                                        {
                                            this.chase();
                                            return;
                                        }
                                        if (!this.isAlarm && (this.myDistance < (this.chaseDistance * 0.1f)))
                                        {
                                            this.chase();
                                            return;
                                        }
                                    }
                                }
                                Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
                                Transform transform2 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                if (this.myDistance < this.chaseDistance)
                                {
                                    if (((this.abnormalType == AbnormalType.TYPE_JUMPER) && ((this.myDistance > this.attackDistance) || (this.myHero.transform.position.y > (transform.transform.position.y + (4f * this.myLevel))))) && ((Mathf.Abs(this.between2) < 120f) && (Vector3.Distance(base.transform.position, this.myHero.transform.position) < (1.5f * this.myHero.transform.position.y))))
                                    {
                                        this.attack("abnormal_jump_bite0");
                                        return;
                                    }
                                    if ((((this.abnormalType == AbnormalType.TYPE_CRAWLER) && (this.myDistance < (this.attackDistance * 3f))) && ((Mathf.Abs(this.between2) < 90f) && (this.myHero.transform.position.y < (transform2.position.y + (30f * this.myLevel))))) && (this.myHero.transform.position.y > (transform2.position.y + (10f * this.myLevel))))
                                    {
                                        this.attack("crawler_jump_0");
                                        return;
                                    }
                                }
                                if (this.myDistance < this.attackDistance)
                                {
                                    if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
                                    {
                                        if (((this.myHero.transform.position.y + 3f) <= (transform2.position.y + (20f * this.myLevel))) && (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.1f))
                                        {
                                            this.chase();
                                        }
                                    }
                                    else
                                    {
                                        string str = string.Empty;
                                        string[] attackStrategy = this.GetAttackStrategy();
                                        if (attackStrategy != null)
                                        {
                                            str = attackStrategy[UnityEngine.Random.Range(0, attackStrategy.Length)];
                                        }
                                        if (((this.abnormalType == AbnormalType.TYPE_JUMPER) || (this.abnormalType == AbnormalType.TYPE_I)) && (Mathf.Abs(this.between2) > 40f))
                                        {
                                            if ((str.Contains("grab") || str.Contains("kick")) || (str.Contains("slap") || str.Contains("bite")))
                                            {
                                                if (UnityEngine.Random.Range(0, 100) < 30)
                                                {
                                                    this.turn(this.between2);
                                                    return;
                                                }
                                            }
                                            else if (UnityEngine.Random.Range(0, 100) < 90)
                                            {
                                                this.turn(this.between2);
                                                return;
                                            }
                                        }
                                        str3 = str;
                                        if (str3 != null)
                                        {
                                            if (SWITCHSMAP4 == null)
                                            {
                                                dictionary = new Dictionary<string, int>(0x12);
                                                dictionary.Add("grab_ground_front_l", 0);
                                                dictionary.Add("grab_ground_front_r", 1);
                                                dictionary.Add("grab_ground_back_l", 2);
                                                dictionary.Add("grab_ground_back_r", 3);
                                                dictionary.Add("grab_head_front_l", 4);
                                                dictionary.Add("grab_head_front_r", 5);
                                                dictionary.Add("grab_head_back_l", 6);
                                                dictionary.Add("grab_head_back_r", 7);
                                                dictionary.Add("attack_abnormal_jump", 8);
                                                dictionary.Add("attack_combo", 9);
                                                dictionary.Add("attack_front_ground", 10);
                                                dictionary.Add("attack_kick", 11);
                                                dictionary.Add("attack_slap_back", 12);
                                                dictionary.Add("attack_slap_face", 13);
                                                dictionary.Add("attack_stomp", 14);
                                                dictionary.Add("attack_bite", 15);
                                                dictionary.Add("attack_bite_l", 0x10);
                                                dictionary.Add("attack_bite_r", 0x11);
                                                SWITCHSMAP4 = dictionary;
                                            }
                                            if (SWITCHSMAP4.TryGetValue(str3, out num19))
                                            {
                                                switch (num19)
                                                {
                                                    case 0:
                                                        this.grab("ground_front_l");
                                                        return;

                                                    case 1:
                                                        this.grab("ground_front_r");
                                                        return;

                                                    case 2:
                                                        this.grab("ground_back_l");
                                                        return;

                                                    case 3:
                                                        this.grab("ground_back_r");
                                                        return;

                                                    case 4:
                                                        this.grab("head_front_l");
                                                        return;

                                                    case 5:
                                                        this.grab("head_front_r");
                                                        return;

                                                    case 6:
                                                        this.grab("head_back_l");
                                                        return;

                                                    case 7:
                                                        this.grab("head_back_r");
                                                        return;

                                                    case 8:
                                                        this.attack("abnormal_jump");
                                                        return;

                                                    case 9:
                                                        this.attack("combo");
                                                        return;

                                                    case 10:
                                                        this.attack("front_ground");
                                                        return;

                                                    case 11:
                                                        this.attack("kick");
                                                        return;

                                                    case 12:
                                                        this.attack("slap_back");
                                                        return;

                                                    case 13:
                                                        this.attack("slap_face");
                                                        return;

                                                    case 14:
                                                        this.attack("stomp");
                                                        return;

                                                    case 15:
                                                        this.attack("bite");
                                                        return;

                                                    case 0x10:
                                                        this.attack("bite_l");
                                                        return;

                                                    case 0x11:
                                                        this.attack("bite_r");
                                                        return;
                                                }
                                            }
                                        }
                                        if (this.abnormalType != AbnormalType.NORMAL)
                                        {
                                            if (Mathf.Abs(this.between2) > 45f)
                                            {
                                                this.turn(this.between2);
                                            }
                                        }
                                        else if ((UnityEngine.Random.Range(0, 100) < 30) && (Mathf.Abs(this.between2) > 45f))
                                        {
                                            this.turn(this.between2);
                                        }
                                    }
                                }
                            }
                        }
                        else if (this.state == "attack")
                        {
                            if (this.attackAnimation == "combo")
                            {
                                if ((base.animation["attack_" + this.attackAnimation].normalizedTime >= 0.13f) && (base.animation["attack_" + this.attackAnimation].normalizedTime <= 0.2f))
                                {
                                    GameObject obj6 = this.checkIfHitHand(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001"));
                                    if (obj6 != null)
                                    {
                                        Vector3 position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                                        {
                                            obj6.GetComponent<HERO>().die((Vector3) (((obj6.transform.position - position) * 15f) * this.myLevel), false);
                                        }
                                        else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) && !obj6.GetComponent<HERO>().HasDied())
                                        {
                                            obj6.GetComponent<HERO>().markDie();
                                            object[] args = new object[] { (Vector3) (((obj6.transform.position - position) * 15f) * this.myLevel), false };
                                            obj6.GetComponent<HERO>().networkView.RPC("netDie", RPCMode.All, args);
                                        }
                                    }
                                }
                                if ((base.animation["attack_" + this.attackAnimation].normalizedTime >= 0.31f) && (base.animation["attack_" + this.attackAnimation].normalizedTime <= 0.4f))
                                {
                                    GameObject obj7 = this.checkIfHitHand(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001"));
                                    if (obj7 != null)
                                    {
                                        Vector3 vector3 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                                        {
                                            obj7.GetComponent<HERO>().die((Vector3) (((obj7.transform.position - vector3) * 15f) * this.myLevel), false);
                                        }
                                        else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) && !obj7.GetComponent<HERO>().HasDied())
                                        {
                                            obj7.GetComponent<HERO>().markDie();
                                            object[] objArray2 = new object[] { (Vector3) (((obj7.transform.position - vector3) * 15f) * this.myLevel), false };
                                            obj7.GetComponent<HERO>().networkView.RPC("netDie", RPCMode.All, objArray2);
                                        }
                                    }
                                }
                            }
                            if ((!this.attacked && (this.attackCheckTime != 0f)) && (base.animation["attack_" + this.attackAnimation].normalizedTime >= this.attackCheckTime))
                            {
                                GameObject obj8;
                                this.attacked = true;
                                this.fxPosition = base.transform.Find("ap_" + this.attackAnimation).position;
                                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                                {
                                    obj8 = (GameObject) Network.Instantiate(Resources.Load("FX/" + this.fxName), this.fxPosition, this.fxRotation, 0);
                                }
                                else
                                {
                                    obj8 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("FX/" + this.fxName), this.fxPosition, this.fxRotation);
                                }
                                obj8.transform.localScale = base.transform.localScale;
                                float b = 1f - (Vector3.Distance(this.currentCamera.transform.position, obj8.transform.position) * 0.05f);
                                b = Mathf.Min(1f, b);
                                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b, b, 0.95f);
                            }
                            if ((this.attackAnimation == "abnormal_jump_bite0") || (this.attackAnimation == "crawler_jump_0"))
                            {
                                if (!this.attacked && (base.animation["attack_" + this.attackAnimation].normalizedTime >= 0.68f))
                                {
                                    this.attacked = true;
                                    float y = this.myHero.rigidbody.velocity.y;
                                    float num3 = -20f;
                                    float gravity = this.gravity;
                                    float num5 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position.y;
                                    float num6 = (num3 - gravity) * 0.5f;
                                    float num7 = y;
                                    float num8 = this.myHero.transform.position.y - num5;
                                    float num9 = Mathf.Abs((float) ((Mathf.Sqrt((num7 * num7) - ((4f * num6) * num8)) - num7) / (2f * num6)));
                                    Vector3 vector4 = (Vector3) ((this.myHero.transform.position + (this.myHero.rigidbody.velocity * num9)) + ((((Vector3.up * 0.5f) * num3) * num9) * num9));
                                    float num10 = vector4.y;
                                    if ((num8 < 0f) || ((num10 - num5) < 0f))
                                    {
                                        this.idle(0f);
                                        num9 = 0.5f;
                                        vector4 = base.transform.position + ((Vector3) ((num5 + 5f) * Vector3.up));
                                        num10 = vector4.y;
                                    }
                                    float num11 = num10 - num5;
                                    float num12 = Mathf.Sqrt((2f * num11) / this.gravity);
                                    float num13 = this.gravity * num12;
                                    Vector3 vector5 = (Vector3) ((vector4 - base.transform.position) / num9);
                                    this.abnorma_jump_bite_horizon_v = new Vector3(vector5.x, 0f, vector5.z);
                                    Vector3 velocity = base.rigidbody.velocity;
                                    Vector3 force = new Vector3(this.abnorma_jump_bite_horizon_v.x, velocity.y, this.abnorma_jump_bite_horizon_v.z) - velocity;
                                    base.rigidbody.AddForce(force, ForceMode.VelocityChange);
                                    base.rigidbody.AddForce((Vector3) (Vector3.up * num13), ForceMode.VelocityChange);
                                    float num14 = Vector2.Angle(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(this.myHero.transform.position.x, this.myHero.transform.position.z));
                                    num14 = Mathf.Atan2(this.myHero.transform.position.x - base.transform.position.x, this.myHero.transform.position.z - base.transform.position.z) * 57.29578f;
                                    base.gameObject.transform.rotation = Quaternion.Euler(0f, num14, 0f);
                                }
                                if (base.animation["attack_" + this.attackAnimation].normalizedTime >= 1f)
                                {
                                    Debug.DrawLine(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + ((Vector3) ((Vector3.up * 1.5f) * this.myLevel)), (Vector3) ((base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + ((Vector3.up * 1.5f) * this.myLevel)) + ((Vector3.up * 3f) * this.myLevel)), Color.green);
                                    Debug.DrawLine(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + ((Vector3) ((Vector3.up * 1.5f) * this.myLevel)), (Vector3) ((base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + ((Vector3.up * 1.5f) * this.myLevel)) + ((Vector3.forward * 3f) * this.myLevel)), Color.green);
                                    GameObject obj9 = this.checkIfHitHead(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head"), 3f);
                                    if (obj9 != null)
                                    {
                                        Vector3 vector8 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                                        {
                                            obj9.GetComponent<HERO>().die((Vector3) (((obj9.transform.position - vector8) * 15f) * this.myLevel), false);
                                        }
                                        else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) && !obj9.GetComponent<HERO>().HasDied())
                                        {
                                            obj9.GetComponent<HERO>().markDie();
                                            object[] objArray3 = new object[] { (Vector3) (((obj9.transform.position - vector8) * 15f) * this.myLevel), true };
                                            obj9.GetComponent<HERO>().networkView.RPC("netDie", RPCMode.All, objArray3);
                                        }
                                        if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
                                        {
                                            this.attackAnimation = "crawler_jump_1";
                                        }
                                        else
                                        {
                                            this.attackAnimation = "abnormal_jump_bite1";
                                        }
                                        this.playAnimation("attack_" + this.attackAnimation);
                                    }
                                    if (((Mathf.Abs(base.rigidbody.velocity.y) < 0.5f) || (base.rigidbody.velocity.y < 0f)) || this.IsGrounded())
                                    {
                                        if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
                                        {
                                            this.attackAnimation = "crawler_jump_1";
                                        }
                                        else
                                        {
                                            this.attackAnimation = "abnormal_jump_bite1";
                                        }
                                        this.playAnimation("attack_" + this.attackAnimation);
                                    }
                                }
                            }
                            else if ((this.attackAnimation == "abnormal_jump_bite1") || (this.attackAnimation == "crawler_jump_1"))
                            {
                                if ((base.animation["attack_" + this.attackAnimation].normalizedTime >= 1f) && this.grounded)
                                {
                                    if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
                                    {
                                        this.attackAnimation = "crawler_jump_2";
                                    }
                                    else
                                    {
                                        this.attackAnimation = "abnormal_jump_bite2";
                                    }
                                    this.crossFade("attack_" + this.attackAnimation, 0.1f);
                                }
                            }
                            else if ((this.attackAnimation == "abnormal_jump_bite2") || (this.attackAnimation == "crawler_jump_2"))
                            {
                                if (base.animation["attack_" + this.attackAnimation].normalizedTime >= 1f)
                                {
                                    this.idle(0f);
                                }
                            }
                            else if (base.animation.IsPlaying("tired"))
                            {
                                if (base.animation["tired"].normalizedTime >= (1f + Mathf.Max((float) (this.attackEndWait * 2f), (float) 3f)))
                                {
                                    this.idle(UnityEngine.Random.Range((float) (this.attackWait - 1f), (float) 3f));
                                }
                            }
                            else if (base.animation["attack_" + this.attackAnimation].normalizedTime >= (1f + this.attackEndWait))
                            {
                                if (this.nextAttackAnimation != null)
                                {
                                    this.attack(this.nextAttackAnimation);
                                }
                                else if ((this.abnormalType == AbnormalType.TYPE_I) || (this.abnormalType == AbnormalType.TYPE_JUMPER))
                                {
                                    this.attackCount++;
                                    if ((this.attackCount > 3) && (this.attackAnimation == "abnormal_getup"))
                                    {
                                        this.attackCount = 0;
                                        this.crossFade("tired", 0.5f);
                                    }
                                    else
                                    {
                                        this.idle(UnityEngine.Random.Range((float) (this.attackWait - 1f), (float) 3f));
                                    }
                                }
                                else
                                {
                                    this.idle(UnityEngine.Random.Range((float) (this.attackWait - 1f), (float) 3f));
                                }
                            }
                        }
                        else if (this.state == "grab")
                        {
                            if (((base.animation["grab_" + this.attackAnimation].normalizedTime >= this.attackCheckTimeA) && (base.animation["grab_" + this.attackAnimation].normalizedTime <= this.attackCheckTimeB)) && (this.grabbedTarget == null))
                            {
                                GameObject grabTarget = this.checkIfHitHand(this.currentGrabHand);
                                if (grabTarget != null)
                                {
                                    if (this.isGrabHandLeft)
                                    {
                                        this.eatSetL(grabTarget);
                                        this.grabbedTarget = grabTarget;
                                    }
                                    else
                                    {
                                        this.eatSet(grabTarget);
                                        this.grabbedTarget = grabTarget;
                                    }
                                }
                            }
                            if (base.animation["grab_" + this.attackAnimation].normalizedTime >= 1f)
                            {
                                if (this.grabbedTarget != null)
                                {
                                    this.eat();
                                }
                                else
                                {
                                    this.idle(UnityEngine.Random.Range((float) (this.attackWait - 1f), (float) 2f));
                                }
                            }
                        }
                        else if (this.state == "eat")
                        {
                            if (!this.attacked && (base.animation[this.attackAnimation].normalizedTime >= 0.48f))
                            {
                                this.attacked = true;
                                this.justEatHero(this.grabbedTarget, this.currentGrabHand);
                            }
                            if ((this.grabbedTarget == null) && (base.animation[this.attackAnimation].normalizedTime > 0.385f))
                            {
                                this.idle(0f);
                            }
                            if (base.animation[this.attackAnimation].normalizedTime >= 1f)
                            {
                                this.idle(0f);
                            }
                        }
                        else if (this.state == "chase")
                        {
                            if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
                            {
                                Vector3 vector9 = this.myHero.transform.position - base.transform.position;
                                float current = -Mathf.Atan2(vector9.z, vector9.x) * 57.29578f;
                                float f = -Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
                                Transform transform3 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                                if ((((this.myDistance < (this.attackDistance * 3f)) && (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.1f)) && ((Mathf.Abs(f) < 90f) && (this.myHero.transform.position.y < (transform3.position.y + (30f * this.myLevel))))) && (this.myHero.transform.position.y > (transform3.position.y + (10f * this.myLevel))))
                                {
                                    this.attack("crawler_jump_0");
                                }
                                else
                                {
                                    GameObject obj11 = this.checkIfHitCrawlerMouth(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head"), 2.2f);
                                    if (obj11 != null)
                                    {
                                        Vector3 vector10 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
                                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                                        {
                                            obj11.GetComponent<HERO>().die((Vector3) (((obj11.transform.position - vector10) * 15f) * this.myLevel), false);
                                        }
                                        else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER) && !obj11.GetComponent<HERO>().HasDied())
                                        {
                                            obj11.GetComponent<HERO>().markDie();
                                            object[] objArray4 = new object[] { (Vector3) (((obj11.transform.position - vector10) * 15f) * this.myLevel), true };
                                            obj11.GetComponent<HERO>().networkView.RPC("netDie", RPCMode.All, objArray4);
                                        }
                                    }
                                    if ((this.myDistance < this.attackDistance) && (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.02f))
                                    {
                                        this.idle(UnityEngine.Random.Range((float) 0.05f, (float) 0.2f));
                                    }
                                }
                            }
                            else
                            {
                                if (this.abnormalType == AbnormalType.TYPE_JUMPER)
                                {
                                    Transform transform4 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
                                    if ((((this.myDistance > this.attackDistance) && (this.myHero.transform.position.y > (transform4.transform.position.y + (4f * this.myLevel)))) || (this.myHero.transform.position.y > (transform4.transform.position.y + (4f * this.myLevel)))) && (Vector3.Distance(base.transform.position, this.myHero.transform.position) < (1.5f * this.myHero.transform.position.y)))
                                    {
                                        this.attack("abnormal_jump_bite0");
                                        return;
                                    }
                                }
                                if (this.myDistance < this.attackDistance)
                                {
                                    this.idle(UnityEngine.Random.Range((float) 0.05f, (float) 0.2f));
                                }
                            }
                        }
                        else if (this.state == "wander")
                        {
                            float num17 = 0f;
                            float num18 = 0f;
                            if ((this.myDistance < this.chaseDistance) || (this.whoHasTauntMe != null))
                            {
                                Vector3 vector11 = this.myHero.transform.position - base.transform.position;
                                num17 = -Mathf.Atan2(vector11.z, vector11.x) * 57.29578f;
                                num18 = -Mathf.DeltaAngle(num17, base.gameObject.transform.rotation.eulerAngles.y - 90f);
                                if (this.isAlarm || (Mathf.Abs(num18) < 90f))
                                {
                                    this.chase();
                                    return;
                                }
                                if (!this.isAlarm && (this.myDistance < (this.chaseDistance * 0.1f)))
                                {
                                    this.chase();
                                    return;
                                }
                            }
                            if (UnityEngine.Random.Range((float) 0f, (float) 1f) < 0.01f)
                            {
                                this.idle(0f);
                            }
                        }
                        else if (this.state == "turn")
                        {
                            base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.desDeg, 0f), (Time.deltaTime * Mathf.Abs(this.turnDeg)) * 0.015f);
                            if (base.animation[this.turnAnimation].normalizedTime >= 1f)
                            {
                                this.idle(0f);
                            }
                        }
                        else if (this.state == "hit_eye")
                        {
                            if (base.animation["hit_eye"].normalizedTime >= 1f)
                            {
                                this.attack("combo");
                            }
                        }
                        else if (this.state == "toCheckPoint")
                        {
                            if ((this.checkPoints.Count <= 0) && (this.myDistance < this.attackDistance))
                            {
                                string str2 = string.Empty;
                                string[] strArray2 = this.GetAttackStrategy();
                                if (strArray2 != null)
                                {
                                    str2 = strArray2[UnityEngine.Random.Range(0, strArray2.Length)];
                                }
                                str3 = str2;
                                if (str3 != null)
                                {
                                    if (SWITCHSMAP5 == null)
                                    {
                                        dictionary = new Dictionary<string, int>(0x12);
                                        dictionary.Add("grab_ground_front_l", 0);
                                        dictionary.Add("grab_ground_front_r", 1);
                                        dictionary.Add("grab_ground_back_l", 2);
                                        dictionary.Add("grab_ground_back_r", 3);
                                        dictionary.Add("grab_head_front_l", 4);
                                        dictionary.Add("grab_head_front_r", 5);
                                        dictionary.Add("grab_head_back_l", 6);
                                        dictionary.Add("grab_head_back_r", 7);
                                        dictionary.Add("attack_abnormal_jump", 8);
                                        dictionary.Add("attack_combo", 9);
                                        dictionary.Add("attack_front_ground", 10);
                                        dictionary.Add("attack_kick", 11);
                                        dictionary.Add("attack_slap_back", 12);
                                        dictionary.Add("attack_slap_face", 13);
                                        dictionary.Add("attack_stomp", 14);
                                        dictionary.Add("attack_bite", 15);
                                        dictionary.Add("attack_bite_l", 0x10);
                                        dictionary.Add("attack_bite_r", 0x11);
                                        SWITCHSMAP5 = dictionary;
                                    }
                                    if (SWITCHSMAP5.TryGetValue(str3, out num19))
                                    {
                                        switch (num19)
                                        {
                                            case 0:
                                                this.grab("ground_front_l");
                                                return;

                                            case 1:
                                                this.grab("ground_front_r");
                                                return;

                                            case 2:
                                                this.grab("ground_back_l");
                                                return;

                                            case 3:
                                                this.grab("ground_back_r");
                                                return;

                                            case 4:
                                                this.grab("head_front_l");
                                                return;

                                            case 5:
                                                this.grab("head_front_r");
                                                return;

                                            case 6:
                                                this.grab("head_back_l");
                                                return;

                                            case 7:
                                                this.grab("head_back_r");
                                                return;

                                            case 8:
                                                this.attack("abnormal_jump");
                                                return;

                                            case 9:
                                                this.attack("combo");
                                                return;

                                            case 10:
                                                this.attack("front_ground");
                                                return;

                                            case 11:
                                                this.attack("kick");
                                                return;

                                            case 12:
                                                this.attack("slap_back");
                                                return;

                                            case 13:
                                                this.attack("slap_face");
                                                return;

                                            case 14:
                                                this.attack("stomp");
                                                return;

                                            case 15:
                                                this.attack("bite");
                                                return;

                                            case 0x10:
                                                this.attack("bite_l");
                                                return;

                                            case 0x11:
                                                this.attack("bite_r");
                                                return;
                                        }
                                    }
                                }
                            }
                            if (Vector3.Distance(base.transform.position, this.targetCheckPt) < this.targetR)
                            {
                                if (this.checkPoints.Count > 0)
                                {
                                    MonoBehaviour.print("close TO TARGET, CHECK POINT LEFT:" + this.checkPoints.Count);
                                    if (this.checkPoints.Count == 1)
                                    {
                                        if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
                                        {
                                            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().gameLose();
                                            this.checkPoints = new ArrayList();
                                            this.idle(0f);
                                        }
                                    }
                                    else
                                    {
                                        if (this.checkPoints.Count == 4)
                                        {
                                            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().sendChatContentInfo("[FF0000]*WARNING!* An abnormal titan is approaching the north gate![-]");
                                        }
                                        Vector3 vector12 = (Vector3) this.checkPoints[0];
                                        this.targetCheckPt = vector12;
                                        this.checkPoints.RemoveAt(0);
                                    }
                                }
                                else
                                {
                                    this.idle(0f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void wander(float sbtime = 0f)
    {
        this.state = "wander";
        if (this.abnormalType == AbnormalType.NORMAL)
        {
            this.crossFade("run_walk", 0.5f);
        }
        else if (this.abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            this.crossFade("crawler_run", 0.5f);
        }
        else
        {
            this.crossFade("run_walk", 0.5f);
        }
    }
}

