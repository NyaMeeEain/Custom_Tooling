#!/usr/bin/env python3
import os
import sys
from cryptography.hazmat.primitives import padding
from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from cryptography.hazmat.backends import default_backend

def aes_encrypt(key, data):
    backend = default_backend()
    iv = os.urandom(16)  # Initialization vector (IV) for CBC mode
    cipher = Cipher(algorithms.AES(key), modes.CBC(iv), backend=backend)
    encryptor = cipher.encryptor()
    padder = padding.PKCS7(128).padder()
    padded_data = padder.update(data) + padder.finalize()
    encrypted_data = encryptor.update(padded_data) + encryptor.finalize()
    return iv + encrypted_data

def print_ciphertext(ciphertext):
    hex_bytes = ', '.join(f'0x{byte:02X}' for byte in ciphertext)
    print('{{', hex_bytes, '}};')

def print_key(key):
    hex_bytes = ', '.join(f'0x{byte:02X}' for byte in key)
    print(f"Key: {{ {hex_bytes} }}")

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

    # Generate a random 32-byte (256-bit) key
    key = os.urandom(32)

    # Print the generated key
    print_key(key)

    # Encrypt the plaintext using AES in CBC mode
    ciphertext = aes_encrypt(key, plaintext)
    print_ciphertext(ciphertext)

if __name__ == "__main__":
    main()
