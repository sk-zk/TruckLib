#!/bin/bash

git clone https://github.com/sk-zk/TruckLib.Core.git ./docfx/TruckLib.Core
git clone https://github.com/sk-zk/TruckLib.Models.git ./docfx/TruckLib.Models
git clone https://github.com/sk-zk/TruckLib.Sii.git ./docfx/TruckLib.Sii
git clone https://github.com/sk-zk/TruckLib.HashFs.git ./docfx/TruckLib.HashFs
docfx docfx/docfx.json
