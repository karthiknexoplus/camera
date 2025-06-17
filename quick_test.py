#!/usr/bin/env python3
"""
Quick Test for Vehicle Recognition System
=========================================

A simple script to quickly test the Vehicle Recognition System
with your IP cameras.
"""

import datetime
import requests
import base64
import xml.etree.ElementTree as ET
from vehicle_recognition import VehicleRecognition

def quick_test():
    """Quick test of the Vehicle Recognition System"""
    
    print("🚗 Quick Vehicle Recognition Test")
    print("=" * 40)
    
    # Camera settings - modify these for your camera
    HOST = "192.168.60.254"  # Change to your camera IP
    PORT = 80                # Change if needed
    USERNAME = "admin"       # Change if needed
    PASSWORD = "admin"       # Change if needed
    
    print(f"📹 Testing with camera: {HOST}:{PORT}")
    print(f"👤 Username: {USERNAME}")
    
    # Initialize the system
    try:
        vehicle_system = VehicleRecognition(HOST, PORT, USERNAME, PASSWORD)
        print("✅ Connected successfully!")
    except Exception as e:
        print(f"❌ Connection failed: {e}")
        return
    
    # Test 1: Check SD Card
    print("\n📋 Test 1: Checking SD Card Status...")
    success, result = vehicle_system.check_sd_status()
    if success:
        print(f"✅ SD Card: {result}")
    else:
        print(f"❌ SD Card: {result}")
    
    # Test 2: Search Today's Vehicles with RAW XML output
    print("\n🔍 Test 2: Searching Today's Vehicles...")
    today = datetime.datetime.now()
    start_time = today.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
    end_time = today.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")
    
    print(f"Searching from {start_time} to {end_time}")
    
    # Make direct request to see raw XML response
    print("\n📤 Making direct request to see raw XML response...")
    
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
    
    # Build XML request
    xml_body = f'''<?xml version="1.0" encoding="utf-8" ?><config><search>  <starttime type="string"><![CDATA[{start_time}]]></starttime>  <endtime type="string"><![CDATA[{end_time}]]></endtime></search></config><token type="string"><![CDATA[259A6EEC-E40D-814C-BD24-82316F24A34C]]></token><sessionId type="string"><![CDATA[07725EA6-6C94-AC46-9F1F-49B2F186305B]]></sessionId>'''
    
    url = f"http://{HOST}:{PORT}/SearchSnapVehicleByTime"
    
    try:
        print(f"📤 Request URL: {url}")
        print("📤 Request Headers:")
        for key, value in headers.items():
            if key != 'Authorization':
                print(f"  {key}: {value}")
            else:
                print(f"  {key}: Basic [HIDDEN]")
        
        print("📤 Request Body:")
        print(xml_body)
        
        response = requests.post(url, headers=headers, data=xml_body.encode('utf-8'), timeout=30)
        
        print(f"\n📥 Response Status Code: {response.status_code}")
        print("📥 Response Headers:")
        for key, value in response.headers.items():
            print(f"  {key}: {value}")
        
        print("\n📥 RAW XML Response:")
        print("=" * 50)
        response_text = response.content.decode('utf-8', errors='replace')
        print(response_text)
        print("=" * 50)
        
        # Now parse with our vehicle recognition system
        print("\n🔍 Parsing with Vehicle Recognition System...")
        success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
        if success:
            data = result
            print(f"✅ Found {data['count']} vehicles today")
            
            if data['count'] > 0:
                print("📋 Sample vehicles:")
                for i, vehicle in enumerate(data['vehicles'][:3], 1):
                    print(f"  {i}. ID: {vehicle.get('vehicleID', 'N/A')}")
                    print(f"     Plate: {vehicle.get('vehiclePlate', 'N/A')}")
                    print(f"     Time: {vehicle.get('snapTime', 'N/A')}")
                    print(f"     Color: {vehicle.get('color', 'N/A')}")
                    print(f"     List Type: {vehicle.get('listType', 'N/A')}")
                    print()
            else:
                print("ℹ️  No vehicles found today")
        else:
            print(f"❌ Search failed: {result}")
            
    except Exception as e:
        print(f"❌ Direct request failed: {e}")
    
    # Test 2.5: Test specific vehicle ID 298
    print("\n🔍 Test 2.5: Testing Specific Vehicle ID 298...")
    
    # Use the exact payload provided by the user
    specific_xml_body = '''<?xml version="1.0" encoding="utf-8" ?><config><search>  <snapTime type="uint64">1750156169843370</snapTime>  <vehicleID type="uint32">298</vehicleID>  <requestPanoramicPic type="boolean">true</requestPanoramicPic></search></config><token type="string"><![CDATA[EA082FB0-1B2F-7B46-ACD0-6477CACBE2C2]]></token><sessionId type="string"><![CDATA[BC67AF63-848E-1243-9396-E36A85F93F78]]></sessionId>'''
    
    specific_url = f"http://{HOST}:{PORT}/SearchSnapVehicleByKey"
    
    try:
        print(f"📤 Request URL: {specific_url}")
        print("📤 Request Body:")
        print(specific_xml_body)
        
        specific_response = requests.post(specific_url, headers=headers, data=specific_xml_body.encode('utf-8'), timeout=30)
        
        print(f"\n📥 Response Status Code: {specific_response.status_code}")
        print("📥 Response Headers:")
        for key, value in specific_response.headers.items():
            print(f"  {key}: {value}")
        
        print("\n📥 RAW XML Response for Vehicle ID 298:")
        print("=" * 50)
        specific_response_text = specific_response.content.decode('utf-8', errors='replace')
        print(specific_response_text)
        print("=" * 50)
        
        # Try to parse the response
        if specific_response.status_code == 200:
            try:
                root = ET.fromstring(specific_response.content)
                ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
                
                # Check for errors
                status = root.get('status')
                if status == 'failed':
                    error_code = root.get('errorCode', 'unknown')
                    print(f"\n❌ Request failed with error code: {error_code}")
                else:
                    print(f"\n✅ Request successful!")
                    
                    # Try to parse vehicle details
                    snap_vehicle = root.find('.//ipc:snapVehicle', ns) if ns else root.find('.//snapVehicle')
                    if snap_vehicle is not None:
                        snap_info = snap_vehicle.find('.//ipc:snapInfo', ns) if ns else snap_vehicle.find('.//snapInfo')
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
            print(f"❌ HTTP request failed with status code: {specific_response.status_code}")
            
    except Exception as e:
        print(f"❌ Specific vehicle request failed: {e}")
    
    # Test 3: Search Yesterday
    print("\n🔍 Test 3: Searching Yesterday's Vehicles...")
    yesterday = today - datetime.timedelta(days=1)
    start_time = yesterday.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
    end_time = yesterday.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")
    
    success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
    if success:
        data = result
        print(f"✅ Found {data['count']} vehicles yesterday")
        
        if data['count'] > 0:
            print("📋 Sample vehicles:")
            for i, vehicle in enumerate(data['vehicles'][:3], 1):
                print(f"  {i}. ID: {vehicle.get('vehicleID', 'N/A')}")
                print(f"     Plate: {vehicle.get('vehiclePlate', 'N/A')}")
                print(f"     Time: {vehicle.get('snapTime', 'N/A')}")
                print(f"     Color: {vehicle.get('color', 'N/A')}")
                print(f"     List Type: {vehicle.get('listType', 'N/A')}")
                print()
    else:
        print(f"❌ Search failed: {result}")
    
    # Test 4: Get Statistics
    print("\n📊 Test 4: Getting Statistics...")
    success, result = vehicle_system.get_vehicle_statistics(start_time, end_time)
    if success:
        stats = result
        print(f"✅ Statistics:")
        print(f"  Total vehicles: {stats['total_vehicles']}")
        
        if stats['list_types']:
            print("  List types:")
            for list_type, count in stats['list_types'].items():
                print(f"    {list_type}: {count}")
    else:
        print(f"❌ Statistics failed: {result}")
    
    # Test 5: Get specific vehicle details (if we found any)
    print("\n🔍 Test 5: Getting Specific Vehicle Details...")
    if success and data['count'] > 0:
        first_vehicle = data['vehicles'][0]
        vehicle_id = first_vehicle.get('vehicleID')
        snap_time = first_vehicle.get('snapTime')
        
        if vehicle_id and snap_time:
            print(f"Getting details for Vehicle ID: {vehicle_id}")
            success, details = vehicle_system.get_vehicle_details(vehicle_id, snap_time, False)
            if success:
                print("✅ Vehicle details retrieved:")
                for key, value in details.items():
                    if key != 'pictureData':
                        print(f"  {key}: {value}")
                    else:
                        print(f"  {key}: Available (base64 encoded, {len(value)} characters)")
            else:
                print(f"❌ Failed to get vehicle details: {details}")
        else:
            print("ℹ️  No vehicle ID or snap time available for detailed lookup")
    else:
        print("ℹ️  No vehicles found to get details for")
    
    print("\n🎉 Quick test completed!")
    print("\n💡 Next steps:")
    print("  • Run 'python get_device_info.py' and select option 29")
    print("  • Run 'python test_vehicle_recognition.py' for detailed testing")
    print("  • Run 'python direct_test.py' to test specific vehicle search")
    print("  • Modify the camera settings in this script if needed")

if __name__ == "__main__":
    quick_test() 