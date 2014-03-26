using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XffectCache : MonoBehaviour
{
    private Dictionary<string, ArrayList> ObjectDic = new Dictionary<string, ArrayList>();

    protected Transform AddObject(string name)
    {
        Transform original = base.transform.Find(name);
        if (original == null)
        {
            Debug.Log("object:" + name + "doesn't exist!");
            return null;
        }
        Transform transform2 = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as Transform;
        this.ObjectDic[name].Add(transform2);
        transform2.gameObject.active = false;
        Xffect component = transform2.GetComponent<Xffect>();
        if (component != null)
        {
            component.Initialize();
        }
        return transform2;
    }

    private void Awake()
    {
        foreach (Transform transform in base.transform)
        {
            this.ObjectDic[transform.name] = new ArrayList();
            this.ObjectDic[transform.name].Add(transform);
            Xffect component = transform.GetComponent<Xffect>();
            if (component != null)
            {
                component.Initialize();
            }
            transform.gameObject.active = false;
        }
    }

    public Transform GetObject(string name)
    {
        ArrayList list = this.ObjectDic[name];
        if (list == null)
        {
            Debug.LogError(name + ": cache doesnt exist!");
            return null;
        }
        foreach (Transform transform in list)
        {
            if (!transform.gameObject.active)
            {
                transform.gameObject.active = true;
                return transform;
            }
        }
        return this.AddObject(name);
    }

    public ArrayList GetObjectCache(string name)
    {
        ArrayList list = this.ObjectDic[name];
        if (list == null)
        {
            Debug.LogError(name + ": cache doesnt exist!");
            return null;
        }
        return list;
    }

    private void Start()
    {
    }
}

