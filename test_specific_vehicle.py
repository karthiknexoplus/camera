#!/usr/bin/env python3
"""
Test Specific Vehicle Search
============================

This script tests the SearchSnapVehicleByKey API with specific parameters
provided by the user.
"""

import requests
import base64
import xml.etree.ElementTree as ET
import datetime

def test_specific_vehicle_search():
    """Test SearchSnapVehicleByKey with specific parameters"""
    
    print("🔍 Testing Specific Vehicle Search")
    print("=" * 50)
    
    # Camera connection details
    print("\n📹 Camera Connection Setup:")
    host = input("Enter camera IP address (e.g., 192.168.60.254): ").strip()
    if not host:
        host = "192.168.60.254"
    
    port = input("Enter port (default 80): ").strip()
    if not port:
        port = "80"
    
    username = input("Enter username (default admin): ").strip()
    if not username:
        username = "admin"
    
    password = input("Enter password (default admin): ").strip()
    if not password:
        password = "admin"
    
    print(f"\n🔗 Connecting to: {host}:{port}")
    print(f"👤 Username: {username}")
    
    # Prepare headers
    auth_str = f"{username}:{password}"
    auth_bytes = auth_str.encode('ascii')
    base64_auth = base64.b64encode(auth_bytes).decode('ascii')
    headers = {
        'Connection': 'Keep-Alive',
        'Content-Type': 'application/xml; charset=UTF-8',
        'Accept': 'application/xml; charset=UTF-8',
        'Authorization': f'Basic {base64_auth}',
        'User-Agent': 'Mozilla/5.0'
    }
    
    # Test 1: Original payload from user
    print("\n" + "="*50)
    print("🔍 Test 1: Original Payload from User")
    print("="*50)
    
    # Your original payload
    original_payload = '''<?xml version="1.0" encoding="utf-8" ?><config><search>  <snapTime type="uint64">1750156411341200</snapTime>  <vehicleID type="uint32">1451</vehicleID>  <requestPanoramicPic type="boolean">true</requestPanoramicPic></search></config><token type="string"><![CDATA[4BB35D8B-A15A-D946-A271-364E0658D934]]></token><sessionId type="string"><![CDATA[D461EED8-B99B-1B4C-ABAB-16F2830502D7]]></sessionId>'''
    
    print("📤 Sending original payload:")
    print(original_payload)
    
    url = f"http://{host}:{port}/SearchSnapVehicleByKey"
    
    try:
        response = requests.post(url, headers=headers, data=original_payload.encode('utf-8'))
        print(f"\n📥 Response Status Code: {response.status_code}")
        print(f"📥 Response Headers:")
        for key, value in response.headers.items():
            print(f"  {key}: {value}")
        
        print(f"\n📥 Response Content:")
        print(response.content.decode('utf-8', errors='replace'))
        
        # Parse response
        if response.status_code == 200:
            try:
                root = ET.fromstring(response.content)
                ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
                
                # Check for errors
                status = root.get('status')
                if status == 'failed':
                    error_code = root.get('errorCode', 'unknown')
                    print(f"\n❌ Request failed with error code: {error_code}")
                else:
                    print(f"\n✅ Request successful!")
                    
                    # Parse vehicle details
                    snap_vehicle = root.find('.//ipc:snapVehicle', ns) if ns else root.find('.//snapVehicle')
                    if snap_vehicle is not None:
                        snap_info = snap_vehicle.find('.//ipc:snapInfo', ns) if ns else snap_vehicle.find('.//snapInfo')
                        if snap_info is not None:
                            print("\n📋 Vehicle Details:")
                            fields = ['time', 'vehiclePlate', 'listType', 'color', 'pictureData']
                            for field in fields:
                                elem = snap_info.find(f'.//ipc:{field}', ns) if ns else snap_info.find(f'.//{field}')
                                if elem is not None:
                                    if field == 'pictureData':
                                        print(f"  {field}: Available (base64 encoded, {len(elem.text)} characters)")
                                    else:
                                        print(f"  {field}: {elem.text}")
                        else:
                            print("❌ No snap info found in response")
                    else:
                        print("❌ No vehicle data found in response")
                        
            except ET.ParseError as e:
                print(f"❌ Failed to parse XML response: {e}")
        else:
            print(f"❌ HTTP request failed with status code: {response.status_code}")
            
    except Exception as e:
        print(f"❌ Request failed: {e}")
    
    # Test 2: Simplified payload (without token and sessionId)
    print("\n" + "="*50)
    print("🔍 Test 2: Simplified Payload (without token/sessionId)")
    print("="*50)
    
    simplified_payload = '''<?xml version="1.0" encoding="utf-8"?>
<config><search>
<snapTime type="uint64">1750156411341200</snapTime>
<vehicleID type="uint32">1451</vehicleID>
<requestPanoramicPic type="boolean">true</requestPanoramicPic>
</search></config>'''
    
    print("📤 Sending simplified payload:")
    print(simplified_payload)
    
    try:
        response = requests.post(url, headers=headers, data=simplified_payload.encode('utf-8'))
        print(f"\n📥 Response Status Code: {response.status_code}")
        
        if response.status_code == 200:
            print("✅ Simplified request successful!")
            print(f"📥 Response Content:")
            print(response.content.decode('utf-8', errors='replace'))
        else:
            print(f"❌ Simplified request failed with status code: {response.status_code}")
            
    except Exception as e:
        print(f"❌ Simplified request failed: {e}")
    
    # Test 3: Convert timestamp to readable format
    print("\n" + "="*50)
    print("🔍 Test 3: Timestamp Analysis")
    print("="*50)
    
    # Convert the timestamp to readable format
    timestamp = 1750156411341200
    
    # Try different timestamp interpretations
    print(f"Original timestamp: {timestamp}")
    
    # If it's microseconds since epoch
    try:
        dt_micro = datetime.datetime.fromtimestamp(timestamp / 1000000)
        print(f"As microseconds since epoch: {dt_micro}")
    except:
        print("Not microseconds since epoch")
    
    # If it's milliseconds since epoch
    try:
        dt_milli = datetime.datetime.fromtimestamp(timestamp / 1000)
        print(f"As milliseconds since epoch: {dt_milli}")
    except:
        print("Not milliseconds since epoch")
    
    # If it's seconds since epoch
    try:
        dt_sec = datetime.datetime.fromtimestamp(timestamp)
        print(f"As seconds since epoch: {dt_sec}")
    except:
        print("Not seconds since epoch")
    
    # Test 4: Try with current timestamp
    print("\n" + "="*50)
    print("🔍 Test 4: Test with Current Timestamp")
    print("="*50)
    
    # Get current timestamp in microseconds
    current_time = datetime.datetime.now()
    current_timestamp = int(current_time.timestamp() * 1000000)
    
    print(f"Current timestamp (microseconds): {current_timestamp}")
    print(f"Current time: {current_time}")
    
    # Create payload with current timestamp
    current_payload = f'''<?xml version="1.0" encoding="utf-8"?>
<config><search>
<snapTime type="uint64">{current_timestamp}</snapTime>
<vehicleID type="uint32">1451</vehicleID>
<requestPanoramicPic type="boolean">false</requestPanoramicPic>
</search></config>'''
    
    print("📤 Sending payload with current timestamp:")
    print(current_payload)
    
    try:
        response = requests.post(url, headers=headers, data=current_payload.encode('utf-8'))
        print(f"\n📥 Response Status Code: {response.status_code}")
        
        if response.status_code == 200:
            print("✅ Current timestamp request successful!")
            print(f"📥 Response Content:")
            print(response.content.decode('utf-8', errors='replace'))
        else:
            print(f"❌ Current timestamp request failed with status code: {response.status_code}")
            
    except Exception as e:
        print(f"❌ Current timestamp request failed: {e}")
    
    print("\n" + "="*50)
    print("🎉 Specific Vehicle Search Test Completed!")
    print("="*50)

