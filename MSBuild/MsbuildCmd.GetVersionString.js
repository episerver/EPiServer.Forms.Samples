// ----------------
//MSBUILD COMMAND will be execute by MSBuild <exec> Target. It can return one string to the MSBuild
// ----------------
var fs = require('fs');


// write to stdout without the newline (not like the console.log())
function echo(p_s) {
    process.stdout.write(p_s);
    // console.log(ASSEMBLY_VERSION_FILE);
}

// read all text from filepath, parse and take the version string group
function GetVersionString(filepath) {
    var ret;
    var alltext = fs.readFileSync(filepath, { encoding: 'utf8', flag: 'r' });


    var extendedPattern = /\[assembly: AssemblyInformationalVersion\("([\d\.]+?.*)"\)\]/ig;

    var matches = extendedPattern.exec(alltext);
    if (matches) {
        ret = matches[1]; // first group
        var ret = ret.trim();
    }
    else {
        var pattern = /\[assembly: AssemblyVersion\("([\d\.]+?)"\)\]/ig;
        var matches = pattern.exec(alltext);
        if (matches) {
            ret = matches[1]; // first group
            var ret = ret.trim();
        }
    }
    return ret;
}


// MAIN EXECUTION:
/*====== GET COMMAND LINE ARGUMENT ========*/
var ASSEMBLY_VERSION_FILE = process.argv[2] || '';  // take first command line argument
echo(GetVersionString(ASSEMBLY_VERSION_FILE));