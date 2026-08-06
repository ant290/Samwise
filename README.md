<div align="center">
  <br>
  <h1>🪴Samwise🪴</h1>
  <strong>A Garden Automation Project</strong>
</div>
<br>

Welcome to [Samwise](https://github.com/ant290/Samwise). This project aims to be an overall garden automation alert / automation tool.

## What is Samwise?

Samwise plans to be a gardening companion tool, consisting of a web component and various IOT devices that help to manage the garden.

## Table of Contents

- [What is Samwise?](#what-is-samwise)
- [Table of Contents](#table-of-contents)
- [Contributing](#contributing)
- [Getting Started](#getting-started)
  - [Installation](#installation)
- [License](#license)


## Contributing

Not currently looking for contributions, but you are welcome to fork the repo and make changes that help with your garden.

## Getting Started

The main Blazor component of the Samwise project can be run by first installing the .net SDK: https://dotnet.microsoft.com/en-us/download/dotnet/10.0

Once that is done, then you can simply open a command line window inside the BlazorWebApp/SamwiseBlazor folder and run:
```
dotnet run
```
This will stat up the blazor app, which you can then access at http://localhost:5010 in your browser.
You can configure the port in the appsettings.json in the same folder. Notice the Kestrel config, this is used to allow other devices on your local network to access the web app by visiting it by IP address i.e. http://192.168.0.256:5010 (there is no need to change the config for that to work).

### Installation

You can also build and run the blazor app as a service. It was designed to be run on a Raspberry Pi and can be run there by doing `dotnet publish` and copying the files to your pi **more details to come later** [Here's a great video explaining how to run a service on a Pi](https://youtu.be/7_2Lg7LNMNM)

The arduino sketches in Embedded Software/GardenSensorReaders show how to connect to a web api and post data, the DeepSleep version will put the esp32 device into a sleep mode between runs saving energy, which in a solar environment is a huge saving. **Circuit diagrams / pictures to come**

## License

This is an open and free repository: you can redistribute parts of it and/or modify them under
the terms of the GNU Affero General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option) any
later version. Please see the [LICENSE](./LICENSE.md) file for
the full text.