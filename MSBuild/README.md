Steps to build EPiServer Forms Sample nuget package:

1. Open EPiServer.Forms.Samples.sln in Visual Studio
2. Rebuild the solution to restore nuget packages
3. Open Developer Command Prompt for Visual Studio.
4. Change working directory to MSBuild folder.
5. Run command "msbuild".

The package EPiServer.Forms.Samples will be created in the root folder.

The styling of the site is done in [less](http://lesscss.org/). The less files will be recompiled into css on every build. From the command line
you can also execute the following command in folder "MSBuild\":

```
msbuild -t:BuildLessFiles
```
