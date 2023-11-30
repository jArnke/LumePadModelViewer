using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SequenceSelectUI : MonoBehaviour
{
    private string seqPath;
    private List<string> ActualPaths;
    [SerializeField] SubjectViewController cam;

    [Header ("Sequence Creation")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button newSequence;

    [Header ("Select Sequence")]
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Button editButton;
    [SerializeField] Button deleteButton;
    [SerializeField] Button viewButton;
    [SerializeField] Button refresh;

    [Header ("Stimuli and Rest Period Setup")]
    [SerializeField] TMP_InputField restLength;
    [SerializeField] TMP_InputField stimLength;
    [SerializeField] float minLength;
    [SerializeField] float maxLength;


    [SerializeField] Toggle randomize;
    [SerializeField] Toggle survey;

    void Awake(){
        restLength.text = "2";
        stimLength.text = "5";
    }

    // Start is called before the first frame update
    void Start()
    {
        ActualPaths = new List<string>();
        Debug.Log(Application.persistentDataPath);
        seqPath = Path.Join(Application.persistentDataPath, "Sequences");
        if(!Directory.Exists(seqPath))
            Directory.CreateDirectory(seqPath);

        Refresh();

        editButton.onClick.AddListener( () => {StartEdit();} );
        deleteButton.onClick.AddListener( DeleteSequence );
        viewButton.onClick.AddListener( () => {StartView();} );
        newSequence.onClick.AddListener( () => {StartCreateSeq();} );
        refresh.onClick.AddListener( () => {Refresh();} );
        stimLength.onValueChanged.AddListener( (m) => {
                if(m.Length == 0) return;
                if(m.Length == 1)
                    if(m == "-") m = "0";
                float x = float.Parse(m);
                stimLength.text = SanitizeLength(x).ToString();
                });
        restLength.onValueChanged.AddListener( (m) => {
                if(m.Length == 0) return;
                if(m.Length == 1)
                    if(m == "-") m = "0";
                float x = float.Parse(m);
                restLength.text = SanitizeLength(x).ToString();
                });
    }

    float SanitizeLength(float x){
        if(x < minLength)
            return minLength;
        if(x > maxLength)
            return maxLength;
        return x;
    }

    void DeleteSequence(){
        if(dropdown.options.Count <= 0)
            return;
        File.Delete(ActualPaths[dropdown.value]);
        Refresh();
    }

    void StartEdit(){
        if(dropdown.options.Count <= 0)
            return;
        cam.StartEditing(ActualPaths[dropdown.value]);
        
    }

    void StartView(){
        if(dropdown.options.Count <= 0)
            return;
        
        cam.StartViewing(
                ActualPaths[dropdown.value], 
                float.Parse(stimLength.text), 
                float.Parse(restLength.text),
                survey.isOn,
                randomize.isOn
                );
    }

    void StartCreateSeq(){
        if(inputField.text == "")return;
        cam.CreateNewSeq(Path.Join(seqPath,inputField.text));
    }

    

    // Update is called once per frame
    void Refresh()
    {
        ActualPaths.Clear();
        List<string> files = new List<string>(); 
        foreach(string file in Directory.GetFiles(seqPath)){
            if(file.Contains(".meta")) continue;
            files.Add(Path.GetFileName(file));
            ActualPaths.Add(file);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(files);
        
    }
}
