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
    private Color saveColor;
    private MeshRenderer _meshRenderer;
    private BoxCollider _collider;
    internal bool isTemp;

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<BoxCollider>();
    }
    private void OnTriggerStay(Collider collider)
    {
        var colliderController = collider.GetComponent<BlockOnTrayController>();
        if (colliderController.isTriggered) return;
        Vector3 direction;
        float distance;
        Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation,
                                                        _collider, _collider.transform.position, _collider.transform.rotation,
                                                        out direction, out distance);
        float overlapPercentage = (distance / (_collider.bounds.size.magnitude + _collider.bounds.size.magnitude)) * 100f;
        if (collider.CompareTag("CubeResource") && overlapPercentage > 5)
        {
            colliderController.isTriggered = true;
            ChangeColor(colliderController);
            Destroy(collider.gameObject);
            isFilled = true;
            LayoutResController.instance.SubtractPos();
        }
    }

    public void Clicked()
    {
        if (!isFilled) return;
        //SetTransparentMaterial
        _meshRenderer.material = blockMaterial;
        //Init a cube in Tray
        LayoutResController.instance.ReturnCube(saveColor);
        //Reset isFilled
        isFilled = false;
    }

    private void ChangeColor(BlockOnTrayController colliderController)
    {
        Color color_new = Color.black;
        UnityEngine.ColorUtility.TryParseHtmlString("#" + colliderController.color, out color_new);
        _meshRenderer.material.color = color_new;
        saveColor = color_new;
    }
}
