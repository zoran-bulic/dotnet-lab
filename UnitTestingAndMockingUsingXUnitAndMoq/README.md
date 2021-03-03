# Unit Testing and Mocking using xUnit and Moq

[![Build Status](https://dev.azure.com/zoranbulic/dotnet-lab/_apis/build/status/dotnet-lab.git?branchName=main)](https://dev.azure.com/zoranbulic/dotnet-lab/_build/latest?definitionId=4&branchName=main)
[![CircleCI](https://circleci.com/gh/zoran-bulic/dotnet-lab.svg?style=shield)](https://circleci.com/gh/zoran-bulic/dotnet-lab?branch=main)

## About
This example was created in order to learn and refresh knowledge of the two very common test frameworks used in .NET Framework: [xUnit](https://xunit.net/docs/getting-started/netfx/visual-studio) and [Moq](https://github.com/Moq/moq4/wiki/Quickstart)

In oder to learning and use the frameworks, I found two very good youtube videos from Tim Corey (IAmTimCorey) explaining the unit testing and mocking:
- [Intro to Unit Testing in C# using XUnit](https://www.youtube.com/watch?v=ub3P8c87cwk)
- [Mocking in C# Unit Tests - How To Test Data Access Code and More](https://www.youtube.com/watch?v=DwbYxP-etMY)

which examples and sample code were used as a basis for my test project.

The application idea was my own: a simple sample application used for handling the orders. For the purpose of using database as the Data Access Layer, I I used [SQLite](https://www.sqlite.org/index.html) as a simple and small SQL database which could easily be deployed with the solution.

I used another youtube video and sample code from Tim Corey as a source:
- [Using SQLite in C# - Building Simple, Powerful, Portable Databases for Your Application](https://www.youtube.com/watch?v=ayp3tHEkRc0)

## Project Structure
- OrdersHandler: class library containing the DAL (Data Access Layer) and BL (Business Logic) (.NET Framework 4.7.2)
- OrdersHandler.UI: WPF application (.NET Framework 4.7.2)
- OrdersHandler.Tests.Unit: unit tests (.NET Core 3.1)
- OrdersHandler.Tests.Integration: integration tests (.NET Core 3.1)

## Running the Application
To run the application, you will need to download and compile the solution. Run "OrdersHandler.UI.exe".

![OrdersHandler.UI.exe](https://raw.githubusercontent.com/zoran-bulic/dotnet-lab/UnitTestingAndMockingUsingXUnitAndMoq/master/images/demo.png)

## Unit Test and Integration Tests
The tests results will hopefully also be integrated in GitHub soon ;)

## License
As this is a reworked example created for learning/testing purpose and was created based upon the example codes and videos listed in chapter above, it is not licensed nor copyrighted and it is published under CC (CreativeCommons) license.

[![CC0](http://mirrors.creativecommons.org/presskit/buttons/88x31/svg/cc-zero.svg)](http://creativecommons.org/publicdomain/zero/1.0)

In case of (re)use, providing a link to this GitHub repository would be highly appreciated.