using UnityEngine;
using System.Collections;

public class OutputVolume : MonoBehaviour
{
    #region PROPERTIES
    /// <summary>
    /// The prefab of bar to use when building.
    /// Refer to the documentation to use a custom prefab.
    /// </summary>
    public GameObject prefab;
    /// <summary>
    /// The number of samples to use when sampling. Must be a power of two.
    /// </summary>
    public int sampleAmount = 256;
    /// <summary>
    /// The AudioSource to take data from.
    /// </summary>
    public AudioSource audioSource;
    /// <summary>
    /// The audio channel to take data from when sampling.
    /// </summary>
    public int channel = 0;
    /// <summary>
    /// The amount of dampening used when the new scale is higher than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    public float attackDamp = 1;
    /// <summary>
    /// The amount of dampening used when the new scale is lower than the bar's existing scale. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    public float decayDamp = 1;
    /// <summary>
    /// The minimum (low value) color.
    /// </summary>
    public Color MinColor = Color.black;
    /// <summary>
    /// The maximum (high value) color.
    /// </summary>
    public Color MaxColor = Color.white;
    /// <summary>
    /// The curve that determines the interpolation between colorMin and colorMax.
    /// </summary>
    public AnimationCurve colorCurve;
    /// <summary>
    /// The amount of dampening used when the new color value is higher than the existing color value. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    public float colorAttackDamp = 1;
    /// <summary>
    /// The amount of dampening used when the new color value is lower than the existing color value. Must be between 0 (slowest) and 1 (fastest).
    /// </summary>
    [Range(0, 1)]
    public float colorDecayDamp = 1;
#endregion

    float[] samples;

    GameObject bar;

    Transform barT;

    float oldScale;
    float oldColorVal = 0;
    Material mat;

    int mat_ValId; 

	void Start () {
        bar = Instantiate(prefab) as GameObject;
        barT = bar.transform;
        barT.SetParent(transform,false);
        barT.localPosition = Vector3.zero;

        mat = barT.GetChild(0).GetComponent<Renderer>().material;
        mat_ValId = Shader.PropertyToID("_Val");
        mat.SetColor("_Color1", MinColor);
        mat.SetColor("_Color2", MaxColor);
	}
	
	void Update () {
        float value = GetRMS(audioSource, sampleAmount, channel);

        float newScale = value > oldScale ? Mathf.Lerp(oldScale, value, attackDamp) : Mathf.Lerp(oldScale, value, decayDamp);

        barT.localScale = new Vector3(1, newScale, 1);

        oldScale = newScale;

        float newColorVal = colorCurve.Evaluate(value);
        if (newColorVal > oldColorVal) //it's attacking
        {
            if (colorAttackDamp != 1)
            {
                newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorAttackDamp);
            }
        }
        else //it's decaying
        {
            if (colorDecayDamp != 1)
            {
                newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorDecayDamp);
            }
        }
        mat.SetFloat(mat_ValId, newColorVal);
        oldColorVal = newColorVal;
	}

    /// <summary>
    /// Returns the current output volume of the AudioSource, using the RMS method.
    /// </summary>
    /// <param name="aSource">The AudioSource to reference.</param>
    /// <param name="sampleSize">The number of samples to take, as a power of two. Higher values mean more precise volume.</param>
    /// <param name="channelUsed">The audio channel to take data from.</param>
    public static float GetRMS(AudioSource aSource, int sampleSize, int channelUsed = 0)
    {
        sampleSize = Mathf.ClosestPowerOfTwo(sampleSize);
        float[] outputSamples = new float[sampleSize];
        aSource.GetOutputData(outputSamples, channelUsed);

        float rms = 0;
        foreach (float f in outputSamples)
        {
            rms += f * f; //sum of squares
        }
        return Mathf.Sqrt(rms / (outputSamples.Length)); //mean and root

    }
}
