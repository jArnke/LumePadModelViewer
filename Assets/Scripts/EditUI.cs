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
    [SerializeField] Button upload;
    [SerializeField] Button resetDistance;
    [SerializeField] Button resetScale;
    [SerializeField] SequenceUI sequenceUI;

    [Header ("Parameteres")]
    [SerializeField, Range(0, float.PositiveInfinity)] float minDistance; 
    [SerializeField, Range(0, float.PositiveInfinity)] float maxDistance; 
    [SerializeField, Range(0, float.PositiveInfinity)] float minScale; 
    [SerializeField, Range(0, float.PositiveInfinity)] float maxScale; 

    string modelPath;
    List<string> ActualPaths;
    

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
        
        RefreshModels();
        PullState();
        modelSelect.onValueChanged.AddListener( (call) => {PushStateModel();});
        toggle3D.onValueChanged.AddListener( (call) => {PushState3D();});
        distance.onValueChanged.AddListener( (call) => {PushStateDistance();});
        scale.onValueChanged.AddListener( (call) => {PushStateScale();});

        resetScale.onClick.AddListener(ResetScale);
        resetDistance.onClick.AddListener(ResetDistance);
    }

    // Update is called once per frame
    void Update()
    {
       PullState(); 
    }

    void PullState(){
        this.modelSelect.SetValueWithoutNotify(viewController.model.model_idx);
        this.toggle3D.SetIsOnWithoutNotify(viewController.is3D);
        this.distance.SetValueWithoutNotify(viewController.distance);
        this.scale.SetValueWithoutNotify(viewController.scale);
    }

    void ResetScale(){
        this.scale.value = 1;
    }
    void ResetDistance(){
        this.distance.value = 10;
    }

    void PushStateModel(){
        this.viewController.LoadModelByIdx(modelSelect.value);
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

    void RefreshModels(){
        modelSelect.ClearOptions();
        List<string> models = new List<string>();
        for(int i = 0;i<viewController.modelPrefabs.Count;i++){
            models.Add(viewController.modelPrefabs[i].name);
        }
        modelSelect.AddOptions(models);
    }

}
