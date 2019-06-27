using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeblocks : MonoBehaviour
{
    Vector3[] block_position = {
        new Vector3 (0.0f, 0.0f, 0.0f),
        new Vector3 (3.0f, 0.0f, 3.3f),
        new Vector3 (5.0f, 0.0f, 5.0f)
    };

    // Start is called before the first frame update
    void Start()
    {
        Object cube = (GameObject)Resources.Load("Cube");
        for(int i = 0; i < block_position.Length; i++)
        {
            Instantiate(cube, block_position[i] , Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
