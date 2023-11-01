using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flexalon;
using Newtonsoft.Json;
using UnityEngine;
using static LayerBuildStateController;

public class LayerBuildController : MonoBehaviour
{
    public static LayerBuildController instance;
    public GameObject closeButton;
    public GameObject breakAllButton;
    public List<GameObject> hideOnViewMode;
    private bool _isOnGuiding;

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
            case "Guid":
                if (_isOnGuiding) return;
                state = BuildLayerState.Guid;
                SetGuidState();
                break;
        }
        LayerBuildStateController.instance.SetBuildLayerState(state);
    }

    private void SetGuidState()
    {

        //Check in Tray
        //if tray have any block, move block to corresponding location
        var cubesResource = GameObject.FindGameObjectsWithTag("CubeResource");
        if (cubesResource.Count() < 1)
        {
            GuidingFull();
        }
        else
        {
            GuidingNull();
        }
        _isOnGuiding = true;
        List<BlockOnTrayController> cubeControllers = new List<BlockOnTrayController>();
        foreach (var cube in cubesResource)
        {
            var trayController = cube.GetComponent<BlockOnTrayController>();
            if (!trayController.isOnFrame && !trayController.isOnTray)
            {
                ActionTrayBlockGuiding(cube);
                break;
            }
            ActionTrayBlockGuiding(cube);
            break;
        }


    }

    private void GuidingNull()
    {
        throw new NotImplementedException();
    }

    private void GuidingFull()
    {
        throw new NotImplementedException();
    }

    private void ActionTrayBlockGuiding(GameObject cube)
    {
        var cubeController = cube.GetComponent<BlockOnTrayController>();
        //Move to frame
        //Get frame block
        cube.GetComponent<BoxCollider>().enabled = false;
        var cubeOnFrameController = GetFrameCorresponding(cubeController);
        float delayTime = 0;
        if (cubeOnFrameController.isFilled)
        {
            cubeOnFrameController.ClickOnFilled(cubeOnFrameController.blockMaterial, cubeOnFrameController.saveColor, cubeOnFrameController.saveX, cubeOnFrameController.saveY);
            delayTime = 0.5f;
        }
        LeanTween.move(cube, cubeOnFrameController.transform.position, 0.5f).setDelay(delayTime).setOnComplete(() =>
        {
            cube.GetComponent<BoxCollider>().enabled = true;
            _isOnGuiding = false;
            ChangeBuildState("Build");
        });
    }

    private BlockOnFrameController GetFrameCorresponding(BlockOnTrayController cubeController)
    {
        return LayoutBlocksController.instance.GetComponentsInChildren<BlockOnFrameController>().ToList().Find(item => item.x == cubeController.x && item.y == cubeController.y);
    }

    private void SetBreakState()
    {
        LayoutBlocksController.instance.Reset();
        foreach (var go in hideOnViewMode)
        {
            go.SetActive(true);
        }
        breakAllButton.SetActive(true);
        LayoutResController.instance.LockDrag(false);
    }

    private void SetBuildState()
    {
        LayoutBlocksController.instance.Reset();
        LayoutResController.instance.lockScroll = false;
        foreach (var go in hideOnViewMode)
        {
            go.SetActive(true);
        }

        closeButton.SetActive(false);
        breakAllButton.SetActive(false);
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

    public void SaveLayer()
    {
        string streamingAssetsPath = "";
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_LINUX
        streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets");
#elif UNITY_IOS
        streamingAssetsPath = Path.Combine(Application.dataPath, "Raw"); 
#elif UNITY_ANDROID || UNITY_WEBGL
        streamingAssetsPath = Application.streamingAssetsPath;
#endif
        string folderPath = Path.Combine(streamingAssetsPath, "Users", LayerDataController.instance.folderName);
        string filePath = Path.Combine(folderPath, LayerDataController.instance.fileName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        ColorData colorData = new ColorData
        {
            colors = new List<SerializableColor>()
        };
        var count = 0;
        for (int y = 0; y < LayoutBlocksController.instance.layoutBlock.Columns; y++)
        {
            for (int x = 0; x < LayoutBlocksController.instance.layoutBlock.Rows; x++)
            {
                BlockOnFrameController itemBlock = LayoutBlocksController.instance.transform.GetChild(count).GetComponent<BlockOnFrameController>();
                colorData.colors.Add(new SerializableColor(itemBlock.saveColor, itemBlock.x, itemBlock.y));
                count++;
            }
        }

        string jsonData = JsonConvert.SerializeObject(colorData, Formatting.Indented);
        File.WriteAllText(filePath, jsonData);

        Debug.Log("Xuất File thành công!");
    }

}
