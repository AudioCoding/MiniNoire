using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;//this calls the FMODUnity namespace (a collection of definitions) which will allow us to use FMODUnity functions

public class AudioMaster : MonoBehaviour
{
    public string banktoLoad;//this is a public variable that allows us to specify an FMOD bank to load for our event in the inspector

    private FMOD.Studio.EventInstance EventInstance;//this adds an FMOD.Studio EventInstance variable
    private FMOD.Studio.EventDescription EventDescription;//this adds an FMOD.Studio EventDescription variable

    public string Event = ""; //allows us to put in the event name in the inspector

    void Awake()
    //this is being called before the start
    {
        LoadSoundBank(banktoLoad);
    }

    void Start()
    {
        Play();//this triggers the RuntimeManager to get the event description from EventDescription string variable, creates an instance (EventInstance), then plays the instance
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
    public void Play()
    {
            EventDescription = RuntimeManager.GetEventDescription(Event); //this assigns the EventDescription from the Event string variable that we set in the editor
            EventDescription.createInstance(out EventInstance); //this creates an EventInstance from the EventDescription
            EventInstance.start(); //this starts the event
    }

}
