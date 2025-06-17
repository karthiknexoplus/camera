import requests
import base64
import xml.etree.ElementTree as ET
import os
import datetime
import time

# Configuration - User will input IP address
def get_device_config():
    print("=== IP Camera Device Information Tool ===")
    print("\nAvailable cameras:")
    print("1. Camera 1 - 192.168.60.252")
    print("2. Camera 2 - 192.168.60.253")
    print("3. Custom IP address")
    
    while True:
        choice = input("\nSelect camera (1-3): ").strip()
        
        if choice == "1":
            host = "192.168.60.252"
            break
        elif choice == "2":
            host = "192.168.60.253"
            break
        elif choice == "3":
            host = input("Enter custom IP address: ").strip()
            if host:
                break
            else:
                print("Invalid IP address. Please try again.")
        else:
            print("Invalid choice. Please enter 1, 2, or 3.")
    
    # Get other configuration
    port = input("Enter port (default 80): ").strip() or "80"
    username = input("Enter username (default admin): ").strip() or "admin"
    password = input("Enter password (default admin): ").strip() or "admin"
    
    print(f"\nConnecting to: {host}:{port}")
    print(f"Username: {username}")
    print("=" * 50)
    
    return host, int(port), username, password

# Get device configuration from user
HOST, PORT, USERNAME, PASSWORD = get_device_config()

# Build URLs
url_device = f"http://{HOST}:{PORT}/GetDeviceInfo"
url_disk = f"http://{HOST}:{PORT}/GetDiskInfo"
url_detail = f"http://{HOST}:{PORT}/GetDeviceDetail"
url_time = f"http://{HOST}:{PORT}/GetDateAndTime"
url_stream = f"http://{HOST}:{PORT}/GetStreamCaps"
url_image = f"http://{HOST}:{PORT}/GetImageConfig"
url_privacy_mask = f"http://{HOST}:{PORT}/GetPrivacyMaskConfig"
url_ptz_caps = f"http://{HOST}:{PORT}/PtzGetCaps"
url_ptz_control = f"http://{HOST}:{PORT}/PtzControl"
url_motion = f"http://{HOST}:{PORT}/GetMotionConfig"
url_alarm_trigger = f"http://{HOST}:{PORT}/GetAlarmTriggerConfig"
url_net_basic = f"http://{HOST}:{PORT}/GetNetBasicConfig"
url_reboot = f"http://{HOST}:{PORT}/Reboot"
url_vfd = f"http://{HOST}:{PORT}/GetSmartVfdConfig"
url_perimeter = f"http://{HOST}:{PORT}/GetSmartPerimeterConfig"
url_vehicle = f"http://{HOST}:{PORT}/GetSmartVehicleConfig"
url_vehicle_plate = f"http://{HOST}:{PORT}/GetVehiclePlate"
url_cdd = f"http://{HOST}:{PORT}/GetSmartCddConfig"
url_cpc = f"http://{HOST}:{PORT}/GetSmartCpcConfig"
url_subscription = f"http://{HOST}:{PORT}/GetSubscriptionConfig"
url_vehicle_plate_progress = f"http://{HOST}:{PORT}/GetVehiclePlateProgress"
url_add_vehicle_plate = f"http://{HOST}:{PORT}/AddVehiclePlate"

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

