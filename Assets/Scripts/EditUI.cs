using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using NativeFilePickerNamespace;

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
        ActualPaths = new List<string>();
        this.modelPath = Path.Join(Application.persistentDataPath, "Models");
        if(!Directory.Exists(modelPath)){
            Directory.CreateDirectory(modelPath);
        }
        string localModelPath = Path.Join(Application.streamingAssetsPath, "Models");
        foreach(string file in Directory.GetFiles(localModelPath)){
            string fileName = Path.GetFileName(file);
            string outputPath = Path.Join(modelPath, fileName);
            if(file.Contains(".meta"))continue;
            if(File.Exists(outputPath))
                continue;
            File.Copy(file, outputPath);
        RefreshModels();
            
        }
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
        upload.onClick.AddListener( UploadFiles ); //(call) => {UploadFiles();});

        
        
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
        this.viewController.LoadMesh(ActualPaths[modelSelect.value]);
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
    void UploadFiles(){
        Debug.Log("Clicked");
        string[] filetypes = {".obj"};
        NativeFilePicker.PickMultipleFiles(OnFilesPicked, filetypes);
    }

    void OnFilesPicked(string[] files){
        if(files == null)
            return;

        foreach(string file in files){
            string[] fileSegements = file.Split(Path.PathSeparator, System.StringSplitOptions.RemoveEmptyEntries);
            string fileName = fileSegements[fileSegements.Length - 1];
            File.Copy(file, Path.Join(modelPath,fileName));
        }
        RefreshModels();
    }
    void RefreshModels(){
        modelSelect.ClearOptions();
        ActualPaths.Clear();
        List<string> options = new List<string>();
        foreach(string file in Directory.GetFiles(modelPath)){
            if(file.Contains(".meta"))
                continue;

            options.Add(Path.GetFileName(file));
            ActualPaths.Add(file);
        }
        modelSelect.AddOptions(options);
    }

    void PushState(){
        Debug.Log("state pushed");
        
        this.viewController.LoadMesh(modelSelect.options[modelSelect.value].text);
        this.viewController.Toggle3D(toggle3D.isOn);
        this.viewController.SetDistance(distance.value);
        this.viewController.SetScale(scale.value);
    }
}
