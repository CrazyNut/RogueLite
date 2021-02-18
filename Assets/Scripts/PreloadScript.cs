using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreloadScript : MonoBehaviour
{
    private TextMeshProUGUI textOBJ;
    public string text = "Loading";
    private int tmpCounter = 0;
    private GameObject LoadCanvas;
    private GameObject GameCanvas;
    // Start is called before the first frame update
    void Start()
    {
        LoadCanvas = GameObject.FindGameObjectWithTag("LoadCanvas");
        GameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
        textOBJ = this.GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateText(0.5f));
    }

    private IEnumerator UpdateText(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        if (tmpCounter == 3)
        {
            tmpCounter = 0;
            textOBJ.text = text;
        }
        else
        {
            tmpCounter++;
            textOBJ.text += ".";
        }
        StartCoroutine(UpdateText(0.5f));
    }

    public void EndPreloader()
    {
        GameCanvas.GetComponent<Canvas>().enabled = true;
        LoadCanvas.GetComponent<Canvas>().enabled = false;
    }
}
