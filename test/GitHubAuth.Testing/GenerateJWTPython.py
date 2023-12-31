﻿
#!/usr/bin/env python3
#python3 GenerateJWTPython.py "sample_key.pem" 123456

import jwt
import time
import sys

# Get PEM file path
if len(sys.argv) > 1:
    pem = sys.argv[1]
else:
    pem = input("Enter path of private PEM file: ")

# Get the App ID
if len(sys.argv) > 2:
    app_id = sys.argv[2]
else:
    app_id = input("Enter your APP ID: ")

# Open PEM
with open(pem, 'rb') as pem_file:
    signing_key = jwt.jwk_from_pem(pem_file.read())

payload = {
    # Issued at time
    #'iat': int(time.time()),
    'iat': 1577836800,
    # JWT expiration time (10 minutes maximum)
    #'exp': int(time.time()) + 600,
    'exp': 1577837280,
    # GitHub App's identifier
    'iss': app_id
}

# Create JWT
jwt_instance = jwt.JWT()
encoded_jwt = jwt_instance.encode(payload, signing_key, alg='RS256')

print(f"JWT:  {encoded_jwt}")


print(f"Issued At: {int(time.time())}")
print(f"Expires At + 600: {int(time.time())+600}")

print(f"January 1st 1970: {time.gmtime(0)}")
print(f"January 1st 2020: {time.gmtime(1577836800)}")