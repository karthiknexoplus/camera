"""
Tests for recording and alarm functionality
"""

import os
import time
import pytest
from unittest.mock import Mock, patch
from device_control.core.exceptions import (
    DeviceError, RecordError, AlarmError,
    StorageError
)
from device_control.factory import create_device

@pytest.fixture
def mock_device():
    """Create a mock device for testing"""
    with patch('device_control.factory.create_device') as mock_create:
        device = Mock()
        device._handle = 1
        device._connected = True
        device._recording = False
        device._alarm_callbacks = []
        mock_create.return_value = device
        yield device

def test_start_recording(mock_device):
    """Test starting a recording"""
    # Test successful recording start
    mock_device._start_recording.return_value = "test_recording.mp4"
    filename = mock_device.start_recording(channel=0)
    assert filename == "test_recording.mp4"
    assert mock_device._recording is True
    mock_device._start_recording.assert_called_once_with(0, None)
    
    # Test recording when already recording
    with pytest.raises(RecordError):
        mock_device.start_recording()
        
    # Test recording with custom filename
    mock_device._recording = False
    mock_device._start_recording.return_value = "custom_recording.mp4"
    filename = mock_device.start_recording(channel=1, filename="custom_recording.mp4")
    assert filename == "custom_recording.mp4"
    mock_device._start_recording.assert_called_with(1, "custom_recording.mp4")

def test_stop_recording(mock_device):
    """Test stopping a recording"""
    # Test successful recording stop
    mock_device._recording = True
    mock_device.stop_recording()
    assert mock_device._recording is False
    mock_device._stop_recording.assert_called_once()
    
    # Test stopping when not recording
    with pytest.raises(RecordError):
        mock_device.stop_recording()

def test_get_storage_info(mock_device):
    """Test getting storage information"""
    # Test successful storage info retrieval
    expected_info = {
        "total_space": 1000000000,
        "free_space": 500000000,
        "used_space": 500000000
    }
    mock_device._get_storage_info.return_value = expected_info
    info = mock_device.get_storage_info()
    assert info == expected_info
    mock_device._get_storage_info.assert_called_once()
    
    # Test storage info retrieval when not connected
    mock_device._connected = False
    with pytest.raises(DeviceError):
        mock_device.get_storage_info()

def test_get_alarm_config(mock_device):
    """Test getting alarm configuration"""
    # Test successful alarm config retrieval
    expected_config = {
        "enable": True,
        "alarm_type": 1,
        "channel": 0,
        "sensitivity": 5
    }
    mock_device._get_alarm_config.return_value = expected_config
    config = mock_device.get_alarm_config()
    assert config == expected_config
    mock_device._get_alarm_config.assert_called_once()
    
    # Test alarm config retrieval when not connected
    mock_device._connected = False
    with pytest.raises(DeviceError):
        mock_device.get_alarm_config()

def test_set_alarm_config(mock_device):
    """Test setting alarm configuration"""
    # Test successful alarm config setting
    config = {
        "enable": True,
        "alarm_type": 1,
        "channel": 0,
        "sensitivity": 5
    }
    mock_device._set_alarm_config.return_value = True
    result = mock_device.set_alarm_config(config)
    assert result is True
    mock_device._set_alarm_config.assert_called_once_with(config)
    
    # Test alarm config setting when not connected
    mock_device._connected = False
    with pytest.raises(DeviceError):
        mock_device.set_alarm_config(config)

def test_alarm_callback_registration(mock_device):
    """Test alarm callback registration and unregistration"""
    # Test callback registration
    callback = Mock()
    mock_device.register_alarm_callback(callback)
    assert callback in mock_device._alarm_callbacks
    mock_device._register_alarm_callback.assert_called_once_with(callback)
    
    # Test callback unregistration
    mock_device.unregister_alarm_callback(callback)
    assert callback not in mock_device._alarm_callbacks
    mock_device._unregister_alarm_callback.assert_called_once_with(callback)
    
    # Test callback registration when not connected
    mock_device._connected = False
    with pytest.raises(DeviceError):
        mock_device.register_alarm_callback(callback)
    
    # Test callback unregistration when not connected
    with pytest.raises(DeviceError):
        mock_device.unregister_alarm_callback(callback)

def test_alarm_callback_execution(mock_device):
    """Test alarm callback execution"""
    # Create a callback that records received alarms
    received_alarms = []
    def alarm_callback(alarm_info):
        received_alarms.append(alarm_info)
    
    # Register the callback
    mock_device.register_alarm_callback(alarm_callback)
    
    # Simulate an alarm event
    alarm_info = {
        "alarm_type": 1,
        "channel": 0,
        "timestamp": time.time()
    }
    mock_device._alarm_callbacks[0](alarm_info)
    
    # Verify the callback was called with correct info
    assert len(received_alarms) == 1
    assert received_alarms[0] == alarm_info

def test_recording_error_handling(mock_device):
    """Test recording error handling"""
    # Test recording start failure
    mock_device._start_recording.side_effect = RecordError("Failed to start recording")
    with pytest.raises(RecordError):
        mock_device.start_recording()
    
    # Test recording stop failure
    mock_device._recording = True
    mock_device._stop_recording.side_effect = RecordError("Failed to stop recording")
    with pytest.raises(RecordError):
        mock_device.stop_recording()

def test_alarm_error_handling(mock_device):
    """Test alarm error handling"""
    # Test alarm config get failure
    mock_device._get_alarm_config.side_effect = AlarmError("Failed to get alarm config")
    with pytest.raises(AlarmError):
        mock_device.get_alarm_config()
    
    # Test alarm config set failure
    mock_device._set_alarm_config.side_effect = AlarmError("Failed to set alarm config")
    with pytest.raises(AlarmError):
        mock_device.set_alarm_config({})
    
    # Test alarm callback registration failure
    mock_device._register_alarm_callback.side_effect = AlarmError("Failed to register callback")
    with pytest.raises(AlarmError):
        mock_device.register_alarm_callback(Mock())

def test_storage_error_handling(mock_device):
    """Test storage error handling"""
    # Test storage info retrieval failure
    mock_device._get_storage_info.side_effect = StorageError("Failed to get storage info")
    with pytest.raises(StorageError):
        mock_device.get_storage_info() 