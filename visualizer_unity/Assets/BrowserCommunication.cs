using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class BrowserCommunication : MonoBehaviour {

    float[] myArraySamples;
    float[] myArrayFrequency;

    [DllImport("__Internal")]
    private static extern void Initialize();

    //[DllImport("__Internal")]
    //private static extern void InitPedro(float[] array, int length);

    [DllImport("__Internal")]
    private static extern void PrintFloatArray(float[] array, int size);
    [DllImport("__Internal")]
    private static extern void PrintFloatArrayFreq(float[] array, int size);

    //[DllImport("__Internal")]
    //private static extern int AddNumbers(int x, int y);

    //[DllImport("__Internal")]
    //private static extern string StringReturnValueFunction();

    //[DllImport("__Internal")]
    //private static extern void BindWebGLTexture(int texture);

    void Start()
    {
        

        myArraySamples = new float[1024];
        myArrayFrequency = new float[1024];

        //InitPedro(myArray, myArray.Length);
        Initialize();
        //PrintFloatArray(myArray, myArray.Length);

        Initialize();
        //HelloString("This is a string.");

        //float[] myArray = new float[10];
        //PrintFloatArray(myArray, myArray.Length);

        //int result = AddNumbers(5, 7);
        //Debug.Log(result);

        //Debug.Log(StringReturnValueFunction());

        //var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        //BindWebGLTexture(texture.GetNativeTextureID());
    }

    //void Update()
    //{
        
    //}

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
}