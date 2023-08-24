using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceUI : MonoBehaviour
{
    [SerializeField] GameObject contentView;
    [SerializeField] GameObject stateUIElement;
    
    [SerializeField] CameraController camController;

    List<StateUIElement> UISequence;
    
    // Start is called before the first frame update
    void Start()
    {
        UISequence = new List<StateUIElement>();
    }

    // Update is called once per frame
    void Update()
    {
        List<ViewState> sequence = camController.GetSequence();

        if (UISequence.Count < sequence.Count){
            for(int i  = UISequence.Count; i<sequence.Count; i++){
                StateUIElement newState = GameObject.Instantiate(stateUIElement, contentView.transform).GetComponent<StateUIElement>();
                UISequence.Add(newState);
            }
        }

        else if(UISequence.Count > sequence.Count){
            for(int i = UISequence.Count; i>sequence.Count; i--){
                Destroy(UISequence[i-1].gameObject);
                UISequence.RemoveAt(i-1);
            }
        }

        for(int i = 0; i<UISequence.Count; i++){
            UISequence[i].index = i;
            UISequence[i].selected = false;
        }

        if(UISequence.Count != 0){
            UISequence[camController.currentState].selected = true;
        }

    }




}
