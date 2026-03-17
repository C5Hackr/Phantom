#Requires dnlib.dll in the same folder, RUN AT YOUR OWN RISK!
#USER PROVIDED SCRIPT!

import re
import subprocess
import os
import sys
import base64
import gzip
import struct
from Crypto.Cipher import AES
from Crypto.Util.Padding import unpad

from pythonnet import load
load("coreclr")

import clr

import System
from System.IO import MemoryStream

dnlib = os.path.abspath(r".\dnlib.dll")
clr.AddReference(dnlib)

from dnlib.DotNet import ModuleDefMD, EmbeddedResource
from dnlib.DotNet.Emit import OpCodes

variables = {}
extracted_resources = []

def analyze_payload(path):
    print(f"--- Analyzing: {os.path.basename(path)} ---")
    module = ModuleDefMD.Load(path)

    # If no entry point or resources → likely AMSI stub
    if module.EntryPoint is None or len(list(module.Resources)) == 0:
        print(f"[*] {path} is likely the AMSI bypass (unimportant)")
        return

    entry = module.EntryPoint
    base64_strings = []

    if entry.HasBody:
        print(f"[*] Scanning EntryPoint for Base64 strings...")
        for instr in entry.Body.Instructions:
            if instr.OpCode == OpCodes.Ldstr:
                val = str(instr.Operand)

                if (
                    len(val) > 20
                    and all(c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=" for c in val)
                ):
                    base64_strings.append(val)
        print(f"    - Found {len(base64_strings)} potential keys/IVs.")

    # Extract Embedded Resources
    print(f"[*] Total Resources Found: {len(list(module.Resources))}")

    for i, res in enumerate(module.Resources):
        safe_name = re.sub(r'[<>:"/\\|?*]', '_', str(res.Name))
        res_type_name = type(res).__name__
        
        print(f"[{i}] Processing: {safe_name}")
        print(f"    - Type: {res_type_name}")
        print(f"    - Attributes: {res.Attributes}")

        try:
            data = None
            
            # Attempt 1: Manual Copy to MemoryStream
            if hasattr(res, "CreateReader"):
                reader = res.CreateReader()
                if reader is not None:
                    print(f"    - Reader Length: {reader.Length} bytes")
                    ms = MemoryStream()
                    reader.CopyTo(ms)
                    data = bytes(ms.ToArray())
                    print(f"    - Extracted via MemoryStream: {len(data)} bytes")
            
            # Attempt 2: Fallback to .Data property
            if data is None or len(data) == 0:
                if hasattr(res, "Data") and res.Data is not None:
                    data = bytes(res.Data)
                    print(f"    - Fallback to .Data: {len(data)} bytes")

            # Final Save
            if data and len(data) > 0:
                out_path = os.path.join("extracted", safe_name + ".res")
                extracted_resources.append(out_path)
                with open(out_path, "wb") as f:
                    f.write(data)
                print(f"    [SUCCESS] Saved to {out_path}")
            else:
                print(f"    [WARNING] Resource contains no data or could not be read.")

        except Exception as e:
            print(f"    [ERROR] Failed to extract {safe_name}: {e}")

    print("--- Analysis Complete ---")
    return base64_strings

def reconstruct_batch(contents): # In case the batch file is scrambled (some custom versions do that)
    print("[*] Reconstructing batch flow...")
    lines = [l.rstrip() for l in contents.split('\n')]

    label_blocks = {}
    current_label = None

    for line in lines:
        if line.startswith("::"): # Commented payload, not a label
            # Note: The original code had lines.remove(line) here which can skip lines during iteration.
            continue
        elif line.startswith(":"):
            current_label = line[1:].strip()
            label_blocks[current_label] = []
        elif current_label:
            label_blocks[current_label].append(line)

    # find entry GOTO
    entry = None
    for line in lines:
        m = re.match(r"GOTO\s+([A-Za-z0-9_]+)", line, re.I)
        if m:
            entry = m.group(1)
            break

    if not entry:
        print('    - Batch file not scrambled.')
        return contents

    print(f"    - Found Entry Label: {entry}")
    ordered = []
    visited = set()
    cur = entry

    while cur and cur not in visited:
        visited.add(cur)
        block = label_blocks.get(cur, [])

        next_label = None
        for l in block:
            if l.upper().startswith("GOTO"):
                next_label = l.split()[1]
            else:
                ordered.append(l)

        cur = next_label

    return "\n".join(ordered)

def aes_decrypt(key_bytes, iv_bytes, ciphertext):
    try:
        cipher = AES.new(key_bytes, AES.MODE_CBC, iv_bytes)
        plaintext = unpad(cipher.decrypt(ciphertext), AES.block_size) # Try to decrypt embedded payloads
        return plaintext
    except:
        # If were unsure which is the key and which is the IV due to them being the same size
        # Then try it the other way around.
        print("    [!] Decryption failed, swapping Key and IV...")
        cipher = AES.new(iv_bytes, AES.MODE_CBC, key_bytes)
        plaintext = unpad(cipher.decrypt(ciphertext), AES.block_size) # Try to decrypt embedded payloads
        return plaintext

def main(filepath):
    print(f"=== Starting Decryption Process: {filepath} ===")
    with open(filepath, 'r') as f:
        contents = f.read()
        
        # Keep original logic: splitting by newline but also using reconstruction
        lines = contents.split('\n')
        setvar = ''
        ps_command = ''
        payload_line = ''
        done = False
        key = ''
        iv = ''
        
        contents = reconstruct_batch(contents)
        
        print("[*] Parsing Batch variables...")
        while True:
            if done == True:
                break

            for line in lines:
                if re.match(r'^::\s*', line): # Detect payload stored in comment like "::   payload"
                    payload_line = re.sub(r'^::\s*', '', line)
                    continue

                if '!e!' in line:  # Find the line where it sets the delayed expanded set variable
                    setter = re.findall('set ".*?=', line, re.I)
                    if not setter:
                        print("[ERROR] Failed to find delayed expansion variable.")
                        exit(1)
                    setvar = setter[0].replace('set "', '').replace('=', '')  # Find the DE set var
                    lines.remove(line)
                    break

                if line.startswith('!' + setvar + '!'):  # Find all lines setting variable chunks
                    var, value = (line.replace('!' + setvar + '! "', '').replace('"', '')).split('=', 1)
                    variables[var] = value

                if line.startswith('%'):  # Find the execution line
                    unwanted = re.findall('%[A-Za-z]+%s%[A-Za-z]+%e%[A-Za-z]+%t%', line) # Filter out the setlocal enabledelayedexpansion line which also begins with %
                    if unwanted:
                        lines.remove(line)
                        continue

                    results = [x for x in line.split("%") if x] # List of all the command chunks in order
                    for result in results:
                        if result in variables:
                            ps_command += variables[result] # Reconstruct the powershell command from the chunks
                    
                    if payload_line != '':
                        done = True
                        break
                    else:
                        continue
            
        print("[*] Extracting Layer 1 Metadata...")
        keys = re.findall(r"\('((?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?)'\)", ps_command) # Find Key and IV in base64 in the powershell command.
        
        if len(base64.b64decode(keys[0])) > len(base64.b64decode(keys[1])): # Key is longer than IV unless its a custom version
            key = keys[0]
            iv = keys[1]
        elif len(base64.b64decode(keys[0])) < len(base64.b64decode(keys[1])):
            key = keys[1]
            iv = keys[0]
        else:
            key = keys[0]
            iv = keys[1]
            print("    [!] Unsure which is the Key and the IV.")

        print(f"    - Key: {key}\n    - IV:  {iv}")

        key_bytes = base64.b64decode(key)
        iv_bytes = base64.b64decode(iv)

        payloads = payload_line.split('\\')
        if not os.path.isdir("extracted"):
            os.mkdir("extracted")

        for payload in range(len(payloads)):
            print(f"[*] Decrypting Layer 1 Payload Chunk [{payload}]...")
            # Decrypt and decompress payload using AES and Gzip
            decrypted = aes_decrypt(
                key_bytes,
                iv_bytes,
                base64.b64decode(payloads[payload]),
            )
            
            decompressed = gzip.decompress(decrypted)
            bin_path = f'extracted\\loader_{payload}.bin'
            with open(bin_path, 'wb') as f:
                f.write(decompressed)
            print(f"    [SUCCESS] Saved to {bin_path}")

            keys = analyze_payload(bin_path)

            if keys != None and len(keys) >= 2:
                print("[*] Extracting Layer 2 Metadata...")
                if len(base64.b64decode(keys[0])) > len(base64.b64decode(keys[1])): # Key is again longer than IV unless its a custom version
                    key = keys[0]
                    iv = keys[1]
                elif len(base64.b64decode(keys[0])) < len(base64.b64decode(keys[1])):
                    key = keys[1]
                    iv = keys[0]
                else:
                    key = keys[0]
                    iv = keys[1]
                
                print(f"    - Second Key: {key}\n    - Second IV:  {iv}")
                
                key_bytes = base64.b64decode(key)
                iv_bytes = base64.b64decode(iv)

                for res_path in extracted_resources:
                    if os.path.exists(res_path):
                        with open(res_path, "rb") as f:
                            resource_data = f.read()

                        print("[*] Decrypting Resource: " + res_path)
                        # Decrypt and decompress final resources using AES and Gzip
                        decrypted = None
                        decompressed = None
                        try:
                            # Only payload.exe is encrypted. UAC is only compressed, and binded files arent changed at all
                            decrypted = aes_decrypt(
                                key_bytes,
                                iv_bytes,
                                resource_data,
                            )
                        except:
                            decrypted = resource_data

                        try:
                            decompressed = gzip.decompress(decrypted)
                        except:
                            decompressed = resource_data

                        final_path = res_path.replace(".res", "").replace(".exe", ".bin")
                        with open(final_path, 'wb') as f:
                            f.write(decompressed)
                        os.remove(res_path)
                        print(f"    [SUCCESS] Final Resource saved to {final_path}")

if __name__ == '__main__':
    inp = ""
    if len(sys.argv) > 1:
        inp = sys.argv[1]
    else:
        inp = input('Enter crypted .bat path: ')
    main(inp)
