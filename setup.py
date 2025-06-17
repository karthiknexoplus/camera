from setuptools import setup, find_packages

setup(
    name="device_control",
    version="0.1.0",
    packages=find_packages(),
    install_requires=[
        "ctypes",
    ],
    extras_require={
        'dev': [
            'pytest>=6.0',
            'pytest-cov>=2.0',
            'pytest-mock>=3.0',
            'flake8>=3.9',
            'mypy>=0.910',
            'black>=21.0',
        ],
    },
    author="Your Name",
    author_email="your.email@example.com",
    description="A cross-platform Python wrapper for device control SDK",
    long_description=open("README.md").read(),
    long_description_content_type="text/markdown",
    url="https://github.com/yourusername/device_control",
    classifiers=[
        "Development Status :: 3 - Alpha",
        "Intended Audience :: Developers",
        "License :: OSI Approved :: MIT License",
        "Operating System :: OS Independent",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.6",
        "Programming Language :: Python :: 3.7",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
    ],
    python_requires=">=3.6",
    entry_points={
        'console_scripts': [
            'device-control=examples.simple_device_control:main',
            'device-control-gui=examples.gui_device_control:main',
        ],
    },
) 