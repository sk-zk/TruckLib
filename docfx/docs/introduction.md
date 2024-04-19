# Introduction

This is the documentation for **TruckLib**, a library for the map format of Euro Truck Simulator 2 and American Truck Simulator.

## Getting started
To get started with the library, check out the [Creating and modifying maps](~/docs/TruckLib.ScsMap/map-class.md) section and the sample code.

There are no builds or NuGet packages at this time, so you will need to clone the repo, add the project(s) to your solution,
and add them to your project as project dependency.

## Supported formats

### Map
TruckLib supports [map format version](https://github.com/sk-zk/map-docs/wiki/Map-format-version) **900** (game version 1.48.5/1.49).

Support for more than one version at a time is not currently planned, so you will need to update existing maps in the editor
whenever this library updates to a newer version.

### HashFS
HashFS v1 is fully supported. HashFS v2 is supported, but .tobj unpacking is not available.

### Prefab descriptors
TruckLib can read .ppd versions 21, 22, and 23.
