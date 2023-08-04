# LumePadModelViewer

Model viewer for LumePad experiment

## Setup

Load the project using Unity LTS Unity 2021

Download [LeiaLoft SDK](https://www.leiainc.com/developer-resources)

Drag LeiaLoftSDK_Public unity package file from download into assets folder in Unity editor and import the SDK

## Python Server

To use the python connection make sure the websocket-client library is installed

```pip install websocket-client```

or to upgrade the websocket client package use

```pip install --upgrade websocket-client```

### Commands

To use the python script first make sure the App is loaded on the tablet and connected to the same network as the location where you intend to run the script

Next run the main.py script

From here you will be asked to issue a command:

#### Create a stimuli sequence for later testing

First create a sequence of stimuli from within the app by selecting the model to view and moving it into the desired orientation.
Next tap the button on the top right labeled "Save State".

Repeat until you have completed your desired sequence.

Now back on the python client issue the command "store_sequence"

A file named my_seq.txt will be created with the desired sequence

#### Loading a sequence back for experimentation

Make sure the sequence file you wish to load is stored in the same directory as the main.py script.

Select the "Next Scene" button on the app to move into the test subject view

Back on the python client issue the command "send_sequence" to load the sequence onto the app

Now to begin cycling through states issue the command "next" after recieving this command the app will load the next state within the sequence and send an LSL marker upon completion


### TODO

- Ability to save and store multiple sequences
  - Currently when storing a sequence it will be saved with the name my_seq.txt can easilly modify this to allow for custom names and having multiple sequences stored at once to send back later
- Improving sent by LSL Markers
  - Currently all Markers simply send the String "StimulousLoaded",  could modify this to allow for the experimenter on the python client to specify information such as sequence name, test subject ID, and state number to allow for easier parsing of the data.




