using System;
using Flexalon;
using UnityEngine;

public class LayoutFrameDemoController : MonoBehaviour
{
    public static LayoutFrameDemoController instance;
    public GameObject border;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    public void SetLayoutDemo()
    {
        var borderDemoScaleFactor = Camera.main.orthographicSize / 17;
        transform.GetComponent<FlexalonObject>().Scale = new Vector3(0.22f, 0.22f, 0.22f);
        Transform objectTransform = transform;

        // Lấy kích thước của màn hình
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Đặt vị trí của GameObject ở góc trên bên trái
        Vector3 topLeftPosition = new(screenWidth, screenHeight, 33);
        if (Camera.main.orthographicSize <= 8)
        {
            transform.position = Camera.main.ScreenToWorldPoint(topLeftPosition) + new Vector3(-1.5f, -1.5f, 49);
        }
        else
        {
            transform.position = Camera.main.ScreenToWorldPoint(topLeftPosition) + new Vector3(-1 - 2 * borderDemoScaleFactor, -1 - 2 * borderDemoScaleFactor, 49);
        }

        border.transform.localScale = new Vector3(borderDemoScaleFactor, borderDemoScaleFactor, borderDemoScaleFactor);
        border.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
