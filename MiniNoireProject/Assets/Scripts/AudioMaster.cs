using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;//this calls the FMODUnity namespace (a collection of definitions) which will allow us to use FMODUnity functions
using System;

enum Music_Key // a variable of pre-defined values, in this case our key changes
{
    Key_BMinor,
    Key_FSharp
}
public class AudioMaster : MonoBehaviour
{


    private FMOD.Studio.EventInstance EventInstance;//this adds an FMOD.Studio EventInstance variable
    private FMOD.Studio.EventDescription EventDescription;//this adds an FMOD.Studio EventDescription variable


    public string Event = ""; //allows us to put in the event name in the inspector
    public List<Transform> cluelist;

    private float distanceToClosestClue;
    public float maxClueDistance;
    private string nameOfClosestClue;

    public static int BeatCounter = -8;
    static Music_Key musicKey = Music_Key.Key_BMinor; // creating a variable to define our first music key, we'll use this variable to determine the current key of the music

    static bool playStinger = false;
    private bool cluePickedUp = false;
    private float deadClueDistanceValue = 0.0f;
    public float clueFadeMultiplier = 8.0f;

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
        distanceToClosestClue = maxClueDistance;
    
        foreach (Transform clue in cluelist)
        {
            
            float dist = Vector3.Distance(clue.position, transform.position);
            if(dist < maxClueDistance && dist < distanceToClosestClue)
            {
                distanceToClosestClue = dist;
                nameOfClosestClue = clue.name;
            }
        
       
        }

        

        if (cluePickedUp == true)
        {
            if (deadClueDistanceValue < distanceToClosestClue)
            {
                deadClueDistanceValue += Time.deltaTime * clueFadeMultiplier;               // += means x = x + 1
                EventInstance.setParameterValue("DistanceToClue", deadClueDistanceValue);
                print("Cached Clue Value is: " + deadClueDistanceValue);
            }
           else
            {
                cluePickedUp = false;
                deadClueDistanceValue = 0.0f; // resets deadClueDistanceValue until we pick up another clue
                EventInstance.setParameterValue("DistanceToClue", distanceToClosestClue);
                print("Closest Clue Is " + nameOfClosestClue + " at: " + distanceToClosestClue);
            }
        }
        else
        {
            EventInstance.setParameterValue("DistanceToClue", distanceToClosestClue);
            print("Closest Clue Is " + nameOfClosestClue + " at: " + distanceToClosestClue);
        }
        
        
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

