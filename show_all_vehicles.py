#!/usr/bin/env python3
"""
Show All Vehicles Script
========================

A simple script to show all vehicles found in the last one hour, sorted by time.
"""

import datetime
from vehicle_recognition import VehicleRecognition

def show_all_vehicles():
    """Show all vehicles found in the last one hour, sorted by time"""
    
    print("🚗 Show All Vehicles (Last One Hour - Sorted by Time)")
    print("=" * 55)
    
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
    
    # Search vehicles from the last one hour
    print("\n🔍 Searching Vehicles from Last One Hour...")
    now = datetime.datetime.now()
    one_hour_ago = now - datetime.timedelta(hours=1)
    
    start_time = one_hour_ago.strftime("%Y-%m-%d %H:%M:%S")
    end_time = now.strftime("%Y-%m-%d %H:%M:%S")
    
    print(f"⏰ Time Range: {start_time} to {end_time}")
    
    success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
    if success:
        data = result
        print(f"✅ Found {data['count']} vehicles in the last hour")
        
        if data['count'] > 0:
            # Sort vehicles by snap time (most recent first)
            sorted_vehicles = sorted(data['vehicles'], 
                                   key=lambda x: int(x['snapTime']), 
                                   reverse=True)
            
            print(f"\n📋 All {data['count']} vehicles (sorted by time, most recent first):")
            print("-" * 60)
            
            for i, vehicle in enumerate(sorted_vehicles, 1):
                snap_time = int(vehicle['snapTime'])
                
                # Convert timestamp to readable time
                timestamp_seconds = snap_time / 1000000
                readable_time = datetime.datetime.fromtimestamp(timestamp_seconds)
                
                # Calculate how long ago this vehicle was detected
                time_diff = now - readable_time
                minutes_ago = int(time_diff.total_seconds() / 60)
                
                print(f"{i:2d}. Vehicle ID: {vehicle['vehicleID']}")
                print(f"    Snap Time: {readable_time.strftime('%Y-%m-%d %H:%M:%S')}")
                print(f"    Time Ago: {minutes_ago} minutes ago")
                print(f"    Raw Timestamp: {snap_time}")
                
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
                    print(f"    API Time: {time_str}")
                else:
                    print(f"    Details: {details}")
                
                print("-" * 60)
            
            # Show which is most recent
            most_recent = sorted_vehicles[0]
            oldest = sorted_vehicles[-1]
            
            print(f"\n🎯 Most Recent Vehicle:")
            print(f"   Vehicle ID: {most_recent['vehicleID']}")
            most_recent_time = datetime.datetime.fromtimestamp(int(most_recent['snapTime']) / 1000000)
            print(f"   Time: {most_recent_time.strftime('%Y-%m-%d %H:%M:%S')}")
            
            # Calculate time difference for most recent
            most_recent_diff = now - most_recent_time
            most_recent_minutes = int(most_recent_diff.total_seconds() / 60)
            print(f"   Detected: {most_recent_minutes} minutes ago")
            
            print(f"\n📊 Summary:")
            print(f"   Most Recent: Vehicle ID {most_recent['vehicleID']} ({most_recent_minutes} min ago)")
            print(f"   Oldest: Vehicle ID {oldest['vehicleID']}")
            print(f"   Total: {len(sorted_vehicles)} vehicles in last hour")
            
        else:
            print("ℹ️  No vehicles found in the last hour")
    else:
        print(f"❌ Search failed: {result}")

if __name__ == "__main__":
    show_all_vehicles() 