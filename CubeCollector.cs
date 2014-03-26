using System;
using UnityEngine;

public class CubeCollector : MonoBehaviour
{
    public int type;

    private void Start()
    {
    }

    private void Update()
    {
        if ((GameObject.FindGameObjectWithTag("Player") != null) && (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, base.transform.position) < 8f))
        {
            IN_GAME_MAIN_CAMERA component = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>();
            component.titanNum--;
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }
}

