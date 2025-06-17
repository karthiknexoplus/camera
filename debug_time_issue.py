#!/usr/bin/env python3
"""
Debug Time Issue Script
=======================

This script helps debug time synchronization issues between
the camera and your system.
"""

import datetime
import requests
import base64
import xml.etree.ElementTree as ET
from vehicle_recognition import VehicleRecognition

def select_camera():
    """Let user select which camera to use"""
    
    print("📹 Camera Selection")
    print("=" * 20)
    print("Available cameras:")
    print("1. Camera 1 - 192.168.60.254")
    print("2. Camera 2 - 192.168.60.253")
    print("3. Custom IP address")
    
    while True:
        choice = input("\nSelect camera (1-3): ").strip()
        
        if choice == "1":
            HOST = "192.168.60.254"
            break
        elif choice == "2":
            HOST = "192.168.60.253"
            break
        elif choice == "3":
            HOST = input("Enter custom IP address: ").strip()
            if HOST:
                break
            else:
                print("Invalid IP address. Please try again.")
        else:
            print("Invalid choice. Please enter 1, 2, or 3.")
    
    # Get other configuration
    PORT = input("Enter port (default 80): ").strip() or "80"
    USERNAME = input("Enter username (default admin): ").strip() or "admin"
    PASSWORD = input("Enter password (default admin): ").strip() or "admin"
    
    print(f"\n📹 Selected camera: {HOST}:{PORT}")
    print(f"👤 Username: {USERNAME}")
    print("=" * 50)
    
    return HOST, int(PORT), USERNAME, PASSWORD

def check_camera_time(HOST, PORT, USERNAME, PASSWORD):
    """Check the camera's current time"""
    
    print("🕐 Camera Time Debug")
    print("=" * 30)
    
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
        print(f"📹 Connecting to camera: {HOST}:{PORT}")
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
                camera_time = current_time.text.strip()
                print(f"📅 Camera Time: {camera_time}")
                
                # Convert to datetime object
                try:
                    camera_dt = datetime.datetime.strptime(camera_time, "%Y-%m-%d %H:%M:%S")
                    system_dt = datetime.datetime.now()
                    
                    print(f"🖥️  System Time: {system_dt.strftime('%Y-%m-%d %H:%M:%S')}")
                    
                    # Calculate time difference
                    time_diff = abs((camera_dt - system_dt).total_seconds())
                    print(f"⏱️  Time Difference: {time_diff:.0f} seconds")
                    
                    if time_diff > 60:
                        print(f"⚠️  WARNING: Time difference is {time_diff/60:.1f} minutes!")
                        print("   This could cause vehicle search issues.")
                    else:
                        print("✅ Time is synchronized (within 1 minute)")
                        
                except ValueError as e:
                    print(f"❌ Error parsing camera time: {e}")
                    print(f"   Raw camera time: {camera_time}")
            else:
                print("❌ Could not find current time in camera response")
        else:
            print("❌ Could not find time synchronization info")
            
    except Exception as e:
        print(f"❌ Error getting camera time: {e}")

def search_with_different_times(HOST, PORT, USERNAME, PASSWORD):
    """Search vehicles with different time ranges to find the right one"""
    
    print("\n🔍 Testing Different Time Ranges")
    print("=" * 40)
    
    try:
        vehicle_system = VehicleRecognition(HOST, PORT, USERNAME, PASSWORD)
        print("✅ Connected to vehicle recognition system")
    except Exception as e:
        print(f"❌ Connection failed: {e}")
        return
    
    # Test different time ranges
    now = datetime.datetime.now()
    
    time_ranges = [
        ("Last 1 hour", now - datetime.timedelta(hours=1), now),
        ("Last 6 hours", now - datetime.timedelta(hours=6), now),
        ("Last 24 hours", now - datetime.timedelta(hours=24), now),
        ("Today (00:00 to now)", now.replace(hour=0, minute=0, second=0), now),
        ("Yesterday", now - datetime.timedelta(days=1), now - datetime.timedelta(days=1) + datetime.timedelta(hours=23, minutes=59, seconds=59)),
        ("Last 7 days", now - datetime.timedelta(days=7), now),
    ]
    
    for description, start_dt, end_dt in time_ranges:
        start_time = start_dt.strftime("%Y-%m-%d %H:%M:%S")
        end_time = end_dt.strftime("%Y-%m-%d %H:%M:%S")
        
        print(f"\n🔍 Testing: {description}")
        print(f"   Range: {start_time} to {end_time}")
        
        success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
        if success:
            data = result
            print(f"   ✅ Found {data['count']} vehicles")
            
            if data['count'] > 0:
                print(f"   🎯 SUCCESS! Found vehicles in {description}")
                print(f"   📋 First few vehicles:")
                for i, vehicle in enumerate(data['vehicles'][:3], 1):
                    snap_time = int(vehicle['snapTime'])
                    timestamp_seconds = snap_time / 1000000
                    readable_time = datetime.datetime.fromtimestamp(timestamp_seconds)
                    print(f"      {i}. ID: {vehicle['vehicleID']}, Time: {readable_time.strftime('%Y-%m-%d %H:%M:%S')}")
                break
        else:
            print(f"   ❌ Search failed: {result}")

