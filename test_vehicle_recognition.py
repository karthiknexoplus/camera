#!/usr/bin/env python3
"""
Vehicle Recognition System Test Examples
========================================

This script provides practical examples to test the Vehicle Recognition System
with your IP cameras.
"""

import sys
import os
import datetime
from vehicle_recognition import VehicleRecognition

def test_vehicle_recognition():
    """Test the Vehicle Recognition System with practical examples"""
    
    print("🚗 Vehicle Recognition System Test Examples")
    print("=" * 50)
    
    # Get camera connection details
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
    
    # Initialize vehicle recognition system
    try:
        vehicle_system = VehicleRecognition(host, int(port), username, password)
        print("✅ Vehicle Recognition System initialized successfully!")
    except Exception as e:
        print(f"❌ Failed to initialize: {e}")
        return
    
    # Example 1: Check SD Card Status
    print("\n" + "="*50)
    print("📋 Example 1: Check SD Card Status")
    print("="*50)
    
    success, result = vehicle_system.check_sd_status()
    if success:
        print(f"✅ SD Card Status: {result}")
    else:
        print(f"❌ SD Card Error: {result}")
        print("⚠️  Some features may not work without proper SD card")
    
    # Example 2: Search Vehicles by Time Range
    print("\n" + "="*50)
    print("🔍 Example 2: Search Vehicles by Time Range")
    print("="*50)
    
    # Use today's date for testing
    today = datetime.datetime.now()
    start_time = today.replace(hour=0, minute=0, second=0, microsecond=0).strftime("%Y-%m-%d %H:%M:%S")
    end_time = today.replace(hour=23, minute=59, second=59, microsecond=0).strftime("%Y-%m-%d %H:%M:%S")
    
    print(f"🔍 Searching vehicles from {start_time} to {end_time}")
    
    success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
    if success:
        data = result
        print(f"✅ Found {data['count']} vehicles")
        
        if data['count'] > 0:
            print("\n📋 First 5 vehicles found:")
            for i, vehicle in enumerate(data['vehicles'][:5], 1):
                print(f"  {i}. Vehicle ID: {vehicle['vehicleID']}")
                print(f"     Snap Time: {vehicle['snapTime']}")
            
            if data['count'] > 5:
                print(f"  ... and {data['count'] - 5} more vehicles")
            
            # Example 3: Get Details for First Vehicle
            print("\n" + "="*50)
            print("📊 Example 3: Get Vehicle Details")
            print("="*50)
            
            first_vehicle = data['vehicles'][0]
            print(f"🔍 Getting details for Vehicle ID: {first_vehicle['vehicleID']}")
            
            success, details = vehicle_system.get_vehicle_details(
                first_vehicle['vehicleID'], 
                first_vehicle['snapTime'], 
                request_panoramic_pic=False  # Don't request image for faster testing
            )
            
            if success:
                print("✅ Vehicle Details:")
                for key, value in details.items():
                    if key != 'pictureData':  # Skip image data
                        print(f"  {key}: {value}")
            else:
                print(f"❌ Failed to get vehicle details: {details}")
        else:
            print("ℹ️  No vehicles found in the specified time range")
            print("💡 Try searching for a different time range or check if vehicle detection is enabled")
    else:
        print(f"❌ Search failed: {result}")
    
    # Example 4: Search with Filters
    print("\n" + "="*50)
    print("🔍 Example 4: Search with Filters")
    print("="*50)
    
    # Example search with plate number filter
    test_plate = input("Enter a license plate to search for (optional, press Enter to skip): ").strip()
    if test_plate:
        print(f"🔍 Searching for vehicles with plate: {test_plate}")
        success, result = vehicle_system.search_vehicles_by_time(start_time, end_time, test_plate)
        if success:
            data = result
            print(f"✅ Found {data['count']} vehicles with plate {test_plate}")
        else:
            print(f"❌ Search failed: {result}")
    
    # Example 5: Get Statistics
    print("\n" + "="*50)
    print("📊 Example 5: Get Vehicle Statistics")
    print("="*50)
    
    print("📈 Analyzing vehicle detection patterns...")
    success, result = vehicle_system.get_vehicle_statistics(start_time, end_time)
    if success:
        stats = result
        print(f"✅ Statistics for {start_time} to {end_time}:")
        print(f"  📊 Total Vehicles: {stats['total_vehicles']}")
        
        if stats['list_types']:
            print("\n  📋 List Type Distribution:")
            for list_type, count in stats['list_types'].items():
                percentage = (count / stats['total_vehicles']) * 100 if stats['total_vehicles'] > 0 else 0
                print(f"    {list_type}: {count} ({percentage:.1f}%)")
        
        if stats['time_distribution']:
            print("\n  🕐 Time Distribution (by hour):")
            for hour in sorted(stats['time_distribution'].keys()):
                count = stats['time_distribution'][hour]
                percentage = (count / stats['total_vehicles']) * 100 if stats['total_vehicles'] > 0 else 0
                print(f"    {hour:02d}:00 - {hour:02d}:59: {count} vehicles ({percentage:.1f}%)")
    else:
        print(f"❌ Failed to get statistics: {result}")
    
    # Example 6: Export Data (Optional)
    print("\n" + "="*50)
    print("📤 Example 6: Export Vehicle Data")
    print("="*50)
    
    export_choice = input("Do you want to export vehicle data? (y/n): ").strip().lower()
    if export_choice == 'y':
        print("📤 Exporting vehicle data...")
        print("⚠️  This may take some time depending on the number of vehicles")
        
        # Export without images for faster testing
        include_images = input("Include images in export? (y/n, default n): ").strip().lower() == 'y'
        
        success, result = vehicle_system.export_vehicle_data(
            start_time, end_time, 
            include_images=include_images,
            output_file="test_vehicle_export.zip"
        )
        
        if success:
            print(f"✅ Export successful: {result}")
            print(f"📁 File saved as: test_vehicle_export.zip")
        else:
            print(f"❌ Export failed: {result}")
    
    print("\n" + "="*50)
    print("🎉 Vehicle Recognition System Test Completed!")
    print("="*50)
    print("\n💡 Tips for testing:")
    print("  • Make sure vehicle detection is enabled on your camera")
    print("  • Check that vehicles have passed through the detection zone")
    print("  • Verify SD card is properly installed and working")
    print("  • Try different time ranges if no vehicles are found")
    print("  • Use the main tool (option 29) for interactive testing")

