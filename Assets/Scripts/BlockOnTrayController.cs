using System;
using System.Collections;
using System.Collections.Generic;
using Flexalon;
using UnityEngine;

public class BlockOnTrayController : MonoBehaviour
{
    private float _oldScale;
    internal bool isTriggered;
    private MeshRenderer _meshRenderer;
    public string color;
    public bool isOnTray;
    public bool isOnFrame;
    public int x;
    public int y;

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.5f);
    }

    public void PopulateData(float oldScale, string color, int x, int y)
    {
        isOnTray = false;
        isOnFrame = false;
        _oldScale = oldScale;
        this.color = color;
        this.x = x;
        this.y = y;

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Tray"))
        {
            gameObject.LeanScale(Vector3.one, 0.2f).setOnComplete(() =>
            {
                transform.localScale = Vector3.one;
            });
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Tray"))
        {
            transform.SetParent(LayoutResController.instance.gameObject.transform);
            LayoutResController.instance.GetComponent<FlexalonFlexibleLayout>().ForceUpdate();
        }
    }

    void OnTriggerStay(Collider collider)
    {
        isOnTray = collider.CompareTag("Tray");
        isOnFrame = collider.CompareTag("Frame");
    }

    public void OpenOrLockCollider(bool isOpen)
    {
        LayoutBlocksController.instance.SetEnableColliders(isOpen);
        LayerBuildStateController.instance.SetControllerState(isOpen ? LayerBuildStateController.ControllerState.Idle : LayerBuildStateController.ControllerState.Holding);
        if (!isOnTray || !isOnFrame)
        {
            LayoutResController.instance.SubtractPos();
        }
    }

    public void SavePos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

}
