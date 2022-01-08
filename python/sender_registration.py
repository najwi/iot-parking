#!/usr/bin/env python3

import paho.mqtt.client as mqtt
import tkinter
import random
import config
### RaspberryPi version: ###

#import time
#import RPi.GPIO as GPIO
#from config import *  # pylint: disable=unused-wildcard-import
#from mfrc522 import MFRC522

#def rfidRead():
#    MIFAREReader = MFRC522()
#    counter = 0
#    uid = 0
#    while counter < 3:
#        (status, TagType) = MIFAREReader.MFRC522_Request(MIFAREReader.PICC_REQIDL)
#        if status == MIFAREReader.MI_OK:
#            (status, uid) = MIFAREReader.MFRC522_Anticoll()
#            if status == MIFAREReader.MI_OK:
#                num = 0
#                for i in range(0, len(uid)):
#                    num += uid[i] << (i*8)
#                print(f"Card read UID: {uid} > {num}")
#                time.sleep(0.5)
#                counter += 1
#    return uid

### END - RaspberryPi version ###

client_id = f'python-mqtt-{random.randint(0, 1000)}'
terminal_id = "Gate1"
topic =  config.topic_register
broker = config.broker #"localhost" #"192.168.1.12" #"127.0.0.1"
client = mqtt.Client(client_id)
window = tkinter.Tk()


def call_worker(card_number):
    client.publish(topic, payload="card:"+card_number, qos=2, retain=False)


def create_main_window():
    window.geometry("400x200")
    window.title("SENDER")

    tkinter.Label(window, text="Card to add: ").grid(row=3, padx=5, pady=5)

    e1 = tkinter.Entry(window)
    e1.grid(row=3, column=2, pady=5, ipady=7)
     
    #card_number = rfidRead() #for raspberry

     
    button_send = tkinter.Button(window, text="Add",
                              command=lambda: call_worker(e1.get())) #(card_number))
    button_send.grid(row=4, column = 0, columnspan=3, sticky=tkinter.E, pady=5, ipady=3, ipadx=7)

    button_stop = tkinter.Button(window, text="Stop", command=window.quit)
    button_stop.grid(row=4, column = 2, columnspan=3, sticky=tkinter.W, pady=5, ipady=3, ipadx=7 )



def connect_to_broker():
    # Connect to the broker.
    client.connect(broker)
    # Send message about conenction.
    call_worker("Client connected")
    client.loop_start()


def disconnect_from_broker():
    call_worker("Client disconnected")
    client.disconnect()


def run_sender():
    connect_to_broker()
    create_main_window()
    window.mainloop()
    disconnect_from_broker()


if __name__ == "__main__":
    run_sender()





