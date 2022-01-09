#!/usr/bin/env python3

import paho.mqtt.client as mqtt
import tkinter
import sqlite3
import time
import os


# The broker name or IP address.
#broker = "localhost"
broker = "localhost"#"127.0.0.1"
# broker = "10.0.0.1"
channel = "reader/1" #"gate/e/1"
# The MQTT client.
client = mqtt.Client()

if os.environ.get('DISPLAY','') == '':
    print('no display found. Using :0.0')
    os.environ.__setitem__('DISPLAY', ':0.0')
# Thw main window.
window = tkinter.Tk()



def call_worker(card_number):
    channel_ret = channel+"/r"
    client.publish(channel_ret, payload="Success for "+card_number, qos=2, retain=False)#channel, "card:"+card_number,)


def process_message(client, userdata, message):
    # Decode message.
    message_decoded = (str(message.payload.decode("utf-8")))
    split = message_decoded.split(":")
    # Print message to console.
    if split[1] != "Client connected" and split[1] != "Client disconnected":
        print(message_decoded)
        #do szlabanu, nie rejestracji
        #call_worker(message_decoded)
    else:
        print(split[1])

    

def connect_to_broker():
    # Connect to the broker.
    client.connect(broker)
    # Send message about conenction.
    client.on_message = process_message
    # Starts client and subscribe.
    client.loop_start()
    client.subscribe(channel)#"card/number")


def disconnect_from_broker():
    # Disconnet the client.
    client.loop_stop()
    client.disconnect()


def run_receiver():
    connect_to_broker()
    # Start to display window (It will stay here until window is displayed)
    window.mainloop()
    disconnect_from_broker()


if __name__ == "__main__":
    run_receiver()
