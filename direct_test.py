#!/usr/bin/env python3
"""
Direct Test for SearchSnapVehicleByKey
======================================

Direct test using the exact payload provided by the user.
"""

import requests
import base64
import xml.etree.ElementTree as ET
import datetime

def direct_test():
    """Direct test with the user's payload"""
    
    print("🔍 Direct Test for SearchSnapVehicleByKey")
    print("=" * 50)
    
    # Camera settings - modify these for your camera
    HOST = "192.168.60.254"  # Change to your camera IP
    PORT = 80                # Change if needed
    USERNAME = "admin"       # Change if needed
    PASSWORD = "admin"       # Change if needed
    
    print(f"📹 Testing with camera: {HOST}:{PORT}")
    print(f"👤 Username: {USERNAME}")
    
    # Prepare headers
    auth_str = f"{USERNAME}:{PASSWORD}"
    auth_bytes = auth_str.encode('ascii')
    base64_auth = base64.b64encode(auth_bytes).decode('ascii')
    headers = {
        'Connection': 'Keep-Alive',
        'Content-Type': 'application/xml; charset=UTF-8',
        'Accept': 'application/xml; charset=UTF-8',
        'Authorization': f'Basic {base64_auth}',
        'User-Agent': 'Mozilla/5.0'
    }
    
    # Your exact payload
    payload = '''<?xml version="1.0" encoding="utf-8" ?><config><search>  <snapTime type="uint64">1750156411341200</snapTime>  <vehicleID type="uint32">1451</vehicleID>  <requestPanoramicPic type="boolean">true</requestPanoramicPic></search></config><token type="string"><![CDATA[4BB35D8B-A15A-D946-A271-364E0658D934]]></token><sessionId type="string"><![CDATA[D461EED8-B99B-1B4C-ABAB-16F2830502D7]]></sessionId>'''
    
    url = f"http://{HOST}:{PORT}/SearchSnapVehicleByKey"
    
    print(f"\n📤 Sending request to: {url}")
    print("📤 Payload:")
    print(payload)
    
    try:
        print("\n🔄 Sending request...")
        response = requests.post(url, headers=headers, data=payload.encode('utf-8'), timeout=30)
        
        print(f"\n📥 Response Status Code: {response.status_code}")
        print(f"📥 Response Headers:")
        for key, value in response.headers.items():
            print(f"  {key}: {value}")
        
        print(f"\n📥 Response Content:")
        response_text = response.content.decode('utf-8', errors='replace')
        print(response_text)
        
        # Parse and analyze response
        if response.status_code == 200:
            try:
                root = ET.fromstring(response.content)
                
                # Check for errors
                status = root.get('status')
                if status == 'failed':
                    error_code = root.get('errorCode', 'unknown')
                    print(f"\n❌ Request failed with error code: {error_code}")
                else:
                    print(f"\n✅ Request successful!")
                    
                    # Try to parse vehicle details
                    snap_vehicle = root.find('.//snapVehicle')
                    if snap_vehicle is not None:
                        snap_info = snap_vehicle.find('.//snapInfo')
                        if snap_info is not None:
                            print("\n📋 Vehicle Details Found:")
                            for child in snap_info:
                                tag = child.tag.split('}')[-1] if '}' in child.tag else child.tag
                                if tag == 'pictureData':
                                    print(f"  {tag}: Available (base64 encoded, {len(child.text)} characters)")
                                else:
                                    print(f"  {tag}: {child.text}")
                        else:
                            print("❌ No snap info found in response")
                    else:
                        print("❌ No vehicle data found in response")
                        
            except ET.ParseError as e:
                print(f"❌ Failed to parse XML response: {e}")
        else:
            print(f"❌ HTTP request failed with status code: {response.status_code}")
            
    except requests.exceptions.Timeout:
        print("❌ Request timed out")
    except requests.exceptions.ConnectionError:
        print("❌ Connection failed - check camera IP and network")
    except Exception as e:
        print(f"❌ Request failed: {e}")
    
    # Also test without token and sessionId
    print("\n" + "="*50)
    print("🔍 Testing Simplified Payload (without token/sessionId)")
    print("="*50)
    
    simplified_payload = '''<?xml version="1.0" encoding="utf-8"?>
<config><search>
<snapTime type="uint64">1750156411341200</snapTime>
<vehicleID type="uint32">1451</vehicleID>
<requestPanoramicPic type="boolean">true</requestPanoramicPic>
</search></config>'''
    
    print("📤 Simplified payload:")
    print(simplified_payload)
    
    try:
        response = requests.post(url, headers=headers, data=simplified_payload.encode('utf-8'), timeout=30)
        print(f"\n📥 Response Status Code: {response.status_code}")
        
        if response.status_code == 200:
            print("✅ Simplified request successful!")
            print("📥 Response:")
            print(response.content.decode('utf-8', errors='replace'))
        else:
            print(f"❌ Simplified request failed with status code: {response.status_code}")
            
    except Exception as e:
        print(f"❌ Simplified request failed: {e}")
    
    # Timestamp analysis
    print("\n" + "="*50)
    print("🔍 Timestamp Analysis")
    print("="*50)
    
    timestamp = 1750156411341200
    print(f"Original timestamp: {timestamp}")
    
    # Try different interpretations
    try:
        dt_micro = datetime.datetime.fromtimestamp(timestamp / 1000000)
        print(f"As microseconds since epoch: {dt_micro}")
    except:
        pass
    
    try:
        dt_milli = datetime.datetime.fromtimestamp(timestamp / 1000)
        print(f"As milliseconds since epoch: {dt_milli}")
    except:
        pass
    
    try:
        dt_sec = datetime.datetime.fromtimestamp(timestamp)
        print(f"As seconds since epoch: {dt_sec}")
    except:
        pass
    
    print("\n🎉 Direct test completed!")

if __name__ == "__main__":
    direct_test() 