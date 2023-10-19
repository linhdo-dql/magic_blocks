using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform content;
    public float cubeSpacing = 10f;

    private float cubeWidth;
    private int cubeCount;
    private float contentWidth;
    private Vector2 lastMousePosition;

    private void Start()
    {
        cubeWidth = cubePrefab.transform.localScale.x;
        cubeCount = Mathf.CeilToInt(content.localScale.x / (cubeWidth + cubeSpacing));
        contentWidth = cubeCount * (cubeWidth + cubeSpacing) - cubeSpacing;

        for (int i = 0; i < cubeCount; i++)
        {
            GameObject cube = Instantiate(cubePrefab, content);
            cube.transform.localPosition = new Vector3((cubeWidth + cubeSpacing) * i, 0f, 0f);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
            content.position += new Vector3(mouseDelta.x, 0f, 0f) * 0.01f;

            if (content.position.x < -contentWidth / 2f)
            {
                content.position = new Vector3(-contentWidth / 2f, content.position.y, content.position.z);
            }
            else if (content.position.x > contentWidth / 2f)
            {
                content.position = new Vector3(contentWidth / 2f, content.position.y, content.position.z);
            }

            lastMousePosition = Input.mousePosition;
        }
    }
}