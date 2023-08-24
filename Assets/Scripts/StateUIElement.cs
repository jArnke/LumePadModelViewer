using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class StateUIElement : MonoBehaviour
{
    public int index;
    public bool selected;
    private CameraController cam;

    private Button remove;
    private Button load;
    private Button save;
    private Image image;
    public Color selectedColor;
    public Color defaultColor;
    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraController>();

        remove = transform.Find("Delete").GetComponent<Button>();
        remove.onClick.AddListener( removeThis );
        
        load = transform.Find("View").GetComponent<Button>();
        load.onClick.AddListener( loadThis );

        save = transform.Find("Overwrite").GetComponent<Button>();
        save.onClick.AddListener( saveThis );
        

        image=gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
       if(selected) image.color = selectedColor; 
       else image.color = defaultColor;
    }

    void removeThis(){
        cam.RemoveState(index);
    }

    void saveThis(){
        cam.OverwriteState(index);
    }

    void loadThis(){
        cam.SelectState(index);
    }
}