        FMOD.Studio.EVENT_CALLBACK callback;//special type of variable defined by FMOD.studio called EVENT_CALLBACK
        callback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallBack);//pointer to the function that we're going to callback which is MusicEventCallBack

        EventInstance.setCallback(callback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);//sets a callback on the event that is created

        FMOD.Studio.PARAMETER_DESCRIPTION distanceToClue_fmodParam;
        EventDescription.getParameter("DistanceToClue", out distanceToClue_fmodParam);
        maxClueDistance = distanceToClue_fmodParam.maximum;
        print("Max Distance to Clue = " + maxClueDistance);
    }
    public FMOD.RESULT BeatEventCallBack(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance eventInstance, IntPtr parameters)//this function needs to return an a FMOD result and contain all of this information
  {
        BeatCounter++;
        print("Callback called" + BeatCounter);
        if(BeatCounter >= 16){

            switch (musicKey) // switch allows us to switch between statements, our variable is musicKey and we'll switch between b minor and f#
            {
                case Music_Key.Key_BMinor:
                    musicKey = Music_Key.Key_FSharp;
                    break; //do some stuff
                case Music_Key.Key_FSharp:
                    musicKey = Music_Key.Key_BMinor;
                    break; //do other stuff
                default:
                    Debug.LogError("Music Key is unknown");
                    
                    break; //if nothing else matches
            } 

            BeatCounter = -8;
        }
        if (playStinger == true)

            switch (musicKey) { 
                case Music_Key.Key_BMinor:
                    RuntimeManager.PlayOneShot("event:/Stinger_Bminor");
                    break; //do some stuff
                case Music_Key.Key_FSharp:
                RuntimeManager.PlayOneShot("event:/Stinger_FSharp");
                break; //do other stuff
                default:
                    Debug.LogError("Music Key is unknown");

                    break; //if nothing else matches
            }
        playStinger = false; //this turns the bool off, so we don't play the stinger over and over again

        return FMOD.RESULT.OK;//this is an enum - a list of states, we need to return a result for callbacks
    }

    void OnTriggerEnter(Collider other)//this will start the music as soon as we enter the trigger ("Environment" or "CrimeScene"), "other is the trigger we enter
    {
        if (other.tag == "Environment")//if the tag of the trigger is equal to "Environment" then we will play music
        {
            PlayMusic();//plays the music 
        }

        if (other.tag == "CrimeScene")//if the tag is set to "CrimeScene" then we set the FadeParameter to 1
        {
            //MusicV2 uses fades in FMOD instead of fades in code
            //StopCoroutine(FadeParameter("CrimeSceneOn", 0, 0.5f, false));//stops the coroutine
            //StartCoroutine(FadeParameter ("CrimeSceneOn", 1, 0.5f,true)); //calling the coroutine function, starts off true because we are fading in when we enter the CrimeScene
            EventInstance.setParameterValue("CrimeSceneOn", 1.0f);

        }
        if (other.tag == "Clue")
        {
            playStinger = true;
            cluePickedUp = true;
            deadClueDistanceValue = Vector3.Distance(other.gameObject.transform.position, transform.position);
            cluelist.Remove(other.gameObject.transform);
            Destroy(other.gameObject); // destroys the object so it can't be triggered again
        }

    }

    private void OnTriggerExit(Collider other)//function for leaving the "CrimeScene" tag
    {
        if (other.tag == "CrimeScene")//if we leave the "CrimeScene" we set the FadeParameter to 0
        {
            //MusicV2 uses fades in FMOD instead of fades in code
            //StopCoroutine(FadeParameter("CrimeSceneOn", 1, 0.5f, true));//stops the coroutine so we don't have multiple instances of it
            //StartCoroutine(FadeParameter("CrimeSceneOn", 0, 0.5f, false));//sets parameterTargetValue to 0 and fadeIn to false
            EventInstance.setParameterValue("CrimeSceneOn", 0f);

        }

    }
    IEnumerator FadeParameter(string parameterName, float parameterTargetValue, float fadeMultiplier, bool fadeIn)//coroutine (can think of it as a function that exists outside of this code)
        //parameterName (the name of the parameter we're adjusting, parameterTargetValue (the value we're trying to reach), fadeMultiplier (the number added to increment finalValue to reach parameterTargetValue), 
        // bool fadeIn (on true, fades parameter in, on false fades out)
    {
        float currentParamValue;//value of the current parameter
        float finalValue;//final value of the parameter
        EventInstance.getParameterValue(parameterName,out currentParamValue, out finalValue);//checks the current  value of the parameter
        if (fadeIn == true)//checks if we're fading in
        {
            while (finalValue < parameterTargetValue)//while finalValue is less than the value we're trying to reach, unity will keep executing the code below this 
            {
                finalValue = finalValue + fadeMultiplier;//increments finalValue with the fadeMultiplier
                EventInstance.setParameterValue(parameterName, finalValue);//adjust the parameter value to new finalValue
                yield return new WaitForSeconds(.1f);//wait for 0.1 seconds before looping again
            }

        }
        else// if fadeIn is false
        {
            while (finalValue > parameterTargetValue)//checks if the finalValue is larger than the parameterTargetValue 
            {
                finalValue = finalValue - fadeMultiplier;//this adjusts the finalValue by subtracting the fadeMultiplier from it
                EventInstance.setParameterValue(parameterName, finalValue);//adjusts the new finalValue
                yield return new WaitForSeconds(.1f);//wait for 0.1 seconds before looping again
            }
        }

        yield return null;
    }

    
}
