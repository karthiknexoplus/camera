#!/usr/bin/env python3
"""
Quick Test for Vehicle Recognition System
=========================================

A simple script to quickly test the Vehicle Recognition System
with your IP cameras.
"""

import datetime
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
    
    # Test 2: Search Today's Vehicles
    print("\n🔍 Test 2: Searching Today's Vehicles...")
    today = datetime.datetime.now()
    start_time = today.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
    end_time = today.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")
    
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