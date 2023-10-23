using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LayerBuildStateController;

public class LayerBuildController : MonoBehaviour
{
    public static LayerBuildController instance;
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
            case "Break": state = BuildLayerState.Break; break;
        }
        LayerBuildStateController.instance.SetBuildLayerState(state);
    }

    private void SetBuildState()
    {
        LayoutBlocksController.instance.Reset();
        LayoutResController.instance.lockScroll = false;
    }

    private void SetViewState()
    {
        LayoutBlocksController.instance.GetComponent<Rotatable>().speed = 0.3f;
        LayoutResController.instance.lockScroll = true;
    }

}
