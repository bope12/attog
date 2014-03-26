using System;
using UnityEngine;

public class UIMainReferences : MonoBehaviour
{
    public GameObject CheckboxPublicServer;
    private static bool isGAMEFirstLaunch = true;
    public GameObject panelCredits;
    public GameObject panelMain;
    public GameObject panelMultiJoin;
    public GameObject PanelMultiJoinPrivate;
    public GameObject panelMultiSet;
    public GameObject panelMultiStart;
    public GameObject PanelMultiWait;
    public GameObject panelOption;
    public GameObject panelSingleSet;
    public static string version = "v01242014";

    private void Start()
    {
        NGUITools.SetActive(this.panelMain, true);
        GameObject.Find("VERSION").GetComponent<UILabel>().text = version + "[58CF40]/ATUG/ Attack on Titan Underground[-]";
        if (isGAMEFirstLaunch)
        {
            isGAMEFirstLaunch = false;
            GameObject target = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("InputManagerController"));
            target.name = "InputManagerController";
            UnityEngine.Object.DontDestroyOnLoad(target);
            NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().CheckboxPublicServer, true);
        }
    }
}

