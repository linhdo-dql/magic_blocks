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

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.5f);
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        // if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
        // {
        //     // Cube has moved out of the screen
        //     Debug.Log("Cube is outside the screen!");

        // }
    }

    public void PopulateData(float oldScale, string color)
    {
        isOnTray = false;
        isOnFrame = false;
        _oldScale = oldScale;
        this.color = color;

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Tray"))
        {
            transform.localScale = Vector3.one;
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

}
