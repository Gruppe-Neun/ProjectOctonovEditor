using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class editorUI : MonoBehaviour
{

    public RawImage imagePrefab;

    public int active = 0;
    // Start is called before the first frame update
    void Start()
    {
        string[] types = Enum.GetNames(typeof(Block.BlockType));
        for(int i = 0; i < types.Length; i++) {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
