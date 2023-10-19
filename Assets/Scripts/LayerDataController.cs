using System;
using System.IO;
using System.Linq;
using Flexalon;
using UnityEngine;
using UnityEngine.Networking;

public class LayerDataController : MonoBehaviour
{
    public string[,] layerColors;
    private string folderName; // Tên thư mục chứa tệp tin
    private string fileName; // Tên tệp tin
    public static LayerDataController instance;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetDataOfLayer("ken", 0);
    }

    public void GetDataOfLayer(string modelName, int layerId)
    {
        folderName = modelName;
        fileName = "colors_" + layerId + ".csv";
        GetColorArrayFromFile();
        string[,] layerColors = new string[,]
        {
            {"2708B2", "00D9FF", "FFFBFB", "E2FF53"},
            {"none", "FD2020", "none", "none"},
            {"none", "24FF00", "C000D4", "none"},
            {"none", "none", "FFA0F9", "none"}
        };
        LayoutBlocksController.instance.GenerateBlock(layerColors);
        Camera.main.orthographicSize = MathF.Max(layerColors.GetLength(0), layerColors.GetLength(1)) + 2;
        LayoutResController.instance.InitResources(layerColors);
        LayoutResController.instance.GetComponent<Transform>().localPosition = new Vector3((float)LayoutResController.instance.range + 0.5f, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 0);
        var trayScaleFactor = Camera.main.orthographicSize / 17;
        TrayController.instance.GetComponent<Transform>().localPosition = new Vector3(0, -Camera.main.orthographicSize + 1 + LayoutResController.instance.scaleRefactor / 2, 3);
        TrayController.instance.GetComponent<Transform>().localScale = new Vector3(25, trayScaleFactor + 0.05f, 1);

        print(LayoutResController.instance.range);
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void GetColorArrayFromFile()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Datas", folderName, fileName);
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            int rowCount = lines.Length;
            int columnCount = lines[0].Split(',').Length;
            layerColors = new string[columnCount, rowCount];
            for (int y = 0; y < rowCount; y++)
            {
                string[] values = lines[y].Split(',');
                for (int x = 0; x < columnCount; x++)
                {
                    layerColors[x, y] = values[x];
                }
            }
            Debug.Log("Đọc File thành công!");
        }
        else
        {
            Debug.LogError("File không tồn tại!");
        }
        // if (File.Exists(filePath))
        // {   

        //     StreamReader reader = new StreamReader(filePath);

        //     // Đọc số hàng và số cột từ file
        //     int columnCount = int.Parse(reader.ReadLine().Trim());
        //     int rowCount = int.Parse(reader.ReadLine().Trim());

        //     // Khởi tạo mảng 2 chiều chuỗi với kích thước từ file
        //     layerColors = new string[columnCount, rowCount];

        //     // Đọc mảng 2 chiều mã màu từ file
        //     for (int y = 0; y < rowCount; y++)
        //     {
        //         string[] colorCodes = reader.ReadLine().Trim().Split(' ');

        //         for (int x = 0; x < columnCount; x++)
        //         {
        //             layerColors[x, y] = colorCodes[x];
        //         }
        //     }

        //     reader.Close();
        // }
        // else
        // {
        //     Debug.LogError("File không tồn tại: " + filePath);
        // }
    }
}
