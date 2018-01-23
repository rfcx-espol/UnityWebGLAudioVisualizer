using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavesController : MonoBehaviour {

    public OutputVis outputWave;
    public SpectrumVis spectrumWave;

    public Text playing_channel;
    public Image panel;

    void Start()
    {
        startScaleCanvas = transform.localScale;
        startPositionCanvas = transform.localPosition;
    }

	public void doStart()
    {
        outputWave.do_start();
        spectrumWave.do_start();
    }

    public void setSourceName(string name)
    {
        playing_channel.text = name;
    }

    public void Enlarge()
    {
        if (transform.localPosition.x == 0)
        {
            transform.localPosition = startPositionCanvas;
            transform.localScale = startScaleCanvas;
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 1f);
        }
        else
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localScale = new Vector3(0.65f, 0.65f, 1f);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0.5f);
        }

    }
    Vector3 startPositionCanvas;
    Vector3 startScaleCanvas;
}
