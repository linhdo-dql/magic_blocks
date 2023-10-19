using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
#if UNITY_EDITOR
public class ColorGridEditorWindow : EditorWindow
{
    private Color[,] colorGrid;
    private int rowCount = 10;
    private int columnCount = 10;
    private string arrayName = "ColorGrid";
    private DefaultAsset folder;
    private int selectedLayer = 0;
    private bool createNewFolder = false;
    private string newFolderName = "";
    private Vector2 scrollPosition;

    [MenuItem("Window/Color Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<ColorGridEditorWindow>("Color Grid Editor");
    }

    private void OnEnable()
    {
        // Khởi tạo mảng 2 chiều với kích thước mặc định là 10x10
        colorGrid = new Color[columnCount, rowCount];
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
                colorGrid[x, y] = EditorGUILayout.ColorField(colorGrid[x, y], GUILayout.Width(40), GUILayout.Height(40));
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(20);

        // Nhập số hàng và số cột từ bàn phím
        rowCount = EditorGUILayout.IntField("Số hàng:", rowCount);
        columnCount = EditorGUILayout.IntField("Số cột:", columnCount);

        // Kiểm tra nếu số hàng hoặc số cột thay đổi, tạo lại mảng 2 chiều với kích thước mới
        if (rowCount != colorGrid.GetLength(1) || columnCount != colorGrid.GetLength(0))
        {
            CreateColorGrid();
        }

        GUILayout.Space(20);

        // Chọn thư mục lưu trữ
        folder = EditorGUILayout.ObjectField("Chọn thư mục:", folder, typeof(DefaultAsset), false) as DefaultAsset;

        GUILayout.Space(20);

        // Checkbox cho phép tạo thư mục mới
        EditorGUI.BeginDisabledGroup(folder != null);
        createNewFolder = EditorGUILayout.Toggle("Tạo thư mục mới", createNewFolder);
        EditorGUI.EndDisabledGroup();

        // Nếu checkbox được chọn, hiển thị ô nhập dữ liệu để nhập tên thư mục mới
        if (createNewFolder)
        {
            newFolderName = EditorGUILayout.TextField("Tên thư mục mới:", newFolderName);
        }

        GUILayout.Space(20);

        // Nhập tên layer
        selectedLayer = EditorGUILayout.IntField("Layer:", selectedLayer);

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

        // Nếu nhấn nút "Xuất File", ghi mảng 2 chiều các mã màu ra file CSV
        if (GUILayout.Button("Xuất File"))
        {
            ExportColorGridToCSV();
        }
    }

    private void CreateColorGrid()
    {
        colorGrid = new Color[columnCount, rowCount];
        ResetColorGrid();
        Debug.Log("Đã tạo mảng 2 chiều với kích thước " + columnCount + " cột và " + rowCount + " hàng!");
    }

    private void ResetColorGrid()
    {
        for (int y = 0; y < colorGrid.GetLength(1); y++)
        {
            for (int x = 0; x < colorGrid.GetLength(0); x++)
            {
                colorGrid[x, y] = Color.clear;
            }
        }
    }

    private void ResetValues()
    {
        rowCount = 0;
        columnCount = 0;
        folder = null;
        selectedLayer = 0;
        createNewFolder = false;
        newFolderName = "";
    }

    private void ExportColorGridToCSV()
    {
        string folderPath = "";
        if (folder != null)
        {
            folderPath = AssetDatabase.GetAssetPath(folder);
        }
        else if (createNewFolder && !string.IsNullOrEmpty(newFolderName))
        {
            folderPath = Path.Combine(Application.dataPath, "StreamingAssets/Datas", newFolderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        if (!string.IsNullOrEmpty(folderPath))
        {
            string fileName = "colors_" + selectedLayer.ToString() + ".csv";
            string filePath = Path.Combine(folderPath, fileName);

            StringBuilder csv = new StringBuilder();

            for (int y = 0; y < colorGrid.GetLength(1); y++)
            {
                for (int x = 0; x < colorGrid.GetLength(0); x++)
                {
                    Color color = colorGrid[x, y];
                    string colorCode = ColorUtility.ToHtmlStringRGB(color);
                    csv.Append(colorCode);
                    if (x < colorGrid.GetLength(0) - 1)
                    {
                        csv.Append(",");
                    }
                }
                csv.AppendLine();
            }

            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);

            Debug.Log("Xuất File thành công!");
        }
        else
        {
            Debug.LogError("Vui lòng chọn thư mục lưu trữ hoặc tạo thư mục mới!");
        }
    }

    private void ReadColorGridFromFile()
    {
        if (folder != null)
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);
            string fileName = "colors_" + selectedLayer.ToString() + ".csv";
            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {

                string[] lines = System.IO.File.ReadAllLines(filePath);
                rowCount = lines.Length;
                columnCount = lines[0].Split(',').Length;
                colorGrid = new Color[rowCount, columnCount];

                for (int y = 0; y < rowCount; y++)
                {
                    string[] values = lines[y].Split(',');

                    for (int x = 0; x < columnCount; x++)
                    {
                        Color color;
                        if (ColorUtility.TryParseHtmlString("#" + values[x], out color))
                        {
                            colorGrid[x, y] = color;
                        }
                    }
                }

                Debug.Log("Đọc File thành công!");
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