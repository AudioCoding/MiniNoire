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
    public List<Transform> cluelist;

    private float distanceToClosestClue;
    public float maxClueDistance;
    private string nameOfClosestClue;


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
        print("Closest Clue Is " + nameOfClosestClue + " at: " + distanceToClosestClue);
        EventInstance.setParameterValue("DistanceToClue", distanceToClosestClue);
    }


        //(int i = 0; i < cluelist.Count; i++ ) //i++ is the same as i = i = 1


       // float dist = Vector3.Distance(cluelist[0].position, transform.position);//prints distance of float between two objects, using this to find distance between clue game objects and FMOD listener
        
        //float dist = Vector3.Distance(clue.position, transform.position);//prints distance of float between two objects, using this to find distance between clue game objects and FMOD listener
        //print("Distance to Clue: " + dist);
        //EventInstance.setParameterValue("DistanceToClue", dist);
    

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

        FMOD.Studio.PARAMETER_DESCRIPTION distanceToClue_fmodParam;
        EventDescription.getParameter("DistanceToClue", out distanceToClue_fmodParam);
        maxClueDistance = distanceToClue_fmodParam.maximum;
        print("Max Distance to Clue = " + maxClueDistance);
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
