"""
Test suite for device control functionality
"""

import unittest
from unittest.mock import MagicMock, patch
from device_control import create_device, DeviceError

class TestDevice(unittest.TestCase):
    def setUp(self):
        """Set up test fixtures"""
        self.host = "192.168.1.100"
        self.port = 8000
        self.username = "admin"
        self.password = "password"
        
    @patch('device_control.platforms.macos.MacOSDevice')
    def test_create_device_macos(self, mock_device):
        """Test device creation on macOS"""
        with patch('platform.system', return_value='Darwin'):
            device = create_device(self.host, self.port, self.username, self.password)
            self.assertIsInstance(device, mock_device)
            
    @patch('device_control.platforms.windows.WindowsDevice')
    def test_create_device_windows(self, mock_device):
        """Test device creation on Windows"""
        with patch('platform.system', return_value='Windows'):
            device = create_device(self.host, self.port, self.username, self.password)
            self.assertIsInstance(device, mock_device)
            
    @patch('device_control.platforms.linux.LinuxDevice')
    def test_create_device_linux(self, mock_device):
        """Test device creation on Linux"""
        with patch('platform.system', return_value='Linux'):
            device = create_device(self.host, self.port, self.username, self.password)
            self.assertIsInstance(device, mock_device)
            
    def test_create_device_unsupported_platform(self):
        """Test device creation on unsupported platform"""
        with patch('platform.system', return_value='Unsupported'):
            with self.assertRaises(DeviceError):
                create_device(self.host, self.port, self.username, self.password)
                
    def test_device_connection(self):
        """Test device connection"""
        device = create_device(self.host, self.port, self.username, self.password)
        
        # Mock SDK initialization
        device._init_sdk = MagicMock(return_value=True)
        device._login = MagicMock(return_value=True)
        
        # Test successful connection
        self.assertTrue(device.connect())
        self.assertTrue(device._connected)
        
        # Test disconnection
        device.disconnect()
        self.assertFalse(device._connected)
        
    def test_device_preview(self):
        """Test video preview functionality"""
        device = create_device(self.host, self.port, self.username, self.password)
        
        # Mock SDK functions
        device._start_preview = MagicMock(return_value=True)
        device._stop_preview = MagicMock()
        
        # Test preview start
        self.assertTrue(device.start_preview(channel=0, stream_type=0))
        device._start_preview.assert_called_once_with(0, 0)
        
        # Test preview stop
        device.stop_preview()
        device._stop_preview.assert_called_once()
        
    def test_ptz_control(self):
        """Test PTZ control functionality"""
        device = create_device(self.host, self.port, self.username, self.password)
        
        # Mock SDK function
        device._ptz_control = MagicMock(return_value=True)
        
        # Test valid commands
        valid_commands = ['up', 'down', 'left', 'right', 'stop']
        for cmd in valid_commands:
            self.assertTrue(device.ptz_control(cmd, speed=5))
            device._ptz_control.assert_called_with(cmd, 5)
            
        # Test invalid command
        with self.assertRaises(DeviceError):
            device.ptz_control('invalid', speed=5)
            
    def test_capture_picture(self):
        """Test picture capture functionality"""
        device = create_device(self.host, self.port, self.username, self.password)
        
        # Mock SDK function
        expected_filename = "test.jpg"
        device._capture_picture = MagicMock(return_value=expected_filename)
        
        # Test capture
        filename = device.capture_picture(channel=0)
        self.assertEqual(filename, expected_filename)
        device._capture_picture.assert_called_once()
        
if __name__ == '__main__':
    unittest.main() 