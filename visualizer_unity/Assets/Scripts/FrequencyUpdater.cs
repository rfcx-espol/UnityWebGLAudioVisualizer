using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyUpdater : MonoBehaviour {

    public OutputVis output;
    public SpectrumVis spectrum;
    public UnityEngine.UI.Text update_rate;

	public void update_time()
    {
        float r = 0.1f;
        try{
            r = float.Parse(update_rate.text);
        }catch(System.Exception){}

        output.updateTime = r;
        spectrum.updateTime = r;
    }
}
