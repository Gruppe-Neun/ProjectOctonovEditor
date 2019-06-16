using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class editorUI : MonoBehaviour
{

    public RawImage imagePrefab;
    private RawImage[] blockTypes;

    // Start is called before the first frame update
    void Awake()
    {
        string[] types = Enum.GetNames(typeof(Block.BlockType));
        blockTypes = new RawImage[types.Length];
        for(int i = 1; i < types.Length; i++) {
            blockTypes[i] = Instantiate(imagePrefab, this.transform);
            blockTypes[i].transform.localPosition = new Vector3(-800 + i * 120, -400, 0);
            blockTypes[i].uvRect = new Rect(Block.blockUVs[i,0], new Vector2(0.0625f, 0.0625f));
        }
        setActive(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setActive(int num) {
        for(int i = 1; i<blockTypes.Length; i++) {
            blockTypes[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        blockTypes[num].color = new Color(1, 1, 1, 1);
    }
}
