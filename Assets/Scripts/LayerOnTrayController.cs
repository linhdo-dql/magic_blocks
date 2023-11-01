using System.Collections;
using System.Collections.Generic;
using Flexalon;
using UnityEngine;

public class LayerOnTrayController : MonoBehaviour
{
    public FlexalonGridLayout layoutBlock;
    public GameObject blockPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void PopulateData(SerializableColor[,] serializableColors, string layerId)
    {
        GenerateLayer(serializableColors);
    }
    private void GenerateLayer(SerializableColor[,] serializableColors)
    {
        layoutBlock.Rows = (uint)serializableColors.GetLength(1);
        layoutBlock.Columns = (uint)serializableColors.GetLength(0);
        int count = 0;
        for (int i = 0; i < layoutBlock.Columns; i++)
        {
            for (int j = 0; j < layoutBlock.Rows; j++)
            {
                var block = Instantiate(blockPrefab, layoutBlock.transform);
                //Lưu tọa độ block
                block.GetComponent<BlockOnFrameController>().SavePos(serializableColors[i, j].x, serializableColors[i, j].y);
                count++;
                block.GetComponent<FlexalonInteractable>().enabled = false;
                block.GetComponent<BoxCollider>().size = Vector3.one;
                var blockTransform = block.GetComponent<Transform>();
                //                print(layerColors[i, j]);
                // Mã màu hợp lệ, bạn có thể sử dụng đối tượng màu
                block.GetComponent<MeshRenderer>().material.color = serializableColors[i, j].ToColor();
                var blockPos = blockTransform.localPosition;
                block.transform.position = new Vector3(blockPos.x, blockPos.y, 0.5f);

            }
        }
        gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }
    // Update is called once per frame
    void Update()
    {
       // transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
