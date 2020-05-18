﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class contactlist : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject Prefabs;
    public GameObject Parent;
    private GroupInformationcontrol controller;
    private Dictionary<string, string> individualList;
    private int i=0;
    // Start is called before the first frame update
    void Start()
    {
        controller = Canvas.GetComponent<GroupInformationcontrol>();
        individualList = controller.GetIndividualMap();
        Transform Master = Parent.GetComponent<Transform>();
        foreach (string x in individualList.Keys)
        {
            instantPrefab(Master, i, individualList[x]);
            i++;
            Debug.Log(individualList[x]);
        }
        Debug.Log(individualList.Count);
    }

    // Update is called once per
    void Update()
    {
        
    }
    
    void instantPrefab(Transform Master, int i,string name)
    { 
        GameObject placeholder = Instantiate(Prefabs, new Vector3(Master.position.x+100,Master.position.y-30-i*50,0), Quaternion.identity);
        TextMeshProUGUI TMP = placeholder.GetComponent<TextMeshProUGUI>();
        TMP.text = name;
        placeholder.transform.SetParent(Master);
        
        Debug.Log(Master.position);
    }
}