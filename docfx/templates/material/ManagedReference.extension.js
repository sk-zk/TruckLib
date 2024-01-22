// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/**
 * This method will be called at the start of exports.transform in ManagedReference.html.primary.js
 */
exports.preTransform = function (model) {
  return model;
}

/**
 * This method will be called at the end of exports.transform in ManagedReference.html.primary.js
 */
exports.postTransform = function (model) {
  // Prevent the <summary> of the class being displayed for properties without <value> tag
  // or method parameters without <param> tag (why is that even the default behavior??).
  model.description = "";
  return model;
}
