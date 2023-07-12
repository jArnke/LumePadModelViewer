import websocket
import time

#Connect to server:
ws = websocket.create_connection("ws://localhost:3000/ModelViewer");


while(True):
    command = input("enter a command: ")
    start = time.time()
    if command == "quit":
        break;
    if command == "store_sequence":
        ws.send("get_sequence")
        seq = ws.recv()
        file = open("my_seq.txt", "w")
        file.write(seq)
        file.close()
        continue
    if command == "send_sequence":
        filename = "my_seq.txt" #input("enter filename: ")
        file = open(filename, "r")
        ws.send("load_sequence " + file.read())
        print(ws.recv())
    else:
        ws.send(command)
        print(ws.recv())
    end = time.time()
    print(1000*(end - start))


ws.close()
