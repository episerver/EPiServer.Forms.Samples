"use strict";

const gulp = require("gulp"),
    del = require('del');

const tasks = (buildHelper) => {

    var outDir = buildHelper.outDir + "/EPiServer.Forms.Samples";
    var formsSampledir = "./src/EPiServer.Forms.Samples/";

    gulp.task("create-moduleconfig", async function() {
        await buildHelper.createModuleConfig(formsSampledir + "module.config", outDir + "/module.config", buildHelper.version);
    });

    gulp.task("copy-clientresources", async function() {
        gulp.src([formsSampledir + 'ClientResources/**/*']).pipe(gulp.dest(outDir + `/${buildHelper.version}/ClientResources`));
    })

    gulp.task("copy-langfiles", async function() {
        gulp.src([formsSampledir + "EmbeddedLangFiles/**/*", "!EmbeddedLangFiles/NewText.xml"]).pipe(gulp.dest(outDir + "/EmbeddedLangFiles"));
    })

    gulp.task("copy-views", async function() {
        gulp.src([formsSampledir + 'Views/**/*']).pipe(gulp.dest(outDir + "/Views"));
    })

    gulp.task("clean", async function() {
        await del([buildHelper.outDir, "nupkgs/"], {
            force: true
        });
    })

};

module.exports = tasks;