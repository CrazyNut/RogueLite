using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Color FrontColor;
    public Color BGColor;
    public bool showValue;
    private float FirstValue = 100;
    private float SecondValue = 100;
    private TextMeshPro text;
    private Transform Front;
    private Transform Back;
    // Start is called before the first frame update
    void Start()
    {
        text = transform.Find("Text").GetComponent<TextMeshPro>();
        Front = transform.Find("Value");
        Back = this.transform;
        Front.GetComponent<Image>().color = FrontColor;
        Back.GetComponent<Image>().color = BGColor;
        if (!showValue)
        {
            text.enabled = false;
        }

    }

    public void InstVals(float First, float Second)
    {
        text = transform.Find("Text").GetComponent<TextMeshPro>();
        Front = transform.Find("Value");
        Back = this.transform;
        this.UpdateVals(First, Second);
    }

    public void UpdateVals(float First, float Second)
    {
        this.FirstValue = First;
        this.SecondValue = Second;
        Front.localScale = new Vector3(First / Second, 1, 1);
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
