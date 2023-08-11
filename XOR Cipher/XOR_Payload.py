#!/usr/bin/env python3
import os
import sys

KEY = "NyaMeeEain"

def xor(data, key):
    key = str(key)
    key_length = len(key)
    ciphertext = bytearray()

    for i, current in enumerate(data):
        current_key = key[i % key_length]
        encrypted_byte = current ^ ord(current_key)
        ciphertext.append(encrypted_byte)

    return ciphertext

def print_ciphertext(ciphertext):
    hex_bytes = ', '.join(f'0x{byte:02X}' for byte in ciphertext)
    print('{{', hex_bytes, '}};')

def main():
    if len(sys.argv) < 2:
        print(f"Usage: {sys.argv[0]} <payload file>")
        sys.exit(1)

    input_file = sys.argv[1]
    
    if not os.path.exists(input_file):
        print(f"Error: File not found: {input_file}")
        sys.exit(1)

    with open(input_file, "rb") as file:
        plaintext = file.read()

    ciphertext = xor(plaintext, KEY)
    print_ciphertext(ciphertext)

if __name__ == "__main__":
    main()
