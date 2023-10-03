using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class EditUI : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] SubjectViewController viewController;
    [SerializeField] TMP_Dropdown modelSelect;
    [SerializeField] Toggle toggle3D;
    [SerializeField] Slider distance;
    [SerializeField] Slider scale;
    [SerializeField] Button saveQuit;
    [SerializeField] SequenceUI sequenceUI;

    [Header ("Parameteres")]
    [SerializeField, Range(0, float.PositiveInfinity)] float minDistance; 
    [SerializeField, Range(0, float.PositiveInfinity)] float maxDistance; 
    [SerializeField, Range(0, float.PositiveInfinity)] float minScale; 
    [SerializeField, Range(0, float.PositiveInfinity)] float maxScale; 
    

    // Start is called before the first frame update
    void Start()
    {
        scale.minValue = minScale;
        if(maxScale < minScale){
            maxScale = minScale;
        }
        scale.maxValue = maxScale;

        distance.minValue = minDistance;
        if(maxDistance < minDistance){
            maxDistance = minDistance;
        }
        distance.maxValue = maxDistance;
        
        List<string> options = new List<string>();
        foreach(string file in Directory.GetFiles("Assets/Models")){
            if(file.Contains(".meta"))
                    continue;
            
            options.Add(file);
        }
        modelSelect.AddOptions(options);

        PullState();

        modelSelect.onValueChanged.AddListener( (call) => {PushStateModel();});
        toggle3D.onValueChanged.AddListener( (call) => {PushState3D();});
        distance.onValueChanged.AddListener( (call) => {PushStateDistance();});
        scale.onValueChanged.AddListener( (call) => {PushStateScale();});

        
        
    }

    // Update is called once per frame
    void Update()
    {
       PullState(); 
    }

    void PullState(){
        for(int i=0;i<modelSelect.options.Count; i++){
            if(modelSelect.options[i].text == viewController.modelPath){
                modelSelect.SetValueWithoutNotify(i);
                break;
            }
        }
        this.toggle3D.SetIsOnWithoutNotify(viewController.is3D);
        this.distance.SetValueWithoutNotify(viewController.distance);
        this.scale.SetValueWithoutNotify(viewController.scale);
    }

    void PushStateModel(){
        this.viewController.LoadMesh(modelSelect.options[modelSelect.value].text);
    }
    void PushState3D(){
        this.viewController.Toggle3D(toggle3D.isOn);
    }
    void PushStateScale(){
        this.viewController.SetScale(scale.value);
    }
    void PushStateDistance(){
        this.viewController.SetDistance(distance.value);
    }

    void PushState(){
        Debug.Log("state pushed");
        
        this.viewController.LoadMesh(modelSelect.options[modelSelect.value].text);
        this.viewController.Toggle3D(toggle3D.isOn);
        this.viewController.SetDistance(distance.value);
        this.viewController.SetScale(scale.value);
    }
}