def test_specific_scenarios():
    """Test specific scenarios with predefined parameters"""
    
    print("\n🧪 Specific Test Scenarios")
    print("=" * 30)
    
    # Test scenario 1: Yesterday's data
    print("\n📅 Scenario 1: Search Yesterday's Data")
    yesterday = datetime.datetime.now() - datetime.timedelta(days=1)
    start_time = yesterday.replace(hour=0, minute=0, second=0, microsecond=0).strftime("%Y-%m-%d %H:%M:%S")
    end_time = yesterday.replace(hour=23, minute=59, second=59, microsecond=0).strftime("%Y-%m-%d %H:%M:%S")
    
    print(f"Searching: {start_time} to {end_time}")
    
    # Test scenario 2: Last 7 days
    print("\n📅 Scenario 2: Search Last 7 Days")
    week_ago = datetime.datetime.now() - datetime.timedelta(days=7)
    start_time = week_ago.strftime("%Y-%m-%d %H:%M:%S")
    end_time = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    
    print(f"Searching: {start_time} to {end_time}")
    
    # Test scenario 3: Specific time range
    print("\n📅 Scenario 3: Morning Hours (6 AM - 12 PM)")
    today = datetime.datetime.now()
    start_time = today.replace(hour=6, minute=0, second=0, microsecond=0).strftime("%Y-%m-%d %H:%M:%S")
    end_time = today.replace(hour=12, minute=0, second=0, microsecond=0).strftime("%Y-%m-%d %H:%M:%S")
    
    print(f"Searching: {start_time} to {end_time}")

def show_usage_examples():
    """Show usage examples for different scenarios"""
    
    print("\n📖 Usage Examples")
    print("=" * 20)
    
    examples = [
        {
            "title": "Search Today's Vehicles",
            "description": "Find all vehicles detected today",
            "code": """
# Search today's vehicles
today = datetime.datetime.now()
start_time = today.replace(hour=0, minute=0, second=0).strftime("%Y-%m-%d %H:%M:%S")
end_time = today.replace(hour=23, minute=59, second=59).strftime("%Y-%m-%d %H:%M:%S")

success, result = vehicle_system.search_vehicles_by_time(start_time, end_time)
"""
        },
        {
            "title": "Search Specific License Plate",
            "description": "Find vehicles with a specific license plate",
            "code": """
# Search for specific license plate
plate_number = "ABC123"
success, result = vehicle_system.search_vehicles_by_time(
    start_time, end_time, 
    vehicle_plate=plate_number
)
"""
        },
        {
            "title": "Export with Images",
            "description": "Export vehicle data including captured images",
            "code": """
# Export with images
success, result = vehicle_system.export_vehicle_data(
    start_time, end_time,
    include_images=True,
    output_file="vehicle_export.zip"
)
"""
        },
        {
            "title": "Get Hourly Statistics",
            "description": "Analyze vehicle detection patterns by hour",
            "code": """
# Get statistics
success, stats = vehicle_system.get_vehicle_statistics(start_time, end_time)
if success:
    print(f"Total vehicles: {stats['total_vehicles']}")
    for hour, count in stats['time_distribution'].items():
        print(f"{hour:02d}:00 - {count} vehicles")
"""
        }
    ]
    
    for i, example in enumerate(examples, 1):
        print(f"\n{i}. {example['title']}")
        print(f"   {example['description']}")
        print(f"   Code:")
        print(example['code'])

if __name__ == "__main__":
    print("🚗 Vehicle Recognition System Test Examples")
    print("Choose an option:")
    print("1. Run full test with your camera")
    print("2. Show specific test scenarios")
    print("3. Show usage examples")
    print("4. Exit")
    
    choice = input("\nEnter your choice (1-4): ").strip()
    
    if choice == "1":
        test_vehicle_recognition()
    elif choice == "2":
        test_specific_scenarios()
    elif choice == "3":
        show_usage_examples()
    elif choice == "4":
        print("Goodbye!")
    else:
        print("Invalid choice. Running full test...")
        test_vehicle_recognition() 