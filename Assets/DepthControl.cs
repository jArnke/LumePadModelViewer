using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using LeiaLoft;

public class DepthControl : MonoBehaviour
{
    [SerializeField] private LeiaFocus focus;
    private Slider slider;
    [SerializeField] private float maxDepth;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdateDepth(){
        float depthValue = maxDepth*slider.value;
        focus.DepthScale = depthValue;
    }
}
