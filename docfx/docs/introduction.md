# Introduction

This is the documentation for [**TruckLib**](https://github.com/sk-zk/TruckLib), a library for the map format of Euro Truck Simulator 2 and American Truck Simulator.

## Install
```
dotnet add package TruckLib
```

## Getting started
To get started with the library, check out the [Creating and modifying maps](~/docs/TruckLib.ScsMap/map-class.md) section and the sample code.

## Supported formats

### Map
TruckLib supports [map format version](https://github.com/sk-zk/map-docs/wiki/Map-format-version) **901** (game version 1.51/1.52).

Support for more than one version at a time is not planned, so you will need to update existing maps in the editor
whenever this library updates to a newer version.

### HashFS
Both HashFS v1 and v2 are supported.

### Prefab descriptors
TruckLib can read .ppd versions 21 to 24.
