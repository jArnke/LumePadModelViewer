using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SequenceSelectUI : MonoBehaviour
{
    private string seqPath;

    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button editButton;
    [SerializeField] Button viewButton;
    [SerializeField] Button newSequence;
    [SerializeField] Button refresh;
    [SerializeField] SubjectViewController cam;

    void Awake(){
    }

    // Start is called before the first frame update
    void Start()
    {
        seqPath = "Assets/Sequences/";
        List<string> files = new List<string>(); 
        foreach(string file in Directory.GetFiles(seqPath)){
            if(file.Contains(".meta")) continue;
            files.Add(file);
        }
        dropdown.AddOptions(files);
        dropdown.SetValueWithoutNotify(0);

        editButton.onClick.AddListener( () => {StartEdit();} );
        viewButton.onClick.AddListener( () => {StartView();} );
        newSequence.onClick.AddListener( () => {StartCreateSeq();} );
        refresh.onClick.AddListener( () => {Refresh();} );
    }

    void StartEdit(){
        cam.StartEditing(dropdown.options[dropdown.value].text);
        
    }

    void StartView(){
        cam.StartViewing(dropdown.options[dropdown.value].text);
    }

    void StartCreateSeq(){
        if(inputField.text == "")return;
        cam.CreateNewSeq(seqPath+inputField.text);
    }

    

    // Update is called once per frame
    void Refresh()
    {
        seqPath = "Assets/Sequences/";
        List<string> files = new List<string>(); 
        foreach(string file in Directory.GetFiles(seqPath)){
            if(file.Contains(".meta")) continue;
            files.Add(file);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(files);
        
    }
}
