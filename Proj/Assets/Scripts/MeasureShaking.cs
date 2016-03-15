//#define MEPI_DEV1
#define MEPI_DEV2

using UnityEngine;
using System.Collections;

public class MeasureShaking : MonoBehaviour
{
#if MEPI_DEV1
    public float avrgTime = 0.5f;
    public float peakLevel = 0.6f;
    public float endCountTime = 0.6f;
    public int shakeDir;
    public int shakeCount;

    Vector3 avrgAcc = Vector3.zero;
    int countPos;
    int countNeg;
    int lastPeak;
    int firstPeak;
    bool counting;
    float timer;
#elif MEPI_DEV2

    // TODO : Catch Device Shake.
    private float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).
    private float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation, or at least according to Brady! ;)
    private float shakeDetectionThreshold = 2.0f;

    private float lowPassFilterFactor;
    private Vector3 lowPassValue = Vector3.zero;
    private Vector3 acceleration;
    private Vector3 deltaAcceleration;
#endif    

#if MEPI_DEV1
    bool ShakeDetector()
    {
        // read acceleration:
        Vector3 curAcc = Input.acceleration;
        // update average value:
        avrgAcc = Vector3.Lerp(avrgAcc, curAcc, avrgTime * Time.deltaTime);
        // calculate peak size:
        curAcc -= avrgAcc;
        // variable peak is zero when no peak detected...
        int peak = 0;
        // or +/- 1 according to the peak polarity:
        if (curAcc.y > peakLevel) peak = 1;
        if (curAcc.y < -peakLevel) peak = -1;
        // do nothing if peak is the same of previous frame:
        if (peak == lastPeak)
            return false;
        // peak changed state: process it
        lastPeak = peak; // update lastPeak
        if (peak != 0)
        { // if a peak was detected...
            timer = 0; // clear end count timer...
            if (peak > 0) // and increment corresponding count
                countPos++;
            else
                countNeg++;
            if (!counting)
            { // if it's the first peak...
                counting = true; // start shake counting
                firstPeak = peak; // save the first peak direction
            }
        }
        else // but if no peak detected...
            if (counting)
            { // and it was counting...
                timer += Time.deltaTime; // increment timer
                if (timer > endCountTime)
                { // if endCountTime reached...
                    counting = false; // finish counting...
                    shakeDir = firstPeak; // inform direction of first shake...
                    if (countPos > countNeg) // and return the higher count
                        shakeCount = countPos;
                    else
                        shakeCount = countNeg;
                    // zero counters and become ready for next shake count
                    countPos = 0;
                    countNeg = 0;
                    return true; // count finished
                }
            }
        return false;
    }
#elif MEPI_DEV2
    void Start()
    {
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }
#endif

    void Update()
    {
        
#if MEPI_DEV1
        if (ShakeDetector())
        { // call ShakeDetector every Update!
            // the device was shaken up and the count is in shakeCount
            // the direction of the first shake is in shakeDir (1 or -1)
        }
        // the variable counting tells when the device is being shaken:
        if (counting)
        {
            //print("Shaking up device");
        }
#elif MEPI_DEV2
        acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        deltaAcceleration = acceleration - lowPassValue;
        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here, with suitable guards in the if check above, if necessary to not, to not fire again if they're already being performed.
            //Debug.LogError("Shake event detected at time " + Time.time);
            Handheld.Vibrate();
        }
#endif
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 10, 100, 32), "Vibrate!"))
            Handheld.Vibrate();
    }
}