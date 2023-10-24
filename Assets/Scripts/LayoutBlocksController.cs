using System;
using System.Collections;
using System.Collections.Generic;
using Flexalon;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class LayoutBlocksController : MonoBehaviour
{
    public static LayoutBlocksController instance;
    public FlexalonGridLayout layoutBlock;
    public GameObject blockPrefab;
    public Material transparentMaterial;
    public Material blockMaterial;
    void Awake()
    {
        instance = this;
    }
    public void GenerateBlock(string[,] layerColors)
    {
        layoutBlock.Rows = (uint)layerColors.GetLength(1);
        layoutBlock.Columns = (uint)layerColors.GetLength(0);
        for (int i = 0; i < layoutBlock.Columns; i++)
        {
            for (int j = 0; j < layoutBlock.Rows; j++)
            {
                var block = Instantiate(blockPrefab, layoutBlock.transform);
                var blockTransform = block.GetComponent<Transform>();
                //                print(layerColors[i, j]);
                // Mã màu hợp lệ, bạn có thể sử dụng đối tượng màu
                Color color;
                if (ColorUtility.TryParseHtmlString("#" + layerColors[i, j], out color))
                {

                    block.GetComponent<MeshRenderer>().material = blockMaterial;
                    block.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.25f);
                }
                else
                {
                    block.GetComponent<MeshRenderer>().material = transparentMaterial;
                    block.GetComponent<BlockOnFrameController>().isTemp = true;
                }
                var blockPos = blockTransform.localPosition;
                block.GetComponent<Transform>().position = new Vector3(blockPos.x, blockPos.y, 0.5f);
            }
        }
    }


    public void SetEnableColliders(bool value)
    {
        foreach (Transform trf in transform)
        {
            var collider = trf.GetComponent<BoxCollider>();
            var colliderController = trf.GetComponent<BlockOnFrameController>();
            if (colliderController.isFilled)
            {
                collider.enabled = true;
                collider.size = Vector3.one;
            }
            else if (colliderController.isTemp)
            {
                collider.enabled = false;
            }
            else
            {
                collider.enabled = value;
                collider.size = new Vector3(1, 1, 5);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public Transform rotationCenter;
    public float rotateSpeed = 5f;
    private Vector2 touchStart;
    public float rotationSpeed = 2f;
    void Update()
    {

    }

    internal void Reset()
    {
        transform.position = new Vector3(0, 0, 2);
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.localScale = Vector3.one;
        GetComponent<Rotatable>().speed = 0;
    }
}
