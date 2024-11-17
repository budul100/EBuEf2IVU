import paho.mqtt.client as mqtt

# Define the MQTT broker details
broker = 'localhost'  # Replace with your broker address
port = 1883  # Replace with your broker port if different
topic = 'mqtt4454'  # Replace with your topic
message = 'ZN SET DISPATCH'  # Replace with your message

# Define the callback function for connection
def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Connected to broker")
        client.publish(topic, message)
        print(f"Message '{message}' sent to topic '{topic}'")
    else:
        print(f"Connection failed with code {rc}")

# Create an MQTT client instance
client = mqtt.Client()

# Assign the on_connect callback function
client.on_connect = on_connect

# Connect to the broker
client.connect(broker, port, 60)

# Start the loop
client.loop_start()

# Wait for the message to be sent
import time
time.sleep(2)

# Stop the loop and disconnect
client.loop_stop()
client.disconnect()