using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarController : MonoBehaviour
{
    public Color FrontColor;
    public Color BGColor;
    public bool showValue;
    public float FirstValue;
    public float SecondValue;
    private TextMeshPro text;
    private Transform Front;
    private Transform Back;
    // Start is called before the first frame update
    void Start()
    {
        text = transform.Find("Text").GetComponent<TextMeshPro>();
        Front = transform.Find("Front");
        Back = transform.Find("BG");
        Front.GetComponent<SpriteRenderer>().color = FrontColor;
        Back.GetComponent<SpriteRenderer>().color = BGColor;
        if (!showValue)
        {
            text.enabled = false; 
        }

    }

    public void UpdateVals(float First, float Second)
    {
        Front = transform.Find("Front");
        text = transform.Find("Text").GetComponent<TextMeshPro>();
        this.FirstValue = First;
        this.SecondValue = Second;
        Front.localScale = new Vector3(FirstValue / SecondValue, 1, 1);
        if (showValue)
        {
            text.SetText(prepareText());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private string prepareText()
    {
        string res = FirstValue.ToString();
        int len = res.Length;
        res += "/" + SecondValue;
        int t = res.Length - len - 1 - SecondValue.ToString().Length;
        for (int i = 0; i < t; i++)
        {
            res = " " + res;
        }
        
        return res;
    }
}
