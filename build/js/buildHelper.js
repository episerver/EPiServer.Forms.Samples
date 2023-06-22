"use strict";

const program = require("commander"),
    outDir = "out",
    path = require("path"),
    fs = require("fs"),
    xmlpoke = require("xmlpoke"),
    mkdirp = require("mkdirp"),
    getVersion = require("../tasks/get-version");

class BuildHelper {
    constructor() {
        this.configuration = this._getConfiguration();
        this.version = this._getVersion();
        this.outDir = path.resolve(outDir);
        this.formsSamplesOutDir = path.resolve(outDir + "/EPiServer.Forms.Samples");
        this.sourceBasePath = path.resolve("src/EPiServer.Forms.Samples/ClientResources/");
    }

    /**
     * @returns {"debug"|"release"}
     */
    _getConfiguration() {
        program.option("--configuration <configuration>", "Set the build configuration", /^(debug|release)$/i, "debug").parseOptions(process.argv);
    }

    _getVersion() {
        var version = getVersion();
        return version.packageVersion;
    }

    createModuleConfig(source, target, version) {
        return new Promise(function(resolve, reject) {
            fs.readFile(source, "utf-8", (err, data) => {
                if (err) {
                    reject(err);
                }
                let xml = xmlpoke(data, (xml) => {
                    xml.set("/module/@clientResourceRelativePath", version);
                });

                mkdirp(path.dirname(target), (err) => {
                    if (err) {
                        reject(err);
                    }

                    fs.writeFile(target, xml, "utf-8", (err) => {
                        if (err) {
                            reject(err);
                        }
                        resolve();
                    });
                });
            });
        });
    }
}

module.exports = BuildHelper