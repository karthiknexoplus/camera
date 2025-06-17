#!/usr/bin/env python3
"""
Test Specific Vehicle ID 1451
=============================

Test script for the specific vehicle ID 1451 with the timestamp
from the user's payload.
"""

import datetime
from vehicle_recognition import VehicleRecognition

def test_vehicle_1451():
    """Test the specific vehicle ID 1451"""
    
    print("🚗 Testing Specific Vehicle ID 1451")
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
    
    # Test 2: Get specific vehicle details for ID 1451
    print("\n🔍 Test 2: Getting Details for Vehicle ID 1451...")
    
    # From your payload: snapTime = 1750156411341200
    vehicle_id = "1451"
    snap_time = "1750156411341200"
    
    print(f"Vehicle ID: {vehicle_id}")
    print(f"Snap Time: {snap_time}")
    
    # Convert timestamp to readable format
    try:
        # Try microseconds first
        timestamp_sec = int(snap_time) / 1000000
        readable_time = datetime.datetime.fromtimestamp(timestamp_sec)
        print(f"Readable Time: {readable_time}")
    except:
        print("Could not convert timestamp to readable format")
    
    # Get vehicle details
    success, details = vehicle_system.get_vehicle_details(vehicle_id, snap_time, True)
    if success:
        print("✅ Vehicle details retrieved successfully!")
        print("\n📋 Vehicle Details:")
        for key, value in details.items():
            if key == 'pictureData':
                print(f"  {key}: Available (base64 encoded, {len(value)} characters)")
            else:
                print(f"  {key}: {value}")
    else:
        print(f"❌ Failed to get vehicle details: {details}")
    
    # Test 3: Search for vehicles around that time
    print("\n🔍 Test 3: Searching for Vehicles Around That Time...")
    
    try:
        # Convert timestamp to datetime
        timestamp_sec = int(snap_time) / 1000000
        target_time = datetime.datetime.fromtimestamp(timestamp_sec)
        
        # Search 1 hour before and after
        start_time = (target_time - datetime.timedelta(hours=1)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = (target_time + datetime.timedelta(hours=1)).strftime("%Y-%m-%d %H:%M:%S")
        
        print(f"Searching from {start_time} to {end_time}")
        
        success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
        if success:
            data = result
            print(f"✅ Found {data['count']} vehicles in that time range")
            
            if data['count'] > 0:
                print("📋 Vehicles found:")
                for i, vehicle in enumerate(data['vehicles'], 1):
                    print(f"  {i}. ID: {vehicle.get('vehicleID', 'N/A')}")
                    print(f"     Plate: {vehicle.get('vehiclePlate', 'N/A')}")
                    print(f"     Time: {vehicle.get('snapTime', 'N/A')}")
                    print(f"     Color: {vehicle.get('color', 'N/A')}")
                    print(f"     List Type: {vehicle.get('listType', 'N/A')}")
                    print()
            else:
                print("ℹ️  No vehicles found in that time range")
        else:
            print(f"❌ Search failed: {result}")
    except Exception as e:
        print(f"❌ Error searching around time: {e}")
    
    # Test 4: Search for vehicles with similar ID
    print("\n🔍 Test 4: Searching for Vehicles with Similar IDs...")
    
    # Search for vehicles with IDs around 1451
    try:
        today = datetime.datetime.now()
        start_time = today.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
        end_time = today.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")
        
        success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
        if success:
            data = result
            print(f"✅ Found {data['count']} vehicles today")
            
            # Look for vehicles with IDs close to 1451
            target_id = 1451
            similar_vehicles = []
            
            for vehicle in data['vehicles']:
                vehicle_id_val = vehicle.get('vehicleID')
                if vehicle_id_val:
                    try:
                        vid = int(vehicle_id_val)
                        if abs(vid - target_id) <= 50:  # Within 50 IDs
                            similar_vehicles.append(vehicle)
                    except:
                        pass
            
            if similar_vehicles:
                print(f"📋 Found {len(similar_vehicles)} vehicles with IDs close to {target_id}:")
                for i, vehicle in enumerate(similar_vehicles, 1):
                    print(f"  {i}. ID: {vehicle.get('vehicleID', 'N/A')}")
                    print(f"     Plate: {vehicle.get('vehiclePlate', 'N/A')}")
                    print(f"     Time: {vehicle.get('snapTime', 'N/A')}")
                    print(f"     Color: {vehicle.get('color', 'N/A')}")
                    print(f"     List Type: {vehicle.get('listType', 'N/A')}")
                    print()
            else:
                print(f"ℹ️  No vehicles found with IDs close to {target_id}")
        else:
            print(f"❌ Search failed: {result}")
    except Exception as e:
        print(f"❌ Error searching for similar IDs: {e}")
    
    print("\n🎉 Test completed!")
    print("\n💡 Next steps:")
    print("  • Run 'python direct_test.py' to test the exact payload")
    print("  • Run 'python test_specific_vehicle_search.py' for more detailed testing")
    print("  • Check if vehicle ID 1451 exists in your camera's database")

if __name__ == "__main__":
    test_vehicle_1451() 