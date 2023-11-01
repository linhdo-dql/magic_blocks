using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
    public void GenerateBlock(SerializableColor[,] serializableColors)
    {
        layoutBlock.Rows = layoutBlockDemo.Rows = (uint)serializableColors.GetLength(1);
        layoutBlock.Columns = layoutBlockDemo.Columns = (uint)serializableColors.GetLength(0);
        int count = 0;
        for (int i = 0; i < layoutBlock.Columns; i++)
        {
            for (int j = 0; j < layoutBlock.Rows; j++)
            {
                var block = Instantiate(blockPrefab, layoutBlock.transform);
                //Lưu tọa độ block
                block.GetComponent<BlockOnFrameController>().SavePos(serializableColors[i, j].x, serializableColors[i, j].y);
                count++;
                var blockDemo = Instantiate(blockPrefab, layoutBlockDemo.transform);
                var blockTransform = block.GetComponent<Transform>();
                var blockDemoTransform = blockDemo.GetComponent<Transform>();
                //                print(layerColors[i, j]);
                // Mã màu hợp lệ, bạn có thể sử dụng đối tượng màu
                block.GetComponent<MeshRenderer>().material = blockMaterial;
                blockDemo.GetComponent<MeshRenderer>().material.color = serializableColors[i, j].ToColor();

                // else
                // {
                //     block.GetComponent<MeshRenderer>().material = transparentMaterial;
                //     block.GetComponent<BlockOnFrameController>().isTemp = true;
                // }
                var blockPos = blockTransform.localPosition;
                block.GetComponent<Transform>().position = new Vector3(blockPos.x, blockPos.y, 0.5f);

                var blockDemoPos = blockDemoTransform.localPosition;
                blockDemo.GetComponent<Transform>().position = new Vector3(blockDemoPos.x, blockDemoPos.y, 0.5f);
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
    public GameObject blockResPrefab;
    public FlexalonGridLayout layoutBlockDemo;
    private float _initialFingersDistance;
    private Vector3 _initialScale;
    private float _minScaleFactor = 0.5f;
    private float _maxScaleFactor = 2.0f;

    void Update()
    {

        if (LayerBuildStateController.instance.crBuildLayerState == LayerBuildStateController.BuildLayerState.View)
        {
            if (Input.touches.Length == 2)
            {
                GetComponent<Rotatable>().speed = 0;
                Touch t1 = Input.touches[0];
                Touch t2 = Input.touches[1];

                if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
                {
                    _initialFingersDistance = Vector2.Distance(t1.position, t2.position);
                    _initialScale = transform.localScale;
                }
                else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
                {
                    float currentFingersDistance = Vector2.Distance(t1.position, t2.position);
                    float scaleFactor = currentFingersDistance / _initialFingersDistance;
                    if (_minScaleFactor < (_initialScale * scaleFactor).x && (_initialScale * scaleFactor).x < _maxScaleFactor)
                        transform.localScale = _initialScale * scaleFactor;
                }
            }
            if (Input.touches.Length == 1)
            {
                GetComponent<Rotatable>().speed = 0.3f;
            }
        }
    }

    internal void Reset()
    {
        transform.position = new Vector3(0, 0, 2);
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.localScale = Vector3.one;
        GetComponent<Rotatable>().speed = 0;
    }

    public void ChangedBlockRes(GameObject blockOnTrayGo)
    {
        var blockOnTray = blockOnTrayGo.GetComponent<BlockOnTrayController>();
        foreach (BlockOnFrameController blockOnFrame in transform.GetComponentsInChildren<BlockOnFrameController>())
        {
            if (blockOnTray.x == blockOnFrame.x && blockOnTray.y == blockOnFrame.y)
            {
                blockOnFrame.CopyBlock(blockOnTray);
                Destroy(blockOnTrayGo);
                break;
            }
        }
    }

    public void ReturnAllCubes()
    {
        foreach (BlockOnFrameController blockOnFrame in transform.GetComponentsInChildren<BlockOnFrameController>())
        {
            if (blockOnFrame.isFilled)
            {
                blockOnFrame.Clicked();
            }
        }
        LayoutResController.instance.ShuffleChildren();
    }
}
