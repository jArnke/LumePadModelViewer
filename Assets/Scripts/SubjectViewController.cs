using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

using LeiaLoft;

class SubjectViewController: MonoBehaviour{
    
    public class Model{
        public float scale;
        public int model_idx;

        public String Serialize(){
            return $"{scale},{model_idx}";
        }
        public static Model Deserialize(string serializedModel){
            Model model = new Model();
            string[] data = serializedModel.Split(",");
            model.scale = float.Parse(data[0]);
            model.model_idx = int.Parse(data[1]);
            return model;
        }
    }

    public List<GameObject> modelPrefabs;
    public Model model;

    public class ViewState{
        public Vector3 position;
        public Quaternion rotation;
        public Model model;
        public bool is3D;

        public String Serialize(){
            return $"{is3D},{position.x},{position.y},{position.z},{rotation.x},{rotation.y},{rotation.z},{rotation.w},{model.Serialize()}";
        }
        public static ViewState Deserialize(string serializedState){
            Debug.Log($"Reading state: {serializedState}");
            ViewState state = new ViewState();
            string[] data = serializedState.Split(",",9);
            state.is3D = bool.Parse(data[0]);
            state.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
            state.rotation = new Quaternion(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));
            state.model = Model.Deserialize(data[8]);
            return state;
        }
    }
    private bool IsTouchOverUI(Touch touch){
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);    
    }

    private enum CamState{
        Rest,
        Question,
        Stimuli,
        Inactive,
        Edit
    };


    private bool isUITouch = false;
    Camera cam;
    float currTime;
    Transform target;
    private Vector2 currentRotation;
    public List<ViewState> states;
    public int currentStateIdx;
    public float distance;
    public bool is3D;
    private float originalScale;
    public float scale;
    string seqPath;
    bool questionsEnabled;
    
    CamState camState = CamState.Inactive;

    [Header ("Experiment Parameters")]
    [SerializeField] float restPeriodLength;
    [SerializeField] float stimuliShownLength;


    [Header ("Model Params")]
    [SerializeField] Transform targetRoot;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Mesh defaultMesh;

    [Header ("Editor Params")]
    [SerializeField] float speed;

    [Header ("References")]
    [SerializeField] GameObject restPanel;
    [SerializeField] LSLHandler lsl;
    [SerializeField] LeiaDisplay leiaDisplay;
    [SerializeField] GameObject editUI;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject questionUI;
    [SerializeField] float questionShowLength;
    


    /* Built-Ins */
    void Awake(){
        this.cam = gameObject.GetComponent<Camera>();
        Debug.Assert(cam != null, "Please attach this script to the Main Camera GameObject");
        Debug.Assert(targetRoot != null, "Please select a root transform for loading models");
        this.states = new List<ViewState>();
        this.OpenMainMenu();
    }

    void Start(){
        //Load oher resources? idk
    }

    /* State Machine */
    void Update(){

        switch (camState){
        case CamState.Inactive:
            break;
        case CamState.Question:
            QuestionUpdate();
            break;
        case CamState.Rest:
            RestUpdate();
            break;
        case CamState.Stimuli:
            StimuliUpdate();
            break;
        case CamState.Edit:
            EditUpdate();
            break;
        default:
            break;
        }

    }

    void QuestionUpdate(){
        questionUI.SetActive(true);
        currTime += Time.deltaTime;
        if(currTime > questionShowLength){
            lsl.RecordSample("Response: None");
            camState = CamState.Rest;
            questionUI.SetActive(false);
            if(currentStateIdx>=states.Count){
                restPanel.SetActive(false);
                FinishSequence();
                camState = CamState.Inactive;
            }
            else{
                lsl.RecordSample($"Rest Started, Stimuli: {currentStateIdx}");
                StartRest();
                camState = CamState.Rest;
            }
            currTime = 0;
        }
    }
    public void SubjectResponse(bool seenBefore){
        lsl.RecordSample($"Response: {seenBefore}");
        questionUI.SetActive(false);
        restPanel.SetActive(false);
        if(currentStateIdx>=states.Count){
            FinishSequence();
            camState = CamState.Inactive;
        }
        else{
            lsl.RecordSample($"Rest Started, Stimuli: {currentStateIdx}");
            StartRest();
            camState = CamState.Rest;
        }
        currTime = 0;
    }
    void StimuliUpdate(){
        currTime += Time.deltaTime;
        if(currTime > stimuliShownLength){
            currentStateIdx++;
            if(questionsEnabled){
                lsl.RecordSample($"Survey Shown");
                StartRest();
                camState = CamState.Question;
            }
            else if(currentStateIdx>=states.Count){
                camState = CamState.Inactive;
                FinishSequence();
            }
            else{
                lsl.RecordSample($"Rest Started, Stimuli: {currentStateIdx}");
                StartRest();
            }
            currTime = 0;
        }
    }
    void RestUpdate(){
        currTime += Time.deltaTime;
        if(currTime > restPeriodLength){
            this.restPanel.SetActive(false);
            camState = CamState.Stimuli;
            lsl.RecordSample($"State Loaded, Stimuli: {currentStateIdx}");
            if(currentStateIdx >= states.Count){
                if(states[currentStateIdx] == null)
                    OpenMainMenu();
            }
            LoadState(states[currentStateIdx]);
            currTime = 0;
        }
    }
    void EditUpdate(){
        if(Input.touchCount >= 1){
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
                isUITouch = IsTouchOverUI(touch);
            if(!isUITouch){
                Vector2 swipe = touch.deltaPosition;
                RotateCamera(new Vector2(speed*swipe.x,-speed*swipe.y));
            }
        }
    }
    void StartEditMode(string sequence){
        this.camState = CamState.Edit;
        if(sequence != null){
            LoadSerializedSequence(sequence);
            this.LoadStateIndex(0);
        }
        else{
            this.states = new List<ViewState>();
            this.currentStateIdx = -1;
            this.LoadModelByIdx(0);
        }
        this.editUI.SetActive(true);
        this.mainMenu.SetActive(false);

    }

    void QuitEditMode(bool save){
        //Store serialized sequence

        this.camState = CamState.Inactive;
        
    }
    /* Helper Methods */
    void LoadState(ViewState state){
        LoadModel(state.model);
        this.transform.position = state.position;
        this.transform.rotation = state.rotation;
        this.distance = Vector3.Distance(this.transform.position, targetRoot.transform.position);
        Toggle3D(state.is3D);
    }
    void LoadModel(Model model){
        if(target != null)
            Destroy(target.gameObject);
        this.model = model;

        Debug.Log("Creating Model Obj"); 
        GameObject go = Instantiate(this.modelPrefabs[model.model_idx]);
        target = go.transform;
        target.parent = targetRoot; 
        this.originalScale = target.localScale.x;
        target.localScale *= model.scale;
        this.scale = model.scale;
    }

    public void LoadModelByIdx(int idx){
        Model model = new Model();
        model.scale = 1;
        model.model_idx = (idx >= 0 && idx  < modelPrefabs.Count) ? idx : 0;
        LoadModel(model);
    }

    public void SetDistance(float distance){
        if(target == null)
            return;
        this.distance = distance;
        
        Vector3 dir = this.transform.position - targetRoot.position;
        this.transform.position = dir.normalized*distance;
    }
    public void SetScale(float scale){
        if(target == null)
            return;
        this.scale = scale;
        target.localScale = Vector3.one * originalScale * scale;
    }

    void RotateCamera(Vector2 rotation){
        if(target==null)
            return;
        this.currentRotation += rotation;
        //Calculate new position
        Vector3 pivotPoint = targetRoot.position;
        this.transform.RotateAround(pivotPoint, this.transform.up, rotation.x);
        this.transform.RotateAround(pivotPoint, this.transform.right, rotation.y); 
    }
    void FinishSequence(){
        lsl.RecordSample("Finished");
        this.OpenMainMenu();
        //send final lsl marker?
        //reload main menu?
    }
    void ResetPosition(float distance){
        if(target == null)
            return;
        this.distance = distance;
        this.transform.position = targetRoot.position - new Vector3(0,0,distance);
        this.transform.LookAt(targetRoot);
    }
    void StartRest(){
        this.restPanel.SetActive(true);
        this.camState = CamState.Rest;
        
    }


    /* Public API */
    //Most if not all should only work in edit mode
    public void StartEditing(string seqPath){
        this.seqPath = seqPath;
        string seq = File.ReadAllText(seqPath);
        this.StartEditMode(seq);
    }
    public void StartViewing(string seqPath, float stimLen, float restLen, bool questionsEnabled, bool randomize){
        this.questionsEnabled = questionsEnabled;
        this.restPeriodLength = restLen;
        this.stimuliShownLength = stimLen;
        this.mainMenu.SetActive(false);
        this.editUI.SetActive(false);
        this.seqPath = seqPath;
        string seq = File.ReadAllText(seqPath);
        this.LoadSerializedSequence(seq);
        this.StartRest();
        if(randomize){
            System.Random rng = new System.Random();
            this.states = states.OrderBy(a => rng.Next()).ToList();
        }
        this.lsl.RecordSample($"Starting Sequence: {SerializeSequence()}");
    }
    public void CreateNewSeq(string seqPath){
        while(File.Exists(seqPath)){
            seqPath += "(1)";
        }
        this.seqPath = seqPath;
        this.states.Clear();
        this.StartEditMode(null);
    }
    public void Toggle3D(bool is3D){
        this.is3D = is3D;
        leiaDisplay.DesiredLightfieldMode = (is3D ? LeiaDisplay.LightfieldMode.On : LeiaDisplay.LightfieldMode.Off);

    }
    public void SaveSequence(){
        string seq = SerializeSequence();
        if(File.Exists(seqPath))
            File.Delete(seqPath);
        byte[] bytes = Encoding.ASCII.GetBytes(seq);
        FileStream stream = File.Create(seqPath);
        stream.Write(bytes, 0, bytes.Length);
        stream.Close();     
        this.OpenMainMenu();
    }
    public void OpenMainMenu(){
        this.Toggle3D(false);
        this.camState = CamState.Inactive;
        if(target != null)
            Destroy(target.gameObject);
        editUI.SetActive(false);
        mainMenu.SetActive(true);

    }
    public List<ViewState> DeserializeSequence(string input){

        List<ViewState> sequence = new List<ViewState>();
        foreach(string state in input.Split("\n")){
            if(state == "")
                break;

            sequence.Add(ViewState.Deserialize(state));
        }
        return sequence;
    }
    public void LoadSerializedSequence(string input){
        LoadSequence(DeserializeSequence(input));
    }
    public void LoadSequence(List<ViewState> sequence){
        this.states = sequence;
        currentStateIdx = 0;
        currTime = 0;
    }
    public void SaveCurrentState(){
        ViewState newState = new ViewState();
        newState.is3D = this.is3D;
        newState.rotation = this.transform.rotation;
        newState.position = this.transform.position;
        //Save current model params
        Model model = new Model();
        model.scale = scale;
        model.model_idx = this.model.model_idx;
        newState.model = model;
        states.Add(newState);
        
        //Save current viewstate
    }
    public String SerializeSequence(){
        string output = "";
        foreach(ViewState state in states){
            output += $"{state.Serialize()}\n";
        };
        return output;
    }
    public void LoadStateIndex(int index){
        if(index < 0 || index >= states.Count){
            return;
        }

        this.currentStateIdx = index;
        this.LoadState(states[index]);
    }
    



}
