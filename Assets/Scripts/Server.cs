using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

using Unity.Serialization.Json;

using LSL;

public class Server : MonoBehaviour
{
    WebSocketServer wssv;

    public void Start(){
        wssv = new WebSocketServer ("ws://localhost:3000");

        wssv.AddWebSocketService<ModelViewerService> ("/ModelViewer");
        wssv.Start();
        

        Debug.Log("[Server] WebSocket opened for ModelViewer at url ws://localhost:3000/ModelViewer");
    }

    void OnDestory(){
        wssv.Stop();
    }

}


public class ViewState{
    public int model_id;
    public bool is3D;
    public Vector2 Orientation;

    public String Serialize(){
        return $"{model_id},{is3D},{Orientation.x},{Orientation.y}";
    }
    public static ViewState LoadFromString(string data){
        string[] split = data.Split(",");
        ViewState state = new ViewState();
        state.model_id = int.Parse(split[0]);
        state.is3D = split[1] == "True";
        state.Orientation.x = float.Parse(split[2]);
        state.Orientation.y = float.Parse(split[3]);
        return state;

    }

}

public class ModelViewerService : WebSocketBehavior
{

    private List<ViewState> current_sequence = new List<ViewState>();
    private int viewStateIdx = 0;

    private StreamInfo info = new StreamInfo("StimulousMarkers", "Markers", 1,
                LSL.LSL.IRREGULAR_RATE, channel_format_t.cf_string, "HashInfoOrSomething");
    private StreamOutlet outlet;
    private string[] sample = {"StimulousLoaded"};
    
    protected override void OnOpen(){
        Debug.Log("[Server][ModelViewer Service] Client connected.");
        outlet = new StreamOutlet(info);
    }
    protected override void OnClose(CloseEventArgs e){
        Debug.Log("[Server][ModelViewer Service] Client disconnected.");
    }

    protected override void OnMessage (MessageEventArgs e)
    {
        //preprocess input
        String data = e.Data;
        var split = data.Split(' ', 2);
        String command = split[0];
        String args = "";
        if(split.Length > 1)
            args = split[1];

        switch (command){
            case "next":
                viewStateIdx++;
                Debug.Log(viewStateIdx);
                if (viewStateIdx < current_sequence.Count)
                {
                    Send("Loading next state");
                    CameraController.LoadState(current_sequence[viewStateIdx]);
                    outlet.push_sample(sample);
                }
                else
                {
                    Send($"Reached end of sequence of length {current_sequence.Count}");
                }
                break;
            case "get_sequence":
                Send(CameraController.GetSerializedSequence());
                break;
            case "load_sequence":
                string[] states = args.Split(' ');
                current_sequence = new List<ViewState>();
                viewStateIdx = -1;
                foreach(string state in states){
                    Debug.Log(state);
                    ViewState newViewState = ViewState.LoadFromString(state);
                    current_sequence.Add(newViewState);
                }
                Send("Sequence Loaded");
                break;
            default: 
                Send($"Error - Invalid Command \"{command}\"");
                break;
        }
    }


    protected override void OnError (ErrorEventArgs e)
    {
        Debug.LogError(e.Message);
        Debug.LogError(e.Exception);
    }

}
