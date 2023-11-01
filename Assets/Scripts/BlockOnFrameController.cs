using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockOnFrameController : MonoBehaviour
{
    public Material defaultMaterial;
    public Material blockMaterial;
    public bool isFilled;
    public Color saveColor;
    private MeshRenderer _meshRenderer;
    private BoxCollider _collider;
    internal bool isTemp;
    public int x;
    public int y;
    public int saveX;
    public int saveY;
    public bool isTrueBlock;

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<BoxCollider>();
        saveColor = Color.clear;
    }
    private void OnTriggerStay(Collider collider)
    {
        var colliderController = collider.GetComponent<BlockOnTrayController>();
        if (colliderController.isTriggered || isFilled) return;
        Vector3 direction;
        float distance;
        Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation,
                                                        _collider, _collider.transform.position, _collider.transform.rotation,
                                                        out direction, out distance);
        float overlapPercentage = (distance / (_collider.bounds.size.magnitude + _collider.bounds.size.magnitude)) * 100f;
        if (collider.CompareTag("CubeResource") && overlapPercentage > 3)
        {
            isTrueBlock = colliderController.x == x && colliderController.y == y;
            if (!isTrueBlock)
            {
                saveX = colliderController.x;
                saveY = colliderController.y;
            }
            else
            {
                saveX = x;
                saveY = y;
            }
            colliderController.isTriggered = true;
            CopyBlock(colliderController);
            Destroy(collider.gameObject);
        }
    }

    public void CopyBlock(BlockOnTrayController colliderController)
    {
        ChangeColor(colliderController);
        saveX = x;
        saveY = y;
        isFilled = true;
        LayoutResController.instance.SubtractPos();
    }
    public void Clicked()
    {
        if (!isFilled)
        {
            if (LayerBuildStateController.instance.crBuildLayerState != LayerBuildStateController.BuildLayerState.Build) return;
            LayoutResController.instance.ChangedBlockRes(gameObject);
            saveX = x;
            saveY = y;
        }
        else
        {
            if (LayerBuildStateController.instance.crBuildLayerState != LayerBuildStateController.BuildLayerState.Break) return;
            ClickOnFilled(blockMaterial, saveColor, saveX, saveY);
        }

    }

    public void ClickOnFilled(Material material, Color saveColor, int x, int y)
    {
        //SetTransparentMaterial
        _meshRenderer.material = material;
        //Init a cube in Tray
        LayoutResController.instance.ReturnCube(saveColor, x, y);
        //Reset true state
        isTrueBlock = false;
        //Reset isFilled
        isFilled = false;
        //Reset colider size
        GetComponent<BoxCollider>().size = new Vector3(1, 1, 4);
    }

    private void ChangeColor(BlockOnTrayController colliderController)
    {
        Color color_new = Color.black;
        UnityEngine.ColorUtility.TryParseHtmlString("#" + colliderController.color, out color_new);
        _meshRenderer.material.color = color_new;
        saveColor = color_new;
    }

    internal void SavePos(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}
