from device_control_rest import NVRDevice
from dataclasses import asdict

def main():
    # Create NVR device instance
    # Replace these values with your actual device details
    host = "192.168.6.37"  # Example IP from protocol specification
    port = 80              # Default HTTP port
    username = "admin"     # Default username
    password = "123456"    # Default password

    try:
        # Initialize device
        device = NVRDevice(host, port, username, password)
        
        # Connect to device
        print("Connecting to device...")
        if device.connect():
            print("Successfully connected to device")
            
            # Get device information
            print("\nGetting device information...")
            device_info = device.get_device_info()
            print("\nDevice Information:")
            print("-" * 50)
            for key, value in asdict(device_info).items():
                print(f"{key.replace('_', ' ').title()}: {value}")
            print("-" * 50)
            
            # Only proceed with PTZ tests if the device supports it
            if device_info.integrated_ptz:
                # Test PTZ control
                print("\nTesting PTZ control...")
                if device.ptz_control("up", channel_id=1, speed=5):
                    print("PTZ control successful")
                
                # Test adding a preset
                print("\nAdding PTZ preset...")
                if device.ptz_add_preset("test_preset", channel_id=1):
                    print("Successfully added PTZ preset")
            else:
                print("\nSkipping PTZ tests - device does not support integrated PTZ")
            
            # Test capturing a picture
            print("\nCapturing picture...")
            filename = device.capture_picture(channel_id=1)
            print(f"Picture saved as: {filename}")
            
            # Test recording
            print("\nStarting recording...")
            if device.start_recording(channel_id=1):
                print("Recording started")
                # Wait for 5 seconds
                import time
                time.sleep(5)
                if device.stop_recording(channel_id=1):
                    print("Recording stopped")
            
        else:
            print("Failed to connect to device")
            
    except Exception as e:
        print(f"Error occurred: {e}")
    finally:
        # Disconnect from device
        device.disconnect()
        print("\nDisconnected from device")

if __name__ == "__main__":
    main() 