using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayController : MonoBehaviour
{

    public static TrayController instance;
    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    public void SetPosition()
    {
        Vector3 spriteSize = GetComponent<SpriteRenderer>().bounds.size;
        // Tính toán vị trí mới cho Sprite Renderer
        Vector3 newPosition = new Vector3(transform.position.x, spriteSize.y / 2f, transform.position.z);
        transform.position = new Vector3(newPosition.x, -(Camera.main.orthographicSize - newPosition.y), newPosition.z);
    }
}
