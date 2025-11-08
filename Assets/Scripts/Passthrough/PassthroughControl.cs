using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;  // plugin types (if you actually use them)
using Meta.XR;  

[ExecuteInEditMode]
public class PassthroughControl : MonoBehaviour
{
    //class that handles creating bands and having them traverse the gradient space
    class PassthroughBeatBand
    {

        private Color bandColor = Color.white;
        private uint bandWidth = 4;
        private uint bandTail = 16;
        private uint bandHead = 8;
        private float bandSpeed = .5f;
        private float bandFade = 0.0f;


        private float[] opacity;

        public float position { get; private set; } = 0;
        public uint bandSize { get; private set; } = 11;
        public uint maxPosition { get; private set; } = 255;


        public PassthroughBeatBand()
        {

            CreateBand();

        }

        public PassthroughBeatBand(Color color, uint width, uint tail, uint head, float speed, float fade)
        {
            bandColor = color;
            bandWidth = width;
            bandTail = tail;
            bandHead = head;
            bandSpeed = speed;
            bandFade = Mathf.Clamp(fade, 0, 5f);
            CreateBand();
        }

        public void Increment()
        {
            position += bandSpeed * Time.deltaTime * 256;
        }

        public List<Color> Display(List<Color> colorArray)
        {
            int pos = (int)position;

            //map band onto color array
            for (int i = 0; i < bandSize; i++)
            {
                if ((i + pos >= 0) && (i + pos < colorArray.Count))
                {
                    if (bandSpeed < 0)
                    {
                        colorArray[i + pos] = (colorArray[i + pos] * (1 - opacity[i] * bandFade * pos / maxPosition)) + (bandColor * opacity[i] * bandFade * pos / maxPosition);
                    }
                    else
                    {
                        colorArray[i + pos] = (colorArray[i + pos] * (1 - opacity[i] * bandFade * (maxPosition - pos) / maxPosition)) + (bandColor * opacity[i] * bandFade * (maxPosition - pos) / maxPosition);
                    }

                }

            }



            return colorArray;
        }

        private void CreateBand()
        {
            bandSize = bandHead + bandWidth + bandTail;

            opacity = new float[bandSize];

            if (bandSpeed > 0)
            {
                position = -bandSize;

                //linear interpolate 
                for (int i = 0; i < bandSize; i++)
                {
                    if (i < bandTail)
                    {
                        opacity[i] = Mathf.Clamp01((float)(i + 1) / (bandTail));
                    }
                    else if (i >= bandWidth + bandTail)
                    {
                        opacity[i] = Mathf.Clamp01((float)1 - (i - bandWidth + bandTail + 1) / bandHead);
                    }
                    else
                    {
                        opacity[i] = 1;
                    }

                }
            }
            else
            {
                position = maxPosition;

                //linear interpolate 
                for (int i = 0; i < bandSize; i++)
                {
                    if (i < bandHead)
                    {
                        opacity[i] = Mathf.Clamp01((float)(i + 1) / (bandHead));
                    }
                    else if (i >= bandWidth + bandHead)
                    {
                        opacity[i] = Mathf.Clamp01((float)1 - (i - bandWidth + bandHead + 1) / bandTail);
                    }
                    else
                    {
                        opacity[i] = 1;
                    }

                }
            }





        }

    }


    public enum IncrementDirection { positive, negative, outward, inward, pingpong, sin };
    public enum GradientMode { spectralColorMap, shiftColorMap, shiftRandomized };

    public float intensity = 0.0f;

    public float spectrumFade = 0.1f;

    public int blackOutBaseOfGradient = 1;
    public int blackOutBaseFade = 1;

    public float fftRemapExponent = 1.0625f;

    public float inputGain = 1.0f;
    public float logRatio = 12f;
    public float frequencyGainExponent = 1.5f;

    public OVRPassthroughLayer passthroughLayer;
    public Gradient baseGradient;
    public Gradient spectralGradient;
    public Gradient spectralGradientMax;
    public Gradient activeGradient;
    public Gradient bandColors;

    public AudioSource audioSource;

