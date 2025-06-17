#!/usr/bin/env python3
"""
Show All Vehicles Script
========================

A simple script to show all vehicles found in a search.
"""

import datetime
from vehicle_recognition import VehicleRecognition

def show_all_vehicles():
    """Show all vehicles found in today's search"""
    
    print("🚗 Show All Vehicles")
    print("=" * 30)
    
    # Camera settings - modify these for your camera
    HOST = "192.168.60.254"  # Change to your camera IP
    PORT = 80                # Change if needed
    USERNAME = "admin"       # Change if needed
    PASSWORD = "admin"       # Change if needed
    
    print(f"📹 Connecting to: {HOST}:{PORT}")
    
    # Initialize the system
    try:
        vehicle_system = VehicleRecognition(HOST, PORT, USERNAME, PASSWORD)
        print("✅ Connected successfully!")
    except Exception as e:
        print(f"❌ Connection failed: {e}")
        return
    
    # Search today's vehicles
    print("\n🔍 Searching Today's Vehicles...")
    today = datetime.datetime.now()
    start_time = today.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
    end_time = today.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")
    
    success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
    if success:
        data = result
        print(f"✅ Found {data['count']} vehicles today")
        
        if data['count'] > 0:
            print(f"\n📋 All {data['count']} vehicles:")
            print("-" * 50)
            
            for i, vehicle in enumerate(data['vehicles'], 1):
                print(f"{i:2d}. Vehicle ID: {vehicle['vehicleID']}")
                print(f"    Snap Time: {vehicle['snapTime']}")
                
                # Get detailed info for each vehicle
                success, details = vehicle_system.get_vehicle_details(
                    vehicle['vehicleID'], 
                    vehicle['snapTime'], 
                    request_panoramic_pic=False
                )
                
                if success:
                    plate = details.get('vehiclePlate', 'Unknown')
                    list_type = details.get('listType', 'Unknown')
                    color = details.get('color', 'Unknown')
                    time_str = details.get('time', 'Unknown')
                    
                    print(f"    Plate: {plate}")
                    print(f"    Type: {list_type}")
                    print(f"    Color: {color}")
                    print(f"    Time: {time_str}")
                else:
                    print(f"    Details: {details}")
                
                print("-" * 50)
        else:
            print("ℹ️  No vehicles found today")
    else:
        print(f"❌ Search failed: {result}")

if __name__ == "__main__":
    show_all_vehicles() 