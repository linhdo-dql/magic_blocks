using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
#if UNITY_EDITOR
public class ColorGridEditorWindow : EditorWindow
{
    private Color[,] colorGrid;
    private int rowCount = 10;
    private int columnCount = 10;
    private string arrayName = "ColorGrid";
    private DefaultAsset folder;
    private string fileName = "";
    private Vector2 scrollPosition;

    [MenuItem("Window/Color Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<ColorGridEditorWindow>("Color Grid Editor");
    }

    private void OnEnable()
    {
        // Khởi tạo mảng 2 chiều với kích thước mặc định là 10x10
        colorGrid = new Color[rowCount, columnCount];
    }

    private void OnGUI()
    {
        GUILayout.Label("Color Grid", EditorStyles.boldLabel);

        // Tạo ScrollView nếu số hàng và số cột vượt quá không gian cửa sổ Editor
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Hiển thị ô chọn màu và gán giá trị màu vào mảng 2 chiều
        for (int y = 0; y < colorGrid.GetLength(1); y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < colorGrid.GetLength(0); x++)
            {
                colorGrid[x, y] = EditorGUILayout.ColorField(colorGrid[x, y], GUILayout.Width(50), GUILayout.Height(50));
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(20);

        // Nhập số hàng và số cột từ bàn phím
        rowCount = EditorGUILayout.IntField("Số hàng:", rowCount);
        columnCount = EditorGUILayout.IntField("Số cột:", columnCount);

        // Kiểm tra nếu số hàng hoặc số cột thay đổi, tạo lại mảng 2 chiều với kích thước mới
        if (rowCount != colorGrid.GetLength(0) || columnCount != colorGrid.GetLength(1))
        {
            CreateColorGrid();
        }

        GUILayout.Space(20);

        // Chọn thư mục lưu trữ
        folder = EditorGUILayout.ObjectField("Chọn thư mục:", folder, typeof(DefaultAsset), false) as DefaultAsset;

        GUILayout.Space(20);

        // Nhập tên file
        fileName = EditorGUILayout.TextField("Layer Index:", fileName);

        GUILayout.Space(20);

        // Nút "Reset" để đặt lại các giá trị về 0
        if (GUILayout.Button("Reset", GUILayout.Width(100)))
        {
            ResetValues();
        }

        GUILayout.Space(20);

        // Nếu nhấn nút "Đọc File", đọc dữ liệu từ file
        if (GUILayout.Button("Đọc File"))
        {
            ReadColorGridFromFile();
        }

        // Nếu nhấn nút "Xuất File", ghi mảng 2 chiều các mã màu ra file JSON
        if (GUILayout.Button("Xuất File"))
        {
            ExportColorGridToJSON();
        }
    }

    private void CreateColorGrid()
    {
        colorGrid = new Color[rowCount, columnCount];
        Debug.Log("Đã tạo mảng 2 chiều với kích thước " + rowCount + " hàng và " + columnCount + " cột!");
    }

    private void ResetValues()
    {
        rowCount = 0;
        columnCount = 0;
        folder = null;
        fileName = "";
    }

    private void ExportColorGridToJSON()
    {
        if (folder != null)
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);
            string filePath = Path.Combine(folderPath, "layer_" + fileName + ".json");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            ColorData colorData = new ColorData();
            colorData.colors = new List<SerializableColor>();

            for (int y = 0; y < colorGrid.GetLength(1); y++)
            {
                for (int x = 0; x < colorGrid.GetLength(0); x++)
                {
                    colorData.colors.Add(new SerializableColor(colorGrid[x, y], x, y));
                }
            }

            string jsonData = JsonConvert.SerializeObject(colorData, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);

            Debug.Log("Xuất File thành công!");
        }
        else
        {
            Debug.LogError("Vui lòng chọn thư mục lưu trữ!");
        }
    }

    private void ReadColorGridFromFile()
    {
        if (folder != null)
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);
            string filePath = Path.Combine(folderPath, "layer_" + fileName + ".json");
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
                        maxRowCount = Mathf.Max(maxRowCount, serializableColor.x);
                        maxColumnCount = Mathf.Max(maxColumnCount, serializableColor.y);
                    }
                    rowCount = maxRowCount + 1;
                    columnCount = maxColumnCount + 1;
                    colorGrid = new Color[rowCount, columnCount];
                    foreach (SerializableColor serializableColor in colorData.colors)
                    {
                        int x = serializableColor.x;
                        int y = serializableColor.y;
                        if (x < rowCount && y < columnCount)
                        {
                            colorGrid[x, y] = serializableColor.ToColor();
                        }
                    }
                    Debug.Log("Đọc File thành công!");
                }
                else
                {
                    Debug.LogError("Dữ liệu không hợp lệ!");
                }
            }
            else
            {
                Debug.LogError("File không tồn tại!");
            }
        }
        else
        {
            Debug.LogError("Vui lòng chọn thư mục lưu trữ!");
        }
    }
}

#endif
[System.Serializable]
public class ColorData
{
    public List<SerializableColor> colors;
}

[System.Serializable]
public class SerializableColor
{
    public float r;
    public float g;
    public float b;
    public float a;
    public int x;
    public int y;

    public SerializableColor(Color color, int xPos, int yPos)
    {
        r = color.r;
        g = color.g;
        b = color.b;
        a = color.a;
        x = xPos;
        y = yPos;
    }

    public Color ToColor()
    {
        return new Color(r, g, b, 1);
    }
}