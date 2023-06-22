# forms-satellites Dev Guide

Satellite (non-Core) projects of Episerver Forms


## Building on Team City

Forms has the following builds (based on the same template). The build compiles, run tests, signs and create nuget packages. This means every checkin will produce valid nuget packages.

* Forms CI: Runs automatically on all branches except master and release/*.
* Forms Release: Runs automatically on master and release/*.

All DLL's and nuget-packages are pre-release tagged except the packages built from master. Nuget packages and assemblies from develop are named "7.5.1001.0-ci-XXXX" where XXXX is the build, release branches "7.5.1001.0-pre--XXXX". This is done automatically by the builds.

## Creating a release

* In Git pull develop and master to make sure you have the latest code.
* Create a new branch from _develop_.
* Name branch after _expected_ version number of the new release, for example "release/7.8.1".
* commit/push this branch.
* Builds will trigger automatically and automatically updates Jira.

Note: Version numbers are _only_ changed on release branches.

## Finishing a release

* In Git, pull release-branch(ie release/7.8.1), develop and master to make sure you have the latest code.
* Merge release-branch to master.
* Tag the latest commit on master with vX.X.X (ie v7.8.1)
* Merge master to develop
* Push master and develop (including tags!)
* Builds will trigger automatically and automatically update Jira.
* Delete older release branches (release/* should only contain the last released version, the tag is used to track versions)
* Copy packages from \\\\t3\\nuget to \\\\t3\\Releases\\thisweek


## Changing code in this repo

* Create branch from item in Jira
* For bugfixes add a unit test to catches the bug before continuing
* Check coverage on integration tests, if there are no integration tests on the area you are planning to code they must be written first
* Write tests, code, write tests, code  :)
* Make sure builds are green, Forms CI runs on every checkin
* Create a pull requests from your branch to develop and add reviewers

## Accepting pull requests

Pull request are the official code review where someone on the team signs off on the code.
It is encouraged to do code reviews continously before you push to the repository but this is not enforced and up to every developer to choose to do so.

All developers are encouraged to read, review, and comment on pull requests to make sure code reviews are a collaborate effort.

When merging, always delete source branch.
Do not merge changes that have not been planned on the task board to make sure we trickle out changes in a continous and controlled pace where the whole team can see what is happening.
It's not a race and nothing is so important that it can't wait a few days, nobody wants quick fixes.

## Branching model

We are using the workflow as described in https://www.atlassian.com/git/workflows#!workflow-gitflow with some minor modifications.

### Master branch

Should always contain tested, working, releasable code. You can only get code onto master via merging tested releases.

### Develop branch

Acts as integration branch for feature and bugfix branches that should go into the next release.
You can only get code onto develop by creating pull requests that then needs to be reviewed and accepted. Should only contain completely implemented work items, but code here might not have been thru QA.

### Feature branches

Created from develop and should be named feature/<key in Jira>-<short description>. Create branch from Jira to get correct naming.
Merge to the develop branch by creating a pull request.

### Bugfix branches

Created from develop and should be named bugfix/<key in Jira>-<short description>. Create branch from Jira to get correct naming.
Merge to the develop branch by creating a pull request.

### Release branches

Created from develop to send the code to QA, the changes are then merged to master and back to develop. This is the only place we change version number.


## Unit tests projects

* Do name test methods using this convention: NameOfMethodBeingTested_WhenConditionIsMet_ShouldDoSomethingThatWeAssert
* The structure of unit test projects should match be a one-to-one match with the project being tested (Core/MyService.cs is tested by Core/MyServiceTest.cs)
* Do not use the ServiceLocator in either unit tests or classes being tested.
* Never change global state in unit tests (like static variables, service locators etc)
* Do not use external data sources as part of a unit test
* Avoid multi-level inheritance and base classes for unit tests since it makes them harder to read
