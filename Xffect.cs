using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Xffect")]
public class Xffect : MonoBehaviour
{
    private List<EffectLayer> EflList = new List<EffectLayer>();
    protected float ElapsedTime;
    public float LifeTime = -1f;
    private Dictionary<string, VertexPool> MatDic = new Dictionary<string, VertexPool>();

    public void Active()
    {
        foreach (Transform transform in base.transform)
        {
            transform.gameObject.active = true;
        }
        base.gameObject.active = true;
        this.ElapsedTime = 0f;
    }

    private void Awake()
    {
        this.Initialize();
    }

    public void DeActive()
    {
        foreach (Transform transform in base.transform)
        {
            transform.gameObject.active = false;
        }
        base.gameObject.active = false;
    }

    public void Initialize()
    {
        if (this.EflList.Count <= 0)
        {
            foreach (Transform transform in base.transform)
            {
                EffectLayer component = (EffectLayer)transform.GetComponent(typeof(EffectLayer));
                if ((component != null) && (component.Material != null))
                {
                    MeshFilter filter;
                    MeshRenderer renderer;
                    Material material = component.Material;
                    this.EflList.Add(component);
                    Transform transform2 = base.transform.Find("mesh " + material.name);
                    if (transform2 != null)
                    {
                        filter = (MeshFilter)transform2.GetComponent(typeof(MeshFilter));
                        renderer = (MeshRenderer)transform2.GetComponent(typeof(MeshRenderer));
                        filter.mesh.Clear();
                        this.MatDic[material.name] = new VertexPool(filter.mesh, material);
                    }
                    if (!this.MatDic.ContainsKey(material.name))
                    {
                        GameObject obj3 = new GameObject("mesh " + material.name);
                        obj3.transform.parent = base.transform;
                        GameObject obj2 = obj3;
                        obj2.AddComponent("MeshFilter");
                        obj2.AddComponent("MeshRenderer");
                        filter = (MeshFilter)obj2.GetComponent(typeof(MeshFilter));
                        renderer = (MeshRenderer)obj2.GetComponent(typeof(MeshRenderer));
                        renderer.castShadows = false;
                        renderer.receiveShadows = false;
                        renderer.renderer.material = material;
                        this.MatDic[material.name] = new VertexPool(filter.mesh, material);
                    }
                }
            }
            foreach (EffectLayer layer2 in this.EflList)
            {
                layer2.Vertexpool = this.MatDic[layer2.Material.name];
            }
        }
    }

    private void LateUpdate()
    {
        foreach (KeyValuePair<string, VertexPool> pair in this.MatDic)
        {
            pair.Value.LateUpdate();
        }
        if ((this.ElapsedTime > this.LifeTime) && (this.LifeTime >= 0f))
        {
            foreach (EffectLayer layer in this.EflList)
            {
                layer.Reset();
            }
            this.DeActive();
            this.ElapsedTime = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
    }

    public void SetClient(Transform client)
    {
        foreach (EffectLayer layer in this.EflList)
        {
            layer.ClientTransform = client;
        }
    }

    public void SetDirectionAxis(Vector3 axis)
    {
        foreach (EffectLayer layer in this.EflList)
        {
            layer.OriVelocityAxis = axis;
        }
    }

    public void SetEmitPosition(Vector3 pos)
    {
        foreach (EffectLayer layer in this.EflList)
        {
            layer.EmitPoint = pos;
        }
    }

    private void Start()
    {
        base.transform.position = Vector3.zero;
        base.transform.rotation = Quaternion.identity;
        base.transform.localScale = Vector3.one;
        foreach (Transform transform in base.transform)
        {
            transform.transform.position = Vector3.zero;
            transform.transform.rotation = Quaternion.identity;
            transform.transform.localScale = Vector3.one;
        }
        foreach (EffectLayer layer in this.EflList)
        {
            layer.StartCustom();
        }
    }

    private void Update()
    {
        this.ElapsedTime += Time.deltaTime;
        foreach (EffectLayer layer in this.EflList)
        {
            if (this.ElapsedTime > layer.StartTime)
            {
                layer.FixedUpdateCustom();
            }
        }
    }
}

