using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutModelController : MonoBehaviour
{
    public static LayoutModelController instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTransform()
    {
        transform.position = new Vector3(Camera.main.orthographicSize / 7, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
