using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class BrowserCommunication : MonoBehaviour {

    float[] myArraySamples;
    float[] myArrayFrequency;
    

    [DllImport("__Internal")]
    private static extern void PrintFloatArray(float[] array, int size);

    [DllImport("__Internal")]
    private static extern void PrintFloatArrayFreq(float[] array, int size);

    [DllImport("__Internal")]
    private static extern void SelectStation(int x);

    [DllImport("__Internal")]
    private static extern void StopStation();

    void Start()
    {
        myArraySamples = new float[1024];
        myArrayFrequency = new float[1024];
    }

    public float[] GetSamples()
    {
        PrintFloatArray(myArraySamples, myArraySamples.Length);
        return myArraySamples;
    }

    public float[] GetFrequency()
    {
        PrintFloatArrayFreq(myArrayFrequency, myArrayFrequency.Length);
        return myArrayFrequency;
    }

    public void select_station(int s) {
        SelectStation(s);
    }

    public void stop_current_station()
    {
        StopStation();
    }
}