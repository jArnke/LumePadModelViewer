using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LeiaLoft;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;
    [SerializeField] private float vertSpeed;

    [SerializeField] private LeiaDisplay leiaDisplay;
    // Start is called before the first frame update
    void Start(){
        ResetDisplay(); 
    }
    // Update is called once per frame
    void Update(){
        //Look for User Input
        if(Input.touchCount >= 1){
            Vector2 swipe = Input.GetTouch(0).deltaPosition;
            Rotate(speed*swipe.x,vertSpeed*swipe.y); 
        }
    }
    public void ResetDisplay(){
        Vector3 pivot = target.position+offset;
        this.transform.position = pivot + new Vector3(distance, 0, 0);
        this.transform.LookAt(pivot);
        
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

       //Calculate new position
       Vector3 pivotPoint = target.position + offset;
       this.transform.RotateAround(pivotPoint, target.up, horizontalAngleShift);
       this.transform.RotateAround(pivotPoint, -this.transform.right, verticalAngleShift);
    }
}
