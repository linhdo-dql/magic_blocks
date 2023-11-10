using System.IO;
using Flexalon;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

public class LayoutLayerTrayController : MonoBehaviour
{
    public static LayoutLayerTrayController instance;
    public GameObject borderTray;
    public GameObject layerPrefab;
    public GameObject blockPrefab;
    public Material transparentMaterial;

    void Awake()
    {
        instance = this;
    }

    public void InstantiateLayer(ColorData colorData, string fileName)
    { 
        var layer = Instantiate(layerPrefab, transform);
        int maxRowCount = 0;
        int maxColumnCount = 0;
        string jsonData = BetterStreamingAssets.ReadAllText(fileName);
        colorData = JsonConvert.DeserializeObject<ColorData>(jsonData);
        if (colorData != null && colorData.colors != null)
        { 
            foreach (SerializableColor serializableColor in colorData.colors)
            {
                maxRowCount = Mathf.Max(maxRowCount, serializableColor.x);
                maxColumnCount = Mathf.Max(maxColumnCount, serializableColor.y);
            }
            int rowCount = maxRowCount + 1;
            int columnCount = maxColumnCount + 1;
            layer.GetComponent<FlexalonGridLayout>().Columns = (uint) rowCount;
            layer.GetComponent<FlexalonGridLayout>().Rows = (uint) columnCount;
            foreach (SerializableColor serializableColor in colorData.colors)
            {
                int x = serializableColor.x;
                int y = serializableColor.y;
                if (x < rowCount && y < columnCount)
                {
                    var block = Instantiate(blockPrefab, layer.transform);
                    //Lưu tọa độ block
                    block.GetComponent<BlockOnFrameController>().SavePos(serializableColor.x, serializableColor.y);
                    block.GetComponent<FlexalonInteractable>().enabled = false;
                    block.GetComponent<BoxCollider>().size = Vector3.one;
                    var blockTransform = block.GetComponent<Transform>();
                    if(serializableColor.a == 0)
                    {
                        block.GetComponent<MeshRenderer>().material = transparentMaterial;
                    }
                    else
                    {
                        block.GetComponent<MeshRenderer>().material.color = serializableColor.ToColor();
                    }
                  
                    var blockPos = blockTransform.localPosition;
                    block.transform.position = new Vector3(blockPos.x, blockPos.y, 0.5f);
                }
            }
            //layer.GetComponent<FlexalonObject>().Scale = new Vector3(0.25f, 0.25f, 0.25f);
            Debug.Log("Đọc File thành công!");
        }
        else
        {
            Debug.LogError("Dữ liệu không hợp lệ!");
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (LayerBuildStateController.instance.crControllerState == LayerBuildStateController.ControllerState.Idle)
        // {
        //     if (lockScroll) return;
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         lastMousePosition = Input.mousePosition;
        //     }
        //     else if (Input.GetMouseButton(0))
        //     {
        //         Vector3 delta = Input.mousePosition - lastMousePosition;
        //         float newXPosition = transform.position.x + delta.x * Time.deltaTime;
        //         newXPosition = Mathf.Clamp(newXPosition, (float)(-range - 0.5f), (float)(range + 0.5f));
        //         transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
        //         lastMousePosition = Input.mousePosition;
        //     }
        //     if (Input.touchCount > 0)
        //     {
        //         Touch touch = Input.GetTouch(0);

        //         if (touch.phase == TouchPhase.Began)
        //         {
        //             touchStartPosition = touch.position;
        //         }
        //         else if (touch.phase == TouchPhase.Moved)
        //         {
        //             Vector2 delta = touch.position - touchStartPosition;
        //             float newXPosition = transform.position.x + delta.x * Time.deltaTime;
        //             newXPosition = Mathf.Clamp(newXPosition, (float)(-range - 0.5f), (float)(range + 0.5f));
        //             transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
        //             touchStartPosition = touch.position;
        //         }
        //     }
        // }
    }

    internal void SetBorderTrayLayout()
    {
        borderTray.transform.position = new Vector3(transform.position.x, transform.position.y, 100);
        borderTray.transform.localScale = new Vector3(Camera.main.orthographicSize / 15, Camera.main.orthographicSize, 1);
    }
}
