using Flexalon;
using UnityEngine;

public class LayoutLayerTrayController : MonoBehaviour
{
    public static LayoutLayerTrayController instance;
    public GameObject borderTray;
    public GameObject layerPrefab;
    private SerializableColor[,] _serializableColors;
    private Vector3 lastMousePosition;
    private int range;
    private Vector2 touchStartPosition;
    private bool lockScroll;

    void Awake()
    {
        instance = this;
    }

    public void InstantiateLayer(ColorData colorData, string fileName)
    {
        int rowCount = 0;
        int columnCount = 0;
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
            _serializableColors = new SerializableColor[rowCount, columnCount];
            foreach (SerializableColor serializableColor in colorData.colors)
            {
                int x = serializableColor.x;
                int y = serializableColor.y;
                if (x < rowCount && y < columnCount)
                {
                    _serializableColors[x, y] = serializableColor;
                }
            }
            var layer = Instantiate(layerPrefab, transform);
            layer.GetComponent<LayerOnTrayController>().PopulateData(_serializableColors, fileName);
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
