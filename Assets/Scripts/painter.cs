using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class painter : MonoBehaviour
{

    [SerializeField] Texture2D image = null;

    // Update is called once per frame
    void Update()
    {
        // checks if click was made, if true run the flood fill algorithm
        if (Input.GetMouseButtonDown(0))
        {
            Color c = GameObject.FindGameObjectWithTag("paintable").GetComponent<newMixedColor>().getCurrColor();
            GameObject.FindGameObjectWithTag("paintable").GetComponent<paintingColor>()
                .floodfillAlgo(Input.mousePosition,image, c);
        }
    }
}
