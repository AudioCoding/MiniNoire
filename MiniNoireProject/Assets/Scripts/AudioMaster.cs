using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;//this calls the FMODUnity namespace (a collection of definitions) which will allow us to use FMODUnity functions

public class AudioMaster : MonoBehaviour
{
    //public string banktoLoad;//this is a public variable that allows us to specify an FMOD bank to load for our event in the inspector
    //no need to load and unload banks

    private FMOD.Studio.EventInstance EventInstance;//this adds an FMOD.Studio EventInstance variable
    private FMOD.Studio.EventDescription EventDescription;//this adds an FMOD.Studio EventDescription variable

    public string Event = ""; //allows us to put in the event name in the inspector

    bool fade = false;
    string fadeParameterName = "";
    float fadeToValue = 0;

    void Awake()
    //this is being called before the start
    {
        LoadSoundBank("Master Bank");// for this project we will only use one bank
    }

    void Start()
    {

    }
    void Update()
    {
     
    }

    void LoadSoundBank(string Bankname)//this loads the function LoadSoundBank with our input Bankname
    {

        try
        {
            RuntimeManager.LoadBank(Bankname);//this accesses RuntimeManager (global functionality to communicate between FMOD and Unity) to load the Bankname variable

        }
        catch (BankLoadException e)//if the bank can't be loaded, we'll see this exception
        {
            UnityEngine.Debug.LogException(e);
        }

        RuntimeManager.WaitForAllLoads();//this tells Unity not to do anything until all the banks are loaded
        print("Bank loaded " + Bankname);//prints "Bank loaded" and the Bankname variable
    }
    public void PlayMusic()
    {
        EventDescription = RuntimeManager.GetEventDescription(Event); //this assigns the EventDescription from the Event string variable that we set in the editor
        EventDescription.createInstance(out EventInstance); //this creates an EventInstance from the EventDescription
        EventInstance.start(); //this starts the event
    }

    void OnTriggerEnter(Collider other)//other is the trigger we enter
    {
        if (other.tag == "Environment")
        {
            PlayMusic();
        }

        if (other.tag == "CrimeScene")
        {
            StopCoroutine(FadeParameter("CrimeSceneOn", 0, 0.5f, false));
            StartCoroutine(FadeParameter ("CrimeSceneOn", 1, 0.5f,true)); //calling the coroutine function, starts of true because we are fading in when we enter the CrimeScene
            //EventInstance.setParameterValue("CrimeSceneOn", 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CrimeScene")
        {
            StopCoroutine(FadeParameter("CrimeSceneOn", 1, 0.5f, true));
            StartCoroutine(FadeParameter("CrimeSceneOn", 0, 0.5f, false));
            //EventInstance.setParameterValue("CrimeSceneOn", 0f);
        }

    }
    IEnumerator FadeParameter(string parameterName, float parameterTargetValue, float fadeMultiplier, bool fadeIn)//coroutine 
    {
        float currentParamValue;
        float finalValue;
        EventInstance.getParameterValue(parameterName,out currentParamValue, out finalValue);
        if(fadeIn == true)
        {
            while (finalValue < parameterTargetValue)//while this is true, unity will keep executing the code below this 
            {
                finalValue = finalValue + fadeMultiplier;
                EventInstance.setParameterValue(parameterName, finalValue);
                yield return new WaitForSeconds(.1f);
            }

        }
        else
        {
            while (finalValue > parameterTargetValue)//while this is true, unity will keep executing the code below this 
            {
                finalValue = finalValue - fadeMultiplier;
                EventInstance.setParameterValue(parameterName, finalValue);
                yield return new WaitForSeconds(.1f);
            }
        }

        yield return null;
    }

    
}
