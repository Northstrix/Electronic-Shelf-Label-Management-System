"""
DIY Electronic Shelf Label Management System
Distributed under the MIT License
© Copyright Maxim Bortnikov 2024
For more information please visit
https://sourceforge.net/projects/esl-management-system/
https://github.com/Northstrix/Electronic-Shelf-Label-Management-System
Required libraries:
https://github.com/me-no-dev/ESPAsyncUDP
https://github.com/adafruit/Adafruit-GFX-Library
https://github.com/adafruit/Adafruit_BusIO
https://github.com/adafruit/Adafruit_ILI9341
https://github.com/adafruit/Adafruit-ST7735-Library
https://github.com/Northstrix/AES_in_CBC_mode_for_microcontrollers
"""
import socket

# Define the address and port to send the UDP packet
target_ip = '192.168.137.154'  # Example IP address
target_port = 19282  # Example port number

# Create a UDP socket
udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Define the message to send
message = b"Hello, UDP!"

try:
    # Send the UDP packet
    udp_socket.sendto(message, (target_ip, target_port))
    print("UDP packet sent successfully!")
except Exception as e:
    print("Error:", e)
finally:
    # Close the socket
    udp_socket.close()