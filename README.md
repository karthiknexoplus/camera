# IP Camera Device Information Tool

A comprehensive Python tool for retrieving and managing information from IP cameras and network video recorders (NVRs). This tool provides a command-line interface to interact with various camera APIs and retrieve detailed device information, configurations, and control capabilities.

## Features

### Basic Device Information
- Device information and capabilities
- Detailed device specifications
- Disk information and storage status
- Date and time configuration
- Network basic configuration

### Stream and Image Management
- Stream capabilities and configurations
- Video stream settings
- Image configuration and parameters
- On-screen display (OSD) settings
- Privacy mask configuration
- Snapshot capture (current and historical)

### PTZ Control
- PTZ capabilities and limitations
- Pan/Tilt/Zoom control operations
- Preset and cruise configurations

### Smart Detection Features
- Motion detection configuration
- Alarm trigger settings
- Video face detection (VFD)
- Perimeter protection
- Vehicle detection and license plate recognition
- Crowd density detection (CDD)
- Cross-line people counting (CPC)

### System Management
- Subscription configuration
- Device reboot functionality
- Vehicle plate management

## Requirements

- Python 3.6 or higher
- `requests` library
- Network access to the IP camera device

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/ip-camera-device-info-tool.git
cd ip-camera-device-info-tool
```

2. Install required dependencies:
```bash
pip install requests
```

3. Run the script and follow the prompts to configure your device:
```bash
python get_device_info.py
```

The tool will prompt you to:
- Select from predefined cameras (192.168.60.252, 192.168.60.253) or enter a custom IP
- Enter port number (default: 80)
- Enter username (default: admin)
- Enter password (default: admin)

## Usage

Run the main script:
```bash
python get_device_info.py
```

The tool provides an interactive menu with the following options:

1. **Get Basic Device Info** - Retrieve basic device information
2. **Get Detailed Device Info** - Get comprehensive device details
3. **Get Disk Info** - Check storage disk information
4. **Get Date and Time Info** - View timezone and synchronization settings
5. **Get Network Basic Config** - Network configuration details
6. **Get Stream Capabilities** - Available video streams and formats
7. **Get Video Stream Config** - Current stream settings
8. **Get Image Configuration** - Image quality and processing settings
9. **Get Image OSD Config** - On-screen display settings
10. **Get Privacy Mask Config** - Privacy masking areas
11. **Get Snapshot** - Capture current camera image
12. **Get Snapshot By Time** - Retrieve historical snapshots
13. **Get PTZ Capabilities** - Pan/Tilt/Zoom capabilities
14. **Control PTZ** - Manual PTZ control
15. **Get Motion Config** - Motion detection settings
16. **Get Alarm Trigger Config** - Alarm trigger configurations
17. **Get Video Face Detection Config** - Face detection settings
18. **Get Perimeter Config** - Perimeter protection settings
19. **Get Vehicle Config** - Vehicle detection configuration
20. **Get Vehicle Plate** - License plate information
21. **Get Vehicle Plate Import Progress** - Import status
22. **Add Vehicle Plate** - Add new license plate entries
23. **Get Crowd Density Detection Config** - Crowd detection settings
24. **Get Cross-line People Counting Config** - People counting settings
25. **Get Subscription Config** - Event subscription settings
26. **Reboot Device** - Restart the device
27. **Get All Information** - Retrieve all available information
28. **Change Camera/Connection** - Switch to a different camera

## Configuration

### Device Connection
The tool connects to IP cameras using HTTP POST requests with XML payloads. Make sure your device supports the following APIs:

- `GetDeviceInfo`
- `GetDiskInfo`
- `GetDeviceDetail`
- `GetDateAndTime`
- `GetStreamCaps`
- `GetImageConfig`
- `GetPrivacyMaskConfig`
- `PtzGetCaps`
- `PtzControl`
- `GetMotionConfig`
- `GetAlarmTriggerConfig`
- `GetNetBasicConfig`
- `Reboot`
- And many more smart detection APIs

### Authentication
The tool uses Basic Authentication with base64-encoded credentials. Ensure your device credentials are correctly configured.

## File Structure

```
ip-camera-device-info-tool/
├── get_device_info.py          # Main application script
├── test_device_control.py      # Test script for device control
├── setup.py                    # Package setup file
├── README.md                   # This file
├── requirements.txt            # Python dependencies
├── .gitignore                  # Git ignore file
└── tests/                      # Test directory
```

## API Endpoints

The tool supports various API endpoints for different device functions:

### Basic Information
- `/GetDeviceInfo` - Device basic information
- `/GetDeviceDetail` - Detailed device specifications
- `/GetDiskInfo` - Storage information
- `/GetDateAndTime` - Time and timezone settings

### Stream Management
- `/GetStreamCaps` - Stream capabilities
- `/GetVideoStreamConfig` - Video stream configuration
- `/GetSnapshot` - Current snapshot
- `/GetSnapshotByTime` - Historical snapshot

### Image Processing
- `/GetImageConfig` - Image settings
- `/GetImageOsdConfig` - OSD configuration
- `/GetPrivacyMaskConfig` - Privacy masking

### PTZ Control
- `/PtzGetCaps` - PTZ capabilities
- `/PtzControl` - PTZ operations

### Smart Detection
- `/GetMotionConfig` - Motion detection
- `/GetAlarmTriggerConfig` - Alarm triggers
- `/GetSmartVfdConfig` - Face detection
- `/GetSmartPerimeterConfig` - Perimeter protection
- `/GetSmartVehicleConfig` - Vehicle detection
- `/GetSmartCddConfig` - Crowd density
- `/GetSmartCpcConfig` - People counting

### System Management
- `/GetNetBasicConfig` - Network settings
- `/GetSubscriptionConfig` - Event subscriptions
- `/Reboot` - Device restart

## Error Handling

The tool includes comprehensive error handling for:
- Network connectivity issues
- Authentication failures
- Invalid API responses
- Device timeout errors
- XML parsing errors

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Disclaimer

This tool is designed for legitimate network administration and device management purposes. Please ensure you have proper authorization before accessing any network devices. The authors are not responsible for any misuse of this tool.

## Support

For issues and questions:
1. Check the existing issues in the GitHub repository
2. Create a new issue with detailed information about your problem
3. Include device model, firmware version, and error messages

## Changelog

### Version 1.0.0
- Initial release
- Basic device information retrieval
- Stream and image configuration
- PTZ control capabilities
- Smart detection features
- Comprehensive error handling 