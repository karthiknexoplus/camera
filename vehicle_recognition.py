import requests
import base64
import xml.etree.ElementTree as ET
import os
import datetime
import time
import csv
import zipfile
from io import BytesIO

class VehicleRecognition:
    def __init__(self, host, port, username, password):
        self.host = host
        self.port = port
        self.username = username
        self.password = password
        
        # Build URLs
        self.url_search_by_time = f"http://{host}:{port}/SearchSnapVehicleByTime"
        self.url_search_by_key = f"http://{host}:{port}/SearchSnapVehicleByKey"
        self.url_sd_status = f"http://{host}:{port}/GetSdCardStatus"
        
        # Prepare headers
        auth_str = f"{username}:{password}"
        auth_bytes = auth_str.encode('ascii')
        base64_auth = base64.b64encode(auth_bytes).decode('ascii')
        self.headers = {
            'Connection': 'Keep-Alive',
            'Content-Type': 'application/xml; charset=UTF-8',
            'Accept': 'application/xml; charset=UTF-8',
            'Authorization': f'Basic {base64_auth}',
            'User-Agent': 'Mozilla/5.0'
        }
    
    def check_sd_status(self):
        """Check SD card status before operations"""
        try:
            response = requests.post(self.url_sd_status, headers=self.headers, timeout=15)
            response.raise_for_status()
            root = ET.fromstring(response.content)
            ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
            
            status = root.find('.//ipc:sdCardInfo/ipc:status', ns) if ns else root.find('.//sdCardInfo/status')
            if status is not None:
                status_text = status.text
                if status_text != "normal":
                    error_messages = {
                        "no card": "No SD card detected",
                        "formatting": "SD card is being formatted",
                        "uninit": "SD card is not initialized",
                        "popup": "SD card has been ejected",
                        "timeout": "SD card operation timeout"
                    }
                    return False, error_messages.get(status_text, f"SD card status: {status_text}")
                return True, "SD card is ready"
            return False, "Unable to determine SD card status"
        except Exception as e:
            return False, f"Error checking SD card status: {e}"
    
    def search_vehicles_by_time(self, start_time, end_time, vehicle_plate="", list_type=""):
        """Search for vehicles within a time range"""
        try:
            # Build XML request
            xml_body = f'''<?xml version="1.0" encoding="utf-8"?>
<config><search>
<starttime type="string"><![CDATA[{start_time}]]></starttime>
<endtime type="string"><![CDATA[{end_time}]]></endtime>
{'' if not vehicle_plate else f'<vehiclePlate type="string"><![CDATA[{vehicle_plate}]]></vehiclePlate>'}
{'' if not list_type else f'<listType type="listType">{list_type}</listType>'}
</search></config>'''
            
            response = requests.post(self.url_search_by_time, headers=self.headers, data=xml_body.encode('utf-8'))
            response.raise_for_status()
            
            root = ET.fromstring(response.content)
            ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
            
            # Check for errors
            status = root.get('status')
            if status == 'failed':
                error_code = root.get('errorCode', 'unknown')
                return False, f"Search failed with error code: {error_code}"
            
            # Parse vehicle list
            vehicle_list = root.find('.//ipc:captureVehicleList', ns) if ns else root.find('.//captureVehicleList')
            if vehicle_list is not None:
                count = int(vehicle_list.get('count', '0'))
                vehicles = []
                
                items = vehicle_list.findall('ipc:item', ns) if ns else vehicle_list.findall('item')
                for item in items:
                    vehicle_id = item.find('.//ipc:vehicleID', ns) if ns else item.find('.//vehicleID')
                    snap_time = item.find('.//ipc:snapTime', ns) if ns else item.find('.//snapTime')
                    
                    if vehicle_id is not None and snap_time is not None:
                        vehicles.append({
                            'vehicleID': vehicle_id.text,
                            'snapTime': snap_time.text
                        })
                
                return True, {'count': count, 'vehicles': vehicles}
            else:
                return False, "No vehicle data found in response"
                
        except Exception as e:
            return False, f"Error searching vehicles: {e}"
    
    def get_vehicle_details(self, vehicle_id, snap_time, request_panoramic_pic=True):
        """Get detailed information for a specific vehicle"""
        try:
            xml_body = f'''<?xml version="1.0" encoding="utf-8"?>
<config><search>
<snapTime type="uint64">{snap_time}</snapTime>
<vehicleID type="uint32">{vehicle_id}</vehicleID>
<requestPanoramicPic type="boolean">{str(request_panoramic_pic).lower()}</requestPanoramicPic>
</search></config>'''
            
            response = requests.post(self.url_search_by_key, headers=self.headers, data=xml_body.encode('utf-8'))
            response.raise_for_status()
            
            root = ET.fromstring(response.content)
            ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
            
            # Check for errors
            status = root.get('status')
            if status == 'failed':
                error_code = root.get('errorCode', 'unknown')
                return False, f"Failed to get vehicle details with error code: {error_code}"
            
            # Parse vehicle details
            snap_vehicle = root.find('.//ipc:snapVehicle', ns) if ns else root.find('.//snapVehicle')
            if snap_vehicle is not None:
                snap_info = snap_vehicle.find('.//ipc:snapInfo', ns) if ns else snap_vehicle.find('.//snapInfo')
                if snap_info is not None:
                    details = {}
                    
                    # Extract all available information
                    fields = ['time', 'vehiclePlate', 'listType', 'color', 'pictureData']
                    for field in fields:
                        elem = snap_info.find(f'.//ipc:{field}', ns) if ns else snap_info.find(f'.//{field}')
                        if elem is not None:
                            details[field] = elem.text
                    
                    return True, details
                else:
                    return False, "No snap info found in response"
            else:
                return False, "No vehicle data found in response"
                
        except Exception as e:
            return False, f"Error getting vehicle details: {e}"
    
    def export_vehicle_data(self, start_time, end_time, vehicle_plate="", list_type="", include_images=True, output_file=None):
        """Export vehicle data to CSV/ZIP file with optional images"""
        try:
            # Check SD card status first
            sd_ok, sd_message = self.check_sd_status()
            if not sd_ok:
                return False, f"SD card not ready: {sd_message}"
            
            # Search for vehicles
            success, result = self.search_vehicles_by_time(start_time, end_time, vehicle_plate, list_type)
            if not success:
                return False, result
            
            vehicle_data = result
            count = vehicle_data['count']
            vehicles = vehicle_data['vehicles']
            
            if count == 0:
                return False, "No vehicles found in the specified time range"
            
            print(f"Found {count} vehicles. Starting export...")
            
            # Prepare export data
            export_data = []
            list_type_map = {
                "outOfList": "Stranger Vehicle",
                "temporaryList": "Temporary Vehicle", 
                "blackList": "Blacklist Vehicle",
                "whiteList": "Whitelist Vehicle"
            }
            
            # Process each vehicle
            for i, vehicle in enumerate(vehicles):
                print(f"Processing vehicle {i+1}/{count}...")
                
                success, details = self.get_vehicle_details(
                    vehicle['vehicleID'], 
                    vehicle['snapTime'], 
                    include_images
                )
                
                if success:
                    # Format time
                    time_str = details.get('time', '')
                    if time_str:
                        time_str = time_str[:19]  # Remove milliseconds
                    
                    # Get list type description
                    list_type_desc = list_type_map.get(details.get('listType', ''), 'Unknown')
                    
                    export_data.append({
                        'vehiclePlate': details.get('vehiclePlate', ''),
                        'listType': list_type_desc,
                        'snapTime': time_str,
                        'color': details.get('color', ''),
                        'pictureData': details.get('pictureData', '') if include_images else '',
                        'snapPicFileName': f"{vehicle['snapTime']}.jpg"
                    })
                else:
                    print(f"Warning: Failed to get details for vehicle {vehicle['vehicleID']}: {details}")
            
            # Generate output file
            if output_file is None:
                timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
                output_file = f"Vehicle_Export_{timestamp}.zip"
            
            # Create ZIP file
            with zipfile.ZipFile(output_file, 'w') as zip_file:
                # Create CSV content
                csv_content = "Index,Vehicle Plate,List Type,Snap Time,SnapPicFileName,Color\n"
                for i, data in enumerate(export_data, 1):
                    csv_content += f"{i},{data['vehiclePlate']},{data['listType']},{data['snapTime']},{data['snapPicFileName']},{data['color']}\n"
                
                # Add BOM for Chinese characters
                csv_content = '\ufeff' + csv_content
                zip_file.writestr("VehicleMatchResult.csv", csv_content)
                
                # Add images if requested
                if include_images:
                    for data in export_data:
                        if data['pictureData']:
                            try:
                                # Decode base64 image data
                                image_data = base64.b64decode(data['pictureData'])
                                zip_file.writestr(f"SnapPic/{data['snapPicFileName']}", image_data)
                            except Exception as e:
                                print(f"Warning: Failed to add image {data['snapPicFileName']}: {e}")
            
            print(f"Export completed successfully: {output_file}")
            return True, f"Exported {len(export_data)} vehicles to {output_file}"
            
        except Exception as e:
            return False, f"Error during export: {e}"
    
    def get_vehicle_statistics(self, start_time, end_time):
        """Get statistics about vehicle detection"""
        try:
            success, result = self.search_vehicles_by_time(start_time, end_time)
            if not success:
                return False, result
            
            vehicles = result['vehicles']
            if not vehicles:
                return True, {
                    'total_vehicles': 0,
                    'list_types': {},
                    'time_distribution': {}
                }
            
            # Analyze data
            list_types = {}
            time_distribution = {}
            
            for vehicle in vehicles:
                # Get vehicle details for analysis
                success, details = self.get_vehicle_details(vehicle['vehicleID'], vehicle['snapTime'], False)
                if success:
                    list_type = details.get('listType', 'unknown')
                    list_types[list_type] = list_types.get(list_type, 0) + 1
                    
                    # Time distribution (by hour)
                    time_str = details.get('time', '')
                    if time_str:
                        try:
                            dt = datetime.datetime.strptime(time_str[:19], '%Y-%m-%d %H:%M:%S')
                            hour = dt.hour
                            time_distribution[hour] = time_distribution.get(hour, 0) + 1
                        except:
                            pass
            
            return True, {
                'total_vehicles': len(vehicles),
                'list_types': list_types,
                'time_distribution': time_distribution
            }
            
        except Exception as e:
            return False, f"Error getting statistics: {e}"

