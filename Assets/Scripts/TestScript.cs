using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeSize(int size)
    {
        ClearAllComponent();
        int fileName = 2;
        switch (size)
        {
            case 1: fileName = 2; break;
            case 2: fileName = 3; break;
            case 3: fileName = 4; break;
        }
        LayerDataController.instance.GetDataOfLayer("1", fileName);
    }

    private void ClearAllComponent()
    {
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("CubeResource"))
        {
            Destroy(t);
        }
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("CubeBlock"))
        {
            Destroy(t);
        }
    }
}
