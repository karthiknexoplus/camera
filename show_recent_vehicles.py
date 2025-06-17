#!/usr/bin/env python3
"""
Show Recent Vehicles Script
===========================

A script to show vehicles sorted by time, with the most recent first.
"""

import datetime
from vehicle_recognition import VehicleRecognition

def show_recent_vehicles():
    """Show vehicles sorted by time, most recent first"""
    
    print("🚗 Show Recent Vehicles (Sorted by Time)")
    print("=" * 45)
    
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
    
    # Ask user for time range
    print("\n⏰ Choose time range:")
    print("1. Last 1 hour")
    print("2. Last 6 hours")
    print("3. Today (full day)")
    print("4. Custom time range")
    
    choice = input("\nEnter your choice (1-4): ").strip()
    
    now = datetime.datetime.now()
    
    if choice == "1":
        # Last 1 hour
        start_time = (now - datetime.timedelta(hours=1)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = now.strftime("%Y-%m-%d %H:%M:%S")
        time_description = "last hour"
    elif choice == "2":
        # Last 6 hours
        start_time = (now - datetime.timedelta(hours=6)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = now.strftime("%Y-%m-%d %H:%M:%S")
        time_description = "last 6 hours"
    elif choice == "3":
        # Today
        start_time = now.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
        end_time = now.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")
        time_description = "today"
    elif choice == "4":
        # Custom time range
        print("\nEnter custom time range:")
        start_time = input("Start time (YYYY-MM-DD HH:MM:SS): ").strip()
        end_time = input("End time (YYYY-MM-DD HH:MM:SS): ").strip()
        time_description = "custom range"
    else:
        print("Invalid choice. Using last hour.")
        start_time = (now - datetime.timedelta(hours=1)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = now.strftime("%Y-%m-%d %H:%M:%S")
        time_description = "last hour"
    
    # Search vehicles
    print(f"\n🔍 Searching Vehicles from {time_description}...")
    print(f"⏰ Time Range: {start_time} to {end_time}")
    
    success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
    if success:
        data = result
        print(f"✅ Found {data['count']} vehicles in {time_description}")
        
        if data['count'] > 0:
            # Sort vehicles by snap time (most recent first)
            sorted_vehicles = sorted(data['vehicles'], 
                                   key=lambda x: int(x['snapTime']), 
                                   reverse=True)
            
            print(f"\n📋 Vehicles sorted by time (most recent first):")
            print("-" * 60)
            
            for i, vehicle in enumerate(sorted_vehicles, 1):
                snap_time = int(vehicle['snapTime'])
                
                # Convert timestamp to readable time
                # The timestamp appears to be in microseconds, so divide by 1000000
                timestamp_seconds = snap_time / 1000000
                readable_time = datetime.datetime.fromtimestamp(timestamp_seconds)
                
                # Calculate how long ago this vehicle was detected
                time_diff = now - readable_time
                minutes_ago = int(time_diff.total_seconds() / 60)
                
                print(f"{i:2d}. Vehicle ID: {vehicle['vehicleID']}")
                print(f"    Raw Snap Time: {snap_time}")
                print(f"    Readable Time: {readable_time.strftime('%Y-%m-%d %H:%M:%S')}")
                print(f"    Time Ago: {minutes_ago} minutes ago")
                
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
            
            # Show summary
            print(f"\n📊 Summary:")
            print(f"  Most Recent: Vehicle ID {sorted_vehicles[0]['vehicleID']}")
            print(f"  Oldest: Vehicle ID {sorted_vehicles[-1]['vehicleID']}")
            print(f"  Total: {len(sorted_vehicles)} vehicles in {time_description}")
            
        else:
            print(f"ℹ️  No vehicles found in {time_description}")
    else:
        print(f"❌ Search failed: {result}")

def analyze_timestamps():
    """Analyze the timestamp format from your data"""
    
    print("\n🔍 Timestamp Analysis")
    print("=" * 25)
    
    # Your example timestamps
    timestamps = [
        1750159401755621,  # Vehicle ID: 679
        1750157447348973,  # Vehicle ID: 450  
        1750156169843370   # Vehicle ID: 298
    ]
    
    print("Your vehicle timestamps:")
    for i, ts in enumerate(timestamps, 1):
        # Try different conversions
        seconds_1 = ts / 1000000  # Microseconds to seconds
        seconds_2 = ts / 1000     # Milliseconds to seconds
        
        time_1 = datetime.datetime.fromtimestamp(seconds_1)
        time_2 = datetime.datetime.fromtimestamp(seconds_2)
        
        print(f"\nVehicle {i}:")
        print(f"  Raw timestamp: {ts}")
        print(f"  As microseconds: {time_1}")
        print(f"  As milliseconds: {time_2}")
        print(f"  Difference: {time_1 - time_2}")

if __name__ == "__main__":
    print("Choose an option:")
    print("1. Show vehicles sorted by time")
    print("2. Analyze timestamp format")
    
    choice = input("\nEnter your choice (1-2): ").strip()
    
    if choice == "1":
        show_recent_vehicles()
    elif choice == "2":
        analyze_timestamps()
    else:
        print("Running both...")
        show_recent_vehicles()
        analyze_timestamps() 