using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LeiaLoft;

//using LSL;

public class CameraController : MonoBehaviour
{
    public static CameraController cam;
    private Transform target;

    [SerializeField] private float distance;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;
    [SerializeField] private float vertSpeed;

    private LeiaDisplay leiaDisplay;

    [SerializeField] private GameObject[] models;

    private Vector2 currOffset;

    private int model_num = 0;
    
    protected bool newSequence = false;
    protected List<ViewState> nextSequence;


    public List<ViewState> ViewSequence;
    public int currentState;
    protected bool loading = false;
    protected bool loadNext = false;
    protected ViewState nextState;

    /*LSL Stuff
    private StreamInfo info = new StreamInfo("StimulousMarkers", "Markers", 1,
                LSL.LSL.IRREGULAR_RATE, channel_format_t.cf_string, "LumePadModelViewer");
    private StreamOutlet outlet;
    private string[] sample = {"StimulousLoaded"};
    */
    private bool shouldRecordSample;
    
    void Awake(){
        shouldRecordSample = false;
        if(CameraController.cam == null) CameraController.cam = this;
        else{
            Destroy(this);
            Debug.LogError("More than one CameraController Exists Destorying");
        }
        leiaDisplay = GameObject.FindGameObjectWithTag("LeiaDisplay").GetComponent<LeiaDisplay>();
     //   HandleLSL();
    }
    // Start is called before the first frame update
    void Start(){
        this.SetTarget(this.LoadNewModel(models[model_num]));
        ResetDisplay(); 
        ViewSequence = new List<ViewState>();
        Debug.Log(cam.ViewSequence);
    }

    // Update is called once per frame
    void Update(){
        //Look for User Input
        if(Input.touchCount >= 1){
            Vector2 swipe = Input.GetTouch(0).deltaPosition;
            Rotate(speed*swipe.x,vertSpeed*swipe.y); 
        }
        if (loading)
            SetState(nextState);
        if (newSequence){
            ViewSequence = nextSequence;
            currentState = 0;
            newSequence = false;
            SetState(ViewSequence[currentState]);
        }
        if(loadNext){
            if (currentState+1 < ViewSequence.Count){
                currentState++;
                SetState(ViewSequence[currentState]);
            }
            loadNext = false;
        }
    }

    public void CycleModel(int dir){
        model_num += dir;
        if(model_num < 0){
            model_num = this.models.Length - 1;
        }
        model_num = model_num % this.models.Length;
        this.SetTarget(this.LoadNewModel(models[model_num]));
        ResetDisplay(); 
    }

    public void ResetDisplay(){
        Vector3 pivot = target.position+offset;
        this.transform.position = pivot + new Vector3(distance, 0, 0);
        this.transform.LookAt(pivot);
        currOffset = Vector2.zero;
        
    }
    public void SetTarget(Transform target){
        this.target = target;
        ResetDisplay();
    }
    public void SetTarget(GameObject target){
        this.target = target.transform;
        ResetDisplay(); 
    }
    public void Toggle3D(){
        leiaDisplay.DesiredLightfieldValue = 1 ^ leiaDisplay.DesiredLightfieldValue;
    }



    void Rotate(float horizontalAngleShift, float verticalAngleShift){
       if(!target)return; 

       currOffset.x += horizontalAngleShift;
       currOffset.y += verticalAngleShift;

       //Calculate new position
       Vector3 pivotPoint = target.position + offset;
       this.transform.RotateAround(pivotPoint, target.up, horizontalAngleShift);
       this.transform.RotateAround(pivotPoint, -this.transform.right, verticalAngleShift);
    }

    public static void LoadState(ViewState state){
        cam.loading = true;
        cam.nextState = state;
    }
    public static void LoadSequence(List<ViewState> states){
        cam.newSequence = true;
        cam.nextSequence = states;
    }
    public static void NextState(){
        cam.loadNext = true;
    }

    public void SetState(ViewState state){
        loading = false;
        cam.SetTarget(cam.LoadNewModel(cam.models[state.model_id]));    
        model_num = state.model_id;
        cam.ResetDisplay();
        cam.leiaDisplay.DesiredLightfieldValue = (state.is3D) ? 1 : 0;
        cam.Rotate(state.Orientation.x, state.Orientation.y);
        shouldRecordSample = true;
    }

    public ViewState GetCurrentState(){
        ViewState state = new ViewState();
        state.model_id = model_num;
        state.is3D = (leiaDisplay.DesiredLightfieldValue == 1);
        state.Orientation = currOffset;
        return state;
    }

    public void AddCurrentStateToSequence(){
        AddStateToSequence(GetCurrentState());
        currentState = ViewSequence.Count - 1;
    }
    public void AddStateToSequence(ViewState state){
        ViewSequence.Add(state);
    }

    public static string GetSerializedSequence(){
        string data = "";
        foreach(ViewState state in cam.ViewSequence){
            data += state.Serialize();
            data += ' ';
        }
        return data;
    }
    public static void LoadSerializedSequence(string sequenceData){
    }
    public List<ViewState> GetSequence(){return ViewSequence;}

    public void RemoveState(int index){
        if(index<0){return;}
        if(ViewSequence.Count < index+1){return;}

        ViewSequence.RemoveAt(index);

        if(index <= currentState){currentState--;}
        if(currentState < 0){currentState = 0;}

        if(ViewSequence.Count > 0){
            SelectState(currentState);
        }

    }

    public void SelectState(int index){
        if(index < 0) {return;}
        if(ViewSequence.Count < index + 1){return;}

        currentState = index;
        LoadState(ViewSequence[index]);
    }

    public void OverwriteState(int index){
        if(index < 0) {return;}
        if(ViewSequence.Count < index + 1){return;}

        ViewSequence[index] = GetCurrentState();
    }

    private GameObject LoadNewModel(GameObject model){
        if(this.target != null)
            Destroy(this.target.gameObject);
        this.target = Instantiate(model).transform;
        return this.target.gameObject;
    }

    /*
    IEnumerator HandleLSL(){
        outlet = new StreamOutlet(info);
        while(true)
        {
            yield return new WaitForEndOfFrame(); 
            if(shouldRecordSample)
            {
                outlet.push_sample(sample);
                shouldRecordSample = false;
            }
        }
        yield return null; 
    }
    */
}
