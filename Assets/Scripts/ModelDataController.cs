using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flexalon;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ModelDataController : MonoBehaviour
{
    public static ModelDataController instance;
    private float _cameraSize;

    public string[] files;
    int maxRowCount = 0;
    int maxColumnCount = 0;
    public LayoutLayerTrayController layoutLayerTrayController;
    void Awake()
    {
        instance = this;
        BetterStreamingAssets.Initialize();
        GetAllLayer("1");
    }
    // Start is called before the first frame update
    public List<ColorData> colorDatas = new List<ColorData>();
    private int count;

    void Start()
    {

        //ShowCurrentLayer("2");
    }

    private void ShowCurrentLayer(string v)
    {
        throw new NotImplementedException();
    }

    public void GetAllLayer(string modelName)
    {
        count = 0;
        files = BetterStreamingAssets.GetFiles("Users/" + modelName, "*.json", SearchOption.AllDirectories);
        foreach (string f in files)
        {
            GetColorArrayFromFile(f);
        }
        SetupLayout();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GetColorArrayFromFile(string file)
    {
            string jsonData = BetterStreamingAssets.ReadAllText(file);
            ColorData colorData = JsonConvert.DeserializeObject<ColorData>(jsonData);
            foreach (SerializableColor serializableColor in colorData.colors)
            {
                maxRowCount = Mathf.Max(maxRowCount, serializableColor.x);
                maxColumnCount = Mathf.Max(maxColumnCount, serializableColor.y);
            }
            _cameraSize = MathF.Max(maxRowCount, maxColumnCount) + 1;
            layoutLayerTrayController.InstantiateLayer(colorData, file);
    }

    private void SetupLayout()
    {
        Camera.main.orthographicSize = MathF.Max(maxRowCount, maxColumnCount) + 2; ;
        print(Camera.main.orthographicSize);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        float leftLimit = worldPoint.x;
        float topLimit = worldPoint.y;
        var layoutLayerTrayTransform = layoutLayerTrayController.transform;
        layoutLayerTrayTransform.position = new Vector3(leftLimit + Camera.main.orthographicSize / 6, -topLimit + transform.childCount - Camera.main.orthographicSize / 3, 0);
        layoutLayerTrayController.SetBorderTrayLayout();
        LayoutModelController.instance.SetTransform();
    }
}


public class LayerData
{
    public ColorData colorData;
    public string layerName;
}
