using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leia;
using LeiaLoft;
using UnityEngine.UI;
public class ToggleCameraShift : MonoBehaviour
{
    [SerializeField] LeiaDisplay display;
    [SerializeField] Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle.onValueChanged.AddListener(ToggleDisplay);
        toggle.SetIsOnWithoutNotify(false);
        ToggleDisplay(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleDisplay(bool value)
    {
        display.SetCameraShiftEnabled(value);
    }
}
