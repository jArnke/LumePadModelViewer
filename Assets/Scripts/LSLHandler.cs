using UnityEngine;
using Unity;

using System;
using System.Collections;
using System.Collections.Generic;

using LSL;

class LSLHandler : MonoBehaviour {

    StreamInfo info;    
    StreamOutlet outlet;
    string[] sample = {"Default Marker Message"};
    bool shouldRecordSample = false;
    Coroutine coroutine;
    
    void Start(){
        SetupLSL();
    }
    void SetupLSL(){
        info = new StreamInfo("Stimulous Markers", "Marker");
        outlet = new StreamOutlet(info);

        coroutine = StartCoroutine(RecordCoroutine());
    }

    public void RecordSample(String message){
        this.sample[0] = message;
        this.shouldRecordSample = true;
    }

    public IEnumerator RecordCoroutine(){
        Debug.Log("Coroutine started");
        while (true){
            yield return new WaitForEndOfFrame();
            if(shouldRecordSample){
                outlet.push_sample(sample);
                Debug.Log(sample);
                shouldRecordSample = false;
            }
        }
    }
    
    
}
