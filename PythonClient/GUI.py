import customtkinter;
import websocket

customtkinter.set_appearance_mode("dark")
customtkinter.set_default_color_theme("dark-blue")

root = customtkinter.CTk()
root.geometry("800x600")

# Functions:

def sendRecv(command):
    maxTries = 5
    result = ""
    for attempt in range(maxTries):
        try:
            ws.send(command)
            result = ws.recv()
        except:
            # reconnect
            server = serverAddr.get()
            ws = websocket.create_connection("ws://localhost:3000/ModelViewer");
        else:
            break
    else:
        # Can't Connect to server...
        print("Connection Refused")
    return result
def saveSequence():
    seq = sendRecv("get_sequence")
    filePath = sequenceName.get()
    file = open("Sequences/"+filePath+".txt", "w")
    file.write(seq)
    file.close()
    return
def loadSequence():
    filename = loadName.get() 
    file = open("Sequences/"+filename+".txt", "r")
    sendRecv("load_sequence " + file.read())
    file.close()
    return
def nextState():
    sendRecv("next")
    return


serverSection = customtkinter.CTkFrame(master=root)
serverSection.pack(pady=20, padx=60, fill="both", expand=True)

serverHeader = customtkinter.CTkLabel(master=serverSection, text = "Server")
serverHeader.pack(pady=12, padx=10)

serverAddr  = customtkinter.CTkEntry(master=serverSection, width = 300)
serverAddr.insert(0, "ws://localhost:3000/ModelViewer")
serverAddr.pack(pady=12, padx=10)


creation = customtkinter.CTkFrame(master=root)
creation.pack(pady=20, padx=60, fill="both", expand=True)

label = customtkinter.CTkLabel(master=creation, text="Create Sequence")
label.pack(pady=12, padx=10)

sequenceName  = customtkinter.CTkEntry(master=creation, width = 300)
sequenceName.insert(0, "MySequence")
sequenceName.pack(pady=12, padx=10)

saveButton = customtkinter.CTkButton(master=creation, corner_radius=10, command=saveSequence, text="Save Sequence")
saveButton.pack()

control = customtkinter.CTkFrame(master=root)
control.pack(pady=20, padx=60, fill="both", expand=True)

controlLabel = customtkinter.CTkLabel(master=control, text="Replay Sequence")
controlLabel.pack(pady=12, padx=10)

loadName  = customtkinter.CTkEntry(master=control, width = 300)
loadName.insert(0, "MySequence")
loadName.pack(pady=12, padx=10)

loadButton = customtkinter.CTkButton(master=control, corner_radius=10, command=loadSequence, text="Load Sequence")
loadButton.pack(pady=12, padx=10)

nextButton = customtkinter.CTkButton(master=control, corner_radius=10, command=nextState, text="Next State")
nextButton.pack(pady=12, padx=10)

# Websocket Setup:
ws = None


root.mainloop()

