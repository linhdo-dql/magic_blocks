using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerBuildStateController : MonoBehaviour
{
    public enum ControllerState
    {
        Idle,
        Holding,
        Moving,
        Selecting
    }

    public enum BuildLayerState
    {
        Build,
        View,
        Break,
    }

    public static LayerBuildStateController instance;
    public ControllerState crControllerState;
    public BuildLayerState crBuildLayerState;

    void Awake()
    {
        instance = this;
        crControllerState = ControllerState.Idle;
        crBuildLayerState = BuildLayerState.Build;
    }
    public void SetControllerState(ControllerState controllerState)
    {
        crControllerState = controllerState;
    }

    public void SetBuildLayerState(BuildLayerState buildLayerState)
    {
        crBuildLayerState = buildLayerState;
    }
}
