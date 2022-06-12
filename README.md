# Community SDK for Windows Subsystem for Linux

> :exclamation: **Fork**: This repository is based on the awsome work of [wslhub/wsl-sdk-dotnet](https://github.com/wslhub/wsl-sdk-dotnet)

This project contains a WSL API wrapper for Windows developers who wants to integrate WSL 
features into existing Windows applications. You can enumerate, query, executing WSL commands via C# classes.

## Supported frameworks

- .NET Standard 2.1
- .Net 6

## Supported Operating Systems

- Windows 10 x64 16299 or higher
- Windows 11 x64

## How to use

There are one default implementations of `IWslApi`.

It uses the `wsl.exe` executable and (mostly) public information stored in the registry.

> :exclamation: Com-Api: WSL has a com-based api that **could** also used instead. Using com-apis has several disadvantages including security related issues. Using the managed alternative with `wsl.exe` has proven to be more versatile and easier to use.

## How to install

This package is available on [nuget.org](https://www.nuget.org/packages/Community.Wsl.Sdk).  
You can add a reference using `dotnet`:

```shell
dotnet add package Community.Wsl.Sdk
```

## API

### WSL Api

| Class     | Description                                      |
| --------- | ------------------------------------------------ |
| `WslApi`  | Get list of installed linux distributions.       |
| `Command` | Execute command in specified linux distribution. |

## Code Example

### Basic usage of wsl api

```csharp
using Community.Wsl.Sdk.Strategies.Api;

// Create instance
var api = new WslApi();

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
var api = new WslApi();

var distroName = api.GetDefaultDistro()!.Value.DistroName;

// Get command result 
var cmd = new Command(
    distroName,
    "echo",
    new string[] { "-n", "test" },
    new CommandExecutionOptions() { StdoutDataProcessingMode = DataProcessingMode.String }
);
// execute the command and wait for the result (blocks current thread)
var result = cmd.StartAndGetResults();
// OR start and wait asynchronously
// var result = await cmd.StartAndGetResultsAsync();

// result.Stdout is "test"
```

# Unit tests and Mocks

Both `WslApi` and `Command` implement interfaces, namely `IWslApi` and `ICommand`. If you already use mocking frameworks & DI, use them to create a `test friedly` class, for example using `FakeItEasy`:

```
var api = A.Fake<IWslApi>();
```

You can also mock specific parts of the implementation by passing custom implementations in the constructor:

```csharp
/*
Signature of the constructor:
public WslApi(
    IRegistry? registry = null,
    IIo? io = null,
    IEnvironment? environment = null
)
*/

// mock only the IIo logic
var api = new WslApi(
    io: A.Fake<IIo>()
);
```

# Migrate from v1 to v2

Breaking changes:

* `ComBasedWslApi` has been removed

* `ComCommand` has been removed

* `ManagedWslApi` has been renamed to `WslApi`

* `ManagedCommand` has been remamed to `Command`

* Changed namespace `Community.Wsl.Sdk.Strategies.Command` to `Community.Wsl.Sdk.Strategies.Commands`

Please use the managed api (`Managed{WslApi,Command}`). It has the same features and is easier to use.

# Migrate from v2 to v3

Breaking changes:

* Support for Net. 5 has been removed. Net. 5 is [out of support since May 10, 2022](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).

Please upgrade to a supported .NET and .NET Core runtime version.
