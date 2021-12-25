# Community SDK for Windows Subsystem for Linux

> :exclamation: **Fork**: This repository is based on the awsome work of https://github.com/wslhub/wsl-sdk-dotnet/tree/main/src/Wslhub.Sdk

This project contains a WSL API wrapper for Windows developers who wants to integrate WSL 
features into existing Windows applications. You can enumerate, query, executing WSL commands via C# classes.

## Supported frameworks

- .NET Standard 2.1
- .Net 5
- .Net 6

## Supported Operating Systems

- Windows 10 x64 16299 or higher
- Windows 11 x64

## How to use

There are two different implementations of `IWslApi`:
- A COM-based one that has specific requirements and requires special setup, too
- An alternative implementation that uses `wsl.exe` and (mostly) public information stored in the registry.

It's recommended to use the alternative because it's more versatile and easier to use.

## How to install

This package is available on `nuget.org`: https://www.nuget.org/packages/Community.Wsl.Sdk  
You can add a reference using `dotnet / nuget`:

```shell
dotnet add package Community.Wsl.Sdk
```

## API

### WSL Api

| Classes            | Description                                       | 
| ------------------ | ------------------------------------------------- |
| `ManagedWslApi`    | Get list of installed linux distributions.        | 
| `ManagedCommand`   | Execute command in specified linux distribution.  |
| `ComCommand`       | Get list of installed linux distributions.        |
| `ComBasedWslApi`   | Execute command in specified linux distribution.  |

### WSL command execution

There are two implementations, one uses the com based api to execute, the other calls `wsl.exe` internally.
The com based implementation has proven to be quite limited and buggy.

All implementations implement `ICommand`. Feel free to use your favorite DI or mocking framework to help with testing.

## Code Example

### Basic usage of wsl api

```csharp
using Community.Wsl.Sdk.Strategies.Api;

// Create instance
var api = new ManagedWslApi();

// When class ComBasedWslApi is used
// Place the code Wsl.InitializeSecurityModel() at the top of your application's Main() method.
// api.InitializeSecurity();

// Check if wsl is supported
bool isSupported = api.IsWslSupported();

// OR check if wsl is supported and also know why not:
string reason;
bool isSupported = api.IsWslSupported(out reason);

// Enumerate distro list
var distros = api.GetDistroList();

// Query default distro details
var defaultDistro = api.GetDefaultDistro();
```

### Basic command execution

```csharp
using Community.Wsl.Sdk.Strategies.Api;

// Setup
var api = new ManagedWslApi();

var distroName = api.GetDefaultDistro()!.Value.DistroName;

// Get command result 
var cmd = new ManagedCommand(
    distroName,
    "echo",
    new string[] { "-n", "test" },
    new CommandExecutionOptions() { StdoutDataProcessingMode = DataProcessingMode.String }
);

cmd.Start(); 

// Wait for the command to finish. Execution is executed asynchronically!
// WaitAndGetResults will fetch the data if instructed or drop it.
var result = cmd.WaitAndGetResults();

// result.Stdout is "test"
```

# How to use com-based `IWslApi` (not recommended)

You will need the below items to use the WSL APIs.

- Add an application manifest which describes your application is compatible with the 
  Windows 10 (GUID: `8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a`) and the `requestedExecutionLevel` tag with `asInvoker` level.
- Create instance of `ComBasedWslApi`
- Call `InitializeSecurityModel()` at the top of your application's `Main()` method.

#### Limitations

Due to the limitation of COM security model requirements of WSL APIs, 
you can not run this SDK within the Visual Studio Test Explorer or LINQPAD.

To circumvent this technical limitation, instruct `ComBasedWslApi` to use a different set of native methods:

```csharp
bool isTestEnv = ...;

// decide on implementation at run time
BaseNativeMethods nativeMethods = isTestEnv ? new StubNativeMethods() : new Win32NativeMethods();

// use first parameter of constructor to change the behaviour
var api = new ComBasedWslApi(nativeMethods);
```

Yet another approach is to use a different implementation of `IWslApi` altogether.
If you already use mocking frameworks & DI, use them to create a `test friedly` class, for example using `FakeItEasy`:

```
var api = A.Fake<IWslApi>();
```