def print_device_info():
    print("\n--- Device Info ---")
    try:
        response = requests.post(url_device, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        device_info = root.find('.//ipc:deviceInfo', ns) if ns else root.find('.//deviceInfo')
        device_description = None
        if device_info is None:
            print("Device info not found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
        else:
            for child in device_info:
                print(f"{child.tag.split('}')[-1]}: {child.text}")
                if child.tag.split('}')[-1] == 'deviceDescription':
                    device_description = child.text.strip() if child.text else None
        return device_description
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")
        return None

def print_disk_info():
    print("\n--- Disk Info ---")
    try:
        response = requests.post(url_disk, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        disk_info = root.find('.//ipc:diskInfo', ns) if ns else root.find('.//diskInfo')
        if disk_info is None:
            print("Disk info not found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
        else:
            items = disk_info.findall('ipc:item', ns) if ns else disk_info.findall('item')
            if not items:
                print("No disk found on device.")
            for item in items:
                print("Disk:")
                for child in item:
                    print(f"  {child.tag.split('}')[-1]}: {child.text}")
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_device_detail():
    print("\n--- Device Detail ---")
    try:
        response = requests.post(url_detail, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Find the detail section
        detail = root.find('.//ipc:detail', ns) if ns else root.find('.//detail')
        if detail is None:
            print("Device detail not found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
            return

        # Process property section
        property_section = detail.find('.//ipc:property', ns) if ns else detail.find('.//property')
        if property_section is not None:
            print("\nDevice Properties:")
            for prop in property_section:
                prop_name = prop.tag.split('}')[-1]
                prop_type = prop.get('type', '')
                prop_value = prop.text.strip() if prop.text else ''
                print(f"  {prop_name} ({prop_type}): {prop_value}")

        # Process smart features
        smart_section = detail.find('.//ipc:smart', ns) if ns else detail.find('.//smart')
        if smart_section is not None:
            print("\nSmart Features:")
            for feature in smart_section:
                feature_name = feature.tag.split('}')[-1]
                feature_value = feature.text.strip() if feature.text else 'false'
                print(f"  {feature_name}: {feature_value}")

        # Process image features
        image_section = detail.find('.//ipc:image', ns) if ns else detail.find('.//image')
        if image_section is not None:
            print("\nImage Features:")
            for feature in image_section:
                feature_name = feature.tag.split('}')[-1]
                feature_value = feature.text.strip() if feature.text else 'false'
                print(f"  {feature_name}: {feature_value}")

        # Process alarm features
        alarm_section = detail.find('.//ipc:alarm', ns) if ns else detail.find('.//alarm')
        if alarm_section is not None:
            print("\nAlarm Features:")
            for feature in alarm_section:
                feature_name = feature.tag.split('}')[-1]
                feature_value = feature.text.strip() if feature.text else 'false'
                print(f"  {feature_name}: {feature_value}")

        # Process system features
        system_section = detail.find('.//ipc:system', ns) if ns else detail.find('.//system')
        if system_section is not None:
            print("\nSystem Features:")
            for feature in system_section:
                feature_name = feature.tag.split('}')[-1]
                feature_value = feature.text.strip() if feature.text else 'false'
                print(f"  {feature_name}: {feature_value}")

    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_date_time():
    print("\n--- Date and Time Information ---")
    try:
        response = requests.post(url_time, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Find the time section
        time_section = root.find('.//ipc:time', ns) if ns else root.find('.//time')
        if time_section is None:
            print("Time information not found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
            return

        # Process timezone information
        timezone_info = time_section.find('.//ipc:timezoneInfo', ns) if ns else time_section.find('.//timezoneInfo')
        if timezone_info is not None:
            print("\nTimezone Information:")
            timezone = timezone_info.find('.//ipc:timeZone', ns) if ns else timezone_info.find('.//timeZone')
            daylight_switch = timezone_info.find('.//ipc:daylightSwitch', ns) if ns else timezone_info.find('.//daylightSwitch')
            
            if timezone is not None:
                timezone_value = timezone.text.strip() if timezone.text else ''
                print(f"  Timezone: {timezone_value}")
                # Parse and explain timezone format
                if ',' in timezone_value:
                    parts = timezone_value.split(',')
                    if len(parts) >= 3:
                        print(f"  Standard Time: {parts[0]}")
                        print(f"  Daylight Saving Time: {parts[1]}")
                        print(f"  Transition Rules: {parts[2]}")
            
            if daylight_switch is not None:
                daylight_value = daylight_switch.text.strip() if daylight_switch.text else 'false'
                print(f"  Daylight Saving Enabled: {daylight_value}")

        # Process synchronization information
        sync_info = time_section.find('.//ipc:synchronizeInfo', ns) if ns else time_section.find('.//synchronizeInfo')
        if sync_info is not None:
            print("\nSynchronization Information:")
            sync_type = sync_info.find('.//ipc:type', ns) if ns else sync_info.find('.//type')
            ntp_server = sync_info.find('.//ipc:ntpServer', ns) if ns else sync_info.find('.//ntpServer')
            current_time = sync_info.find('.//ipc:currentTime', ns) if ns else sync_info.find('.//currentTime')
            
            if sync_type is not None:
                sync_value = sync_type.text.strip() if sync_type.text else ''
                print(f"  Synchronization Type: {sync_value}")
            
            if ntp_server is not None:
                ntp_value = ntp_server.text.strip() if ntp_server.text else ''
                print(f"  NTP Server: {ntp_value}")
            
            if current_time is not None:
                time_value = current_time.text.strip() if current_time.text else ''
                print(f"  Current Time: {time_value}")

    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_stream_caps():
    print("\n--- Stream Capabilities ---")
    try:
        response = requests.post(url_stream, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Get RTSP port
        rtsp_port = root.find('.//ipc:rtspPort', ns) if ns else root.find('.//rtspPort')
        if rtsp_port is not None:
            print(f"RTSP Port: {rtsp_port.text}")

        # Get stream list
        stream_list = root.find('.//ipc:streamList', ns) if ns else root.find('.//streamList')
        if stream_list is None:
            print("Stream capabilities not found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
            return

        # Get available resolutions
        resolutions = root.findall('.//ipc:resolution/ipc:enum', ns) if ns else root.findall('.//resolution/enum')
        if resolutions:
            print("\nAvailable Resolutions:")
            for res in resolutions:
                print(f"  - {res.text}")

        # Get available encode types
        encode_types = root.findall('.//ipc:encodeType/ipc:enum', ns) if ns else root.findall('.//encodeType/enum')
        if encode_types:
            print("\nAvailable Encode Types:")
            for enc in encode_types:
                print(f"  - {enc.text}")

        # Get available encode levels
        encode_levels = root.findall('.//ipc:encodeLevel/ipc:enum', ns) if ns else root.findall('.//encodeLevel/enum')
        if encode_levels:
            print("\nAvailable Encode Levels:")
            for level in encode_levels:
                print(f"  - {level.text}")

        # Process each stream
        streams = stream_list.findall('ipc:item', ns) if ns else stream_list.findall('item')
        if streams:
            print("\nStream Profiles:")
            for stream in streams:
                stream_id = stream.get('id', 'unknown')
                stream_name = stream.find('.//ipc:streamName', ns) if ns else stream.find('.//streamName')
                stream_name = stream_name.text if stream_name is not None else 'unknown'
                
                print(f"\nStream {stream_id} ({stream_name}):")
                
                # Get resolution capabilities
                res_caps = stream.find('.//ipc:resolutionCaps', ns) if ns else stream.find('.//resolutionCaps')
                if res_caps is not None:
                    print("  Resolution Capabilities:")
                    for res in res_caps.findall('ipc:item', ns) if ns else res_caps.findall('item'):
                        max_fps = res.get('maxFrameRate', 'N/A')
                        print(f"    - {res.text} (Max FPS: {max_fps})")

                # Get encode type capabilities
                enc_caps = stream.find('.//ipc:encodeTypeCaps', ns) if ns else stream.find('.//encodeTypeCaps')
                if enc_caps is not None:
                    print("  Encode Type Capabilities:")
                    for enc in enc_caps.findall('ipc:item', ns) if ns else enc_caps.findall('item'):
                        print(f"    - {enc.text}")

                # Get encode level capabilities
                level_caps = stream.find('.//ipc:encodeLevelCaps', ns) if ns else stream.find('.//encodeLevelCaps')
                if level_caps is not None:
                    print("  Encode Level Capabilities:")
                    for level in level_caps.findall('ipc:item', ns) if ns else level_caps.findall('item'):
                        print(f"    - {level.text}")

                # Print RTSP URL format
                print(f"  RTSP URL Format: rtsp://{HOST}:{rtsp_port.text}/{stream_name}")

    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_image_config():
    print("\n--- Image Configuration ---")
    try:
        response = requests.post(url_image, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Get available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Types:")
            
            # Frequency options
            frequencies = types.findall('.//ipc:frequency/ipc:enum', ns) if ns else types.findall('.//frequency/enum')
            if frequencies:
                print("\nFrequency Options:")
                for freq in frequencies:
                    print(f"  - {freq.text}")
            
            # White balance modes
            wb_modes = types.findall('.//ipc:whitebalanceMode/ipc:enum', ns) if ns else types.findall('.//whitebalanceMode/enum')
            if wb_modes:
                print("\nWhite Balance Modes:")
                for mode in wb_modes:
                    print(f"  - {mode.text}")
            
            # IR Cut modes
            ircut_modes = types.findall('.//ipc:IRCutMode/ipc:enum', ns) if ns else types.findall('.//IRCutMode/enum')
            if ircut_modes:
                print("\nIR Cut Modes:")
                for mode in ircut_modes:
                    print(f"  - {mode.text}")

        # Get current image settings
        image = root.find('.//ipc:image', ns) if ns else root.find('.//image')
        if image is None:
            print("Image configuration not found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
            return

        print("\nCurrent Image Settings:")
        
        # Basic settings
        basic_settings = ['frequency', 'bright', 'contrast', 'hue', 'saturation', 
                         'mirrorSwitch', 'flipSwitch', 'irisSwitch', 'IRCutMode']
        for setting in basic_settings:
            elem = image.find(f'.//ipc:{setting}', ns) if ns else image.find(f'.//{setting}')
            if elem is not None:
                value = elem.text.strip() if elem.text else ''
                default = elem.get('default', 'N/A')
                print(f"  {setting}: {value} (Default: {default})")

        # WDR settings
        wdr = image.find('.//ipc:WDR', ns) if ns else image.find('.//WDR')
        if wdr is not None:
            print("\nWDR Settings:")
            switch = wdr.find('.//ipc:switch', ns) if ns else wdr.find('.//switch')
            value = wdr.find('.//ipc:value', ns) if ns else wdr.find('.//value')
            if switch is not None:
                print(f"  Switch: {switch.text} (Default: {switch.get('default', 'N/A')})")
            if value is not None:
                print(f"  Value: {value.text} (Default: {value.get('default', 'N/A')})")

        # White Balance settings
        wb = image.find('.//ipc:whiteBalance', ns) if ns else image.find('.//whiteBalance')
        if wb is not None:
            print("\nWhite Balance Settings:")
            mode = wb.find('.//ipc:mode', ns) if ns else wb.find('.//mode')
            red = wb.find('.//ipc:red', ns) if ns else wb.find('.//red')
            blue = wb.find('.//ipc:blue', ns) if ns else wb.find('.//blue')
            if mode is not None:
                print(f"  Mode: {mode.text} (Default: {mode.get('default', 'N/A')})")
            if red is not None:
                print(f"  Red: {red.text} (Default: {red.get('default', 'N/A')})")
            if blue is not None:
                print(f"  Blue: {blue.text} (Default: {blue.get('default', 'N/A')})")

        # Denoise settings
        denoise = image.find('.//ipc:denoise', ns) if ns else image.find('.//denoise')
        if denoise is not None:
            print("\nDenoise Settings:")
            switch = denoise.find('.//ipc:switch', ns) if ns else denoise.find('.//switch')
            value = denoise.find('.//ipc:value', ns) if ns else denoise.find('.//value')
            if switch is not None:
                print(f"  Switch: {switch.text} (Default: {switch.get('default', 'N/A')})")
            if value is not None:
                print(f"  Value: {value.text} (Default: {value.get('default', 'N/A')})")

        # Sharpen settings
        sharpen = image.find('.//ipc:sharpen', ns) if ns else image.find('.//sharpen')
        if sharpen is not None:
            print("\nSharpen Settings:")
            switch = sharpen.find('.//ipc:switch', ns) if ns else sharpen.find('.//switch')
            value = sharpen.find('.//ipc:value', ns) if ns else sharpen.find('.//value')
            if switch is not None:
                print(f"  Switch: {switch.text} (Default: {switch.get('default', 'N/A')})")
            if value is not None:
                print(f"  Value: {value.text} (Default: {value.get('default', 'N/A')})")

        # Back Light Adjust settings
        backlight = image.find('.//ipc:backLightAdjust', ns) if ns else image.find('.//backLightAdjust')
        if backlight is not None:
            print("\nBack Light Adjust Settings:")
            switch = backlight.find('.//ipc:switch', ns) if ns else backlight.find('.//switch')
            value = backlight.find('.//ipc:value', ns) if ns else backlight.find('.//value')
            if switch is not None:
                print(f"  Switch: {switch.text} (Default: {switch.get('default', 'N/A')})")
            if value is not None:
                print(f"  Value: {value.text} (Default: {value.get('default', 'N/A')})")

    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_snapshot():
    print("\n--- Get Snapshot ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/GetSnapshot/{channel_id}" if channel_id else f"http://{HOST}:{PORT}/GetSnapshot"
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        # Save the image
        now = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        filename = f"snapshot_ch{channel_id}_{now}.jpg"
        with open(filename, "wb") as f:
            f.write(response.content)
        print(f"Snapshot saved as {filename}")
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'content', None):
            print(f"Response content: {e.response.content}")

def print_snapshot_by_time():
    print("\n--- Get Snapshot By Time ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        time_str = input("Enter time (YYYY-MM-DD HH:MM:SS): ").strip()
        if not time_str:
            print("Time is required.")
            return
        length = input("Enter length in seconds (default 10): ").strip()
        if not length:
            length = "10"
        url = f"http://{HOST}:{PORT}/GetSnapshotByTime/{channel_id}" if channel_id else f"http://{HOST}:{PORT}/GetSnapshotByTime"
        xml_body = f'''<?xml version="1.0" encoding="UTF-8"?>\n<config version="1.0" xmlns="http://www.ipc.com/ver10">\n  <search>\n    <time type="string"><![CDATA[{time_str}]]></time>\n    <length type="uint16">{length}</length>\n  </search>\n</config>'''
        response = requests.post(url, headers=headers, data=xml_body.encode('utf-8'))
        response.raise_for_status()
        # Determine file extension from Content-Type
        content_type = response.headers.get('Content-Type', '').lower()
        if 'jpeg' in content_type:
            ext = 'jpg'
        elif 'h264' in content_type:
            ext = 'h264'
        elif 'h265' in content_type:
            ext = 'h265'
        else:
            ext = 'bin'
        safe_time = time_str.replace(':', '').replace(' ', '_').replace('-', '')
        now = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        filename = f"snapshotByTime_ch{channel_id}_{safe_time}_{now}.{ext}"
        with open(filename, "wb") as f:
            f.write(response.content)
        print(f"Snapshot by time saved as {filename} (Content-Type: {content_type})")
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'content', None):
            print(f"Response content: {e.response.content}")

def print_video_stream_config():
    print("\n--- Video Stream Configuration ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/GetVideoStreamConfig/{channel_id}" if channel_id else f"http://{HOST}:{PORT}/GetVideoStreamConfig"
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Types:")
            # bitRateType
            br_types = types.findall('.//ipc:bitRateType/ipc:enum', ns) if ns else types.findall('.//bitRateType/enum')
            if br_types:
                print("  Bit Rate Types:")
                for br in br_types:
                    print(f"    - {br.text}")
            # quality
            qualities = types.findall('.//ipc:quality/ipc:enum', ns) if ns else types.findall('.//quality/enum')
            if qualities:
                print("  Quality Levels:")
                for q in qualities:
                    print(f"    - {q.text}")
            # encodeType
            enc_types = types.findall('.//ipc:encodeType/ipc:enum', ns) if ns else types.findall('.//encodeType/enum')
            if enc_types:
                print("  Encode Types:")
                for enc in enc_types:
                    print(f"    - {enc.text}")

        # Print stream configs
        streams = root.find('.//ipc:streams', ns) if ns else root.find('.//streams')
        if streams is not None:
            count = streams.get('count', 'unknown')
            print(f"\nStreams (count={count}):")
            items = streams.findall('ipc:item', ns) if ns else streams.findall('item')
            for item in items:
                stream_id = item.get('id', 'unknown')
                name = item.find('.//ipc:name', ns) if ns else item.find('.//name')
                resolution = item.find('.//ipc:resolution', ns) if ns else item.find('.//resolution')
                frame_rate = item.find('.//ipc:frameRate', ns) if ns else item.find('.//frameRate')
                bit_rate_type = item.find('.//ipc:bitRateType', ns) if ns else item.find('.//bitRateType')
                max_bit_rate = item.find('.//ipc:maxBitRate', ns) if ns else item.find('.//maxBitRate')
                bit_rate_lists = item.find('.//ipc:bitRateLists', ns) if ns else item.find('.//bitRateLists')
                encode_type_caps = item.find('.//ipc:encodeTypeCaps', ns) if ns else item.find('.//encodeTypeCaps')
                encode_type = item.find('.//ipc:encodeType', ns) if ns else item.find('.//encodeType')
                encode_level = item.find('.//ipc:encodeLevel', ns) if ns else item.find('.//encodeLevel')
                quality = item.find('.//ipc:quality', ns) if ns else item.find('.//quality')
                gop = item.find('.//ipc:GOP', ns) if ns else item.find('.//GOP')
                print(f"\n  Stream ID: {stream_id}")
                if name is not None:
                    print(f"    Name: {name.text}")
                if resolution is not None:
                    print(f"    Resolution: {resolution.text}")
                if frame_rate is not None:
                    print(f"    Frame Rate: {frame_rate.text}")
                if bit_rate_type is not None:
                    print(f"    Bit Rate Type: {bit_rate_type.text}")
                if max_bit_rate is not None:
                    print(f"    Max Bit Rate: {max_bit_rate.text}")
                if bit_rate_lists is not None:
                    print("    Bit Rate List:")
                    for br in bit_rate_lists.findall('ipc:item', ns) if ns else bit_rate_lists.findall('item'):
                        print(f"      - {br.text}")
                if encode_type_caps is not None:
                    print("    Encode Type Caps:")
                    for enc in encode_type_caps.findall('ipc:item', ns) if ns else encode_type_caps.findall('item'):
                        print(f"      - {enc.text}")
                if encode_type is not None:
                    print(f"    Encode Type: {encode_type.text}")
                if encode_level is not None:
                    print(f"    Encode Level: {encode_level.text}")
                if quality is not None:
                    print(f"    Quality: {quality.text}")
                if gop is not None:
                    print(f"    GOP: {gop.text}")
        else:
            print("No stream configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_image_osd_config():
    print("\n--- Image OSD Configuration ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/GetImageOsdConfig/{channel_id}" if channel_id else f"http://{HOST}:{PORT}/GetImageOsdConfig"
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Types:")
            # dateFormat
            date_formats = types.findall('.//ipc:dateFormat/ipc:enum', ns) if ns else types.findall('.//dateFormat/enum')
            if date_formats:
                print("  Date Formats:")
                for fmt in date_formats:
                    print(f"    - {fmt.text}")
            # osdOverlayType
            overlay_types = types.findall('.//ipc:osdOverlayType/ipc:enum', ns) if ns else types.findall('.//osdOverlayType/enum')
            if overlay_types:
                print("  OSD Overlay Types:")
                for ot in overlay_types:
                    print(f"    - {ot.text}")

        # Print OSD configuration
        image_osd = root.find('.//ipc:imageOsd', ns) if ns else root.find('.//imageOsd')
        if image_osd is not None:
            print("\nOSD Configuration:")
            
            # Time settings
            time = image_osd.find('.//ipc:time', ns) if ns else image_osd.find('.//time')
            if time is not None:
                print("\nTime Display:")
                switch = time.find('.//ipc:switch', ns) if ns else time.find('.//switch')
                x = time.find('.//ipc:X', ns) if ns else time.find('.//X')
                y = time.find('.//ipc:Y', ns) if ns else time.find('.//Y')
                date_format = time.find('.//ipc:dateFormat', ns) if ns else time.find('.//dateFormat')
                if switch is not None:
                    print(f"  Enabled: {switch.text}")
                if x is not None:
                    print(f"  X Position: {x.text}")
                if y is not None:
                    print(f"  Y Position: {y.text}")
                if date_format is not None:
                    print(f"  Date Format: {date_format.text}")

            # Channel name settings
            channel_name = image_osd.find('.//ipc:channelName', ns) if ns else image_osd.find('.//channelName')
            if channel_name is not None:
                print("\nChannel Name Display:")
                switch = channel_name.find('.//ipc:switch', ns) if ns else channel_name.find('.//switch')
                x = channel_name.find('.//ipc:X', ns) if ns else channel_name.find('.//X')
                y = channel_name.find('.//ipc:Y', ns) if ns else channel_name.find('.//Y')
                name = channel_name.find('.//ipc:name', ns) if ns else channel_name.find('.//name')
                if switch is not None:
                    print(f"  Enabled: {switch.text}")
                if x is not None:
                    print(f"  X Position: {x.text}")
                if y is not None:
                    print(f"  Y Position: {y.text}")
                if name is not None:
                    print(f"  Name: {name.text}")

            # Text overlay settings
            text_overlay = image_osd.find('.//ipc:textOverLay', ns) if ns else image_osd.find('.//textOverLay')
            if text_overlay is not None:
                count = text_overlay.get('count', 'unknown')
                print(f"\nText Overlays (count={count}):")
                items = text_overlay.findall('ipc:item', ns) if ns else text_overlay.findall('item')
                for i, item in enumerate(items, 1):
                    print(f"\n  Overlay {i}:")
                    switch = item.find('.//ipc:switch', ns) if ns else item.find('.//switch')
                    x = item.find('.//ipc:X', ns) if ns else item.find('.//X')
                    y = item.find('.//ipc:Y', ns) if ns else item.find('.//Y')
                    value = item.find('.//ipc:value', ns) if ns else item.find('.//value')
                    show_level = item.find('.//ipc:showLevel', ns) if ns else item.find('.//showLevel')
                    flicker = item.find('.//ipc:flickerSwitch', ns) if ns else item.find('.//flickerSwitch')
                    overlay_type = item.find('.//ipc:osdOverlayType', ns) if ns else item.find('.//osdOverlayType')
                    
                    if switch is not None:
                        print(f"    Enabled: {switch.text}")
                    if x is not None:
                        print(f"    X Position: {x.text}")
                    if y is not None:
                        print(f"    Y Position: {y.text}")
                    if value is not None:
                        print(f"    Value: {value.text}")
                    if show_level is not None:
                        print(f"    Show Level: {show_level.text}")
                    if flicker is not None:
                        print(f"    Flicker: {flicker.text}")
                    if overlay_type is not None:
                        print(f"    Overlay Type: {overlay_type.text}")
        else:
            print("No OSD configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_privacy_mask_config():
    print("\n--- Privacy Mask Configuration ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/GetPrivacyMaskConfig/{channel_id}" if channel_id else f"http://{HOST}:{PORT}/GetPrivacyMaskConfig"
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Types:")
            # color options
            colors = types.findall('.//ipc:color/ipc:enum', ns) if ns else types.findall('.//color/enum')
            if colors:
                print("  Available Colors:")
                for color in colors:
                    print(f"    - {color.text}")

        # Print privacy mask configuration
        privacy_mask = root.find('.//ipc:privacyMask', ns) if ns else root.find('.//privacyMask')
        if privacy_mask is not None:
            count = privacy_mask.get('count', 'unknown')
            print(f"\nPrivacy Masks (count={count}):")
            items = privacy_mask.findall('ipc:item', ns) if ns else privacy_mask.findall('item')
            for i, item in enumerate(items, 1):
                print(f"\n  Mask {i}:")
                switch = item.find('.//ipc:switch', ns) if ns else item.find('.//switch')
                rectangle = item.find('.//ipc:rectangle', ns) if ns else item.find('.//rectangle')
                color = item.find('.//ipc:color', ns) if ns else item.find('.//color')
                
                if switch is not None:
                    print(f"    Enabled: {switch.text}")
                if rectangle is not None:
                    x = rectangle.find('.//ipc:X', ns) if ns else rectangle.find('.//X')
                    y = rectangle.find('.//ipc:Y', ns) if ns else rectangle.find('.//Y')
                    width = rectangle.find('.//ipc:width', ns) if ns else rectangle.find('.//width')
                    height = rectangle.find('.//ipc:height', ns) if ns else rectangle.find('.//height')
                    
                    if x is not None and y is not None and width is not None and height is not None:
                        print(f"    Position: ({x.text}, {y.text})")
                        print(f"    Size: {width.text}x{height.text}")
                        print(f"    Note: Position is based on 640x480 resolution")
                if color is not None:
                    print(f"    Color: {color.text}")
        else:
            print("No privacy mask configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_ptz_caps():
    print("\n--- PTZ Capabilities ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/PtzGetCaps/{channel_id}" if channel_id else f"http://{HOST}:{PORT}/PtzGetCaps"
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print PTZ capabilities
        caps = root.find('.//ipc:caps', ns) if ns else root.find('.//caps')
        if caps is not None:
            print("\nPTZ Capabilities:")
            
            # Control speed range
            min_speed = caps.find('.//ipc:controlMinSpeed', ns) if ns else caps.find('.//controlMinSpeed')
            max_speed = caps.find('.//ipc:controlMaxSpeed', ns) if ns else caps.find('.//controlMaxSpeed')
            if min_speed is not None and max_speed is not None:
                print(f"  Control Speed Range: {min_speed.text} - {max_speed.text}")
            
            # Preset count
            preset_count = caps.find('.//ipc:presetMaxCount', ns) if ns else caps.find('.//presetMaxCount')
            if preset_count is not None:
                print(f"  Maximum Preset Count: {preset_count.text}")
            
            # Cruise settings
            cruise_count = caps.find('.//ipc:cruiseMaxCount', ns) if ns else caps.find('.//cruiseMaxCount')
            if cruise_count is not None:
                print(f"  Maximum Cruise Count: {cruise_count.text}")
            
            # Cruise preset speed range
            cruise_min_speed = caps.find('.//ipc:cruisePresetMinSpeed', ns) if ns else caps.find('.//cruisePresetMinSpeed')
            cruise_max_speed = caps.find('.//ipc:cruisePresetMaxSpeed', ns) if ns else caps.find('.//cruisePresetMaxSpeed')
            if cruise_min_speed is not None and cruise_max_speed is not None:
                print(f"  Cruise Preset Speed Range: {cruise_min_speed.text} - {cruise_max_speed.text}")
            
            # Cruise preset hold time
            cruise_hold_time = caps.find('.//ipc:cruisePresetMaxHoldTime', ns) if ns else caps.find('.//cruisePresetMaxHoldTime')
            if cruise_hold_time is not None:
                print(f"  Maximum Cruise Preset Hold Time: {cruise_hold_time.text} seconds")
            
            # Cruise preset count
            cruise_preset_count = caps.find('.//ipc:cruisePresetMaxCount', ns) if ns else caps.find('.//cruisePresetMaxCount')
            if cruise_preset_count is not None:
                print(f"  Maximum Cruise Preset Count: {cruise_preset_count.text}")
        else:
            print("No PTZ capabilities found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_ptz_control():
    print("\n--- PTZ Control ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        
        # Display available actions
        print("\nAvailable PTZ Actions:")
        actions = [
            "Up", "Down", "Left", "Right",
            "LeftUp", "LeftDown", "RightUp", "RightDown",
            "Near", "Far", "ZoomIn", "ZoomOut",
            "IrisOpen", "IrisClose", "Stop"
        ]
        for i, action in enumerate(actions, 1):
            print(f"{i}. {action}")
        
        # Get action choice
        while True:
            try:
                action_choice = int(input("\nSelect action (1-15): "))
                if 1 <= action_choice <= len(actions):
                    action = actions[action_choice - 1]
                    break
                print(f"Invalid choice. Please enter a number between 1 and {len(actions)}.")
            except ValueError:
                print("Invalid input. Please enter a number.")
        
        # Get speed
        while True:
            try:
                speed = int(input("Enter speed (1-8): "))
                if 1 <= speed <= 8:
                    break
                print("Invalid speed. Please enter a number between 1 and 8.")
            except ValueError:
                print("Invalid input. Please enter a number.")
        
        # Build URL and XML request
        url = f"http://{HOST}:{PORT}/PtzControl/{channel_id}/{action}" if channel_id else f"http://{HOST}:{PORT}/PtzControl/{action}"
        xml_body = f'''<?xml version="1.0" encoding="utf-8" ?>
<actionInfo version="1.0" xmlns="http://www.ipc.com/ver10">
<speed>{speed}</speed>
</actionInfo>'''
        
        # Send request
        response = requests.post(url, headers=headers, data=xml_body.encode('utf-8'))
        response.raise_for_status()
        
        print(f"\nPTZ Control command sent successfully:")
        print(f"  Action: {action}")
        print(f"  Speed: {speed}")
        print(f"  Channel: {channel_id}")
        
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_motion_config():
    print("\n--- Motion Detection Configuration ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/GetMotionConfig/{channel_id}" if channel_id else f"http://{HOST}:{PORT}/GetMotionConfig"
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print motion configuration
        motion = root.find('.//ipc:motion', ns) if ns else root.find('.//motion')
        if motion is not None:
            # Basic settings
            switch = motion.find('.//ipc:switch', ns) if ns else motion.find('.//switch')
            sensitivity = motion.find('.//ipc:sensitivity', ns) if ns else motion.find('.//sensitivity')
            alarm_hold_time = motion.find('.//ipc:alarmHoldTime', ns) if ns else motion.find('.//alarmHoldTime')
            
            print("\nBasic Settings:")
            if switch is not None:
                print(f"  Motion Detection: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            if sensitivity is not None:
                min_val = sensitivity.get('min', 'N/A')
                max_val = sensitivity.get('max', 'N/A')
                print(f"  Sensitivity: {sensitivity.text} (Range: {min_val}-{max_val})")
            if alarm_hold_time is not None:
                print(f"  Alarm Hold Time: {alarm_hold_time.text} seconds")

            # Motion detection areas
            area = motion.find('.//ipc:area', ns) if ns else motion.find('.//area')
            if area is not None:
                count = area.get('count', 'unknown')
                print(f"\nMotion Detection Areas (22x{count} grid):")
                items = area.findall('ipc:item', ns) if ns else area.findall('item')
                for i, item in enumerate(items, 1):
                    area_map = item.text.strip() if item.text else ''
                    print(f"\n  Row {i}:")
                    # Split the string into individual characters and display as a grid
                    for j, cell in enumerate(area_map, 1):
                        status = "Active" if cell == "1" else "Inactive"
                        print(f"    Cell {j}: {status}")

            # Alarm output triggers
            trigger_alarm = motion.find('.//ipc:triggerAlarmOut', ns) if ns else motion.find('.//triggerAlarmOut')
            if trigger_alarm is not None:
                count = trigger_alarm.get('count', 'unknown')
                print(f"\nAlarm Output Triggers (count={count}):")
                items = trigger_alarm.findall('ipc:item', ns) if ns else trigger_alarm.findall('item')
                for item in items:
                    alarm_id = item.get('id', 'unknown')
                    status = "Enabled" if item.text.lower() == 'true' else "Disabled"
                    print(f"  Alarm Output {alarm_id}: {status}")
        else:
            print("No motion configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_alarm_trigger_config():
    print("\n--- Alarm Trigger Configuration ---")
    try:
        # Display available action types
        print("\nAvailable Action Types:")
        actions = [
            "alarmIn", "motion", "avd", "cdd", "cpc", "ipd",
            "tripwire", "osc", "perimeter", "vfd", "vehicle",
            "aoientry", "aoileave", "passlinecount"
        ]
        for i, action in enumerate(actions, 1):
            print(f"{i}. {action}")
        
        # Get action choice
        while True:
            try:
                action_choice = int(input("\nSelect action type (1-14): "))
                if 1 <= action_choice <= len(actions):
                    action = actions[action_choice - 1]
                    break
                print(f"Invalid choice. Please enter a number between 1 and {len(actions)}.")
            except ValueError:
                print("Invalid input. Please enter a number.")
        
        # Get channel ID
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        
        # Build URL and make request
        url = f"http://{HOST}:{PORT}/GetAlarmTriggerConfig/{channel_id}/{action}"
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print trigger configuration
        trigger_config = root.find('.//ipc:triggerConfig', ns) if ns else root.find('.//triggerConfig')
        if trigger_config is not None:
            print(f"\nTrigger Configuration for {action} (Channel {channel_id}):")
            
            # Snapshot settings
            snap = trigger_config.find('.//ipc:snap', ns) if ns else trigger_config.find('.//snap')
            if snap is not None:
                print("\nSnapshot Settings:")
                items = snap.findall('ipc:item', ns) if ns else snap.findall('item')
                for item in items:
                    ch_id = item.find('.//ipc:channelId', ns) if ns else item.find('.//channelId')
                    switch = item.find('.//ipc:switch', ns) if ns else item.find('.//switch')
                    if ch_id is not None and switch is not None:
                        print(f"  Channel {ch_id.text}: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            
            # Record settings
            record = trigger_config.find('.//ipc:record', ns) if ns else trigger_config.find('.//record')
            if record is not None:
                print("\nRecording Settings:")
                items = record.findall('ipc:item', ns) if ns else record.findall('item')
                for item in items:
                    ch_id = item.find('.//ipc:channelId', ns) if ns else item.find('.//channelId')
                    switch = item.find('.//ipc:switch', ns) if ns else item.find('.//switch')
                    if ch_id is not None and switch is not None:
                        print(f"  Channel {ch_id.text}: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            
            # Alarm output settings
            trigger_alarm = trigger_config.find('.//ipc:triggerAlarmOut', ns) if ns else trigger_config.find('.//triggerAlarmOut')
            if trigger_alarm is not None:
                print("\nAlarm Output Settings:")
                alarm_list = trigger_alarm.find('.//ipc:alarmOutList', ns) if ns else trigger_alarm.find('.//alarmOutList')
                if alarm_list is not None:
                    items = alarm_list.findall('ipc:item', ns) if ns else alarm_list.findall('item')
                    for item in items:
                        alarm_id = item.find('.//ipc:alarmOutId', ns) if ns else item.find('.//alarmOutId')
                        if alarm_id is not None:
                            print(f"  Alarm Output {alarm_id.text}")
            
            # Audio settings
            audio = trigger_config.find('.//ipc:audio', ns) if ns else trigger_config.find('.//audio')
            if audio is not None:
                print("\nAudio Settings:")
                items = audio.findall('ipc:item', ns) if ns else audio.findall('item')
                for item in items:
                    switch = item.find('.//ipc:switch', ns) if ns else item.find('.//switch')
                    if switch is not None:
                        print(f"  Audio: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            
            # White light settings
            white_light = trigger_config.find('.//ipc:whiteLight', ns) if ns else trigger_config.find('.//whiteLight')
            if white_light is not None:
                print("\nWhite Light Settings:")
                items = white_light.findall('ipc:item', ns) if ns else white_light.findall('item')
                for item in items:
                    switch = item.find('.//ipc:switch', ns) if ns else item.find('.//switch')
                    if switch is not None:
                        print(f"  White Light: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
        else:
            print("No trigger configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_net_basic_config():
    print("\n--- Basic Network Configuration ---")
    try:
        response = requests.post(url_net_basic, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable IP Setting Modes:")
            ip_modes = types.findall('.//ipc:ipSettingMode/ipc:enum', ns) if ns else types.findall('.//ipSettingMode/enum')
            for mode in ip_modes:
                print(f"  - {mode.text}")

        # Print TCP/IP configuration
        tcp_ip = root.find('.//ipc:tcpIp', ns) if ns else root.find('.//tcpIp')
        if tcp_ip is not None:
            print("\nTCP/IP Configuration:")
            
            # IP Setting Mode
            ip_mode = tcp_ip.find('.//ipc:ipSettingMode', ns) if ns else tcp_ip.find('.//ipSettingMode')
            if ip_mode is not None:
                print(f"  IP Setting Mode: {ip_mode.text}")
            
            # Static IP settings
            static_ip = tcp_ip.find('.//ipc:staticIp', ns) if ns else tcp_ip.find('.//staticIp')
            if static_ip is not None:
                print(f"  Static IP: {static_ip.text}")
            
            static_route = tcp_ip.find('.//ipc:staticIpRoute', ns) if ns else tcp_ip.find('.//staticIpRoute')
            if static_route is not None:
                print(f"  Default Gateway: {static_route.text}")
            
            static_mask = tcp_ip.find('.//ipc:staticIpMask', ns) if ns else tcp_ip.find('.//staticIpMask')
            if static_mask is not None:
                print(f"  Subnet Mask: {static_mask.text}")
            
            # DNS settings
            dns_dhcp = tcp_ip.find('.//ipc:dnsFromDhcpSwitch', ns) if ns else tcp_ip.find('.//dnsFromDhcpSwitch')
            if dns_dhcp is not None:
                print(f"  DNS from DHCP: {'Enabled' if dns_dhcp.text.lower() == 'true' else 'Disabled'}")
            
            dns1 = tcp_ip.find('.//ipc:dnsServer1', ns) if ns else tcp_ip.find('.//dnsServer1')
            if dns1 is not None:
                print(f"  Primary DNS: {dns1.text}")
            
            dns2 = tcp_ip.find('.//ipc:dnsServer2', ns) if ns else tcp_ip.find('.//dnsServer2')
            if dns2 is not None:
                print(f"  Secondary DNS: {dns2.text}")
        else:
            print("No network configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_reboot():
    print("\n--- Device Reboot ---")
    try:
        # Confirm with user
        confirm = input("Are you sure you want to reboot the device? (yes/no): ").strip().lower()
        if confirm != 'yes':
            print("Reboot cancelled.")
            return

        print("Sending reboot command...")
        response = requests.post(url_reboot, headers=headers)
        response.raise_for_status()
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Check for result
        result = root.find('.//ipc:result', ns) if ns else root.find('.//result')
        if result is not None:
            print(f"\nReboot command result: {result.text}")
            if result.text.lower() == 'success':
                print("Device is rebooting. Please wait a few minutes before attempting to reconnect.")
        else:
            print("No result found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_vfd_config():
    print("\n--- Video Face Detection Configuration ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/GetSmartVfdConfig/{channel_id}" if channel_id else url_vfd
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Types:")
            
            # Mutex Object Types
            mutex_types = types.findall('.//ipc:mutexObjectType/ipc:enum', ns) if ns else types.findall('.//mutexObjectType/enum')
            if mutex_types:
                print("\nMutex Object Types:")
                for mt in mutex_types:
                    print(f"  - {mt.text}")
            
            # Detect Mode Types
            detect_modes = types.findall('.//ipc:detectModeType/ipc:enum', ns) if ns else types.findall('.//detectModeType/enum')
            if detect_modes:
                print("\nDetect Mode Types:")
                for dm in detect_modes:
                    print(f"  - {dm.text}")
            
            # Alarm List Types
            alarm_lists = types.findall('.//ipc:alarmListType/ipc:enum', ns) if ns else types.findall('.//alarmListType/enum')
            if alarm_lists:
                print("\nAlarm List Types:")
                for al in alarm_lists:
                    print(f"  - {al.text}")
            
            # Alarm Mode Types
            alarm_modes = types.findall('.//ipc:alarmModeType/ipc:enum', ns) if ns else types.findall('.//alarmModeType/enum')
            if alarm_modes:
                print("\nAlarm Mode Types:")
                for am in alarm_modes:
                    print(f"  - {am.text}")
            
            # Scene Mode Types
            scene_modes = types.findall('.//ipc:senceModeType/ipc:enum', ns) if ns else types.findall('.//senceModeType/enum')
            if scene_modes:
                print("\nScene Mode Types:")
                for sm in scene_modes:
                    print(f"  - {sm.text}")

        # Print VFD configuration
        vfd = root.find('.//ipc:vfd', ns) if ns else root.find('.//vfd')
        if vfd is not None:
            print("\nVFD Configuration:")
            
            # Basic settings
            function_status = vfd.find('.//ipc:functionStatus', ns) if ns else vfd.find('.//functionStatus')
            switch = vfd.find('.//ipc:switch', ns) if ns else vfd.find('.//switch')
            if function_status is not None:
                print(f"\nFunction Status: {function_status.text}")
            if switch is not None:
                print(f"VFD Enabled: {switch.text}")
            
            # Mutex List
            mutex_list = vfd.find('.//ipc:mutexList', ns) if ns else vfd.find('.//mutexList')
            if mutex_list is not None:
                count = mutex_list.get('count', 'unknown')
                print(f"\nMutex List (count={count}):")
                items = mutex_list.findall('ipc:item', ns) if ns else mutex_list.findall('item')
                for item in items:
                    obj = item.find('.//ipc:object', ns) if ns else item.find('.//object')
                    status = item.find('.//ipc:status', ns) if ns else item.find('.//status')
                    if obj is not None and status is not None:
                        print(f"  {obj.text}: {'Enabled' if status.text.lower() == 'true' else 'Disabled'}")
            
            # Detect Mode
            detect_mode = vfd.find('.//ipc:detectMode', ns) if ns else vfd.find('.//detectMode')
            if detect_mode is not None:
                print("\nDetect Mode Settings:")
                mode = detect_mode.find('.//ipc:mode', ns) if ns else detect_mode.find('.//mode')
                interval = detect_mode.find('.//ipc:intervalTime', ns) if ns else detect_mode.find('.//intervalTime')
                cycle = detect_mode.find('.//ipc:captureCycle', ns) if ns else detect_mode.find('.//captureCycle')
                if mode is not None:
                    print(f"  Mode: {mode.text}")
                if interval is not None:
                    min_val = interval.get('min', 'N/A')
                    max_val = interval.get('max', 'N/A')
                    default = interval.get('default', 'N/A')
                    print(f"  Interval Time: {interval.text}ms (Range: {min_val}-{max_val}, Default: {default})")
                if cycle is not None:
                    min_val = cycle.get('min', 'N/A')
                    max_val = cycle.get('max', 'N/A')
                    default = cycle.get('default', 'N/A')
                    print(f"  Capture Cycle: {cycle.text} (Range: {min_val}-{max_val}, Default: {default})")
            
            # Alarm Settings
            alarm_hold = vfd.find('.//ipc:alarmHoldTime', ns) if ns else vfd.find('.//alarmHoldTime')
            save_face = vfd.find('.//ipc:saveFacePicture', ns) if ns else vfd.find('.//saveFacePicture')
            save_source = vfd.find('.//ipc:saveSourcePicture', ns) if ns else vfd.find('.//saveSourcePicture')
            if alarm_hold is not None:
                print(f"\nAlarm Hold Time: {alarm_hold.text} seconds")
            if save_face is not None:
                print(f"Save Face Picture: {save_face.text}")
            if save_source is not None:
                print(f"Save Source Picture: {save_source.text}")
            
            # Region Info
            region_info = vfd.find('.//ipc:regionInfo', ns) if ns else vfd.find('.//regionInfo')
            if region_info is not None:
                count = region_info.get('count', 'unknown')
                max_count = region_info.get('maxCount', 'unknown')
                print(f"\nDetection Region (count={count}, max={max_count}):")
                items = region_info.findall('ipc:item', ns) if ns else region_info.findall('item')
                for item in items:
                    x1 = item.find('.//ipc:X1', ns) if ns else item.find('.//X1')
                    y1 = item.find('.//ipc:Y1', ns) if ns else item.find('.//Y1')
                    x2 = item.find('.//ipc:X2', ns) if ns else item.find('.//X2')
                    y2 = item.find('.//ipc:Y2', ns) if ns else item.find('.//Y2')
                    if all([x1, y1, x2, y2]):
                        print(f"  Rectangle: ({x1.text}, {y1.text}) to ({x2.text}, {y2.text})")
                        print("  Note: Coordinates represent diagonal points of the rectangle")
            
            # Face Frame Settings
            max_frame = vfd.find('.//ipc:maxFaceFrame', ns) if ns else vfd.find('.//maxFaceFrame')
            min_frame = vfd.find('.//ipc:minFaceFrame', ns) if ns else vfd.find('.//minFaceFrame')
            if max_frame is not None:
                print(f"\nMaximum Face Frame: {max_frame.text}")
            if min_frame is not None:
                print(f"Minimum Face Frame: {min_frame.text}")
            
            # Face Match Settings
            face_match = vfd.find('.//ipc:faceMatch', ns) if ns else vfd.find('.//faceMatch')
            if face_match is not None:
                print("\nFace Match Settings:")
                
                # Push Mode
                push_mode = face_match.find('.//ipc:pushMode', ns) if ns else face_match.find('.//pushMode')
                if push_mode is not None:
                    mode = push_mode.find('.//ipc:mode', ns) if ns else push_mode.find('.//mode')
                    interval = push_mode.find('.//ipc:intervalTime', ns) if ns else push_mode.find('.//intervalTime')
                    if mode is not None:
                        print(f"  Push Mode: {mode.text}")
                    if interval is not None:
                        min_val = interval.get('min', 'N/A')
                        max_val = interval.get('max', 'N/A')
                        default = interval.get('default', 'N/A')
                        print(f"  Interval Time: {interval.text} (Range: {min_val}-{max_val}, Default: {default})")
                
                # Similarity and Alarm Settings
                similarity = face_match.find('.//ipc:similarityThreshold', ns) if ns else face_match.find('.//similarityThreshold')
                alarm_mode = face_match.find('.//ipc:alarmMode', ns) if ns else face_match.find('.//alarmMode')
                alarm_list = face_match.find('.//ipc:alarmList', ns) if ns else face_match.find('.//alarmList')
                if similarity is not None:
                    min_val = similarity.get('min', 'N/A')
                    max_val = similarity.get('max', 'N/A')
                    default = similarity.get('default', 'N/A')
                    print(f"  Similarity Threshold: {similarity.text}% (Range: {min_val}-{max_val}, Default: {default})")
                if alarm_mode is not None:
                    print(f"  Alarm Mode: {alarm_mode.text}")
                if alarm_list is not None:
                    print(f"  Alarm List: {alarm_list.text}")
                
                # Alarm Output Triggers
                trigger_alarm = face_match.find('.//ipc:triggerAlarmOut', ns) if ns else face_match.find('.//triggerAlarmOut')
                if trigger_alarm is not None:
                    io = trigger_alarm.find('.//ipc:Io', ns) if ns else trigger_alarm.find('.//Io')
                    if io is not None:
                        count = io.get('count', 'unknown')
                        max_count = io.get('maxCount', 'unknown')
                        print(f"\nAlarm Output Triggers (count={count}, max={max_count}):")
                        items = io.findall('ipc:item', ns) if ns else io.findall('item')
                        for item in items:
                            alarm_id = item.find('.//ipc:alarmId', ns) if ns else item.find('.//alarmId')
                            switch = item.find('.//ipc:switch', ns) if ns else item.find('.//switch')
                            if alarm_id is not None and switch is not None:
                                print(f"  Alarm Output {alarm_id.text}: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            
            # Face Exposure Settings
            face_exp = vfd.find('.//ipc:faceExp', ns) if ns else vfd.find('.//faceExp')
            if face_exp is not None:
                print("\nFace Exposure Settings:")
                switch = face_exp.find('.//ipc:switch', ns) if ns else face_exp.find('.//switch')
                strength = face_exp.find('.//ipc:faceExpStrength', ns) if ns else face_exp.find('.//faceExpStrength')
                if switch is not None:
                    print(f"  Enabled: {switch.text}")
                if strength is not None:
                    min_val = strength.get('min', 'N/A')
                    max_val = strength.get('max', 'N/A')
                    default = strength.get('default', 'N/A')
                    print(f"  Strength: {strength.text} (Range: {min_val}-{max_val}, Default: {default})")
            
            # Scene Mode Settings
            scene_mode = vfd.find('.//ipc:senceMode', ns) if ns else vfd.find('.//senceMode')
            if scene_mode is not None:
                print("\nScene Mode Settings:")
                mode = scene_mode.find('.//ipc:mode', ns) if ns else scene_mode.find('.//mode')
                spare_time = scene_mode.find('.//ipc:spareTimeMatch', ns) if ns else scene_mode.find('.//spareTimeMatch')
                near_priority = scene_mode.find('.//ipc:nearPriority', ns) if ns else scene_mode.find('.//nearPriority')
                if mode is not None:
                    print(f"  Mode: {mode.text}")
                if spare_time is not None:
                    print(f"  Spare Time Match: {spare_time.text}")
                if near_priority is not None:
                    print(f"  Near Priority: {near_priority.text}")
            
            # Scene Mode Info
            scene_info = vfd.find('.//ipc:senceModeInfo', ns) if ns else vfd.find('.//senceModeInfo')
            if scene_info is not None:
                print("\nScene Mode Information:")
                
                # Access Control Mode
                access_control = scene_info.find('.//ipc:accessControlMode', ns) if ns else scene_info.find('.//accessControlMode')
                if access_control is not None:
                    print("\nAccess Control Mode:")
                    interval = access_control.find('.//ipc:intervalTime', ns) if ns else access_control.find('.//intervalTime')
                    cycle = access_control.find('.//ipc:captureCycle', ns) if ns else access_control.find('.//captureCycle')
                    spare_time = access_control.find('.//ipc:spareTimeMatch', ns) if ns else access_control.find('.//spareTimeMatch')
                    near_priority = access_control.find('.//ipc:nearPriority', ns) if ns else access_control.find('.//nearPriority')
                    if interval is not None:
                        print(f"  Interval Time: {interval.text}ms")
                    if cycle is not None:
                        print(f"  Capture Cycle: {cycle.text}")
                    if spare_time is not None:
                        print(f"  Spare Time Match: {spare_time.text}")
                    if near_priority is not None:
                        print(f"  Near Priority: {near_priority.text}")
                
                # Security Monitor Mode
                security_monitor = scene_info.find('.//ipc:securityMonitorMode', ns) if ns else scene_info.find('.//securityMonitorMode')
                if security_monitor is not None:
                    print("\nSecurity Monitor Mode:")
                    interval = security_monitor.find('.//ipc:intervalTime', ns) if ns else security_monitor.find('.//intervalTime')
                    cycle = security_monitor.find('.//ipc:captureCycle', ns) if ns else security_monitor.find('.//captureCycle')
                    spare_time = security_monitor.find('.//ipc:spareTimeMatch', ns) if ns else security_monitor.find('.//spareTimeMatch')
                    near_priority = security_monitor.find('.//ipc:nearPriority', ns) if ns else security_monitor.find('.//nearPriority')
                    if interval is not None:
                        print(f"  Interval Time: {interval.text}ms")
                    if cycle is not None:
                        print(f"  Capture Cycle: {cycle.text}")
                    if spare_time is not None:
                        print(f"  Spare Time Match: {spare_time.text}")
                    if near_priority is not None:
                        print(f"  Near Priority: {near_priority.text}")
        else:
            print("No VFD configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_perimeter_config():
    print("\n--- Perimeter Configuration ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        url = f"http://{HOST}:{PORT}/GetSmartPerimeterConfig/{channel_id}" if channel_id else url_perimeter
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print perimeter configuration
        perimeter = root.find('.//ipc:perimeter', ns) if ns else root.find('.//perimeter')
        if perimeter is not None:
            print("\nPerimeter Configuration:")
            
            # Basic settings
            switch = perimeter.find('.//ipc:switch', ns) if ns else perimeter.find('.//switch')
            alarm_hold = perimeter.find('.//ipc:alarmHoldTime', ns) if ns else perimeter.find('.//alarmHoldTime')
            if switch is not None:
                print(f"Perimeter Detection: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            if alarm_hold is not None:
                print(f"Alarm Hold Time: {alarm_hold.text} seconds")
            
            # Object Filter settings
            object_filter = perimeter.find('.//ipc:objectFilter', ns) if ns else perimeter.find('.//objectFilter')
            if object_filter is not None:
                print("\nObject Filter Settings:")
                
                # Car settings
                car = object_filter.find('.//ipc:car', ns) if ns else object_filter.find('.//car')
                if car is not None:
                    print("\nCar Detection:")
                    car_switch = car.find('.//ipc:switch', ns) if ns else car.find('.//switch')
                    car_sensitivity = car.find('.//ipc:sensitivity', ns) if ns else car.find('.//sensitivity')
                    if car_switch is not None:
                        print(f"  Enabled: {car_switch.text}")
                    if car_sensitivity is not None:
                        min_val = car_sensitivity.get('min', 'N/A')
                        max_val = car_sensitivity.get('max', 'N/A')
                        default = car_sensitivity.get('default', 'N/A')
                        print(f"  Sensitivity: {car_sensitivity.text} (Range: {min_val}-{max_val}, Default: {default})")
                
                # Person settings
                person = object_filter.find('.//ipc:person', ns) if ns else object_filter.find('.//person')
                if person is not None:
                    print("\nPerson Detection:")
                    person_switch = person.find('.//ipc:switch', ns) if ns else person.find('.//switch')
                    person_sensitivity = person.find('.//ipc:sensitivity', ns) if ns else person.find('.//sensitivity')
                    if person_switch is not None:
                        print(f"  Enabled: {person_switch.text}")
                    if person_sensitivity is not None:
                        min_val = person_sensitivity.get('min', 'N/A')
                        max_val = person_sensitivity.get('max', 'N/A')
                        default = person_sensitivity.get('default', 'N/A')
                        print(f"  Sensitivity: {person_sensitivity.text} (Range: {min_val}-{max_val}, Default: {default})")
                
                # Motor settings
                motor = object_filter.find('.//ipc:motor', ns) if ns else object_filter.find('.//motor')
                if motor is not None:
                    print("\nMotor Detection:")
                    motor_switch = motor.find('.//ipc:switch', ns) if ns else motor.find('.//switch')
                    motor_sensitivity = motor.find('.//ipc:sensitivity', ns) if ns else motor.find('.//sensitivity')
                    if motor_switch is not None:
                        print(f"  Enabled: {motor_switch.text}")
                    if motor_sensitivity is not None:
                        min_val = motor_sensitivity.get('min', 'N/A')
                        max_val = motor_sensitivity.get('max', 'N/A')
                        default = motor_sensitivity.get('default', 'N/A')
                        print(f"  Sensitivity: {motor_sensitivity.text} (Range: {min_val}-{max_val}, Default: {default})")
            
            # Target Frame Settings
            max_frame = perimeter.find('.//ipc:maxTargetFrame', ns) if ns else perimeter.find('.//maxTargetFrame')
            min_frame = perimeter.find('.//ipc:minTargetFrame', ns) if ns else perimeter.find('.//minTargetFrame')
            if max_frame is not None:
                print(f"\nMaximum Target Frame: {max_frame.text}")
            if min_frame is not None:
                print(f"Minimum Target Frame: {min_frame.text}")
            
            # Picture Save Settings
            save_target = perimeter.find('.//ipc:saveTargetPicture', ns) if ns else perimeter.find('.//saveTargetPicture')
            save_source = perimeter.find('.//ipc:saveSourcePicture', ns) if ns else perimeter.find('.//saveSourcePicture')
            if save_target is not None:
                print(f"Save Target Picture: {save_target.text}")
            if save_source is not None:
                print(f"Save Source Picture: {save_source.text}")
            
            # Region Info
            region_info = perimeter.find('.//ipc:regionInfo', ns) if ns else perimeter.find('.//regionInfo')
            if region_info is not None:
                count = region_info.get('count', 'unknown')
                max_count = region_info.get('maxCount', 'unknown')
                print(f"\nDetection Regions (count={count}, max={max_count}):")
                items = region_info.findall('ipc:item', ns) if ns else region_info.findall('item')
                for i, item in enumerate(items, 1):
                    point_group = item.find('.//ipc:pointGroup', ns) if ns else item.find('.//pointGroup')
                    if point_group is not None:
                        group_count = point_group.get('count', 'unknown')
                        group_max = point_group.get('maxCount', 'unknown')
                        print(f"\n  Region {i} (points={group_count}, max={group_max}):")
                        points = point_group.findall('ipc:item', ns) if ns else point_group.findall('item')
                        for j, point in enumerate(points, 1):
                            x = point.find('.//ipc:X', ns) if ns else point.find('.//X')
                            y = point.find('.//ipc:Y', ns) if ns else point.find('.//Y')
                            if x is not None and y is not None:
                                print(f"    Point {j}: ({x.text}, {y.text})")
        else:
            print("No perimeter configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_vehicle_config():
    print("\n--- Vehicle Configuration ---")
    try:
        response = requests.post(url_vehicle, headers=headers)
        response.raise_for_status()
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}

        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Types:")
            
            # Detect Mode Types
            detect_modes = types.findall('.//ipc:detectModeType/ipc:enum', ns) if ns else types.findall('.//detectModeType/enum')
            if detect_modes:
                print("\nDetect Mode Types:")
                for mode in detect_modes:
                    print(f"  - {mode.text}")
            
            # Mutex Object Types
            mutex_types = types.findall('.//ipc:mutexObjectType/ipc:enum', ns) if ns else types.findall('.//mutexObjectType/enum')
            if mutex_types:
                print("\nMutex Object Types:")
                for mt in mutex_types:
                    print(f"  - {mt.text}")
            
            # Plate Area Types
            plate_areas = types.findall('.//ipc:plateAreaType/ipc:enum', ns) if ns else types.findall('.//plateAreaType/enum')
            if plate_areas:
                print("\nPlate Area Types:")
                for pa in plate_areas:
                    print(f"  - {pa.text}")
            
            # Alarm List Types
            alarm_lists = types.findall('.//ipc:alarmListType/ipc:enum', ns) if ns else types.findall('.//alarmListType/enum')
            if alarm_lists:
                print("\nAlarm List Types:")
                for al in alarm_lists:
                    print(f"  - {al.text}")

        # Print vehicle configuration
        vehicle = root.find('.//ipc:vehicle', ns) if ns else root.find('.//vehicle')
        if vehicle is not None:
            print("\nVehicle Configuration:")
            
            # Basic settings
            switch = vehicle.find('.//ipc:switch', ns) if ns else vehicle.find('.//switch')
            plate_sensitivity = vehicle.find('.//ipc:plateSencitivity', ns) if ns else vehicle.find('.//plateSencitivity')
            plate_area = vehicle.find('.//ipc:plateSupportArea', ns) if ns else vehicle.find('.//plateSupportArea')
            fault_tolerance = vehicle.find('.//ipc:faultTolerance', ns) if ns else vehicle.find('.//faultTolerance')
            
            if switch is not None:
                print(f"Vehicle Detection: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            if plate_sensitivity is not None:
                print(f"Plate Sensitivity: {plate_sensitivity.text}")
            if plate_area is not None:
                print(f"Plate Support Area: {plate_area.text}")
            if fault_tolerance is not None:
                print(f"Fault Tolerance: {fault_tolerance.text}")
            
            # Mutex List
            mutex_list = vehicle.find('.//ipc:mutexList', ns) if ns else vehicle.find('.//mutexList')
            if mutex_list is not None:
                count = mutex_list.get('count', 'unknown')
                print(f"\nMutex List (count={count}):")
                items = mutex_list.findall('ipc:item', ns) if ns else mutex_list.findall('item')
                for item in items:
                    obj = item.find('.//ipc:object', ns) if ns else item.find('.//object')
                    status = item.find('.//ipc:status', ns) if ns else item.find('.//status')
                    if obj is not None and status is not None:
                        print(f"  {obj.text}: {'Enabled' if status.text.lower() == 'true' else 'Disabled'}")
            
            # Region Info
            region_info = vehicle.find('.//ipc:regionInfo', ns) if ns else vehicle.find('.//regionInfo')
            if region_info is not None:
                count = region_info.get('count', 'unknown')
                max_count = region_info.get('maxCount', 'unknown')
                print(f"\nDetection Regions (count={count}, max={max_count}):")
                items = region_info.findall('ipc:item', ns) if ns else region_info.findall('item')
                for i, item in enumerate(items, 1):
                    x1 = item.find('.//ipc:X1', ns) if ns else item.find('.//X1')
                    y1 = item.find('.//ipc:Y1', ns) if ns else item.find('.//Y1')
                    x2 = item.find('.//ipc:X2', ns) if ns else item.find('.//X2')
                    y2 = item.find('.//ipc:Y2', ns) if ns else item.find('.//Y2')
                    if all([x1, y1, x2, y2]):
                        print(f"  Region {i}: ({x1.text}, {y1.text}) to ({x2.text}, {y2.text})")
            
            # Plate Size
            plate_size = vehicle.find('.//ipc:plateSize', ns) if ns else vehicle.find('.//plateSize')
            if plate_size is not None:
                print("\nPlate Size Settings:")
                items = plate_size.findall('ipc:item', ns) if ns else plate_size.findall('item')
                for item in items:
                    min_width = item.find('.//ipc:MinWidth', ns) if ns else item.find('.//MinWidth')
                    min_height = item.find('.//ipc:MinHeight', ns) if ns else item.find('.//MinHeight')
                    max_width = item.find('.//ipc:MaxWidth', ns) if ns else item.find('.//MaxWidth')
                    max_height = item.find('.//ipc:MaxHeight', ns) if ns else item.find('.//MaxHeight')
                    if all([min_width, min_height, max_width, max_height]):
                        print(f"  Minimum Size: {min_width.text}x{min_height.text}")
                        print(f"  Maximum Size: {max_width.text}x{max_height.text}")
            
            # Plate Match Settings
            plate_match = vehicle.find('.//ipc:plateMatch', ns) if ns else vehicle.find('.//plateMatch')
            if plate_match is not None:
                print("\nPlate Match Settings:")
                items = plate_match.findall('ipc:item', ns) if ns else plate_match.findall('item')
                for i, item in enumerate(items, 1):
                    alarm_list = item.find('.//ipc:alarmList', ns) if ns else item.find('.//alarmList')
                    trigger_alarm = item.find('.//ipc:triggerAlarmOut', ns) if ns else item.find('.//triggerAlarmOut')
                    if alarm_list is not None:
                        print(f"\n  Match {i}:")
                        print(f"    Alarm List: {alarm_list.text}")
                        if trigger_alarm is not None:
                            io = trigger_alarm.find('.//ipc:Io', ns) if ns else trigger_alarm.find('.//Io')
                            if io is not None:
                                items = io.findall('ipc:item', ns) if ns else io.findall('item')
                                for alarm in items:
                                    alarm_id = alarm.find('.//ipc:alarmId', ns) if ns else alarm.find('.//alarmId')
                                    switch = alarm.find('.//ipc:switch', ns) if ns else alarm.find('.//switch')
                                    if alarm_id is not None and switch is not None:
                                        print(f"    Alarm Output {alarm_id.text}: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
            
            # Trigger Configuration
            trigger_config = vehicle.find('.//ipc:triggerConfig', ns) if ns else vehicle.find('.//triggerConfig')
            if trigger_config is not None:
                print("\nTrigger Configuration:")
                
                # Basic settings
                alarm_hold = trigger_config.find('.//ipc:alarmHoldTime', ns) if ns else trigger_config.find('.//alarmHoldTime')
                sd_snap = trigger_config.find('.//ipc:sdSnapSwitch', ns) if ns else trigger_config.find('.//sdSnapSwitch')
                sd_rec = trigger_config.find('.//ipc:sdRecSwitch', ns) if ns else trigger_config.find('.//sdRecSwitch')
                
                if alarm_hold is not None:
                    print(f"  Alarm Hold Time: {alarm_hold.text} seconds")
                if sd_snap is not None:
                    print(f"  SD Snapshot: {'Enabled' if sd_snap.text.lower() == 'true' else 'Disabled'}")
                if sd_rec is not None:
                    print(f"  SD Recording: {'Enabled' if sd_rec.text.lower() == 'true' else 'Disabled'}")
                
                # Alarm Output settings
                trigger_alarm = trigger_config.find('.//ipc:triggerAlarmOut', ns) if ns else trigger_config.find('.//triggerAlarmOut')
                if trigger_alarm is not None:
                    alarm_list = trigger_alarm.find('.//ipc:alarmOutList', ns) if ns else trigger_alarm.find('.//alarmOutList')
                    if alarm_list is not None:
                        items = alarm_list.findall('ipc:item', ns) if ns else alarm_list.findall('item')
                        for item in items:
                            alarm_id = item.find('.//ipc:alarmOutId', ns) if ns else item.find('.//alarmOutId')
                            alarm_switch = item.find('.//ipc:alarmSwitch', ns) if ns else item.find('.//alarmSwitch')
                            if alarm_id is not None and alarm_switch is not None:
                                print(f"  Alarm Output {alarm_id.text}: {'Enabled' if alarm_switch.text.lower() == 'true' else 'Disabled'}")
                
                # Email settings
                trigger_mail = trigger_config.find('.//ipc:triggerMail', ns) if ns else trigger_config.find('.//triggerMail')
                if trigger_mail is not None:
                    print("\nEmail Settings:")
                    switch = trigger_mail.find('.//ipc:switch', ns) if ns else trigger_mail.find('.//switch')
                    subject = trigger_mail.find('.//ipc:subject', ns) if ns else trigger_mail.find('.//subject')
                    content = trigger_mail.find('.//ipc:content', ns) if ns else trigger_mail.find('.//content')
                    recv_list = trigger_mail.find('.//ipc:recvList', ns) if ns else trigger_mail.find('.//recvList')
                    
                    if switch is not None:
                        print(f"  Email Alerts: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
                    if subject is not None:
                        print(f"  Subject: {subject.text}")
                    if content is not None:
                        print(f"  Content: {content.text}")
                    if recv_list is not None:
                        count = recv_list.get('count', '0')
                        print(f"  Recipients: {count}")
                
                # FTP settings
                trigger_ftp = trigger_config.find('.//ipc:triggerFtp', ns) if ns else trigger_config.find('.//triggerFtp')
                if trigger_ftp is not None:
                    print("\nFTP Settings:")
                    switch = trigger_ftp.find('.//ipc:switch', ns) if ns else trigger_ftp.find('.//switch')
                    server_list = trigger_ftp.find('.//ipc:ftpServerList', ns) if ns else trigger_ftp.find('.//ftpServerList')
                    
                    if switch is not None:
                        print(f"  FTP Upload: {'Enabled' if switch.text.lower() == 'true' else 'Disabled'}")
                    if server_list is not None:
                        count = server_list.get('count', '0')
                        print(f"  FTP Servers: {count}")
        else:
            print("No vehicle configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_vehicle_plate():
    print("\n--- Vehicle Plate Information ---")
    try:
        # First test authentication with GetDeviceInfo
        print("Testing authentication...")
        test_response = requests.post(url_device, headers=headers)
        test_response.raise_for_status()
        print("Authentication successful!")
        
        # Get channel ID
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        
        # Get search parameters
        print("\nSearch Parameters:")
        page_index = input("Enter page index (default 1): ").strip() or "1"
        page_size = input("Enter page size (default 1): ").strip() or "1"
        list_type = input("Enter list type (default 0): ").strip() or "0"
        plate_number = input("Enter plate number to search (leave empty for all): ").strip()
        
        # Build XML request body with the correct format
        # For empty plate number search, we'll omit the carPlateNum element entirely
        xml_body = f'''<?xml version="1.0" encoding="utf-8"?>
<config xmlns="http://www.ipc.com/ver10" version="1.7">
<vehiclePlates type="list" maxCount="10000" count="1">
<searchFilter>
<pageIndex type="uint32">{page_index}</pageIndex>
<pageSize type="uint32">{page_size}</pageSize>
<listType type="uint32">{list_type}</listType>
{'' if not plate_number else f'<carPlateNum type="string"><![CDATA[{plate_number}]]></carPlateNum>'}
</searchFilter>
</vehiclePlates>
</config>'''
        
        # Make request with retry logic for authentication
        url = f"http://{HOST}:{PORT}/GetVehiclePlate/{channel_id}" if channel_id else url_vehicle_plate
        max_retries = 3
        for attempt in range(max_retries):
            try:
                print(f"\nMaking request to {url}")
                print("Request headers:")
                for key, value in headers.items():
                    print(f"{key}: {value}")
                print("\nRequest body:")
                print(xml_body)
                
                # Add a small delay between retries
                if attempt > 0:
                    time.sleep(1)
                
                response = requests.post(url, headers=headers, data=xml_body.encode('utf-8'))
                
                print(f"\nResponse status code: {response.status_code}")
                print("Response headers:")
                for key, value in response.headers.items():
                    print(f"{key}: {value}")
                print("\nResponse content:")
                print(response.content.decode(errors='replace'))
                
                # Parse response to check for authentication error
                root = ET.fromstring(response.content)
                status = root.get('status')
                error_code = root.get('errorCode')
                
                if status == 'failed':
                    if error_code == '499':
                        if attempt < max_retries - 1:
                            print(f"\nAuthentication error detected. Retry attempt {attempt + 1} of {max_retries}...")
                            # Try to refresh authentication
                            print("Refreshing authentication...")
                            refresh_response = requests.post(url_device, headers=headers)
                            refresh_response.raise_for_status()
                            print("Authentication refreshed successfully.")
                            continue
                        else:
                            print("\nMaximum retry attempts reached. Authentication failed.")
                            print("Please check your username and password.")
                            print("Note: Initial authentication succeeded but subsequent request failed.")
                            print("This might indicate a session timeout or permission issue.")
                            return
                    else:
                        print(f"\nRequest failed with error code: {error_code}")
                        print("This might indicate an issue with the request format or parameters.")
                        return
                
                response.raise_for_status()
                break
            except requests.exceptions.RequestException as e:
                if attempt < max_retries - 1:
                    print(f"\nRequest failed. Retry attempt {attempt + 1} of {max_retries}...")
                    print(f"Error details: {str(e)}")
                    continue
                else:
                    raise
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Get vehicle plates list
        vehicle_plates = root.find('.//ipc:vehiclePlates', ns) if ns else root.find('.//vehiclePlates')
        if vehicle_plates is not None:
            count = vehicle_plates.get('count', '0')
            max_count = vehicle_plates.get('maxCount', 'unknown')
            print(f"\nVehicle Plates (count={count}, max={max_count}):")
            
            # Process each plate
            items = vehicle_plates.findall('ipc:item', ns) if ns else vehicle_plates.findall('item')
            for i, item in enumerate(items, 1):
                print(f"\nPlate {i}:")
                
                # Get plate details
                key_id = item.find('.//ipc:keyId', ns) if ns else item.find('.//keyId')
                plate_number = item.find('.//ipc:carPlateNumber', ns) if ns else item.find('.//carPlateNumber')
                begin_time = item.find('.//ipc:beginTime', ns) if ns else item.find('.//beginTime')
                end_time = item.find('.//ipc:endTime', ns) if ns else item.find('.//endTime')
                plate_color = item.find('.//ipc:carPlateColor', ns) if ns else item.find('.//carPlateColor')
                plate_type = item.find('.//ipc:carPlateType', ns) if ns else item.find('.//carPlateType')
                car_type = item.find('.//ipc:carType', ns) if ns else item.find('.//carType')
                car_owner = item.find('.//ipc:carOwner', ns) if ns else item.find('.//carOwner')
                car_color = item.find('.//ipc:carColor', ns) if ns else item.find('.//carColor')
                plate_item_type = item.find('.//ipc:plateItemType', ns) if ns else item.find('.//plateItemType')
                
                if key_id is not None:
                    print(f"  Key ID: {key_id.text}")
                if plate_number is not None:
                    print(f"  Plate Number: {plate_number.text}")
                if begin_time is not None:
                    print(f"  Begin Time: {begin_time.text}")
                if end_time is not None:
                    print(f"  End Time: {end_time.text}")
                if plate_color is not None:
                    print(f"  Plate Color: {plate_color.text}")
                if plate_type is not None:
                    print(f"  Plate Type: {plate_type.text}")
                if car_type is not None:
                    print(f"  Car Type: {car_type.text}")
                if car_owner is not None:
                    print(f"  Car Owner: {car_owner.text}")
                if car_color is not None:
                    print(f"  Car Color: {car_color.text}")
                if plate_item_type is not None:
                    print(f"  Plate Item Type: {plate_item_type.text}")
        else:
            print("No vehicle plates found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_cdd_config():
    print("\n--- Crowd Density Detection Configuration ---")
    try:
        # Get channel ID
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        
        # Make request
        url = f"http://{HOST}:{PORT}/GetSmartCddConfig/{channel_id}" if channel_id else url_cdd
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Refresh Frequencies:")
            frequencies = types.findall('.//ipc:refreshFrequency/ipc:enum', ns) if ns else types.findall('.//refreshFrequency/enum')
            for freq in frequencies:
                print(f"  - {freq.text}ms")
        
        # Print CDD configuration
        cdd = root.find('.//ipc:cdd', ns) if ns else root.find('.//cdd')
        if cdd is not None:
            print("\nCDD Configuration:")
            
            # Basic settings
            switch = cdd.find('.//ipc:switch', ns) if ns else cdd.find('.//switch')
            alarm_hold = cdd.find('.//ipc:alarmHoldTime', ns) if ns else cdd.find('.//alarmHoldTime')
            detect_freq = cdd.find('.//ipc:detectFrequency', ns) if ns else cdd.find('.//detectFrequency')
            trigger_level = cdd.find('.//ipc:triggerAlarmLevel', ns) if ns else cdd.find('.//triggerAlarmLevel')
            
            if switch is not None:
                print(f"CDD Enabled: {switch.text}")
            if alarm_hold is not None:
                print(f"Alarm Hold Time: {alarm_hold.text} seconds")
            if detect_freq is not None:
                print(f"Detection Frequency: {detect_freq.text}ms")
            if trigger_level is not None:
                min_val = trigger_level.get('min', 'N/A')
                max_val = trigger_level.get('max', 'N/A')
                print(f"Trigger Alarm Level: {trigger_level.text} (Range: {min_val}-{max_val})")
            
            # Region Info
            region_info = cdd.find('.//ipc:regionInfo', ns) if ns else cdd.find('.//regionInfo')
            if region_info is not None:
                count = region_info.get('count', '0')
                max_count = region_info.get('maxCount', 'unknown')
                print(f"\nDetection Regions (count={count}, max={max_count}):")
                
                items = region_info.findall('ipc:item', ns) if ns else region_info.findall('item')
                for i, item in enumerate(items, 1):
                    x1 = item.find('.//ipc:X1', ns) if ns else item.find('.//X1')
                    y1 = item.find('.//ipc:Y1', ns) if ns else item.find('.//Y1')
                    x2 = item.find('.//ipc:X2', ns) if ns else item.find('.//X2')
                    y2 = item.find('.//ipc:Y2', ns) if ns else item.find('.//Y2')
                    
                    if all([x1, y1, x2, y2]):
                        print(f"\n  Region {i}:")
                        print(f"    Top Left: ({x1.text}, {y1.text})")
                        print(f"    Bottom Right: ({x2.text}, {y2.text})")
                        print("    Note: Coordinates represent diagonal points of the rectangle")
        else:
            print("No CDD configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_cpc_config():
    print("\n--- Cross-line People Counting Configuration ---")
    try:
        # Get channel ID
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        
        # Make request
        url = f"http://{HOST}:{PORT}/GetSmartCpcConfig/{channel_id}" if channel_id else url_cpc
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            print("\nAvailable Statistical Periods:")
            periods = types.findall('.//ipc:statisticalPeriod/ipc:enum', ns) if ns else types.findall('.//statisticalPeriod/enum')
            for period in periods:
                print(f"  - {period.text}")
        
        # Print CPC configuration
        cpc = root.find('.//ipc:cpc', ns) if ns else root.find('.//cpc')
        if cpc is not None:
            print("\nCPC Configuration:")
            
            # Basic settings
            switch = cpc.find('.//ipc:switch', ns) if ns else cpc.find('.//switch')
            alarm_hold = cpc.find('.//ipc:alarmHoldTime', ns) if ns else cpc.find('.//alarmHoldTime')
            detect_sensitivity = cpc.find('.//ipc:detectSensitivity', ns) if ns else cpc.find('.//detectSensitivity')
            cross_in = cpc.find('.//ipc:crossInThreshold', ns) if ns else cpc.find('.//crossInThreshold')
            cross_out = cpc.find('.//ipc:crossOutThreshold', ns) if ns else cpc.find('.//crossOutThreshold')
            two_way_diff = cpc.find('.//ipc:twoWayDiffThreshold', ns) if ns else cpc.find('.//twoWayDiffThreshold')
            force_reset = cpc.find('.//ipc:forceReset', ns) if ns else cpc.find('.//forceReset')
            stat_period = cpc.find('.//ipc:statisticalPeriod', ns) if ns else cpc.find('.//statisticalPeriod')
            
            if switch is not None:
                print(f"CPC Enabled: {switch.text}")
            if alarm_hold is not None:
                print(f"Alarm Hold Time: {alarm_hold.text} seconds")
            if detect_sensitivity is not None:
                min_val = detect_sensitivity.get('min', 'N/A')
                max_val = detect_sensitivity.get('max', 'N/A')
                print(f"Detection Sensitivity: {detect_sensitivity.text} (Range: {min_val}-{max_val})")
            if cross_in is not None:
                min_val = cross_in.get('min', 'N/A')
                max_val = cross_in.get('max', 'N/A')
                print(f"Cross In Threshold: {cross_in.text} (Range: {min_val}-{max_val})")
            if cross_out is not None:
                min_val = cross_out.get('min', 'N/A')
                max_val = cross_out.get('max', 'N/A')
                print(f"Cross Out Threshold: {cross_out.text} (Range: {min_val}-{max_val})")
            if two_way_diff is not None:
                min_val = two_way_diff.get('min', 'N/A')
                max_val = two_way_diff.get('max', 'N/A')
                print(f"Two-way Difference Threshold: {two_way_diff.text} (Range: {min_val}-{max_val})")
            if force_reset is not None:
                print(f"Force Reset: {force_reset.text}")
            if stat_period is not None:
                print(f"Statistical Period: {stat_period.text}")
            
            # Region Info
            region_info = cpc.find('.//ipc:regionInfo', ns) if ns else cpc.find('.//regionInfo')
            if region_info is not None:
                count = region_info.get('count', '0')
                max_count = region_info.get('maxCount', 'unknown')
                print(f"\nDetection Regions (count={count}, max={max_count}):")
                
                items = region_info.findall('ipc:item', ns) if ns else region_info.findall('item')
                for i, item in enumerate(items, 1):
                    x1 = item.find('.//ipc:X1', ns) if ns else item.find('.//X1')
                    y1 = item.find('.//ipc:Y1', ns) if ns else item.find('.//Y1')
                    x2 = item.find('.//ipc:X2', ns) if ns else item.find('.//X2')
                    y2 = item.find('.//ipc:Y2', ns) if ns else item.find('.//Y2')
                    
                    if all([x1, y1, x2, y2]):
                        print(f"\n  Region {i}:")
                        print(f"    Top Left: ({x1.text}, {y1.text})")
                        print(f"    Bottom Right: ({x2.text}, {y2.text})")
                        print("    Note: Coordinates represent diagonal points of the rectangle")
            
            # Direction Info
            direction_info = cpc.find('.//ipc:directionInfo', ns) if ns else cpc.find('.//directionInfo')
            if direction_info is not None:
                count = direction_info.get('count', '0')
                max_count = direction_info.get('maxCount', 'unknown')
                print(f"\nDirection Lines (count={count}, max={max_count}):")
                
                items = direction_info.findall('ipc:item', ns) if ns else direction_info.findall('item')
                for i, item in enumerate(items, 1):
                    start_x = item.find('.//ipc:startX', ns) if ns else item.find('.//startX')
                    start_y = item.find('.//ipc:startY', ns) if ns else item.find('.//startY')
                    end_x = item.find('.//ipc:endX', ns) if ns else item.find('.//endX')
                    end_y = item.find('.//ipc:endY', ns) if ns else item.find('.//endY')
                    
                    if all([start_x, start_y, end_x, end_y]):
                        print(f"\n  Line {i}:")
                        print(f"    Start Point: ({start_x.text}, {start_y.text})")
                        print(f"    End Point: ({end_x.text}, {end_y.text})")
        else:
            print("No CPC configuration found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_subscription_config():
    print("\n--- Subscription Configuration ---")
    try:
        # Get channel ID
        channel_id = input("Enter channel ID (default 0 for all channels): ").strip()
        if not channel_id:
            channel_id = "0"
        
        # Make request
        url = f"http://{HOST}:{PORT}/GetSubscriptionConfig/{channel_id}" if channel_id else url_subscription
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Print available types
        types = root.find('.//ipc:types', ns) if ns else root.find('.//types')
        if types is not None:
            # Print Open Alarm Objects
            alarm_objs = types.findall('.//ipc:openAlramObj/ipc:enum', ns) if ns else types.findall('.//openAlramObj/enum')
            if alarm_objs:
                print("\nAvailable Alarm Objects:")
                for obj in alarm_objs:
                    print(f"  - {obj.text}")
            
            # Print Subscribe Relations
            relations = types.findall('.//ipc:subscribeRelation/ipc:enum', ns) if ns else types.findall('.//subscribeRelation/enum')
            if relations:
                print("\nAvailable Subscribe Relations:")
                for rel in relations:
                    print(f"  - {rel.text}")
            
            # Print Subscribe Types
            sub_types = types.findall('.//ipc:subscribeTypes/ipc:enum', ns) if ns else types.findall('.//subscribeTypes/enum')
            if sub_types:
                print("\nAvailable Subscribe Types:")
                for st in sub_types:
                    print(f"  - {st.text}")
        
        # Print current configuration
        channel_id = root.find('.//ipc:channelID', ns) if ns else root.find('.//channelID')
        init_term_time = root.find('.//ipc:initTermTime', ns) if ns else root.find('.//initTermTime')
        subscribe_flag = root.find('.//ipc:subscribeFlag', ns) if ns else root.find('.//subscribeFlag')
        
        print("\nCurrent Configuration:")
        if channel_id is not None:
            print(f"Channel ID: {channel_id.text} (0 = all channels)")
        if init_term_time is not None:
            print(f"Initialization/Termination Time: {init_term_time.text}")
        if subscribe_flag is not None:
            print(f"Subscribe Flag: {subscribe_flag.text}")
        
        # Print subscription list
        subscribe_list = root.find('.//ipc:subscribeList', ns) if ns else root.find('.//subscribeList')
        if subscribe_list is not None:
            count = subscribe_list.get('count', '0')
            print(f"\nActive Subscriptions (count={count}):")
            
            items = subscribe_list.findall('ipc:item', ns) if ns else subscribe_list.findall('item')
            for i, item in enumerate(items, 1):
                smart_type = item.find('.//ipc:smartType', ns) if ns else item.find('.//smartType')
                subscribe_relation = item.find('.//ipc:subscribeRelation', ns) if ns else item.find('.//subscribeRelation')
                
                print(f"\n  Subscription {i}:")
                if smart_type is not None:
                    print(f"    Smart Type: {smart_type.text}")
                if subscribe_relation is not None:
                    print(f"    Subscribe Relation: {subscribe_relation.text}")
        else:
            print("No active subscriptions found.")
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def print_vehicle_plate_progress():
    print("\n--- Vehicle Plate Import Progress ---")
    try:
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        
        # Make request
        url = f"http://{HOST}:{PORT}/GetVehiclePlateProgress/{channel_id}" if channel_id else url_vehicle_plate_progress
        response = requests.post(url, headers=headers)
        response.raise_for_status()
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Get progress
        vehicle_plates_reply = root.find('.//ipc:vehiclePlatesReply', ns) if ns else root.find('.//vehiclePlatesReply')
        if vehicle_plates_reply is not None:
            progress_value = int(vehicle_plates_reply.text)
            progress_percentage = progress_value / 100
            print(f"\nImport Progress: {progress_percentage:.2f}%")
            print(f"Raw Progress Value: {progress_value}")
        else:
            print("No progress information found in response.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def add_vehicle_plate():
    print("\n--- Add Vehicle Plate Information ---")
    try:
        # Get channel ID
        channel_id = input("Enter channel ID (default 1): ").strip()
        if not channel_id:
            channel_id = "1"
        
        # Get vehicle plate information
        print("\nEnter Vehicle Plate Information:")
        plate_number = input("Plate Number (e.g., 粤B123456): ").strip()
        begin_time = input("Begin Time (YYYY/MM/DD HH:MM:SS): ").strip()
        end_time = input("End Time (YYYY/MM/DD HH:MM:SS): ").strip()
        plate_color = input("Plate Color (leave empty for default): ").strip() or ""
        plate_type = input("Plate Type (e.g., 小型车): ").strip()
        car_type = input("Car Type (leave empty for undefined): ").strip() or "undefined"
        car_owner = input("Car Owner: ").strip()
        car_color = input("Car Color (leave empty for undefined): ").strip() or "undefined"
        plate_item_type = input("Plate Item Type (e.g., strangerList): ").strip()
        
        # Build XML request body
        xml_body = f'''<?xml version="1.0" encoding="utf-8" ?>
<config>
<vehiclePlates type="list" count="1">
<item>
<carPlateNumber type="string"><![CDATA[{plate_number}]]></carPlateNumber>
<beginTime type="string"><![CDATA[{begin_time}]]></beginTime>
<endTime type="string"><![CDATA[{end_time}]]></endTime>
<carPlateColor type="string"><![CDATA[{plate_color}]]></carPlateColor>
<carPlateType type="string"><![CDATA[{plate_type}]]></carPlateType>
<carType type="unit32"><![CDATA[{car_type}]]></carType>
<carOwner type="string"><![CDATA[{car_owner}]]></carOwner>
<carColor type="string"><![CDATA[{car_color}]]></carColor>
<plateItemType type="string">{plate_item_type}</plateItemType>
</item>
</vehiclePlates>
</config>'''
        
        # Make request
        url = f"http://{HOST}:{PORT}/AddVehiclePlate/{channel_id}" if channel_id else url_add_vehicle_plate
        response = requests.post(url, headers=headers, data=xml_body.encode('utf-8'))
        response.raise_for_status()
        
        # Parse response
        root = ET.fromstring(response.content)
        ns = {'ipc': root.tag.split('}')[0].strip('{')} if '}' in root.tag else {}
        
        # Check response
        vehicle_plates_reply = root.find('.//ipc:vehiclePlatesReply', ns) if ns else root.find('.//vehiclePlatesReply')
        if vehicle_plates_reply is not None:
            count = vehicle_plates_reply.get('count', '0')
            if count == '0':
                print("\nSuccess: All vehicle plates were added successfully.")
            else:
                print(f"\nWarning: {count} vehicle plates failed to be added.")
                # Print failed items if any
                items = vehicle_plates_reply.findall('ipc:item', ns) if ns else vehicle_plates_reply.findall('item')
                for item in items:
                    print(f"Failed to add plate: {item.text}")
        else:
            print("No response information found.")
            print("Raw XML response:")
            print(response.content.decode(errors='replace'))
    except Exception as e:
        print(f"Error: {e}")
        if hasattr(e, 'response') and getattr(e.response, 'text', None):
            print(f"Response content: {e.response.text}")

def display_menu():
    print(f"\n=== IP Camera Device Information Tool ===")
    print(f"Connected to: {HOST}:{PORT}")
    print(f"Username: {USERNAME}")
    print("=" * 50)
    
    print("\nBasic Device Information:")
    print("1. Get Basic Device Info")
    print("2. Get Detailed Device Info")
    print("3. Get Disk Info")
    print("4. Get Date and Time Info")
    print("5. Get Network Basic Config")
    
    print("\nStream and Image Settings:")
    print("6. Get Stream Capabilities")
    print("7. Get Video Stream Config")
    print("8. Get Image Configuration")
    print("9. Get Image OSD Config")
    print("10. Get Privacy Mask Config")
    print("11. Get Snapshot")
    print("12. Get Snapshot By Time")
    
    print("\nPTZ Control:")
    print("13. Get PTZ Capabilities")
    print("14. Control PTZ")
    
    print("\nSmart Detection Features:")
    print("15. Get Motion Config")
    print("16. Get Alarm Trigger Config")
    print("17. Get Video Face Detection Config")
    print("18. Get Perimeter Config")
    print("19. Get Vehicle Config")
    print("20. Get Vehicle Plate")
    print("21. Get Vehicle Plate Import Progress")
    print("22. Add Vehicle Plate")
    print("23. Get Crowd Density Detection Config")
    print("24. Get Cross-line People Counting Config")
    
    print("\nSystem and Subscription:")
    print("25. Get Subscription Config")
    print("26. Reboot Device")
    
    print("\nOther:")
    print("27. Get All Information")
    print("28. Change Camera/Connection")
    print("0. Exit")
    print("=" * 50)

def get_user_choice():
    while True:
        try:
            choice = int(input("\nEnter your choice (0-28): "))
            if 0 <= choice <= 28:
                return choice
            print("Invalid choice. Please enter a number between 0 and 28.")
        except ValueError:
            print("Invalid input. Please enter a number.")

def get_all_info():
    print("\n=== Retrieving All Device Information ===")
    print_device_info()
    print_disk_info()
    print_device_detail()
    print_date_time()
    print_stream_caps()
    print_image_config()
    print_snapshot()
    print_snapshot_by_time()
    print_video_stream_config()
    print_image_osd_config()
    print_privacy_mask_config()
    print_ptz_caps()
    print_motion_config()
    print_alarm_trigger_config()
    print_net_basic_config()
    print_vfd_config()
    print_perimeter_config()
    print_vehicle_config()
    print_vehicle_plate()
    print_vehicle_plate_progress()
    add_vehicle_plate()
    print_cdd_config()
    print_cpc_config()
    print_subscription_config()
    print("\n=== All Information Retrieved ===")

def change_camera_connection():
    global HOST, PORT, USERNAME, PASSWORD
    global url_device, url_disk, url_detail, url_time, url_stream, url_image
    global url_privacy_mask, url_ptz_caps, url_ptz_control, url_motion, url_alarm_trigger
    global url_net_basic, url_reboot, url_vfd, url_perimeter, url_vehicle
    global url_vehicle_plate, url_cdd, url_cpc, url_subscription
    global url_vehicle_plate_progress, url_add_vehicle_plate
    
    print("\n=== Change Camera Connection ===")
    HOST, PORT, USERNAME, PASSWORD = get_device_config()
    
    # Rebuild URLs with new connection
    url_device = f"http://{HOST}:{PORT}/GetDeviceInfo"
    url_disk = f"http://{HOST}:{PORT}/GetDiskInfo"
    url_detail = f"http://{HOST}:{PORT}/GetDeviceDetail"
    url_time = f"http://{HOST}:{PORT}/GetDateAndTime"
    url_stream = f"http://{HOST}:{PORT}/GetStreamCaps"
    url_image = f"http://{HOST}:{PORT}/GetImageConfig"
    url_privacy_mask = f"http://{HOST}:{PORT}/GetPrivacyMaskConfig"
    url_ptz_caps = f"http://{HOST}:{PORT}/PtzGetCaps"
    url_ptz_control = f"http://{HOST}:{PORT}/PtzControl"
    url_motion = f"http://{HOST}:{PORT}/GetMotionConfig"
    url_alarm_trigger = f"http://{HOST}:{PORT}/GetAlarmTriggerConfig"
    url_net_basic = f"http://{HOST}:{PORT}/GetNetBasicConfig"
    url_reboot = f"http://{HOST}:{PORT}/Reboot"
    url_vfd = f"http://{HOST}:{PORT}/GetSmartVfdConfig"
    url_perimeter = f"http://{HOST}:{PORT}/GetSmartPerimeterConfig"
    url_vehicle = f"http://{HOST}:{PORT}/GetSmartVehicleConfig"
    url_vehicle_plate = f"http://{HOST}:{PORT}/GetVehiclePlate"
    url_cdd = f"http://{HOST}:{PORT}/GetSmartCddConfig"
    url_cpc = f"http://{HOST}:{PORT}/GetSmartCpcConfig"
    url_subscription = f"http://{HOST}:{PORT}/GetSubscriptionConfig"
    url_vehicle_plate_progress = f"http://{HOST}:{PORT}/GetVehiclePlateProgress"
    url_add_vehicle_plate = f"http://{HOST}:{PORT}/AddVehiclePlate"
    
    print(f"\nSuccessfully changed connection to: {HOST}:{PORT}")
    print("=" * 50)

def main():
    while True:
        display_menu()
        choice = get_user_choice()
        
        if choice == 0:
            print("\nExiting program. Goodbye!")
            break
        elif choice == 1:
            print_device_info()
        elif choice == 2:
            print_device_detail()
        elif choice == 3:
            print_disk_info()
        elif choice == 4:
            print_date_time()
        elif choice == 5:
            print_net_basic_config()
        elif choice == 6:
            print_stream_caps()
        elif choice == 7:
            print_video_stream_config()
        elif choice == 8:
            print_image_config()
        elif choice == 9:
            print_image_osd_config()
        elif choice == 10:
            print_privacy_mask_config()
        elif choice == 11:
            print_snapshot()
        elif choice == 12:
            print_snapshot_by_time()
        elif choice == 13:
            print_ptz_caps()
        elif choice == 14:
            print_ptz_control()
        elif choice == 15:
            print_motion_config()
        elif choice == 16:
            print_alarm_trigger_config()
        elif choice == 17:
            print_vfd_config()
        elif choice == 18:
            print_perimeter_config()
        elif choice == 19:
            print_vehicle_config()
        elif choice == 20:
            print_vehicle_plate()
        elif choice == 21:
            print_vehicle_plate_progress()
        elif choice == 22:
            add_vehicle_plate()
        elif choice == 23:
            print_cdd_config()
        elif choice == 24:
            print_cpc_config()
        elif choice == 25:
            print_subscription_config()
        elif choice == 26:
            print_reboot()
        elif choice == 27:
            get_all_info()
        elif choice == 28:
            change_camera_connection()
        else:
            print("Invalid choice. Please enter a number between 0 and 28.")
        
        input("\nPress Enter to continue...")

if __name__ == "__main__":
    main()  