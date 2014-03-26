using System;
using System.Collections;
using UnityEngine;

public class TriggerColliderWeapon : MonoBehaviour
{
    public bool active_me;
    public AudioSource audio_ally;
    public GameObject currentCamera;
    public ArrayList currentHits = new ArrayList();
    public ArrayList currentHitsII = new ArrayList();
    public AudioSource meatDie;
    public float scoreMulti = 1f;

    private bool checkIfBehind(GameObject titan)
    {
        Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        Vector3 to = base.transform.position - transform.transform.position;
        Debug.DrawRay(transform.transform.position, (Vector3) (-transform.transform.forward * 10f), Color.white, 5f);
        Debug.DrawRay(transform.transform.position, (Vector3) (to * 10f), Color.green, 5f);
        return (Vector3.Angle(-transform.transform.forward, to) < 70f);
    }

    public void clearHits()
    {
        this.currentHitsII = new ArrayList();
        this.currentHits = new ArrayList();
    }

    private void OnTriggerStay(Collider other)
    {
        if (this.active_me)
        {
            if (!this.currentHitsII.Contains(other.gameObject))
            {
                GameObject obj2;
                this.currentHitsII.Add(other.gameObject);
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.1f, 0.1f, 0.95f);
                this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>().slashHit.Play();
                if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
                {
                    obj2 = (GameObject) Network.Instantiate(Resources.Load("hitMeat"), base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
                }
                else
                {
                    obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("hitMeat"));
                }
                obj2.transform.position = base.transform.position;
                base.transform.root.GetComponent<HERO>().useBlade(0);
            }
            if (other.gameObject.tag == "titanneck")
            {
                HitBox component = other.gameObject.GetComponent<HitBox>();
                if (((component != null) && this.checkIfBehind(component.transform.root.gameObject)) && !this.currentHits.Contains(component))
                {
                    component.hitPosition = (Vector3) ((base.transform.position + component.transform.position) * 0.5f);
                    this.currentHits.Add(component);
                    this.meatDie.Play();
                    if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                    {
                        if ((component.transform.root.GetComponent<TITAN>() != null) && !component.transform.root.GetComponent<TITAN>().hasDie)
                        {
                            Vector3 vector = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component.transform.root.rigidbody.velocity;
                            int b = (int) ((vector.magnitude * 10f) * this.scoreMulti);
                            b = Mathf.Max(10, b);
                            component.transform.root.GetComponent<TITAN>().die();
                            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().netShowDamage(b);
                            GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().playerKillInfoUpdate(GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().players[0], b);
                        }
                    }
                    else if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SERVER)
                    {
                        if (component.transform.root.GetComponent<TITAN>() != null)
                        {
                            if (!component.transform.root.GetComponent<TITAN>().hasDie)
                            {
                                Vector3 vector2 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component.transform.root.rigidbody.velocity;
                                int num2 = (int) ((vector2.magnitude * 10f) * this.scoreMulti);
                                num2 = Mathf.Max(10, num2);
                                object[] args = new object[] { base.transform.root.gameObject.networkView.viewID, num2 };
                                component.transform.root.GetComponent<TITAN>().networkView.RPC("titanGetHit", RPCMode.Server, args);
                            }
                        }
                        else if (component.transform.root.GetComponent<FEMALE_TITAN>() != null)
                        {
                            base.transform.root.GetComponent<HERO>().useBlade(0x7fffffff);
                            Vector3 vector3 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component.transform.root.rigidbody.velocity;
                            int num3 = (int) ((vector3.magnitude * 10f) * this.scoreMulti);
                            num3 = Mathf.Max(10, num3);
                            if (!component.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                object[] objArray2 = new object[] { base.transform.root.gameObject.networkView.viewID, num3 };
                                component.transform.root.GetComponent<FEMALE_TITAN>().networkView.RPC("titanGetHit", RPCMode.Server, objArray2);
                            }
                        }
                        else if (component.transform.root.GetComponent<COLOSSAL_TITAN>() != null)
                        {
                            base.transform.root.GetComponent<HERO>().useBlade(0x7fffffff);
                            if (!component.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                            {
                                Vector3 vector4 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component.transform.root.rigidbody.velocity;
                                int num4 = (int) ((vector4.magnitude * 10f) * this.scoreMulti);
                                num4 = Mathf.Max(10, num4);
                                object[] objArray3 = new object[] { base.transform.root.gameObject.networkView.viewID, num4 };
                                component.transform.root.GetComponent<COLOSSAL_TITAN>().networkView.RPC("titanGetHit", RPCMode.Server, objArray3);
                            }
                        }
                    }
                    else if (component.transform.root.GetComponent<TITAN>() != null)
                    {
                        if (!component.transform.root.GetComponent<TITAN>().hasDie)
                        {
                            Vector3 vector5 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component.transform.root.rigidbody.velocity;
                            int num5 = (int) ((vector5.magnitude * 10f) * this.scoreMulti);
                            num5 = Mathf.Max(10, num5);
                            component.transform.root.GetComponent<TITAN>().titanGetHit(base.transform.root.gameObject.networkView.viewID, num5);
                        }
                    }
                    else if (component.transform.root.GetComponent<FEMALE_TITAN>() != null)
                    {
                        base.transform.root.GetComponent<HERO>().useBlade(0x7fffffff);
                        if (!component.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            Vector3 vector6 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component.transform.root.rigidbody.velocity;
                            int num6 = (int) ((vector6.magnitude * 10f) * this.scoreMulti);
                            num6 = Mathf.Max(10, num6);
                            component.transform.root.GetComponent<FEMALE_TITAN>().titanGetHit(base.transform.root.gameObject.networkView.viewID, num6);
                        }
                    }
                    else if (component.transform.root.GetComponent<COLOSSAL_TITAN>() != null)
                    {
                        base.transform.root.GetComponent<HERO>().useBlade(0x7fffffff);
                        if (!component.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                        {
                            Vector3 vector7 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component.transform.root.rigidbody.velocity;
                            int num7 = (int) ((vector7.magnitude * 10f) * this.scoreMulti);
                            num7 = Mathf.Max(10, num7);
                            component.transform.root.GetComponent<COLOSSAL_TITAN>().titanGetHit(base.transform.root.gameObject.networkView.viewID, num7);
                        }
                    }
                    this.showCriticalHitFX();
                }
            }
            else if (other.gameObject.tag == "titaneye")
            {
                if (!this.currentHits.Contains(other.gameObject))
                {
                    this.currentHits.Add(other.gameObject);
                    GameObject gameObject = other.gameObject.transform.root.gameObject;
                    if (gameObject.GetComponent<FEMALE_TITAN>() != null)
                    {
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                        {
                            if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject.GetComponent<FEMALE_TITAN>().hitEye();
                            }
                        }
                        else if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SERVER)
                        {
                            if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                object[] objArray4 = new object[] { base.transform.root.gameObject.networkView.viewID };
                                gameObject.GetComponent<FEMALE_TITAN>().networkView.RPC("hitEyeRPC", RPCMode.Server, objArray4);
                            }
                        }
                        else if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            gameObject.GetComponent<FEMALE_TITAN>().hitEyeRPC(base.transform.root.gameObject.networkView.viewID);
                        }
                    }
                    else if (gameObject.GetComponent<TITAN>().abnormalType != AbnormalType.TYPE_CRAWLER)
                    {
                        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                        {
                            if (!gameObject.GetComponent<TITAN>().hasDie)
                            {
                                gameObject.GetComponent<TITAN>().hitEye();
                            }
                        }
                        else if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SERVER)
                        {
                            if (!gameObject.GetComponent<TITAN>().hasDie)
                            {
                                object[] objArray5 = new object[] { base.transform.root.gameObject.networkView.viewID };
                                gameObject.GetComponent<TITAN>().networkView.RPC("hitEyeRPC", RPCMode.Server, objArray5);
                            }
                        }
                        else if (!gameObject.GetComponent<TITAN>().hasDie)
                        {
                            gameObject.GetComponent<TITAN>().hitEyeRPC(base.transform.root.gameObject.networkView.viewID);
                        }
                        this.showCriticalHitFX();
                    }
                }
            }
            else if ((other.gameObject.tag == "titanankle") && !this.currentHits.Contains(other.gameObject))
            {
                this.currentHits.Add(other.gameObject);
                GameObject obj4 = other.gameObject.transform.root.gameObject;
                HitBox box2 = other.gameObject.GetComponent<HitBox>();
                Vector3 vector8 = this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - obj4.rigidbody.velocity;
                int num8 = (int) ((vector8.magnitude * 10f) * this.scoreMulti);
                num8 = Mathf.Max(10, num8);
                if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
                {
                    if (other.gameObject.name == "ankleR")
                    {
                        if ((obj4.GetComponent<FEMALE_TITAN>() != null) && !obj4.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            obj4.GetComponent<FEMALE_TITAN>().hitAnkleR(num8);
                        }
                    }
                    else if ((obj4.GetComponent<FEMALE_TITAN>() != null) && !obj4.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        obj4.GetComponent<FEMALE_TITAN>().hitAnkleL(num8);
                    }
                }
                else if (other.gameObject.name == "ankleR")
                {
                    if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SERVER)
                    {
                        if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            object[] objArray6 = new object[] { base.transform.root.gameObject.networkView.viewID, num8 };
                            obj4.GetComponent<FEMALE_TITAN>().networkView.RPC("hitAnkleRRPC", RPCMode.Server, objArray6);
                        }
                    }
                    else if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        obj4.GetComponent<FEMALE_TITAN>().hitAnkleRRPC(base.transform.root.gameObject.networkView.viewID, num8);
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SERVER)
                {
                    if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        object[] objArray7 = new object[] { base.transform.root.gameObject.networkView.viewID, num8 };
                        obj4.GetComponent<FEMALE_TITAN>().networkView.RPC("hitAnkleLRPC", RPCMode.Server, objArray7);
                    }
                }
                else if (!obj4.GetComponent<FEMALE_TITAN>().hasDie)
                {
                    obj4.GetComponent<FEMALE_TITAN>().hitAnkleLRPC(base.transform.root.gameObject.networkView.viewID, num8);
                }
                this.showCriticalHitFX();
            }
        }
    }

    private void showCriticalHitFX()
    {
        GameObject obj2;
        this.currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f, 0.95f);
        if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE)
        {
            obj2 = (GameObject) Network.Instantiate(Resources.Load("redCross"), base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            obj2 = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("redCross"));
        }
        obj2.transform.position = base.transform.position;
    }

    private void Start()
    {
        this.currentCamera = GameObject.Find("MainCamera");
    }
}

