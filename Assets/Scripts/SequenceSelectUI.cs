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
        ActualPaths = new List<string>();
        seqPath = Path.Join(Application.persistentDataPath, "Sequences");
        if(!Directory.Exists(seqPath))
            Directory.CreateDirectory(seqPath);

        Refresh();

        editButton.onClick.AddListener( () => {StartEdit();} );
        viewButton.onClick.AddListener( () => {StartView();} );
        newSequence.onClick.AddListener( () => {StartCreateSeq();} );
        refresh.onClick.AddListener( () => {Refresh();} );
    }

    void StartEdit(){
        if(dropdown.options.Count <= 0)
            return;
        cam.StartEditing(ActualPaths[dropdown.value]);
        
    }

    void StartView(){
        if(dropdown.options.Count <= 0)
            return;
        cam.StartViewing(ActualPaths[dropdown.value]);
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