def print_vehicle_search_menu():
    """Display vehicle search menu"""
    print("\n=== Vehicle Recognition System ===")
    print("1. Search Vehicles by Time Range")
    print("2. Get Vehicle Details")
    print("3. Export Vehicle Data")
    print("4. Get Vehicle Statistics")
    print("5. Check SD Card Status")
    print("0. Back to Main Menu")
    print("=" * 40)

def vehicle_search_interface(host, port, username, password):
    """Interactive interface for vehicle recognition features"""
    vehicle_system = VehicleRecognition(host, port, username, password)
    
    while True:
        print_vehicle_search_menu()
        choice = input("Enter your choice (0-5): ").strip()
        
        if choice == "0":
            break
        elif choice == "1":
            print("\n--- Search Vehicles by Time Range ---")
            start_time = input("Enter start time (YYYY-MM-DD HH:MM:SS): ").strip()
            end_time = input("Enter end time (YYYY-MM-DD HH:MM:SS): ").strip()
            vehicle_plate = input("Enter vehicle plate (optional): ").strip()
            list_type = input("Enter list type (optional - outOfList/temporaryList/blackList/whiteList): ").strip()
            
            success, result = vehicle_system.search_vehicles_by_time(start_time, end_time, vehicle_plate, list_type)
            if success:
                data = result
                print(f"\nFound {data['count']} vehicles:")
                for i, vehicle in enumerate(data['vehicles'][:10], 1):  # Show first 10
                    print(f"  {i}. Vehicle ID: {vehicle['vehicleID']}, Time: {vehicle['snapTime']}")
                if data['count'] > 10:
                    print(f"  ... and {data['count'] - 10} more vehicles")
            else:
                print(f"Error: {result}")
        
        elif choice == "2":
            print("\n--- Get Vehicle Details ---")
            vehicle_id = input("Enter vehicle ID: ").strip()
            snap_time = input("Enter snap time: ").strip()
            include_image = input("Include image data? (y/n): ").strip().lower() == 'y'
            
            success, result = vehicle_system.get_vehicle_details(vehicle_id, snap_time, include_image)
            if success:
                details = result
                print("\nVehicle Details:")
                for key, value in details.items():
                    if key != 'pictureData':  # Don't print image data
                        print(f"  {key}: {value}")
                if 'pictureData' in details and details['pictureData']:
                    print("  Image data: Available (base64 encoded)")
            else:
                print(f"Error: {result}")
        
        elif choice == "3":
            print("\n--- Export Vehicle Data ---")
            start_time = input("Enter start time (YYYY-MM-DD HH:MM:SS): ").strip()
            end_time = input("Enter end time (YYYY-MM-DD HH:MM:SS): ").strip()
            vehicle_plate = input("Enter vehicle plate (optional): ").strip()
            list_type = input("Enter list type (optional): ").strip()
            include_images = input("Include images? (y/n): ").strip().lower() == 'y'
            output_file = input("Enter output filename (optional): ").strip() or None
            
            success, result = vehicle_system.export_vehicle_data(
                start_time, end_time, vehicle_plate, list_type, include_images, output_file
            )
            if success:
                print(f"Success: {result}")
            else:
                print(f"Error: {result}")
        
        elif choice == "4":
            print("\n--- Get Vehicle Statistics ---")
            start_time = input("Enter start time (YYYY-MM-DD HH:MM:SS): ").strip()
            end_time = input("Enter end time (YYYY-MM-DD HH:MM:SS): ").strip()
            
            success, result = vehicle_system.get_vehicle_statistics(start_time, end_time)
            if success:
                stats = result
                print(f"\nVehicle Statistics:")
                print(f"  Total Vehicles: {stats['total_vehicles']}")
                
                if stats['list_types']:
                    print("  List Type Distribution:")
                    for list_type, count in stats['list_types'].items():
                        print(f"    {list_type}: {count}")
                
                if stats['time_distribution']:
                    print("  Time Distribution (by hour):")
                    for hour in sorted(stats['time_distribution'].keys()):
                        print(f"    {hour:02d}:00 - {hour:02d}:59: {stats['time_distribution'][hour]} vehicles")
            else:
                print(f"Error: {result}")
        
        elif choice == "5":
            print("\n--- Check SD Card Status ---")
            success, result = vehicle_system.check_sd_status()
            if success:
                print(f"Status: {result}")
            else:
                print(f"Error: {result}")
        
        else:
            print("Invalid choice. Please enter a number between 0 and 5.")
        
        input("\nPress Enter to continue...")

# Integration with main tool
def add_vehicle_recognition_to_menu():
    """Add vehicle recognition option to main menu"""
    return "29. Vehicle Recognition System"

def handle_vehicle_recognition_choice():
    """Handle vehicle recognition menu choice"""
    print("\n=== Vehicle Recognition System ===")
    print("This feature provides advanced vehicle detection and license plate recognition capabilities.")
    print("Features include:")
    print("- Time-based vehicle search")
    print("- Detailed vehicle information retrieval")
    print("- Data export with images")
    print("- Statistical analysis")
    print("- SD card status monitoring")
    
    proceed = input("\nProceed to Vehicle Recognition System? (y/n): ").strip().lower()
    if proceed == 'y':
        vehicle_search_interface(HOST, PORT, USERNAME, PASSWORD)
    else:
        print("Returning to main menu...")

if __name__ == "__main__":
    # Test the vehicle recognition system
    print("Vehicle Recognition System Test")
    print("Enter camera connection details:")
    host = input("IP Address: ").strip()
    port = input("Port (default 80): ").strip() or "80"
    username = input("Username (default admin): ").strip() or "admin"
    password = input("Password (default admin): ").strip() or "admin"
    
    vehicle_search_interface(host, int(port), username, password) 