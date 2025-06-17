"""
Tests for ANPR functionality
"""

import pytest
from unittest.mock import Mock, patch
from device_control.core.exceptions import (
    DeviceError, ANPRError, ANPRConfigError, ANPRDetectionError
)
from device_control.platforms.windows import WindowsDevice
from device_control.platforms.linux import LinuxDevice
from device_control.platforms.macos import MacOSDevice

@pytest.fixture
def mock_device():
    """Create a mock device for testing"""
    device = WindowsDevice("192.168.1.100", 8000, "admin", "password")
    device._handle = 1
    device._connected = True
    device._sdk = Mock()
    return device

def test_get_anpr_config(mock_device):
    """Test getting ANPR configuration"""
    mock_device._sdk.GetANPRConfig.return_value = True
    
    config = mock_device.get_anpr_config()
    
    assert isinstance(config, dict)
    assert "enabled" in config
    assert "sensitivity" in config
    assert "min_confidence" in config
    assert "region" in config
    assert "country" in config
    
    mock_device._sdk.GetANPRConfig.assert_called_once()

def test_get_anpr_config_error(mock_device):
    """Test getting ANPR configuration with error"""
    mock_device._sdk.GetANPRConfig.return_value = False
    
    with pytest.raises(ANPRConfigError):
        mock_device.get_anpr_config()

def test_set_anpr_config(mock_device):
    """Test setting ANPR configuration"""
    mock_device._sdk.SetANPRConfig.return_value = True
    
    config = {
        "enabled": True,
        "sensitivity": 75,
        "min_confidence": 0.8,
        "region": "US",
        "country": "USA"
    }
    
    result = mock_device.set_anpr_config(config)
    
    assert result is True
    mock_device._sdk.SetANPRConfig.assert_called_once()

def test_set_anpr_config_error(mock_device):
    """Test setting ANPR configuration with error"""
    mock_device._sdk.SetANPRConfig.return_value = False
    
    config = {
        "enabled": True,
        "sensitivity": 75,
        "min_confidence": 0.8,
        "region": "US",
        "country": "USA"
    }
    
    with pytest.raises(ANPRConfigError):
        mock_device.set_anpr_config(config)

def test_register_anpr_callback(mock_device):
    """Test registering ANPR callback"""
    mock_device._sdk.RegisterANPRCallback.return_value = True
    
    def callback(result):
        pass
    
    mock_device.register_anpr_callback(callback)
    
    assert callback in mock_device._anpr_callbacks
    mock_device._sdk.RegisterANPRCallback.assert_called_once()

def test_register_anpr_callback_error(mock_device):
    """Test registering ANPR callback with error"""
    mock_device._sdk.RegisterANPRCallback.return_value = False
    
    def callback(result):
        pass
    
    with pytest.raises(ANPRError):
        mock_device.register_anpr_callback(callback)

def test_unregister_anpr_callback(mock_device):
    """Test unregistering ANPR callback"""
    mock_device._sdk.UnregisterANPRCallback.return_value = True
    
    def callback(result):
        pass
    
    mock_device._anpr_callbacks.append(callback)
    mock_device.unregister_anpr_callback(callback)
    
    assert callback not in mock_device._anpr_callbacks
    mock_device._sdk.UnregisterANPRCallback.assert_called_once()

def test_unregister_anpr_callback_error(mock_device):
    """Test unregistering ANPR callback with error"""
    mock_device._sdk.UnregisterANPRCallback.return_value = False
    
    def callback(result):
        pass
    
    mock_device._anpr_callbacks.append(callback)
    
    with pytest.raises(ANPRError):
        mock_device.unregister_anpr_callback(callback)

def test_anpr_callback_execution(mock_device):
    """Test ANPR callback execution"""
    mock_device._sdk.RegisterANPRCallback.return_value = True
    
    callback_results = []
    
    def callback(result):
        callback_results.append(result)
    
    mock_device.register_anpr_callback(callback)
    
    # Simulate ANPR event
    result = {
        "plate_number": "ABC123",
        "confidence": 0.95,
        "timestamp": 1234567890,
        "image_path": "/path/to/image.jpg"
    }
    
    # Get the callback function that was registered
    callback_func = mock_device._sdk.RegisterANPRCallback.call_args[0][1]
    
    # Create a mock result structure
    class MockResult:
        def __init__(self, data):
            self.contents = type("MockContents", (), data)
    
    # Call the callback with the mock result
    callback_func(MockResult(result))
    
    assert len(callback_results) == 1
    assert callback_results[0] == result 