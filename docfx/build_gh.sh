#!/bin/bash

rm -rf ./_site
git clone https://github.com/sk-zk/TruckLib.Core.git
git clone https://github.com/sk-zk/TruckLib.Models.git
git clone https://github.com/sk-zk/TruckLib.Sii.git
git clone https://github.com/sk-zk/TruckLib.HashFs.git
docfx docfx/docfx.json
