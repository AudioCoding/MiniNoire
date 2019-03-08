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
    private float maxClueDistance;
    private string nameOfClosestClue; 

    public static int beatCounter = -8; // this is offset by -8 to account for the intro to the music in fmod which is not included in the loop
    static Music_Key musicKey = Music_Key.Key_BMinor; // creating a variable to define our first music key, we'll use this variable to determine the current key of the music

    static bool playStinger = false;
    private bool cluePickedUp = false;
    private float deadClueDistanceValue = 0.0f; //this is the distance from the player to the clue that was just picked up
    public float clueFadeMultiplier = 3.0f;

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
        distanceToClosestClue = maxClueDistance; // sets distancetoClosestClue as maxClueDistance which is set in PlayMusic() 

        foreach (Transform clue in cluelist)
        {
            
            float dist = Vector3.Distance(clue.position, transform.position); // sets dist to the position of each clue
            if(dist < maxClueDistance && dist < distanceToClosestClue) // if the dist of the clue is less than maxClueDistance and is less than distancetoClosestClue 
            {
                distanceToClosestClue = dist; // set the distancetoclosestClue to dist
                nameOfClosestClue = clue.name; //update the nameofClosetClue with correct clue name
            }
        
        }

        if (cluePickedUp == true) //cluePickedUp is set to true once we pick up a clue, This is done to gradually fade the gameparamter down instead it right away assigning the parameter to the closest
        {
            if (deadClueDistanceValue < distanceToClosestClue) // if the distance from the player to the most recently picked up clue is less than the distance to the next closest clue
            {
                deadClueDistanceValue += Time.deltaTime * clueFadeMultiplier;               // += means x = x + 1, as long as the deadClueDistanceValue is less than distanceToClosestClue we increment the deadClueDistanceValue
                EventInstance.setParameterValue("DistanceToClue", deadClueDistanceValue); // we assign the parameter to be the deadClueDistanceValue instead of the distancetoClosestClue value
                print("Cached Clue Value is: " + deadClueDistanceValue);
            }
           else
            {
                cluePickedUp = false; 
                deadClueDistanceValue = 0.0f; // resets deadClueDistanceValue until we pick up another clue
                EventInstance.setParameterValue("DistanceToClue", distanceToClosestClue); // if we didn't pick up a clue, we default to setting the parameter to the distance of the closest clue
                print("Closest Clue Is " + nameOfClosestClue + " at: " + distanceToClosestClue);
            }
        }
        else
        {
            EventInstance.setParameterValue("DistanceToClue", distanceToClosestClue); // once the deadClueDistanceValue is no longer less than the distanceToClosestClue, we set the parameter to distancetoClosestClue again
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

        EventInstance.setCallback(callback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);// sets a callback on the event that is created with the flag to call this on every Timeline beat

        FMOD.Studio.PARAMETER_DESCRIPTION distanceToClue_fmodParam;
        EventDescription.getParameter("DistanceToClue", out distanceToClue_fmodParam);
        maxClueDistance = distanceToClue_fmodParam.maximum; // sets maxClueDistance to the maximum value of the game parameter in the fmod parameter (30)
        print("Max Distance to Clue = " + maxClueDistance);
    }

    public FMOD.RESULT BeatEventCallBack(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance eventInstance, IntPtr parameters)//this function is called on each beat and it needs to return an a FMOD result and contain all of this information
    {
        beatCounter++;
        print("Callback called" + beatCounter);
        if (beatCounter >= 16) { //after 16 beats we will change the key

            switch (musicKey) // switch allows us to switch between statements, our variable is musicKey and we'll switch between b minor and f#
            {
                case Music_Key.Key_BMinor:
                    musicKey = Music_Key.Key_FSharp; // when the musicKey is b minor, switch to fsharp
                    break; //exits the function
                case Music_Key.Key_FSharp:
                    musicKey = Music_Key.Key_BMinor; // when the musicKey is fsharp, switch to bminor
                    break;
                default:
                    Debug.LogError("Music Key is unknown");

                    break; //if nothing else matches
            }

            beatCounter = 0;
        }

        if (playStinger == true)
        { 
            switch (musicKey) {
                case Music_Key.Key_BMinor:
                    RuntimeManager.PlayOneShot("event:/Stinger_Bminor"); // if the muskcKey is bminor, play the bminor stinger
                    break; 
                case Music_Key.Key_FSharp:
                    RuntimeManager.PlayOneShot("event:/Stinger_FSharp"); // if the musicKey is fsharp, play the fsharp stinger
                    break; 
                default:
                    Debug.LogError("Music Key is unknown");

                    break; //if nothing else matches
            }

        playStinger = false; //this turns the bool off, so we don't play the stinger over and over again
        }
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
            EventInstance.setParameterValue("CrimeSceneOn", 1.0f);

        }
        if (other.tag == "Clue")
        {
            RuntimeManager.PlayOneShot("event:/SFXPickUpClue");
            playStinger = true; //plays musical stinger if we have picked up a clue
            cluePickedUp = true;
            deadClueDistanceValue = Vector3.Distance(other.gameObject.transform.position, transform.position); //we calculate the current distance from the clue we just picked up
            cluelist.Remove(other.gameObject.transform); //removes clue from cluelist once it has been picked up
            Destroy(other.gameObject); // destroys the object so it can't be triggered again
        }

    }

    private void OnTriggerExit(Collider other)//function for leaving the "CrimeScene" tag
    {
        if (other.tag == "CrimeScene")//if we leave the "CrimeScene" we set the FadeParameter to 0
        {
            EventInstance.setParameterValue("CrimeSceneOn", 0f);

        }

    }
         IEnumerator FadeParameter(string parameterName, float parameterTargetValue, float fadeMultiplier, bool fadeIn)//coroutine (can think of it as a function that exists outside of this code)
        // Code not used. This coroutine was used to fade the game parameter but that functionality was later moved to Fmod for simplicity.    
        // parameterName (the name of the parameter we're adjusting, parameterTargetValue (the value we're trying to reach), fadeMultiplier (the number added to increment finalValue to reach parameterTargetValue), 
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