def search_all_vehicles(HOST, PORT, USERNAME, PASSWORD):
    """Search for ALL vehicles without time filter"""
    
    print("\n🔍 Searching ALL Vehicles (No Time Filter)")
    print("=" * 45)
    
    try:
        vehicle_system = VehicleRecognition(HOST, PORT, USERNAME, PASSWORD)
        print("✅ Connected to vehicle recognition system")
    except Exception as e:
        print(f"❌ Connection failed: {e}")
        return
    
    # Use a very wide time range (last 30 days)
    now = datetime.datetime.now()
    start_time = (now - datetime.timedelta(days=30)).strftime("%Y-%m-%d %H:%M:%S")
    end_time = now.strftime("%Y-%m-%d %H:%M:%S")
    
    print(f"⏰ Searching from: {start_time} to {end_time}")
    
    success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
    if success:
        data = result
        print(f"✅ Found {data['count']} vehicles in last 30 days")
        
        if data['count'] > 0:
            print(f"\n📋 Recent vehicles:")
            # Sort by time and show most recent
            sorted_vehicles = sorted(data['vehicles'], 
                                   key=lambda x: int(x['snapTime']), 
                                   reverse=True)
            
            for i, vehicle in enumerate(sorted_vehicles[:5], 1):
                snap_time = int(vehicle['snapTime'])
                timestamp_seconds = snap_time / 1000000
                readable_time = datetime.datetime.fromtimestamp(timestamp_seconds)
                time_diff = now - readable_time
                days_ago = time_diff.days
                hours_ago = int(time_diff.total_seconds() / 3600) % 24
                
                print(f"  {i}. ID: {vehicle['vehicleID']}")
                print(f"     Time: {readable_time.strftime('%Y-%m-%d %H:%M:%S')}")
                print(f"     Ago: {days_ago} days, {hours_ago} hours ago")
        else:
            print("ℹ️  No vehicles found in last 30 days")
    else:
        print(f"❌ Search failed: {result}")

def main():
    print("🔧 Vehicle Time Debug Tool")
    print("=" * 30)
    
    # Let user select camera first
    HOST, PORT, USERNAME, PASSWORD = select_camera()
    
    print("\nChoose an option:")
    print("1. Check camera time vs system time")
    print("2. Test different time ranges")
    print("3. Search all vehicles (no time filter)")
    print("4. Run all tests")
    
    choice = input("\nEnter your choice (1-4): ").strip()
    
    if choice == "1":
        check_camera_time(HOST, PORT, USERNAME, PASSWORD)
    elif choice == "2":
        search_with_different_times(HOST, PORT, USERNAME, PASSWORD)
    elif choice == "3":
        search_all_vehicles(HOST, PORT, USERNAME, PASSWORD)
    elif choice == "4":
        check_camera_time(HOST, PORT, USERNAME, PASSWORD)
        search_with_different_times(HOST, PORT, USERNAME, PASSWORD)
        search_all_vehicles(HOST, PORT, USERNAME, PASSWORD)
    else:
        print("Running all tests...")
        check_camera_time(HOST, PORT, USERNAME, PASSWORD)
        search_with_different_times(HOST, PORT, USERNAME, PASSWORD)
        search_all_vehicles(HOST, PORT, USERNAME, PASSWORD)

if __name__ == "__main__":
    main() 