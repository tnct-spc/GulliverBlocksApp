using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    private struct Block
    {
        public Vector3 position;
        public int ID;

        public Block(float x, float y, float z, int id)
        {
            position = new Vector3(x, y, z);
            ID = id;
        }
    }

    private Block[] block_list = {
        new Block(0.0f, 0.0f, 0.0f, 1),
        new Block(3.0f, 0.0f, 3.3f, 2),
        new Block(5.0f, 0.0f, 5.0f, 3),
    };

    void placeBlock(Block[] block_list)
    {
        Object cube = (GameObject)Resources.Load("Cube");
        for (int i = 0; i < block_list.Length; i++)
        {
            Instantiate(cube, block_list[i].position, Quaternion.identity);
        }
    }

    private void Start()
    {
        placeBlock(block_list);
    }
}
