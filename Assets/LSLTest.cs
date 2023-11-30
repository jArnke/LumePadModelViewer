using UnityEngine;
using LSL;
using TMPro;

public class LSLTest : MonoBehaviour
{
    StreamInfo info;
    StreamOutlet outlet;
    [SerializeField] TMP_Text status;
    // Start is called before the first frame update
    void Start()
    {
        try{
            info = new StreamInfo("MarkerStream", "Marker", 1, 0, channel_format_t.cf_string, "LumePad");
            outlet = new StreamOutlet(info);
            status.text = "success";
        }
        catch(System.Exception e){
            status.text = e.Message;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
