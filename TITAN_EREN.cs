﻿using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class TITAN_EREN : MonoBehaviour
{
    private string attackAnimation;
    private Transform attackBox;
    private bool attackChkOnce;
    public GameObject bottomObject;
    public bool canJump = true;
    private ArrayList checkPoints = new ArrayList();
    public Camera currentCamera;
    private Vector3 dashDirection;
    private float dieTime;
    private float facingDirection;
    private float gravity = 500f;
    private bool grounded;
    public bool hasDied;
    private bool hasDieSteam;
    private string hitAnimation;
    private float hitPause;
    private ArrayList hitTargets;
    public FengCustomInputs inputManager;
    private bool isAttack;
    public bool isHit;
    private bool isHitWhileCarryingRock;
    private bool isNextAttack;
    private bool isPlayRoar;
    private bool isROCKMOVE;
    public float jumpHeight = 2f;
    private bool justGrounded;
    public float lifeTime = 9999f;
    private float lifeTimeMax = 9999f;
    public float maxVelocityChange = 100f;
    private GameObject myNetWorkName;
    private float myR;
    private bool needFreshCorePosition;
    private bool needRoar;
    private Vector3 oldCorePosition;
    public GameObject realBody;
    public GameObject rock;
    private bool rockHitGround;
    public bool rockLift;
    public int rockPhase;
    public float speed = 80f;
    private float sqrt2 = Mathf.Sqrt(2f);
    private int stepSoundPhase = 2;
    private Vector3 targetCheckPt;
    private float waitCounter;

    public void born()
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if (obj2.GetComponent<FEMALE_TITAN>() != null)
            {
                obj2.GetComponent<FEMALE_TITAN>().erenIsHere(base.gameObject);
            }
        }
        if (!this.bottomObject.GetComponent<CheckHitGround>().isGrounded)
        {
            this.playAnimation("jump_air");
            this.needRoar = true;
        }
        else
        {
            this.needRoar = false;
            this.playAnimation("born");
            this.isPlayRoar = false;
        }
        this.playSound("snd_eren_shift");
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            UnityEngine.Object.Instantiate(Resources.Load("FX/Thunder"), base.transform.position + ((Vector3) (Vector3.up * 23f)), Quaternion.Euler(270f, 0f, 0f));
        }
        else if (base.networkView.isMine)
        {
            Network.Instantiate(Resources.Load("FX/Thunder"), base.transform.position + ((Vector3) (Vector3.up * 23f)), Quaternion.Euler(270f, 0f, 0f), 0);
        }
        this.lifeTimeMax = this.lifeTime = 30f;
    }

    private void crossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
        if ((Network.peerType != NetworkPeerType.Disconnected) && base.networkView.isMine)
        {
            object[] args = new object[] { aniName, time };
            base.networkView.RPC("netCrossFade", RPCMode.Others, args);
        }
    }

    [RPC]
    private void endMovingRock()
    {
        this.isROCKMOVE = false;
    }
    private void falseAttack()
    {
        this.isAttack = false;
        this.isNextAttack = false;
        this.hitTargets = new ArrayList();
        this.attackChkOnce = false;
    }


    private void FixedUpdate()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing || (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE))
        {
            if (this.rockLift)
            {
                this.RockUpdate();
            }
            else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine)
            {
                if (this.hitPause > 0f)
                {
                    base.rigidbody.velocity = Vector3.zero;
                }
                else if (this.hasDied)
                {
                    base.rigidbody.velocity = Vector3.zero + ((Vector3)(Vector3.up * base.rigidbody.velocity.y));
                    base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
                }
                else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine)
                {
                    if (base.rigidbody.velocity.magnitude > 50f)
                    {
                        this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, base.rigidbody.velocity.magnitude), 0.1f);
                    }
                    else
                    {
                        this.currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
                    }
                    if (this.bottomObject.GetComponent<CheckHitGround>().isGrounded)
                    {
                        if (!this.grounded)
                        {
                            this.justGrounded = true;
                        }
                        this.grounded = true;
                        this.bottomObject.GetComponent<CheckHitGround>().isGrounded = false;
                    }
                    else
                    {
                        this.grounded = false;
                    }
                    float x = 0f;
                    float z = 0f;
                    if (!IN_GAME_MAIN_CAMERA.isTyping)
                    {
                        if (this.inputManager.isInput[0])
                        {
                            z = 1f;
                        }
                        else if (this.inputManager.isInput[1])
                        {
                            z = -1f;
                        }
                        else
                        {
                            z = 0f;
                        }
                        if (this.inputManager.isInput[2])
                        {
                            x = -1f;
                        }
                        else if (this.inputManager.isInput[3])
                        {
                            x = 1f;
                        }
                        else
                        {
                            x = 0f;
                        }
                    }
                    if (this.needFreshCorePosition)
                    {
                        this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
                        this.needFreshCorePosition = false;
                    }
                    if (this.isAttack || this.isHit)
                    {
                        Vector3 vector = (base.transform.position - base.transform.Find("Amarture/Core").position) - this.oldCorePosition;
                        this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
                        base.rigidbody.velocity = (Vector3)((vector / Time.deltaTime) + (Vector3.up * base.rigidbody.velocity.y));
                        base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
                        if (this.justGrounded)
                        {
                            this.justGrounded = false;
                        }
                    }
                    else if (this.grounded)
                    {
                        Vector3 zero = Vector3.zero;
                        if (this.justGrounded)
                        {
                            this.justGrounded = false;
                            zero = base.rigidbody.velocity;
                            if (base.animation.IsPlaying("jump_air"))
                            {
                                GameObject obj2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/boom2_eren"), base.transform.position, Quaternion.Euler(270f, 0f, 0f));
                                obj2.transform.localScale = (Vector3)(Vector3.one * 1.5f);
                                if (this.needRoar)
                                {
                                    this.playAnimation("born");
                                    this.needRoar = false;
                                    this.isPlayRoar = false;
                                }
                                else
                                {
                                    this.playAnimation("jump_land");
                                }
                            }
                        }
                        if ((!base.animation.IsPlaying("jump_land") && !this.isAttack) && (!this.isHit && !base.animation.IsPlaying("born")))
                        {
                            Vector3 vector3 = new Vector3(x, 0f, z);
                            float y = this.currentCamera.transform.rotation.eulerAngles.y;
                            float num6 = Mathf.Atan2(z, x) * 57.29578f;
                            num6 = -num6 + 90f;
                            float num3 = y + num6;
                            float num4 = -num3 + 90f;
                            float num7 = Mathf.Cos(num4 * 0.01745329f);
                            float num8 = Mathf.Sin(num4 * 0.01745329f);
                            zero = new Vector3(num7, 0f, num8);
                            float num9 = (vector3.magnitude <= 0.95f) ? ((vector3.magnitude >= 0.25f) ? vector3.magnitude : 0f) : 1f;
                            zero = (Vector3)(zero * num9);
                            zero = (Vector3)(zero * this.speed);
                            if ((x != 0f) || (z != 0f))
                            {
                                if ((!base.animation.IsPlaying("run") && !base.animation.IsPlaying("jump_start")) && !base.animation.IsPlaying("jump_air"))
                                {
                                    this.crossFade("run", 0.1f);
                                }
                            }
                            else
                            {
                                if (((!base.animation.IsPlaying("idle") && !base.animation.IsPlaying("dash_land")) && (!base.animation.IsPlaying("dodge") && !base.animation.IsPlaying("jump_start"))) && (!base.animation.IsPlaying("jump_air") && !base.animation.IsPlaying("jump_land")))
                                {
                                    this.crossFade("idle", 0.1f);
                                    zero = (Vector3)(zero * 0f);
                                }
                                num3 = -874f;
                            }
                            if (num3 != -874f)
                            {
                                this.facingDirection = num3;
                            }
                        }
                        Vector3 velocity = base.rigidbody.velocity;
                        Vector3 force = zero - velocity;
                        force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                        force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                        force.y = 0f;
                        if (base.animation.IsPlaying("jump_start") && (base.animation["jump_start"].normalizedTime >= 1f))
                        {
                            this.playAnimation("jump_air");
                            force.y += 240f;
                            MonoBehaviour.print("jumpspeeeeed");
                        }
                        else if (base.animation.IsPlaying("jump_start"))
                        {
                            force = -base.rigidbody.velocity;
                        }
                        base.rigidbody.AddForce(force, ForceMode.VelocityChange);
                        base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
                    }
                    else
                    {
                        if (base.animation.IsPlaying("jump_start") && (base.animation["jump_start"].normalizedTime >= 1f))
                        {
                            this.playAnimation("jump_air");
                            base.rigidbody.AddForce((Vector3)(Vector3.up * 240f), ForceMode.VelocityChange);
                        }
                        if (!base.animation.IsPlaying("jump") && !this.isHit)
                        {
                            Vector3 vector6 = new Vector3(x, 0f, z);
                            float num10 = this.currentCamera.transform.rotation.eulerAngles.y;
                            float num11 = Mathf.Atan2(z, x) * 57.29578f;
                            num11 = -num11 + 90f;
                            float num12 = num10 + num11;
                            float num13 = -num12 + 90f;
                            float num14 = Mathf.Cos(num13 * 0.01745329f);
                            float num15 = Mathf.Sin(num13 * 0.01745329f);
                            Vector3 vector7 = new Vector3(num14, 0f, num15);
                            float num16 = (vector6.magnitude <= 0.95f) ? ((vector6.magnitude >= 0.25f) ? vector6.magnitude : 0f) : 1f;
                            vector7 = (Vector3)(vector7 * num16);
                            vector7 = (Vector3)(vector7 * (this.speed * 2f));
                            if ((x != 0f) || (z != 0f))
                            {
                                base.rigidbody.AddForce(vector7, ForceMode.Impulse);
                            }
                            else
                            {
                                num12 = -874f;
                            }
                            if (num12 != -874f)
                            {
                                this.facingDirection = num12;
                            }
                            if ((!base.animation.IsPlaying(string.Empty) && !base.animation.IsPlaying("attack3_2")) && !base.animation.IsPlaying("attack5"))
                            {
                                base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 6f);
                            }
                        }
                    }
                    base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
                }
            }
        }
    }

    public void hitByFT(int phase)
    {
        if (!this.hasDied)
        {
            this.isHit = true;
            this.hitAnimation = "hit_annie_" + phase;
            this.falseAttack();
            this.playAnimation(this.hitAnimation);
            this.needFreshCorePosition = true;
            if (phase == 3)
            {
                GameObject obj2;
                this.hasDied = true;
                Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
                Transform transform2 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                {
                    obj2 = (GameObject) Network.Instantiate(Resources.Load("bloodExplore"), transform2.position + ((Vector3) ((Vector3.up * 1f) * 4f)), Quaternion.Euler(270f, 0f, 0f), 0);
                }
                else
                {
                    obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("bloodExplore"), transform2.position + ((Vector3) ((Vector3.up * 1f) * 4f)), Quaternion.Euler(270f, 0f, 0f));
                }
                obj2.transform.localScale = base.transform.localScale;
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                {
                    obj2 = (GameObject) Network.Instantiate(Resources.Load("bloodsplatter"), transform2.position, Quaternion.Euler(90f + transform2.rotation.eulerAngles.x, transform2.rotation.eulerAngles.y, transform2.rotation.eulerAngles.z), 0);
                }
                else
                {
                    obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("bloodsplatter"), transform2.position, Quaternion.Euler(90f + transform2.rotation.eulerAngles.x, transform2.rotation.eulerAngles.y, transform2.rotation.eulerAngles.z));
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
            }
        }
    }

    public void hitByFTByServer(int phase)
    {
        object[] args = new object[] { phase };
        base.networkView.RPC("hitByFTRPC", RPCMode.All, args);
    }

    [RPC]
    private void hitByFTRPC(int phase)
    {
        if (base.networkView.isMine)
        {
            this.hitByFT(phase);
        }
    }


    public void hitByTitan()
    {
        if ((!this.isHit && !this.hasDied) && !base.animation.IsPlaying("born"))
        {
            if (this.rockLift)
            {
                this.crossFade("die", 0.1f);
                this.isHitWhileCarryingRock = true;
                GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().gameLose();
                object[] args = new object[] { "set" };
                base.networkView.RPC("rockPlayAnimation", RPCMode.All, args);
            }
            else
            {
                this.isHit = true;
                this.hitAnimation = "hit_titan";
                this.falseAttack();
                this.playAnimation(this.hitAnimation);
                this.needFreshCorePosition = true;
            }
        }
    }

    public void hitByTitanByServer()
    {
        base.networkView.RPC("hitByTitanRPC", RPCMode.All, new object[0]);
    }

    [RPC]
    private void hitByTitanRPC()
    {
        if (base.networkView.isMine)
        {
            this.hitByTitan();
        }
    }

    public bool IsGrounded()
    {
        return this.bottomObject.GetComponent<CheckHitGround>().isGrounded;
    }

    private void LateUpdate()
    {
        if (((!IN_GAME_MAIN_CAMERA.isPausing || (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)) && !this.rockLift) && ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine))
        {
            Quaternion to = Quaternion.Euler(GameObject.Find("MainCamera").transform.rotation.eulerAngles.x, GameObject.Find("MainCamera").transform.rotation.eulerAngles.y, 0f);
            GameObject.Find("MainCamera").transform.rotation = Quaternion.Lerp(GameObject.Find("MainCamera").transform.rotation, to, Time.deltaTime * 2f);
        }
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
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
    private void netTauntAttack(float tauntTime, float distance = 100f)
    {
        foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("titan"))
        {
            if ((Vector3.Distance(obj2.transform.position, base.transform.position) < distance) && (obj2.GetComponent<TITAN>() != null))
            {
                obj2.GetComponent<TITAN>().beTauntedBy(base.gameObject, tauntTime);
            }
            if (obj2.GetComponent<FEMALE_TITAN>() != null)
            {
                obj2.GetComponent<FEMALE_TITAN>().erenIsHere(base.gameObject);
            }
        }
    }

    public void playAnimation(string aniName)
    {
        base.animation.Play(aniName);
        if ((Network.peerType != NetworkPeerType.Disconnected) && base.networkView.isMine)
        {
            object[] args = new object[] { aniName };
            base.networkView.RPC("netPlayAnimation", RPCMode.Others, args);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        base.animation.Play(aniName);
        base.animation[aniName].normalizedTime = normalizedTime;
        if ((Network.peerType != NetworkPeerType.Disconnected) && base.networkView.isMine)
        {
            object[] args = new object[] { aniName, normalizedTime };
            base.networkView.RPC("netPlayAnimationAt", RPCMode.Others, args);
        }
    }

    private void playSound(string sndname)
    {
        this.playsoundRPC(sndname);
        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
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
    [RPC]
    private void rockPlayAnimation(string anim)
    {
        this.rock.animation.Play(anim);
        this.rock.animation[anim].speed = 1f;
    }
    private void RockUpdate()
    {
        if (!this.isHitWhileCarryingRock)
        {
            if (this.isROCKMOVE)
            {
                this.rock.transform.position = base.transform.position;
                this.rock.transform.rotation = base.transform.rotation;
            }
            if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine)
            {
                if (this.rockPhase == 0)
                {
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 20f)
                    {
                        this.rockPhase++;
                        this.crossFade("idle", 1f);
                        this.waitCounter = 0f;
                        this.setRoute();
                    }
                }
                else if (this.rockPhase == 1)
                {
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 2f)
                    {
                        this.rockPhase++;
                        this.crossFade("run", 0.2f);
                        this.waitCounter = 0f;
                    }
                }
                else if (this.rockPhase == 2)
                {
                    Vector3 vector = (Vector3)(base.transform.forward * 30f);
                    Vector3 velocity = base.rigidbody.velocity;
                    Vector3 force = vector - velocity;
                    force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                    force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                    force.y = 0f;
                    base.rigidbody.AddForce(force, ForceMode.VelocityChange);
                    if (base.transform.position.z < -238f)
                    {
                        base.transform.position = new Vector3(base.transform.position.x, 0f, -238f);
                        this.rockPhase++;
                        this.crossFade("idle", 0.2f);
                        this.waitCounter = 0f;
                    }
                }
                else if (this.rockPhase == 3)
                {
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 1f)
                    {
                        this.rockPhase++;
                        this.crossFade("rock_lift", 0.1f);
                        object[] args = new object[] { "lift" };
                        base.networkView.RPC("rockPlayAnimation", RPCMode.All, args);
                        this.waitCounter = 0f;
                        this.targetCheckPt = (Vector3)this.checkPoints[0];
                    }
                }
                else if (this.rockPhase == 4)
                {
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
                    this.waitCounter += Time.deltaTime;
                    if (this.waitCounter > 4.2f)
                    {
                        this.rockPhase++;
                        this.crossFade("rock_walk", 0.1f);
                        object[] objArray3 = new object[] { "move" };
                        base.networkView.RPC("rockPlayAnimation", RPCMode.All, objArray3);
                        this.rock.animation["move"].normalizedTime = base.animation["rock_walk"].normalizedTime;
                        this.waitCounter = 0f;
                        base.networkView.RPC("startMovingRock", RPCMode.All, new object[0]);
                    }
                }
                else if (this.rockPhase == 5)
                {
                    if (Vector3.Distance(base.transform.position, this.targetCheckPt) < 10f)
                    {
                        if (this.checkPoints.Count > 0)
                        {
                            if (this.checkPoints.Count == 1)
                            {
                                this.rockPhase++;
                            }
                            else
                            {
                                Vector3 vector4 = (Vector3)this.checkPoints[0];
                                this.targetCheckPt = vector4;
                                this.checkPoints.RemoveAt(0);
                                GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn2");
                                GameObject obj2 = GameObject.Find("titanRespawn" + (7 - this.checkPoints.Count));
                                if (obj2 != null)
                                {
                                    foreach (GameObject obj3 in objArray)
                                    {
                                        if (obj3.transform.parent.gameObject == obj2)
                                        {
                                            GameObject obj4 = GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().spawnTitan(70, obj3.transform.position, obj3.transform.rotation);
                                            obj4.GetComponent<TITAN>().isAlarm = true;
                                            obj4.GetComponent<TITAN>().chaseDistance = 999999f;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.rockPhase++;
                        }
                    }
                    Vector3 vector5 = (Vector3)(base.transform.forward * 6f);
                    Vector3 vector6 = base.rigidbody.velocity;
                    Vector3 vector7 = vector5 - vector6;
                    vector7.x = Mathf.Clamp(vector7.x, -this.maxVelocityChange, this.maxVelocityChange);
                    vector7.z = Mathf.Clamp(vector7.z, -this.maxVelocityChange, this.maxVelocityChange);
                    vector7.y = 0f;
                    base.rigidbody.AddForce(vector7, ForceMode.VelocityChange);
                    base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
                    Vector3 vector8 = this.targetCheckPt - base.transform.position;
                    float current = -Mathf.Atan2(vector8.z, vector8.x) * 57.29578f;
                    float num3 = -Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
                    base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num3, 0f), 0.8f * Time.deltaTime);
                }
                else if (this.rockPhase == 6)
                {
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
                    base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    this.rockPhase++;
                    this.crossFade("rock_fix_hole", 0.1f);
                    object[] objArray4 = new object[] { "set" };
                    base.networkView.RPC("rockPlayAnimation", RPCMode.All, objArray4);
                    base.networkView.RPC("endMovingRock", RPCMode.All, new object[0]);
                }
                else if (this.rockPhase == 7)
                {
                    base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
                    base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
                    if (base.animation["rock_fix_hole"].normalizedTime >= 1.2f)
                    {
                        this.crossFade("die", 0.1f);
                        this.rockPhase++;
                        GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().gameWin();
                    }
                    if ((base.animation["rock_fix_hole"].normalizedTime >= 0.62f) && !this.rockHitGround)
                    {
                        this.rockHitGround = true;
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SERVER)
                        {
                            Network.Instantiate(Resources.Load("FX/boom1_CT_KICK"), new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f), 0);
                        }
                        else
                        {
                            UnityEngine.Object.Instantiate(Resources.Load("FX/boom1_CT_KICK"), new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f));
                        }
                    }
                }
            }
        }
    }
    public void setRoute()
    {
        GameObject obj2 = GameObject.Find("route");
        this.checkPoints = new ArrayList();
        for (int i = 1; i <= 7; i++)
        {
            this.checkPoints.Add(obj2.transform.Find("r" + i).position);
        }
        this.checkPoints.Add("end");
    }
    private void showAimUI()
    {
        GameObject obj2 = GameObject.Find("cross1");
        GameObject obj3 = GameObject.Find("cross2");
        GameObject obj4 = GameObject.Find("crossL1");
        GameObject obj5 = GameObject.Find("crossL2");
        GameObject obj6 = GameObject.Find("crossR1");
        GameObject obj7 = GameObject.Find("crossR2");
        GameObject obj8 = GameObject.Find("LabelDistance");
        Vector3 vector = (Vector3) (Vector3.up * 10000f);
        obj7.transform.localPosition = vector;
        obj6.transform.localPosition = vector;
        obj5.transform.localPosition = vector;
        obj4.transform.localPosition = vector;
        obj8.transform.localPosition = vector;
        obj3.transform.localPosition = vector;
        obj2.transform.localPosition = vector;
    }

    private void showSkillCD()
    {
        GameObject.Find("skill_cd_eren").GetComponent<UISprite>().fillAmount = this.lifeTime / this.lifeTimeMax;
    }

    private void Start()
    {
        if (this.rockLift)
        {
            this.rock = GameObject.Find("rock");
            this.rock.animation["lift"].speed = 0f;
        }
        else
        {
            this.currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            this.inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
            this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
            this.myR = this.sqrt2 * 6f;
            base.animation["hit_annie_1"].speed = 0.8f;
            base.animation["hit_annie_2"].speed = 0.7f;
            base.animation["hit_annie_3"].speed = 0.7f;
        }
    }

    [RPC]
    private void startMovingRock()
    {
        this.isROCKMOVE = true;
    }
    private void Update()
    {
        if ((!IN_GAME_MAIN_CAMERA.isPausing || (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)) && !this.rockLift)
        {
            if (base.animation.IsPlaying("run"))
            {
                if ((((base.animation["run"].normalizedTime % 1f) > 0.3f) && ((base.animation["run"].normalizedTime % 1f) < 0.75f)) && (this.stepSoundPhase == 2))
                {
                    this.stepSoundPhase = 1;
                    Transform transform = base.transform.Find("snd_eren_foot");
                    transform.GetComponent<AudioSource>().Stop();
                    transform.GetComponent<AudioSource>().Play();
                }
                if (((base.animation["run"].normalizedTime % 1f) > 0.75f) && (this.stepSoundPhase == 1))
                {
                    this.stepSoundPhase = 2;
                    Transform transform2 = base.transform.Find("snd_eren_foot");
                    transform2.GetComponent<AudioSource>().Stop();
                    transform2.GetComponent<AudioSource>().Play();
                }
            }
            if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine)
            {
                if (this.hasDied)
                {
                    if ((base.animation["die"].normalizedTime >= 1f) || (this.hitAnimation == "hit_annie_3"))
                    {
                        if (this.realBody != null)
                        {
                            this.realBody.GetComponent<HERO>().backToHuman();
                            this.realBody.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position + ((Vector3) (Vector3.up * 2f));
                            this.realBody = null;
                        }
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
                            }
                        }
                    }
                }
                else if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine)
                {
                    if (this.isHit)
                    {
                        if (base.animation[this.hitAnimation].normalizedTime >= 1f)
                        {
                            this.isHit = false;
                            this.falseAttack();
                            this.playAnimation("idle");
                        }
                    }
                    else
                    {
                        if (this.lifeTime > 0f)
                        {
                            this.lifeTime -= Time.deltaTime;
                            if (this.lifeTime <= 0f)
                            {
                                this.hasDied = true;
                                this.playAnimation("die");
                                return;
                            }
                        }
                        if (((this.grounded && !this.isAttack) && (!base.animation.IsPlaying("jump_land") && !this.isAttack)) && !base.animation.IsPlaying("born"))
                        {
                            if (this.inputManager.isInputDown[10] || this.inputManager.isInputDown[11])
                            {
                                bool flag = false;
                                if (((IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW) && this.inputManager.isInput[1]) || this.inputManager.isInputDown[11])
                                {
                                    if (((IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW) && this.inputManager.isInputDown[11]) && (this.inputManager.inputKey[11] == KeyCode.Mouse1))
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        this.attackAnimation = "attack_kick";
                                    }
                                }
                                else
                                {
                                    this.attackAnimation = "attack_combo_001";
                                }
                                if (!flag)
                                {
                                    this.playAnimation(this.attackAnimation);
                                    base.animation[this.attackAnimation].time = 0f;
                                    this.isAttack = true;
                                    this.needFreshCorePosition = true;
                                    if ((this.attackAnimation == "attack_combo_001") || (this.attackAnimation == "attack_combo_001"))
                                    {
                                        this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
                                    }
                                    else if (this.attackAnimation == "attack_combo_002")
                                    {
                                        this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
                                    }
                                    else if (this.attackAnimation == "attack_kick")
                                    {
                                        this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
                                    }
                                    this.hitTargets = new ArrayList();
                                }
                            }
                            if (this.inputManager.isInputDown[12])
                            {
                                this.crossFade("born", 0.1f);
                                base.animation["born"].normalizedTime = 0.28f;
                                this.isPlayRoar = false;
                            }
                        }
                        if (!this.isAttack)
                        {
                            if ((this.grounded || base.animation.IsPlaying("idle")) && ((!base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air")) && (!base.animation.IsPlaying("jump_land") && this.inputManager.isInput[8])))
                            {
                                this.crossFade("jump_start", 0.1f);
                            }
                        }
                        else
                        {
                            if ((base.animation[this.attackAnimation].time >= 0.1f) && this.inputManager.isInputDown[10])
                            {
                                this.isNextAttack = true;
                            }
                            float num = 0f;
                            float num2 = 0f;
                            float num3 = 0f;
                            string str = string.Empty;
                            if (this.attackAnimation == "attack_combo_001")
                            {
                                num = 0.4f;
                                num2 = 0.5f;
                                num3 = 0.66f;
                                str = "attack_combo_002";
                            }
                            else if (this.attackAnimation == "attack_combo_002")
                            {
                                num = 0.15f;
                                num2 = 0.25f;
                                num3 = 0.43f;
                                str = "attack_combo_003";
                            }
                            else if (this.attackAnimation == "attack_combo_003")
                            {
                                num3 = 0f;
                                num = 0.31f;
                                num2 = 0.37f;
                            }
                            else if (this.attackAnimation == "attack_kick")
                            {
                                num3 = 0f;
                                num = 0.32f;
                                num2 = 0.38f;
                            }
                            else
                            {
                                num = 0.5f;
                                num2 = 0.85f;
                            }
                            if (this.hitPause > 0f)
                            {
                                this.hitPause -= Time.deltaTime;
                                if (this.hitPause <= 0f)
                                {
                                    base.animation[this.attackAnimation].speed = 1f;
                                    this.hitPause = 0f;
                                }
                            }
                            if (((num3 > 0f) && this.isNextAttack) && (base.animation[this.attackAnimation].normalizedTime >= num3))
                            {
                                if (this.hitTargets.Count > 0)
                                {
                                    Transform transform3 = (Transform) this.hitTargets[0];
                                    if (transform3 != null)
                                    {
                                        base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(transform3.position - base.transform.position).eulerAngles.y, 0f);
                                        this.facingDirection = base.transform.rotation.eulerAngles.y;
                                    }
                                }
                                this.falseAttack();
                                this.attackAnimation = str;
                                this.crossFade(this.attackAnimation, 0.1f);
                                base.animation[this.attackAnimation].time = 0f;
                                base.animation[this.attackAnimation].speed = 1f;
                                this.isAttack = true;
                                this.needFreshCorePosition = true;
                                if (this.attackAnimation == "attack_combo_002")
                                {
                                    this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
                                }
                                else if (this.attackAnimation == "attack_combo_003")
                                {
                                    this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
                                }
                                this.hitTargets = new ArrayList();
                            }
                            if (((base.animation[this.attackAnimation].normalizedTime >= num) && (base.animation[this.attackAnimation].normalizedTime <= num2)) || (!this.attackChkOnce && (base.animation[this.attackAnimation].normalizedTime >= num)))
                            {
                                if (!this.attackChkOnce)
                                {
                                    if (this.attackAnimation == "attack_combo_002")
                                    {
                                        this.playSound("snd_eren_swing2");
                                    }
                                    else if (this.attackAnimation == "attack_combo_001")
                                    {
                                        this.playSound("snd_eren_swing1");
                                    }
                                    else if (this.attackAnimation == "attack_combo_003")
                                    {
                                        this.playSound("snd_eren_swing3");
                                    }
                                    this.attackChkOnce = true;
                                }
                                LayerMask mask = LayerMask.NameToLayer("EnemyAABB");
                                Collider[] colliderArray = Physics.OverlapSphere(this.attackBox.transform.position, 8f);
                                for (int i = 0; i < colliderArray.Length; i++)
                                {
                                    if (colliderArray[i].gameObject.transform.root.GetComponent<TITAN>() == null)
                                    {
                                        continue;
                                    }
                                    bool flag2 = false;
                                    for (int j = 0; j < this.hitTargets.Count; j++)
                                    {
                                        if (colliderArray[i].gameObject.transform.root == this.hitTargets[j])
                                        {
                                            flag2 = true;
                                            break;
                                        }
                                    }
                                    if (!flag2 && !colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().hasDie)
                                    {
                                        base.animation[this.attackAnimation].speed = 0f;
                                        if (this.attackAnimation == "attack_combo_002")
                                        {
                                            this.hitPause = 0.05f;
                                            colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().hitL(base.transform.position, this.hitPause);
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(1f, 0.03f, 0.95f);
                                        }
                                        else if (this.attackAnimation == "attack_combo_001")
                                        {
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(1.2f, 0.04f, 0.95f);
                                            this.hitPause = 0.08f;
                                            colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().hitR(base.transform.position, this.hitPause);
                                        }
                                        else if (this.attackAnimation == "attack_combo_003")
                                        {
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(3f, 0.1f, 0.95f);
                                            this.hitPause = 0.3f;
                                            colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().dieHeadBlow(base.transform.position, this.hitPause);
                                        }
                                        else if (this.attackAnimation == "attack_kick")
                                        {
                                            this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(3f, 0.1f, 0.95f);
                                            this.hitPause = 0.2f;
                                            if (colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().abnormalType == AbnormalType.TYPE_CRAWLER)
                                            {
                                                colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().dieBlow(base.transform.position, this.hitPause);
                                            }
                                            else if (colliderArray[i].gameObject.transform.root.transform.localScale.x < 2f)
                                            {
                                                colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().dieBlow(base.transform.position, this.hitPause);
                                            }
                                            else
                                            {
                                                colliderArray[i].gameObject.transform.root.GetComponent<TITAN>().hitR(base.transform.position, this.hitPause);
                                            }
                                        }
                                        this.hitTargets.Add(colliderArray[i].gameObject.transform.root);
                                        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
                                        {
                                            Network.Instantiate(Resources.Load("hitMeatBIG"), (Vector3) ((colliderArray[i].transform.position + this.attackBox.position) * 0.5f), Quaternion.Euler(270f, 0f, 0f), 0);
                                        }
                                        else
                                        {
                                            UnityEngine.Object.Instantiate(Resources.Load("hitMeatBIG"), (Vector3) ((colliderArray[i].transform.position + this.attackBox.position) * 0.5f), Quaternion.Euler(270f, 0f, 0f));
                                        }
                                    }
                                }
                            }
                            if (base.animation[this.attackAnimation].normalizedTime >= 1f)
                            {
                                this.falseAttack();
                                this.playAnimation("idle");
                            }
                        }
                        if (base.animation.IsPlaying("jump_land") && (base.animation["jump_land"].normalizedTime >= 1f))
                        {
                            this.crossFade("idle", 0.1f);
                        }
                        if (base.animation.IsPlaying("born"))
                        {
                            if ((base.animation["born"].normalizedTime >= 0.28f) && !this.isPlayRoar)
                            {
                                this.isPlayRoar = true;
                                this.playSound("snd_eren_roar");
                            }
                            if ((base.animation["born"].normalizedTime >= 0.5f) && (base.animation["born"].normalizedTime <= 0.7f))
                            {
                                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.5f, 1f, 0.95f);
                            }
                            if (base.animation["born"].normalizedTime >= 1f)
                            {
                                this.crossFade("idle", 0.1f);
                                if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
                                {
                                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.CLIENT)
                                    {
                                        object[] args = new object[] { 10f, 500f };
                                        base.networkView.RPC("netTauntAttack", RPCMode.Server, args);
                                    }
                                    else
                                    {
                                        this.netTauntAttack(10f, 500f);
                                    }
                                }
                                else
                                {
                                    this.netTauntAttack(10f, 500f);
                                }
                            }
                        }
                        this.showAimUI();
                        this.showSkillCD();
                    }
                }
            }
        }
    }
}

