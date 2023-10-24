using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class LayerDataController : MonoBehaviour
{
    public string[,] layerColors;
    public List<SerializableColor> serializableColors;
    private string folderName; // Tên thư mục chứa tệp tin
    private string fileName; // Tên tệp tin
    public static LayerDataController instance;

    void Awake()
    {
        instance = this;
        serializableColors = new List<SerializableColor>();
    }
    // Start is called before the first frame update
    void Start()
    {
        GetDataOfLayer("1", 2);
    }

    public void GetDataOfLayer(string modelName, int layerId)
    {
        folderName = modelName;
        fileName = "layer_" + layerId + ".json";
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_LINUX || UNITY_IOS
        GetColorArrayFromFile();
#else  
        StartCoroutine(GetDataFromFileAndroidWebGL());
#endif

        // string[,] layerColors = new string[,]
        // {
        //     {"2708B2", "00D9FF", "FFFBFB", "E2FF53"},
        //     {"none", "FD2020", "none", "none"},
        //     {"none", "24FF00", "C000D4", "none"},
        //     {"none", "none", "FFA0F9", "none"}
        // };

    }
    private void SetupLayout()
    {
        LayoutBlocksController.instance.GenerateBlock(layerColors, serializableColors);
        Camera.main.orthographicSize = MathF.Max(layerColors.GetLength(0), layerColors.GetLength(1)) + 4;
        LayoutResController.instance.InitResources(layerColors, serializableColors);
        LayoutResController.instance.GetComponent<Transform>().localPosition = new Vector3((float)LayoutResController.instance.range + 1f, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);

        print("zo");
        LayoutFrameDemoController.instance.SetLayoutDemo();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void GetColorArrayFromFile()
    {
        string streamingAssetsPath = "";
        var rowCount = 0;
        var columnCount = 0;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_LINUX
        streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets");
#elif UNITY_IOS
        streamingAssetsPath = Path.Combine(Application.dataPath, "Raw"); 
#endif
        string filePath = Path.Combine(streamingAssetsPath, "Datas", folderName, fileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            ColorData colorData = JsonConvert.DeserializeObject<ColorData>(jsonData);
            if (colorData != null && colorData.colors != null)
            {
                int maxRowCount = 0;
                int maxColumnCount = 0;
                foreach (SerializableColor serializableColor in colorData.colors)
                {
                    serializableColors.Add(serializableColor);
                    maxRowCount = Mathf.Max(maxRowCount, serializableColor.x);
                    maxColumnCount = Mathf.Max(maxColumnCount, serializableColor.y);
                }
                rowCount = maxRowCount + 1;
                columnCount = maxColumnCount + 1;
                layerColors = new string[rowCount, columnCount];
                foreach (SerializableColor serializableColor in colorData.colors)
                {
                    int x = serializableColor.x;
                    int y = serializableColor.y;
                    if (x < rowCount && y < columnCount)
                    {
                        layerColors[x, y] = ColorUtility.ToHtmlStringRGBA(serializableColor.ToColor());
                    }
                }
                Debug.Log("Đọc File thành công!");
            }
            else
            {
                Debug.LogError("Dữ liệu không hợp lệ!");
            }
            SetupLayout();
        }
        else
        {
            Debug.Log("File không tồn tại");
        }

    }

    private IEnumerator GetDataFromFileAndroidWebGL()
    {
        var rowCount = 0;
        var columnCount = 0;
        var streamingAssetsPath = Application.streamingAssetsPath;
        string filePath = Path.Combine(streamingAssetsPath, "Datas", folderName, fileName);
        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonData = www.downloadHandler.text;
                ColorData colorData = JsonConvert.DeserializeObject<ColorData>(jsonData);
                if (colorData != null && colorData.colors != null)
                {
                    int maxRowCount = 0;
                    int maxColumnCount = 0;
                    foreach (SerializableColor serializableColor in colorData.colors)
                    {
                        maxRowCount = Mathf.Max(maxRowCount, serializableColor.x);
                        maxColumnCount = Mathf.Max(maxColumnCount, serializableColor.y);
                    }
                    rowCount = maxRowCount + 1;
                    columnCount = maxColumnCount + 1;
                    layerColors = new string[rowCount, columnCount];
                    foreach (SerializableColor serializableColor in colorData.colors)
                    {
                        int x = serializableColor.x;
                        int y = serializableColor.y;
                        if (x < rowCount && y < columnCount)
                        {
                            layerColors[x, y] = ColorUtility.ToHtmlStringRGBA(serializableColor.ToColor());
                        }
                    }
                    Debug.Log("Đọc File thành công!");
                }
                else
                {
                    Debug.LogError("Dữ liệu không hợp lệ!");
                }
                SetupLayout();
            }
            else
            {
                Debug.Log("Error: " + www.error);
            }
        }
    }
}
