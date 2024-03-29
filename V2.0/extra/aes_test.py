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
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
import random
import string
import secrets

string_for_data = ""
array_for_CBC_mode = bytearray(16)
back_aes_key = bytearray(32)
decract = 0

aes_key = bytearray([
   0xd1,0xf0,0x68,0x5b,
   0x33,0xa0,0xb1,0x73,
   0xb6,0x25,0x54,0xf9,
   0xdd,0x2c,0xd3,0x1d,
   0xc1,0x93,0xb3,0x14,
   0x16,0x76,0x28,0x59,
   0x04,0x85,0xd4,0x24,
   0x9d,0xe0,0x2a,0x74
])

def back_aes_k():
    global back_aes_key
    back_aes_key = bytearray(aes_key)

def rest_aes_k():
    global aes_key
    aes_key = bytearray(back_aes_key)

def incr_aes_key():
    global aes_key
    i = 15
    while i >= 0:
        if aes_key[i] == 255:
            aes_key[i] = 0
            i -= 1
        else:
            aes_key[i] += 1
            break

def encrypt_iv_for_aes(iv):
    global array_for_CBC_mode
    array_for_CBC_mode = bytearray(iv)
    encrypt_with_aes(bytearray(iv))

def encrypt_with_aes(to_be_encrypted):
    global string_for_data
    global decract
    to_be_encrypted = bytearray(to_be_encrypted)  # Convert to mutable bytearray
    if decract > 0:
        for i in range(16):
            to_be_encrypted[i] ^= array_for_CBC_mode[i]
            
    cipher = AES.new(aes_key, AES.MODE_ECB)
    encrypted_data = cipher.encrypt(pad(to_be_encrypted, AES.block_size))
    incr_aes_key()
    if decract > 0:
        for i in range(16):
            if i < 16:
                array_for_CBC_mode[i] = int(encrypted_data[i])
    
    for i in range(16):
        if encrypted_data[i] < 16:
            string_for_data += "0"
        string_for_data += hex(encrypted_data[i])[2:]
    
    decract += 11
    
def decrypt_string_with_aes_in_cbc(ct):
    global decract
    global array_for_CBC_mode
    global string_for_data
    back_aes_k()
    clear_variables()
    ct_bytes = bytes.fromhex(ct)
    ext = 0
    decract = -1
    while len(ct) > ext:
        split_for_decr(ct_bytes, ext)
        ext += 16
        decract += 10

    rest_aes_k()

def split_for_decr(ct, p):
    global decract
    global array_for_CBC_mode
    global string_for_data

    res = bytearray(16)
    prev_res = bytearray(16)
    br = False

    for i in range(0, 16):
        if i + p > len(ct) - 1:
            br = True
            break
        res[i] = ct[i + p]

    for i in range(0, 16):
        if i + p - 16 > len(ct) - 1:
            break  # Skip if index is out of bounds
        prev_res[i] = ct[i + p - 16]

    if not br:
        if decract > 16:
            array_for_CBC_mode = prev_res[:]

        cipher_text = res
        ret_text = bytearray(16)

        cipher = AES.new(bytes(aes_key), AES.MODE_ECB)
        ret_text = bytearray(cipher.decrypt(bytes(cipher_text)))

        incr_aes_key()

        if decract > 2:
            for i in range(16):
                ret_text[i] ^= array_for_CBC_mode[i]

            for byte in ret_text:
                if byte > 0:
                    string_for_data += chr(byte)
                    

        if decract == -1:
            array_for_CBC_mode = ret_text[:]

        decract += 1

def decrypt_hex_str_with_aes_in_cbc(ct):
    global decract
    global array_for_CBC_mode
    global string_for_data
    back_aes_k()
    clear_variables()
    ct_bytes = bytes.fromhex(ct)
    ext = 0
    decract = -1
    while len(ct) > ext:
        split_for_decr_hex_str(ct_bytes, ext)
        ext += 16
        decract += 10

    rest_aes_k()

def split_for_decr_hex_str(ct, p):
    global decract
    global array_for_CBC_mode
    global string_for_data

    res = bytearray(16)
    prev_res = bytearray(16)
    br = False

    for i in range(0, 16):
        if i + p > len(ct) - 1:
            br = True
            break
        res[i] = ct[i + p]

    for i in range(0, 16):
        if i + p - 16 > len(ct) - 1:
            break  # Skip if index is out of bounds
        prev_res[i] = ct[i + p - 16]

    if not br:
        if decract > 16:
            array_for_CBC_mode = prev_res[:]

        cipher_text = res
        ret_text = bytearray(16)

        cipher = AES.new(bytes(aes_key), AES.MODE_ECB)
        ret_text = bytearray(cipher.decrypt(bytes(cipher_text)))

        incr_aes_key()

        if decract > 2:
            for i in range(16):
                ret_text[i] ^= array_for_CBC_mode[i]

            string_for_data += ''.join(format(byte, '02x') for byte in ret_text)
                    

        if decract == -1:
            array_for_CBC_mode = ret_text[:]

        decract += 1

def clear_variables():
    global string_for_data
    global decract
    string_for_data = ""
    decract = 0

def encr_str_with_aes():
    global string_for_data
    global decract
    back_aes_k()
    string_for_data = ""
    decract = 0
    
    iv = [secrets.randbelow(256) for _ in range(16)]  # Initialization vector
    encrypt_iv_for_aes(iv)

def encrypt_string_with_aes_in_cbc(input_string):
    global string_for_data
    global decract
    back_aes_k()
    string_for_data = ""
    decract = 0
    
    iv = [secrets.randbelow(256) for _ in range(16)]  # Initialization vector
    encrypt_iv_for_aes(iv)
    padded_length = (len(input_string) + 15) // 16 * 16
    padded_string = input_string.ljust(padded_length, '\x00')
    byte_arrays = [bytearray(padded_string[i:i+16], 'utf-8') for i in range(0, len(padded_string), 16)]
    
    for i, byte_array in enumerate(byte_arrays):
        encrypt_with_aes(byte_array)
    
    rest_aes_k()
    
def encrypt_hex_str_with_aes_in_cbc(input_string):
    global string_for_data
    global decract
    back_aes_k()
    string_for_data = ""
    decract = 0
    hex_array = bytes.fromhex(input_string)
    iv = [secrets.randbelow(256) for _ in range(16)]  # Initialization vector
    encrypt_iv_for_aes(iv)
    split_arrays = [hex_array[i:i+16] for i in range(0, len(hex_array), 16)]
    for i, arr in enumerate(split_arrays):
        encrypt_with_aes(arr)
    
    rest_aes_k()

encrypt_hex_str_with_aes_in_cbc("1234ABCD1234abcd1234ABCD1234abcd")
#print(string_for_data)
ciphertext = string_for_data
decrypt_hex_str_with_aes_in_cbc(ciphertext)
print(string_for_data)