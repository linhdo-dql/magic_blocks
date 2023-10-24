using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LayerBuildStateController;

public class LayerBuildController : MonoBehaviour
{
    public static LayerBuildController instance;
    public GameObject closeButton;
    public List<GameObject> hideOnViewMode;
    void Awake()
    {
        instance = this;
    }
    public void ChangeBuildState(string nameState)
    {
        BuildLayerState state = BuildLayerState.Build;
        switch (nameState)
        {
            case "Build":
                state = BuildLayerState.Build;
                SetBuildState(); break;
            case "View":
                state = BuildLayerState.View;
                SetViewState(); break;
            case "Break":
                state = BuildLayerState.Break;
                SetBreakState();
                break;
        }
        LayerBuildStateController.instance.SetBuildLayerState(state);
    }

    private void SetBreakState()
    {
        LayoutBlocksController.instance.Reset();
        foreach (var go in hideOnViewMode)
        {
            go.SetActive(true);
        }
        LayoutResController.instance.LockDrag(false);
    }

    private void SetBuildState()
    {
        LayoutBlocksController.instance.Reset();
        LayoutResController.instance.lockScroll = false;
        closeButton.SetActive(false);
        foreach (var go in hideOnViewMode)
        {
            go.SetActive(true);
        }
        LayoutResController.instance.LockDrag(true);
    }

    private void SetViewState()
    {
        LayoutBlocksController.instance.GetComponent<Rotatable>().speed = 0.3f;
        LayoutResController.instance.lockScroll = true;
        foreach (var go in hideOnViewMode)
        {
            go.SetActive(false);
        }
        LayoutResController.instance.LockDrag(true);
    }

}
