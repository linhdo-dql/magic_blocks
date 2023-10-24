using System;
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
    public void InitResources(string[,] layerColors, System.Collections.Generic.List<SerializableColor> serializableColors)
    {
        scaleRefactor = Camera.main.orthographicSize / 6;
        GetComponent<FlexalonObject>().Scale = new Vector3(scaleRefactor, scaleRefactor, scaleRefactor);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        print(screenPosition);
        int countTmp = 0;
        for (int i = 0; i < layerColors.GetLength(1); i++)
        {
            for (int j = 0; j < layerColors.GetLength(0); j++)
            {
                // Mã màu hợp lệ, bạn có thể sử dụng đối tượng màu
                Color color;
                if (ColorUtility.TryParseHtmlString("#" + layerColors[j, i], out color))
                {
                    var block = Instantiate(blockPrefab, layoutBlock.transform);
                    block.GetComponent<MeshRenderer>().material.color = color;
                    block.GetComponent<BoxCollider>().enabled = true;
                    block.GetComponent<BlockOnTrayController>().PopulateData(scaleRefactor, layerColors[j, i], serializableColors[countTmp].x, serializableColors[countTmp].y);
                    countTmp++;
                }
            }
        }
        _isInitDone = true;
        range = ((countTmp + (countTmp - 1) * 0.15) * LayoutResController.instance.scaleRefactor - Camera.main.orthographicSize) / 2;
        print(transform.GetComponentsInChildren<MeshRenderer>().Where(c => c.isVisible).Count());
        var trayScaleFactor = Camera.main.orthographicSize / 17;
        trayBorder.GetComponent<Transform>().localPosition = new Vector3(0, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 3);
        trayBorder.GetComponent<Transform>().localScale = new Vector3(25, trayScaleFactor + 0.05f, 1);
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
        if ((transform.childCount + (transform.childCount - 1) * 0.15f) < Camera.main.orthographicSize)
        {
            transform.localPosition = new Vector3(0, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
            lockScroll = true;
        }
        else
        {
            transform.localPosition = new Vector3((float)LayoutResController.instance.range + 0.5f, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
            lockScroll = false;
        }
        print(range);

        GetComponent<FlexalonObject>().ForceUpdate();
        GetComponent<FlexalonFlexibleLayout>().ForceUpdate();
    }
    public void AddPos()
    {
        if (LayerBuildStateController.instance.crBuildLayerState != LayerBuildStateController.BuildLayerState.Break) return;
        range = ((transform.childCount + (transform.childCount - 1) * 0.15) * scaleRefactor - Camera.main.orthographicSize) / 2;
        if ((transform.childCount + (transform.childCount - 1) * 0.15f) < Camera.main.orthographicSize)
        {
            transform.localPosition = new Vector3(0, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
        }
        else
        {
            lockScroll = false;
            transform.localPosition = new Vector3((float)LayoutResController.instance.range - 0.15f, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
        }
        print(range);
        GetComponent<FlexalonObject>().ForceUpdate();
        GetComponent<FlexalonFlexibleLayout>().ForceUpdate();
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

    public void ReturnCube(Color saveColor, int x, int y)
    {
        var block = Instantiate(blockPrefab, layoutBlock.transform);
        block.GetComponent<MeshRenderer>().material.color = saveColor;
        block.GetComponent<BoxCollider>().enabled = true;
        block.GetComponent<BlockOnTrayController>().PopulateData(scaleRefactor, ColorUtility.ToHtmlStringRGBA(saveColor), x, y);
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

    internal void ChangedBlockRes(GameObject blockOnFrame)
    {
        //
    }
}

