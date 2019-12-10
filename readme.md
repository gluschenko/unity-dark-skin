# UnityDarkSkin

## About

This tool makes Dark Theme in Unity Editor and turns it back to white, if you wish. It's being useful for Unity Personal, where Dark Theme is disabled by default. **It is not a crack! It's simply changing a couple of bits in Editor.exe**

## Usage

1. Compile it with Visual Studio and copy UnityDarkSkin.exe to folder where Unity.exe is located
2. Run UnityDarkSkin.exe as **administrator**

## Requirements

* Windows 10
* Visual Studio 2017 (or newer)
* .NET Framework >= 4.5.2

## Supported versions

| Version | Support | Version tested |
| :--- | :---: | :--- |
| 5.3    | ✅ | 5.3.5f1    |
| 5.4    | ✅ | 5.4        |
| 2017.2 | ✅ | 2017.2     |
| 2018.2 | ✅ | 2018.2     |
| 2018.3 | ✅ | 2018.3.0f2 |
| 2018.4 | ✅ | 2018.4.5f1, 2018.4.13f1 (LTS) |
| 2019.1 | ✅ | 2019.1.0f2 |
| 2019.2 | ✅ | 2019.2.0f1, 2019.2.14f1 |
| 2019.3 | ✅ | 2019.3.0f1 |
| 2020.1 | ✅ | 2020.1.0a14 |

✅ - Supported | ⚠️ - Work in progress | ❌ - Not supported

## How it works

| Before | After |
| :---: | :---: |
| ![Default theme](Media/LightSkin.jpg) | ![Dark theme](Media/DarkSkin.jpg) |

## Showcase

| UnityDarkSkin.App | UnityDarkSkin |
| :---: | :---: |
| ![GUI](Media/gui_1.png) | ![Console](Media/console.png) |
| ![GUI](Media/gui_2.png) |  |
| ![GUI](Media/gui_3.png) |  |
| ![GUI](Media/gui_4.png) |  |

## Structure

| Namespace         | .NET Framework | Description             |
| --- | --- | --- |
| UnityDarkSkin     | >= 4.5.2       | Legacy Console edition  |
| UnityDarkSkin.App | >= 4.5.2       | WPF app                 |
| UnityDarkSkin.Lib | >= 4.5.2       | Patcher lib, versions data |
