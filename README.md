# LumePadModelViewer

Model viewer for LumePad experiment

## Setup

Load the project using Unity LTS Unity 2021

Download [LeiaLoft SDK](https://www.leiainc.com/developer-resources)

Drag LeiaLoftSDK_Public unity package file from download into assets folder in Unity editor and import the SDK

## Python Server

To use the python connection make sure the websocket-client library is installed
aswell as customtkinter in order to use the GUI client

```
pip install websocket-client
pip install customtkinter
```

### Commands

To use the python script first make sure the App is loaded on the tablet and connected to the same network as the location where you intend to run the script

Then run the GUI.py script.

#### Create a stimuli sequence for later testing

Open the App on the LumePad and use the Add state button to add the current state to the sequence.
You can overwrite states by first getting to the orientation you would like, then tapping the save button on the state you wish to overwrite

On the python client in the Create Sequence section type in the name of the sequence, and click the Save button

A file will be created locally under the ./Sequences/ with the desired sequence

#### Loading a sequence back for experimentation

Type in the name of the sequence you would like to load in the Load Section on the python client

Press Load to load the sequence and switch to the first state of that sequence

Press Next to iterate through the sequence


### TODO

- Fix LSL 
- Implement test subject view with desired functionality, spacial orientation test stuff




