using System;
using System.Collections;
using System.Collections.Generic;
using Flexalon;
using Unity.Barracuda;
using UnityEngine;

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
                //                print(layerColors[i, j]);
                // Mã màu hợp lệ, bạn có thể sử dụng đối tượng màu
                Color color;
                if (ColorUtility.TryParseHtmlString("#" + layerColors[i, j], out color))
                {
                    block.GetComponent<MeshRenderer>().material = blockMaterial;
                }
                else
                {
                    block.GetComponent<MeshRenderer>().material = transparentMaterial;
                    block.GetComponent<BlockOnFrameController>().isTemp = true;
                }
                var blockPos = block.GetComponent<Transform>().localPosition;
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
        transform.eulerAngles = new Vector3(10, 0, 0);
        transform.localScale = Vector3.one;
        GetComponent<Rotatable>().speed = 0;
    }
}
