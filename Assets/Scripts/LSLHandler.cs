using UnityEngine;
using System;
using System.Collections;
using LSL;
using TMPro;
using LSL4Unity;

class LSLHandler : MonoBehaviour {
    [SerializeField] TMP_Text status;

    StreamInfo info;    
    StreamOutlet outlet;
    string[] sample = {"Default Marker Message"};
    bool shouldRecordSample = false;
    Coroutine coroutine;
    
    void Start(){
        SetupLSL();
    }
    void SetupLSL(){
        try{

            info = new StreamInfo("Stimulous Markers", "Marker", 1, 0, channel_format_t.cf_string, "LumePad");
            outlet = new StreamOutlet(info);

            coroutine = StartCoroutine(RecordCoroutine());
        }
        catch(Exception e){
            status.text = e.Message;
        }
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
                shouldRecordSample = false;
            }
        }
    }
    
    
}