def test_vehicle_recognition_integration():
    """Test using the VehicleRecognition class with the specific parameters"""
    
    print("\n" + "="*50)
    print("🔍 Test 5: Using VehicleRecognition Class")
    print("="*50)
    
    try:
        from vehicle_recognition import VehicleRecognition
        
        # Get camera details
        host = input("Enter camera IP address (e.g., 192.168.60.254): ").strip() or "192.168.60.254"
        port = int(input("Enter port (default 80): ").strip() or "80")
        username = input("Enter username (default admin): ").strip() or "admin"
        password = input("Enter password (default admin): ").strip() or "admin"
        
        # Initialize vehicle recognition system
        vehicle_system = VehicleRecognition(host, port, username, password)
        print("✅ Vehicle Recognition System initialized!")
        
        # Test with the specific vehicle ID and timestamp
        vehicle_id = "1451"
        snap_time = "1750156411341200"
        
        print(f"\n🔍 Testing with Vehicle ID: {vehicle_id}")
        print(f"🔍 Snap Time: {snap_time}")
        
        success, result = vehicle_system.get_vehicle_details(vehicle_id, snap_time, True)
        
        if success:
            print("✅ Vehicle details retrieved successfully!")
            print("\n📋 Vehicle Details:")
            for key, value in result.items():
                if key != 'pictureData':
                    print(f"  {key}: {value}")
                else:
                    print(f"  {key}: Available (base64 encoded, {len(value)} characters)")
        else:
            print(f"❌ Failed to get vehicle details: {result}")
            
    except ImportError:
        print("❌ VehicleRecognition module not available")
    except Exception as e:
        print(f"❌ Test failed: {e}")

if __name__ == "__main__":
    print("🚗 Specific Vehicle Search Test")
    print("Choose an option:")
    print("1. Test with original payload")
    print("2. Test with VehicleRecognition class")
    print("3. Run all tests")
    
    choice = input("\nEnter your choice (1-3): ").strip()
    
    if choice == "1":
        test_specific_vehicle_search()
    elif choice == "2":
        test_vehicle_recognition_integration()
    elif choice == "3":
        test_specific_vehicle_search()
        test_vehicle_recognition_integration()
    else:
        print("Invalid choice. Running all tests...")
        test_specific_vehicle_search()
        test_vehicle_recognition_integration() 