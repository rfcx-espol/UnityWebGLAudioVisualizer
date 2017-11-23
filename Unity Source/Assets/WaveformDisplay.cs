using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WaveformDisplay : MonoBehaviour {


    public int width = 500; // texture width 
    public int height = 100; // texture height 
    public Color backgroundColor = Color.black;
    public Color waveformColor = Color.green;
    public int size = 2048; // size of sound segment displayed in texture

    public int channel;

    private Color[] blank; // blank image array 
    private Texture2D texture;
    private float[] samples; // audio samples array
    float timer = 0.0f;

    public Text debugText;

    public BrowserCommunication comm;

    void Start()
    {

        // create the samples array 
        samples = new float[size];

        // create the texture and assign to the guiTexture: 
        texture = new Texture2D(width, height);

        GetComponent<RawImage>().texture = texture;

        // create a 'blank screen' image 
        blank = new Color[width * height];

        for (int i = 0; i < blank.Length; i++)
        {
            blank[i] = backgroundColor;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 0.1f)
        {
            GetCurWave();
            timer = 0.0f;
            
        }

    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        //Debug.Log(start);
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    void GetCurWave()
    {
        // clear the texture 
        texture.SetPixels(blank, 0);

        // get samples from channel 0 (left) 
        //audioSource.GetOutputData(samples, channel);
        //float[] spectrum = new float[256];
        
        StringBuilder str = new StringBuilder();
        samples = comm.GetSamples();

        for(int i = 0; i < 10; i++)
        {
            str.Append(samples[i] + "\n");
        }

        debugText.text = str.ToString();

        // draw the waveform 
        for (int i = 1; i < samples.Length; i++)
        {
            texture.SetPixel((int)(width * i / size), (int)(height * (samples[i] + 1f) / 2f), waveformColor);
            //texture.SetPixel((int)(Mathf.Log(spectrum[i - 1]) + 10), (int)(Mathf.Log(spectrum[i]) + 10), waveformColor);
        } // upload to the graphics card 

        texture.Apply();
    }
}