    [Tooltip("Must be a power of 2 below 256 (1,2,4,8,16,32,64,128)")] public int colorBandWidth = 1;

    public float gradientMusicFrequency = 1.0f;
    public float bandFrequency = 1.0f;

    public GradientMode gradientMode = GradientMode.shiftColorMap;
    public IncrementDirection incrementDirection = IncrementDirection.positive;

    public Material outputMaterial;
    public RenderTexture renderTexture;
    public bool triggerBand = false;

    private List<Color> baseColorMap = new List<Color>(new Color[256]);
    private List<Color> generatedColorMap = new List<Color>(new Color[256]);
    private List<Color> colorMapOutput = new List<Color>(new Color[256]);

    private List<float> spectrumSmoothingVelocities = new List<float>(new float[256]);
    private List<float> smoothedSpectrumValues = new List<float>(new float[256]);



    private GradientMode oldGradientMode;
    private int oldColorBandWidth = 1;
    private float gradientIncrement = 0;
    private float gradientSinIncrement = 0;
    private float sinIncrement = 0;
    private List<PassthroughBeatBand> passthroughBeatBands = new List<PassthroughBeatBand>();
    List<PassthroughBeatBand> toRemove = new List<PassthroughBeatBand>();
    Texture2D gradientTexture;

    void OnValidate()
    {
        if (!triggerBand)
        {
            oldColorBandWidth = colorBandWidth;
            oldGradientMode = gradientMode;
            /*
            if(baseColorMap.Count == 0)
            {
                baseColorMap = new List<Color>(256);
            }
            if(generatedColorMap.Count == 0)
            {
                generatedColorMap = new List<Color>(256);
            }
            */
            Gradient gradient;
            if (gradientMode == GradientMode.spectralColorMap)
            {
                gradient = spectralGradient;
            }
            else
            {
                gradient = activeGradient;
            }

            baseColorMap = GenerateColorMap(baseGradient, 256, 1, GradientMode.shiftColorMap);
            generatedColorMap = GenerateColorMap(gradient, 256, colorBandWidth, gradientMode);

            if (colorMapOutput.Count == 0)
            {
                colorMapOutput = new List<Color>(256);
                Debug.Log("colorMapOutput count:" + colorMapOutput.Count);
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        oldColorBandWidth = colorBandWidth;
        oldGradientMode = gradientMode;

        gradientTexture = new Texture2D(256, 1);
        gradientTexture.wrapMode = TextureWrapMode.Clamp;

    }

    // Update is called once per frame
    void Update()
    {

        if (oldGradientMode != gradientMode || oldColorBandWidth != colorBandWidth)
        {

            Gradient gradient;
            if (gradientMode == GradientMode.spectralColorMap)
            {
                gradient = spectralGradient;
            }
            else
            {
                gradient = activeGradient;
            }

            generatedColorMap = GenerateColorMap(gradient, 256, colorBandWidth, gradientMode);
        }


        //either update the spectral display or increment the gradient map
        if (gradientMode == GradientMode.spectralColorMap)
        {
            MapSpectrumToColors();
            //colorMapOutput = new List<Color>(generatedColorMap);
        }
        else
        {
            IncrementGradient();
        }

        //vary between gradients based on intensity
        for (int i = 0; i < colorMapOutput.Count; i++)
        {
            colorMapOutput[i] = (baseColorMap[i] * (1 - intensity)) + (generatedColorMap[i] * intensity);
        }

        // generate a test band when the boolean is activated in the editor.
        if (triggerBand)
        {
            GenerateRandomBand();
            triggerBand = false;
        }



        //increment all bands and then remove any that have passed the extent of the gradient.
        

        foreach (PassthroughBeatBand band in passthroughBeatBands)
        {
            band.Increment();
            if ((band.position > band.maxPosition) || band.position < -band.bandSize)
            {
                toRemove.Add(band);
            }
            else
            {
                colorMapOutput = band.Display(colorMapOutput);
            }

        }

        if (blackOutBaseOfGradient > 0)
        {
            for (int i = 0; i < blackOutBaseOfGradient; i++)
            {
                colorMapOutput[i] = Color.black;
            }
        }

        if (toRemove.Count > 0)
        {
            passthroughBeatBands.RemoveAll(toRemove.Contains);
            toRemove.Clear();
        }
        


        Color[] colors = colorMapOutput.ToArray();

        if (renderTexture || outputMaterial)
        {
            gradientTexture.SetPixels(colors);

            gradientTexture.Apply();

            if (renderTexture)
                Graphics.Blit(gradientTexture, renderTexture);

            if (outputMaterial)
                outputMaterial.SetTexture("GradientMap", gradientTexture);
        }


        if (passthroughLayer)
            passthroughLayer.SetColorMap(colors);

        //set old values for checking if they've changed later
        oldColorBandWidth = colorBandWidth;
        oldGradientMode = gradientMode;

    }

    public void GenerateBand(float intensity)
    {

    }

    public void GenerateRandomBand()
    {
        float speed = bandFrequency;// * 2*(Mathf.Round(Random.value)-.5f);

        uint tail;
        uint head;

        if(bandFrequency>0)
        {
            tail = (uint)Random.Range(4, 24);
            head = (uint)Random.Range(1, 6);
        }
        else
        {
            head = (uint)Random.Range(4, 24);
            tail = (uint)Random.Range(1, 6);

        }

        passthroughBeatBands.Add(new PassthroughBeatBand(bandColors.Evaluate(Random.value), (uint)Random.Range(2, 12),tail, head, speed, 1.0f));
    }

    void MapSpectrumToColors()
    {
        float[] samples = new float[8192];//sample into 2048 bins (we only look at the first 256 so at 2048 we look at the first 2750 Hz (assuming 44000 Hz sample rate) (1024 = 5500 Hz, etc.)
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hanning);
        
        //float[] mappedSamples = new float[generatedColorMap.Count];

        /*
        for (int i = 1; i < (samples.Length / 4)-1; i++)
        {

            //logarithmic mapping to adjust to logarithmic nature of frequency perception
            int logi = Mathf.RoundToInt(255 * Mathf.Log(i) / Mathf.Log((samples.Length/4) - 1))-64;
            
            if(logi>0 && logi < mappedSamples.Length)
                mappedSamples[logi] += samples[i];//(Mathf.Log(samples[i])+24)/24; //map logarithmically with -24 dB being the lowest threshold.
        }
        */

        for (int i = 0; i < generatedColorMap.Count; i++)
        {
            int remap = Mathf.RoundToInt(Mathf.Pow(i, fftRemapExponent));
            //instant response to higher values, slow descent to lower values
            if (samples[remap] > smoothedSpectrumValues[i])
            {
                smoothedSpectrumValues[i] = samples[remap];
                spectrumSmoothingVelocities[i] = 0; //reset velocity for smoothing calcualtor;
            }
            else
            {
                float vel = spectrumSmoothingVelocities[i];
                smoothedSpectrumValues[i] = Mathf.SmoothDamp(smoothedSpectrumValues[i], samples[remap], ref vel, spectrumFade);
                spectrumSmoothingVelocities[i] = vel;
            }


            float value = inputGain * Mathf.Exp(frequencyGainExponent * i / 255)*(Mathf.Log(smoothedSpectrumValues[i]) + logRatio) / logRatio;// Mathf.Pow(smoothedSpectrumValues[i], inputExponent);// * Mathf.Exp(frequencyGainExponent * i / 255); //exponential function is needed to scale to account for reduced powers in higher frequency bins
                                                                                                                                        //generatedColorMap[i] = spectralGradientMax.Evaluate(value);

            Color a = spectralGradient.Evaluate((float)i / 255);
            Color b = spectralGradientMax.Evaluate((float)i / 255);
            generatedColorMap[i] = Color.Lerp(a, b, value);

        }


    }

    List<Color> GenerateColorMap(Gradient gradient, int listSize, int bandWidth, GradientMode mode)
    {

        List<Color> colorMapOutput = new List<Color>();

        Color[] colorMap = new Color[listSize / bandWidth];


        //map colors based on bandwidth
        for (int i = 0; i < colorMap.Length; i++)
        {
            float f = (float)i / (colorMap.Length - 1);
            colorMap[i] = gradient.Evaluate(f);
        }

        //randomize bands
        if (mode == GradientMode.shiftRandomized)
        {
            for (int i = colorMap.Length - 1; i > 0; i--)
            {
                int r = Random.Range(0, i + 1);
                Color temp = colorMap[i];
                colorMap[i] = colorMap[r];
                colorMap[r] = temp;

            }
        }

        //map to 256 array for compatibility with Oculus Passthrough
        for (int i = 0; i < listSize; i++)
        {
            int j = i / bandWidth;
            colorMapOutput.Add(colorMap[j]);
        }

        return colorMapOutput;

    }

    //shift the gradient along, wrapping at ends
    void IncrementGradient()
    {

        gradientIncrement += (gradientMusicFrequency * Time.deltaTime * 256);

        int increment = (int)gradientIncrement;

        switch (incrementDirection)
        {
            case IncrementDirection.positive:
                {



                    if (increment > 256)
                    {
                        increment %= 256;
                    }

                    generatedColorMap.InsertRange(0, generatedColorMap.GetRange(generatedColorMap.Count - increment, increment));
                    generatedColorMap.RemoveRange(generatedColorMap.Count - increment, increment);


                    break;
                }
            case IncrementDirection.negative:
                {
                    if (increment > 256)
                    {
                        increment %= 256;
                    }

                    generatedColorMap.AddRange(generatedColorMap.GetRange(0, increment));
                    generatedColorMap.RemoveRange(0, increment);

                    break;
                }
            case IncrementDirection.outward:
                {

                    if (increment > 128)
                    {
                        increment %= 128;
                    }

                    List<Color> tempColorArray = generatedColorMap.GetRange(0, increment);
                    tempColorArray.AddRange(generatedColorMap.GetRange(generatedColorMap.Count - increment, increment));

                    generatedColorMap.InsertRange(generatedColorMap.Count / 2, tempColorArray);

                    generatedColorMap.RemoveRange(0, increment);
                    generatedColorMap.RemoveRange(generatedColorMap.Count - increment, increment);




                    break;
                }
            case IncrementDirection.inward:
                {
                    if (increment > 128)
                    {
                        increment %= 128;
                    }

                    generatedColorMap.InsertRange(0, generatedColorMap.GetRange(generatedColorMap.Count / 2 - increment, increment));
                    generatedColorMap.RemoveRange((generatedColorMap.Count - increment) / 2, increment);

                    generatedColorMap.AddRange(generatedColorMap.GetRange(generatedColorMap.Count / 2, increment));
                    generatedColorMap.RemoveRange((generatedColorMap.Count - increment) / 2, increment);

                    break;
                }
            case IncrementDirection.pingpong:
                {

                    sinIncrement += (gradientMusicFrequency * Time.deltaTime);

                    gradientSinIncrement = Mathf.Sin(2 * Mathf.PI * sinIncrement);

                    if (increment > 256)
                    {
                        increment %= 256;
                    }

                    if (gradientSinIncrement > 0)
                    {
                        generatedColorMap.InsertRange(0, generatedColorMap.GetRange(generatedColorMap.Count - increment, increment));
                        generatedColorMap.RemoveRange(generatedColorMap.Count - increment, increment);
                    }
                    else
                    {
                        generatedColorMap.AddRange(generatedColorMap.GetRange(0, increment));
                        generatedColorMap.RemoveRange(0, increment);
                    }


                    if (sinIncrement >= 1.0f)
                    {
                        sinIncrement %= 1;
                    }

                    break;
                }

        }

        if (gradientIncrement >= 1.0f)
        {
            gradientIncrement %= 1;
        }



    }

    public void SetNewGradients(Gradient base_gradient, Gradient spectral_gradient, Gradient spectral_gradient_max, Gradient active_gradient, Gradient band_colors )
    {
        baseGradient = base_gradient;
        spectralGradient = spectral_gradient;
        spectralGradientMax = spectral_gradient_max;
        activeGradient = active_gradient;
        bandColors = band_colors;
    }
}
