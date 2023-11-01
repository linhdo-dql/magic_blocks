using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Flexalon;
using UnityEngine;

public class LayoutResController : MonoBehaviour
{
    public static LayoutResController instance;
    public FlexalonFlexibleLayout layoutBlock;
    public GameObject trayBorder;
    public GameObject blockPrefab;
    public float scaleRefactor;
    private bool _isInitDone;
    public double range;
    private Vector2 touchStartPosition;
    private bool isSliding;
    private Vector3 mousePosition;
    private float lastMousePoint;
    private Vector3 lastMousePosition;
    public bool lockScroll;

    void Awake()
    {
        instance = this;
    }
    public void InitResources(SerializableColor[,] serializableColors)
    {
        scaleRefactor = Camera.main.orthographicSize / 6;
        GetComponent<FlexalonObject>().Scale = new Vector3(scaleRefactor, scaleRefactor, scaleRefactor);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        print(screenPosition);
        int countTmp = 0;
        for (int i = 0; i < serializableColors.GetLength(0); i++)
        {
            for (int j = 0; j < serializableColors.GetLength(1); j++)
            {
                // Mã màu hợp lệ, bạn có thể sử dụng đối tượng màu

                var block = Instantiate(blockPrefab, layoutBlock.transform);
                block.GetComponent<MeshRenderer>().material.color = serializableColors[i, j].ToColor();
                block.GetComponent<BoxCollider>().enabled = true;
                block.GetComponent<BlockOnTrayController>().PopulateData(scaleRefactor, ColorUtility.ToHtmlStringRGBA(serializableColors[i, j].ToColor()), serializableColors[i, j].x, serializableColors[i, j].y);
                countTmp++;

            }
        }
        _isInitDone = true;
        range = ((countTmp + (countTmp - 1) * 0.15) * LayoutResController.instance.scaleRefactor - Camera.main.orthographicSize) / 2;
        print(transform.GetComponentsInChildren<MeshRenderer>().Where(c => c.isVisible).Count());
        var trayScaleFactor = Camera.main.orthographicSize / 17;
        trayBorder.GetComponent<Transform>().localPosition = new Vector3(0, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 3);
        trayBorder.GetComponent<Transform>().localScale = new Vector3(25, trayScaleFactor + 0.05f, 1);
        ShuffleChildren();
    }

    private float GetMaxXPosition(Transform[] transforms)
    {
        float maxX = float.MinValue;

        foreach (Transform t in transforms)
        {
            if (t.position.x > maxX)
            {
                maxX = t.position.x;
            }
        }

        return maxX;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SubtractPos()
    {
        if (LayerBuildStateController.instance.crBuildLayerState == LayerBuildStateController.BuildLayerState.View) return;
        range = ((transform.childCount + (transform.childCount - 1) * 0.15) * scaleRefactor - Camera.main.orthographicSize) / 2;
        if ((transform.childCount + (transform.childCount - 1) * 0.15f) < Camera.main.orthographicSize - 1)
        {
            transform.localPosition = new Vector3(0, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
            lockScroll = true;
        }
        else
        {
            transform.localPosition = new Vector3((float)LayoutResController.instance.range + 0.5f, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
            lockScroll = false;
        }
    }
    public void AddPos()
    {
        if (LayerBuildStateController.instance.crBuildLayerState != LayerBuildStateController.BuildLayerState.Break) return;
        AddAItem();
    }
    public void AddAItem()
    {
        range = ((transform.childCount + (transform.childCount - 1) * 0.15) * scaleRefactor - Camera.main.orthographicSize) / 2;
        if ((transform.childCount + (transform.childCount - 1) * 0.15f) < Camera.main.orthographicSize - 1)
        {
            transform.localPosition = new Vector3(0, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
        }
        else
        {
            lockScroll = false;
            transform.localPosition = new Vector3((float)LayoutResController.instance.range - 0.15f, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (LayerBuildStateController.instance.crControllerState == LayerBuildStateController.ControllerState.Idle)
        {
            if (lockScroll) return;
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                float newXPosition = transform.position.x + delta.x * Time.deltaTime;
                newXPosition = Mathf.Clamp(newXPosition, (float)(-range - 0.5f), (float)(range + 0.5f));
                transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
                lastMousePosition = Input.mousePosition;
            }
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.position - touchStartPosition;
                    float newXPosition = transform.position.x + delta.x * Time.deltaTime;
                    newXPosition = Mathf.Clamp(newXPosition, (float)(-range - 0.5f), (float)(range + 0.5f));
                    transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
                    touchStartPosition = touch.position;
                }
            }
        }
    }

    public void ReturnCube(Color saveColor, int _x, int _y)
    {
        var block = Instantiate(blockPrefab, layoutBlock.transform);
        block.GetComponent<MeshRenderer>().material.color = saveColor;
        block.GetComponent<BoxCollider>().enabled = true;
        block.GetComponent<BlockOnTrayController>().PopulateData(scaleRefactor, ColorUtility.ToHtmlStringRGBA(saveColor), _x, _y);
        block.GetComponent<FlexalonInteractable>().Draggable = LayerBuildStateController.instance.crBuildLayerState != LayerBuildStateController.BuildLayerState.Break;
        AddPos();
    }

    public void LockDrag(bool isLock)
    {
        foreach (Transform tr in transform)
        {
            tr.GetComponent<FlexalonInteractable>().Draggable = isLock;
        }
    }

    public void ShuffleChildren()
    {
        // Lấy số lượng GameObject con trong cha
        int childCount = transform.childCount;

        // Tạo danh sách các chỉ số của GameObject con
        List<int> indices = new List<int>();
        for (int i = 0; i < childCount; i++)
        {
            indices.Add(i);
        }

        // Xáo trộn danh sách chỉ số
        for (int i = 0; i < childCount - 1; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, childCount);
            int temp = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // Sắp xếp lại vị trí của các GameObject con dựa trên thứ tự đã xáo trộn
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(indices[i]);
            child.SetSiblingIndex(i);
        }
    }
    internal void ChangedBlockRes(GameObject blockOnFrameObject)
    {
        var blockOnFrame = blockOnFrameObject.GetComponent<BlockOnFrameController>();
        foreach (BlockOnTrayController blockOnTray in transform.GetComponentsInChildren<BlockOnTrayController>())
        {
            if (blockOnTray.x == blockOnFrame.x && blockOnTray.y == blockOnFrame.y)
            {
                blockOnFrame.CopyBlock(blockOnTray);
                Destroy(blockOnTray.gameObject);
                break;
            }
        }
    }
}

