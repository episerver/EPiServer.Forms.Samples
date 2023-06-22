"use strict";

const gulp = require("gulp"),
    fs = require("fs"),
    archiver = require("archiver"),
    del = require("del"),
    log = require('fancy-log');

const tasks = (buildHelper) => {
    const compress = () => {
        const promise = new Promise((resolve, reject) => {
            const clientResourceBasePath = buildHelper.version + "/ClientResources",
                outDir = buildHelper.formsSamplesOutDir,
                formSamplesRootPath = "./src/EPiServer.Forms.Samples/",
                zip = () => {
                    const promise = new Promise((resolve, reject) => {
                        let target = fs.createWriteStream(outDir + "/EPiServer.Forms.Samples.zip");
                        let archive = archiver("zip", { zlib: { level: 9 } });

                        target.on("close", resolve);
                        target.on("error", reject);

                        archive.pipe(target);

                        archive.glob("**/!(*.tsx)", { cwd: formSamplesRootPath + "ClientResources/" }, { prefix: clientResourceBasePath });
                        archive.glob("**/*.*", { cwd: formSamplesRootPath + "FormsViews/" }, { prefix: "FormsViews/" });
                        archive.glob("**/*.*", { cwd: formSamplesRootPath + "EmbeddedLangFiles/" }, { prefix: "EmbeddedLangFiles/" });
                        archive.glob("module.config", { cwd: outDir });

                        archive.finalize();
                    });

                    return promise;
                };

            buildHelper.createModuleConfig("./src/EPiServer.Forms.Samples/module.config", outDir + "/module.config", buildHelper.version)
                .then(zip)
                .then(() => {
                    // Delete generated module file
                    del.sync([outDir + "/module.config"]);
                })
                .then(resolve)
                .catch(reject);
        });

        return promise;
    };

    gulp.task("zip:forms-samples", (done) => {
        Promise.all([compress()])
            .then(() => {
                done();
            })
            .catch((err) => {
                log(err);
            });
    });
};

module.exports = tasks;