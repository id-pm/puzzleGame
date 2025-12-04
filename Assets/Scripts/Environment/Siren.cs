using System.Collections.Generic;
using UnityEngine;

public class Siren : MonoBehaviour
{
    public Light[] redLight;
    public Light[] blueLight;
    public float flashInterval = 0.2f;
    public Color redColor = Color.red;
    public Color blueColor = Color.blue;

    private void Start()
    {
        foreach(Light light in redLight)
        {
            light.color = redColor;
        }
        foreach(Light light in blueLight)
        {
            light.color = blueColor;
        }
        StartCoroutine(FlashSiren());
    }

    private System.Collections.IEnumerator FlashSiren()
    {
        bool isRed = true;
        while (true)
        {
            for(int i = 0; i < redLight.Length; i++)
            {
                redLight[i].enabled = isRed;
            }
            for(int i = 0; i < blueLight.Length; i++)
            {
                blueLight[i].enabled = !isRed;
            }
            isRed = !isRed;
            yield return new WaitForSeconds(flashInterval);
        }
    }
}
