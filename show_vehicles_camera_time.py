#!/usr/bin/env python3
"""
Show Vehicles Using Camera Time
===============================

This script uses the camera's time instead of system time
to avoid time synchronization issues.
"""

import datetime
import requests
import base64
import xml.etree.ElementTree as ET
from vehicle_recognition import VehicleRecognition

def get_camera_time():
    """Get the current time from the camera"""
    
    # Camera settings
    HOST = "192.168.60.254"
    PORT = 80
    USERNAME = "admin"
    PASSWORD = "admin"
    
    # Build URL for getting camera time
    url_time = f"http://{HOST}:{PORT}/GetDateAndTime"
    
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
    
    try:
        response = requests.post(url_time, headers=headers)
        response.raise_for_status()
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Get current time from camera
        sync_info = root.find('.//ipc:synchronizeInfo', ns) if ns else root.find('.//synchronizeInfo')
        if sync_info is not None:
            current_time = sync_info.find('.//ipc:currentTime', ns) if ns else sync_info.find('.//currentTime')
            if current_time is not None:
                camera_time_str = current_time.text.strip()
                camera_dt = datetime.datetime.strptime(camera_time_str, "%Y-%m-%d %H:%M:%S")
                return camera_dt
                
    except Exception as e:
        print(f"❌ Error getting camera time: {e}")
    
    # Fallback to system time if camera time fails
    return datetime.datetime.now()

def show_vehicles_using_camera_time():
    """Show vehicles using camera time instead of system time"""
    
    print("🚗 Show Vehicles (Using Camera Time)")
    print("=" * 40)
    
    # Camera settings
    HOST = "192.168.60.254"
    PORT = 80
    USERNAME = "admin"
    PASSWORD = "admin"
    
    print(f"📹 Connecting to: {HOST}:{PORT}")
    
    # Get camera time
    print("\n🕐 Getting camera time...")
    camera_now = get_camera_time()
    print(f"📅 Camera Time: {camera_now.strftime('%Y-%m-%d %H:%M:%S')}")
    
    # Initialize the system
    try:
        vehicle_system = VehicleRecognition(HOST, PORT, USERNAME, PASSWORD)
        print("✅ Connected successfully!")
    except Exception as e:
        print(f"❌ Connection failed: {e}")
        return
    
    # Ask user for time range
    print("\n⏰ Choose time range (using camera time):")
    print("1. Last 1 hour")
    print("2. Last 6 hours")
    print("3. Last 24 hours")
    print("4. Today (00:00 to now)")
    print("5. Yesterday")
    
    choice = input("\nEnter your choice (1-5): ").strip()
    
    if choice == "1":
        # Last 1 hour
        start_time = (camera_now - datetime.timedelta(hours=1)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = camera_now.strftime("%Y-%m-%d %H:%M:%S")
        time_description = "last hour"
    elif choice == "2":
        # Last 6 hours
        start_time = (camera_now - datetime.timedelta(hours=6)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = camera_now.strftime("%Y-%m-%d %H:%M:%S")
        time_description = "last 6 hours"
    elif choice == "3":
        # Last 24 hours
        start_time = (camera_now - datetime.timedelta(hours=24)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = camera_now.strftime("%Y-%m-%d %H:%M:%S")
        time_description = "last 24 hours"
    elif choice == "4":
        # Today
        start_time = camera_now.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
        end_time = camera_now.strftime("%Y-%m-%d %H:%M:%S")
        time_description = "today"
    elif choice == "5":
        # Yesterday
        yesterday = camera_now - datetime.timedelta(days=1)
        start_time = yesterday.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
        end_time = yesterday.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")
        time_description = "yesterday"
    else:
        print("Invalid choice. Using last hour.")
        start_time = (camera_now - datetime.timedelta(hours=1)).strftime("%Y-%m-%d %H:%M:%S")
        end_time = camera_now.strftime("%Y-%m-%d %H:%M:%S")
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
                timestamp_seconds = snap_time / 1000000
                readable_time = datetime.datetime.fromtimestamp(timestamp_seconds)
                
                # Calculate how long ago this vehicle was detected (using camera time)
                time_diff = camera_now - readable_time
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
            
            # Show summary
            print(f"\n📊 Summary:")
            print(f"  Most Recent: Vehicle ID {sorted_vehicles[0]['vehicleID']}")
            print(f"  Oldest: Vehicle ID {sorted_vehicles[-1]['vehicleID']}")
            print(f"  Total: {len(sorted_vehicles)} vehicles in {time_description}")
            
        else:
            print(f"ℹ️  No vehicles found in {time_description}")
            print("\n💡 Try a wider time range or check if vehicles were detected recently.")
    else:
        print(f"❌ Search failed: {result}")

if __name__ == "__main__":
    show_vehicles_using_camera_time() 