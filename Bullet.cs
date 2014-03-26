using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 humanHeightOffset;
    private bool isdestroying;
    private float killTime;
    private float killTime2;
    private bool left = true;
    public bool leviMode;
    public float leviShootTime;
    private LineRenderer lineRenderer;
    public GameObject master;
    private ArrayList nodes = new ArrayList();
    private int phase;
    private GameObject rope;
    private int spiralcount;
    private ArrayList spiralNodes;
    private Vector3 velocity = Vector3.zero;

    public void disable()
    {
        this.phase = 2;
        this.killTime = 0f;
    }

    private void FixedUpdate()
    {
        if ((this.phase == 2) && this.leviMode)
        {
            this.spiralcount++;
            if (this.spiralcount >= 60)
            {
                this.isdestroying = true;
                this.removeMe();
            }
        }
    }

    private void getSpiral(Vector3 masterposition, Vector3 masterrotation)
    {
        float num = 1.2f;
        float num2 = 30f;
        float num3 = 2f;
        float num4 = 0.5f;
        num = 30f;
        num3 = 0.05f + (this.spiralcount * 0.03f);
        if (this.spiralcount < 5)
        {
            num = Vector2.Distance(new Vector2(masterposition.x, masterposition.z), new Vector2(base.gameObject.transform.position.x, base.gameObject.transform.position.z));
        }
        else
        {
            num = 1.2f + ((60 - this.spiralcount) * 0.1f);
        }
        num4 -= this.spiralcount * 0.06f;
        float num6 = num / num2;
        float num7 = num3 / num2;
        float num8 = (num7 * 2f) * 3.141593f;
        num4 *= 6.283185f;
        this.spiralNodes = new ArrayList();
        for (int i = 1; i <= num2; i++)
        {
            float num10 = (i * num6) * (1f + (0.05f * i));
            float f = (((i * num8) + num4) + 1.256637f) + (masterrotation.y * 0.0173f);
            float x = Mathf.Cos(f) * num10;
            float z = -Mathf.Sin(f) * num10;
            this.spiralNodes.Add(new Vector3(x, 0f, z));
        }
    }

    public bool isHooked()
    {
        return (this.phase == 1);
    }

    [RPC]
    private void killObject()
    {
        UnityEngine.Object.Destroy(this.rope);
        UnityEngine.Object.Destroy(base.gameObject);
    }

    //private void LateUpdate()
    private void Update()
    {
        if (this.master == null)
        {
            this.removeMe();
        }
        else if (!this.isdestroying && ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || base.networkView.isMine))
        {
            if (this.leviMode)
            {
                this.leviShootTime += Time.deltaTime;
                if (this.leviShootTime > 0.4f)
                {
                    this.phase = 2;
                    base.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            if (this.phase == 0)
            {
                RaycastHit hit;
                LayerMask mask3;
                Transform transform = base.gameObject.transform;
                transform.position += (Vector3) ((this.velocity * Time.deltaTime) * 50f);
                LayerMask mask = ((int) 1) << LayerMask.NameToLayer("EnemyBox");
                LayerMask mask2 = ((int) 1) << LayerMask.NameToLayer("Ground");
                if(GameObject.Find("MultiplayerManager").GetComponent<FengMultiplayerScript>().nohook)
                mask3 = mask2;
                else               
                mask3 = mask | mask2;
                    if (Physics.Linecast((Vector3)this.nodes[this.nodes.Count - 1], base.gameObject.transform.position, out hit, mask3.value))
                    {
                        this.master.GetComponent<HERO>().launch(hit.point, this.left, this.leviMode);
                        if (hit.collider.transform.gameObject.layer == LayerMask.NameToLayer("EnemyBox"))
                        {
                            this.master.GetComponent<HERO>().lastHook = hit.collider.transform.root;
                        }
                        else
                        {
                            this.master.GetComponent<HERO>().lastHook = null;
                        }
                        base.transform.parent = hit.collider.transform;
                        base.transform.position = hit.point;
                        if (this.phase == 0)
                        {
                            this.phase = 1;
                        }
                        if (this.leviMode)
                        {
                            this.getSpiral(this.master.transform.position, this.master.transform.rotation.eulerAngles);
                        }
                    }
                    this.nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
                    Vector3 vector = (this.master.transform.position - ((Vector3)this.nodes[0])) + this.humanHeightOffset;
                    this.lineRenderer.SetVertexCount(this.nodes.Count);
                    for (int i = 0; i <= (this.nodes.Count - 1); i++)
                    {
                        this.lineRenderer.SetPosition(i, ((Vector3)this.nodes[i]) + ((Vector3)(vector * Mathf.Pow(0.5f, (float)i))));
                    }
                    this.killTime2 += Time.deltaTime;
                    if (this.killTime2 > 0.8f)
                    {
                        this.phase = 4;
                    }
                    else if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
                    {
                        object[] args = new object[] { base.transform.position, this.master.transform.position + this.humanHeightOffset };
                        base.networkView.RPC("netUpdatePhase0", RPCMode.Others, args);
                    }
                
            }
            else if (this.phase == 1)
            {
                this.lineRenderer.SetVertexCount(2);
                this.lineRenderer.SetPosition(0, base.transform.position);
                this.lineRenderer.SetPosition(1, this.master.transform.position + this.humanHeightOffset);
                if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
                {
                    object[] objArray2 = new object[] { base.transform.position, this.master.transform.position + this.humanHeightOffset };
                    base.networkView.RPC("netUpdatePhase1", RPCMode.Others, objArray2);
                }
            }
            else if (this.phase == 2)
            {
                if (this.leviMode)
                {
                    this.getSpiral(this.master.transform.position, this.master.transform.rotation.eulerAngles);
                    Vector3 vector2 = (this.master.transform.position - ((Vector3) this.spiralNodes[0])) + this.humanHeightOffset;
                    this.lineRenderer.SetVertexCount(this.spiralNodes.Count - ((int) (this.spiralcount * 0.5f)));
                    for (int j = 0; j <= ((this.spiralNodes.Count - 1) - (this.spiralcount * 0.5f)); j++)
                    {
                        if (this.spiralcount < 5)
                        {
                            Vector3 position = ((Vector3) this.spiralNodes[j]) + vector2;
                            float num3 = (this.spiralNodes.Count - 1) - (this.spiralcount * 0.5f);
                            position = new Vector3(position.x, (position.y * ((num3 - j) / num3)) + (base.gameObject.transform.position.y * (((float) j) / num3)), position.z);
                            this.lineRenderer.SetPosition(j, position);
                        }
                        else
                        {
                            this.lineRenderer.SetPosition(j, ((Vector3) this.spiralNodes[j]) + vector2);
                        }
                    }
                    if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
                    {
                        object[] objArray3 = new object[] { base.transform.position, this.master.transform.position + this.humanHeightOffset, this.master.transform.rotation.eulerAngles };
                        base.networkView.RPC("netUpdateLeviSpiral", RPCMode.Others, objArray3);
                    }
                }
                else
                {
                    this.lineRenderer.SetVertexCount(2);
                    this.lineRenderer.SetPosition(0, base.transform.position);
                    this.lineRenderer.SetPosition(1, this.master.transform.position + this.humanHeightOffset);
                    if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
                    {
                        object[] objArray4 = new object[] { base.transform.position, this.master.transform.position + this.humanHeightOffset };
                        base.networkView.RPC("netUpdatePhase1", RPCMode.Others, objArray4);
                    }
                    this.killTime += Time.deltaTime * 0.2f;
                    this.lineRenderer.SetWidth(0.1f - this.killTime, 0.1f - this.killTime);
                    if (this.killTime > 0.1f)
                    {
                        this.removeMe();
                    }
                }
            }
            else if (this.phase == 4)
            {
                Transform transform2 = base.gameObject.transform;
                transform2.position += this.velocity;
                this.nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
                Vector3 vector4 = (this.master.transform.position - ((Vector3) this.nodes[0])) + this.humanHeightOffset;
                for (int k = 0; k <= (this.nodes.Count - 1); k++)
                {
                    this.lineRenderer.SetVertexCount(this.nodes.Count);
                    this.lineRenderer.SetPosition(k, ((Vector3) this.nodes[k]) + ((Vector3) (vector4 * Mathf.Pow(0.5f, (float) k))));
                }
                this.killTime2 += Time.deltaTime;
                if (this.killTime2 > 0.8f)
                {
                    this.killTime += Time.deltaTime * 0.2f;
                    this.lineRenderer.SetWidth(0.1f - this.killTime, 0.1f - this.killTime);
                    if (this.killTime > 0.1f)
                    {
                        this.removeMe();
                    }
                }
            }
        }
    }

    public void launch(Vector3 v, Vector3 des, Vector3 offset, bool left = true, bool leviMode = false)
    {
        if (this.phase != 2)
        {
            this.velocity = v;
            this.left = left;
            this.nodes = new ArrayList();
            this.nodes.Add(new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z));
            this.phase = 0;
            this.humanHeightOffset = offset;
            this.leviMode = leviMode;
            if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
            {
                object[] args = new object[] { base.transform.position };
                base.networkView.RPC("netLaunch", RPCMode.Others, args);
            }
        }
    }

    [RPC]
    private void netLaunch(Vector3 newPosition)
    {
        this.nodes = new ArrayList();
        this.nodes.Add(newPosition);
    }

    [RPC]
    private void netUpdateLeviSpiral(Vector3 newPosition, Vector3 masterPosition, Vector3 masterrotation)
    {
        this.phase = 2;
        this.leviMode = true;
        this.getSpiral(masterPosition, masterrotation);
        Vector3 vector = masterPosition - ((Vector3) this.spiralNodes[0]);
        this.lineRenderer.SetVertexCount(this.spiralNodes.Count - ((int) (this.spiralcount * 0.5f)));
        for (int i = 0; i <= ((this.spiralNodes.Count - 1) - (this.spiralcount * 0.5f)); i++)
        {
            if (this.spiralcount < 5)
            {
                Vector3 position = ((Vector3) this.spiralNodes[i]) + vector;
                float num2 = (this.spiralNodes.Count - 1) - (this.spiralcount * 0.5f);
                position = new Vector3(position.x, (position.y * ((num2 - i) / num2)) + (newPosition.y * (((float) i) / num2)), position.z);
                this.lineRenderer.SetPosition(i, position);
            }
            else
            {
                this.lineRenderer.SetPosition(i, ((Vector3) this.spiralNodes[i]) + vector);
            }
        }
    }

    [RPC]
    private void netUpdatePhase0(Vector3 newPosition, Vector3 masterPosition)
    {
        if (this.lineRenderer != null)
        {
            this.nodes.Add(newPosition);
            Vector3 vector = masterPosition - ((Vector3) this.nodes[0]);
            this.lineRenderer.SetVertexCount(this.nodes.Count);
            for (int i = 0; i <= (this.nodes.Count - 1); i++)
            {
                this.lineRenderer.SetPosition(i, ((Vector3) this.nodes[i]) + ((Vector3) (vector * Mathf.Pow(0.5f, (float) i))));
            }
            base.transform.position = newPosition;
        }
    }

    [RPC]
    private void netUpdatePhase1(Vector3 newPosition, Vector3 masterPosition)
    {
        this.lineRenderer.SetVertexCount(2);
        this.lineRenderer.SetPosition(0, newPosition);
        this.lineRenderer.SetPosition(1, masterPosition);
        base.transform.position = newPosition;
    }

    private void OnDestroy()
    {
        UnityEngine.Object.Destroy(this.rope);
    }

    public void removeMe()
    {
        if ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE) && base.networkView.isMine)
        {
            base.networkView.RPC("killObject", RPCMode.AllBuffered, new object[0]);
        }
        else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
        {
            UnityEngine.Object.Destroy(this.rope);
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    private void Start()
    {
        this.rope = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("rope"));
        this.lineRenderer = this.rope.GetComponent<LineRenderer>();
    }
}

