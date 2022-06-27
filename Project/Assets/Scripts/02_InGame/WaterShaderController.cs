using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterShaderController : MonoBehaviour
{
    private Material mat;
    float x = .0f;
    float y = .0f;

    private void Awake()
    {
        mat = GetComponent<Image>().material;
    }

    private void Update()
    {
        x += Time.deltaTime * Random.Range(-.1f, .6f);
        y += Time.deltaTime * Random.Range(-.1f, .6f);

        x %= 1;
        y %= 1;

        mat.SetTextureOffset("_BumpMap", new Vector2(x, y));
    }
}
