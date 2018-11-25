using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioMaster : MonoBehaviour
{
    public string bankToLoad;

    private FMOD.Studio.EventInstance EventInstance;
    private FMOD.Studio.EventDescription EventDescription;

    public string Event = "";
    // This is being called before the start
    void Awake ()
    {
        LoadSoundBank(bankToLoad);
    }

    void Start()
    {
        Play();
    }
        // Update is called once per frame
        void Update ()
    {
		
	}

    void LoadSoundBank(string Bankname)
    {
        try
        {
            RuntimeManager.LoadBank(Bankname);
        }
        catch (BankLoadException e)
        {
            UnityEngine.Debug.LogException(e);
        }
        RuntimeManager.WaitForAllLoads();
    }
    public void Play()
    {
        EventDescription = RuntimeManager.GetEventDescription(Event); //This assigns the EventDescription from the Event string varable  that we set in the editor
        EventDescription.createInstance(out EventInstance); // creating EventInstance from EventDescription
        EventInstance.start(); //Starts the event
    }
}
