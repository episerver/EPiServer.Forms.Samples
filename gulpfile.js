"use strict";
const gulp = require("gulp"),
    BuildHelper = require("./build/js/buildHelper");

// get build options from command line arguments
let buildHelper = new BuildHelper();

// Import all build tasks
// require("./build/tasks/set-version")(process.argv);
require("./build/tasks/nuget")(buildHelper);
require("./build/tasks/build")(buildHelper);
require("./build/tasks/zip-forms-samples")(buildHelper);
require("./build/tasks/less")();

gulp.task("build", gulp.parallel("less:compile", "create-moduleconfig"));
gulp.task("pack", gulp.series("clean", "build", "zip:forms-samples"));