using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LeiaLoft;

public class CameraController : MonoBehaviour
{
    public static CameraController cam;
    [SerializeField] private Transform target;

    [SerializeField] private float distance;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;
    [SerializeField] private float vertSpeed;

    [SerializeField] private LeiaDisplay leiaDisplay;

    [SerializeField] private GameObject[] models;

    private Vector2 currOffset;
    


    protected List<ViewState> ViewSequence;
    protected bool loading = false;
    protected ViewState nextState;

    void Awake(){
        if(CameraController.cam == null) CameraController.cam = this;
        else{
            Destroy(this);
            Debug.LogError("More than one CameraController Exists Destorying");
        }
    }
    // Start is called before the first frame update
    void Start(){
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
    public void SetState(ViewState state){
        loading = false;
        Debug.Log("loading state");
        //cam.SetTarget(cam.LoadNewModel(cam.models[state.model_id]));    
        cam.ResetDisplay();
        cam.leiaDisplay.DesiredLightfieldValue = (state.is3D) ? 1 : 0;
        Debug.Log($"rotating to {state.Orientation.x}, {state.Orientation.y}");
        cam.Rotate(state.Orientation.x, state.Orientation.y);

    }

    public ViewState GetCurrentState(){
        ViewState state = new ViewState();
        state.model_id = 0;
        state.is3D = (leiaDisplay.DesiredLightfieldValue == 1);
        state.Orientation = currOffset;
        return state;
    }

    public void AddCurrentStateToSequence(){
        AddStateToSequence(GetCurrentState());
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

    private GameObject LoadNewModel(GameObject model){
        return this.target.gameObject;
    }
}
