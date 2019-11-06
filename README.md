# ListAppCenterUsers
### A small utility to dump the users from a Microsoft App Center organization, and all of its apps and groups.

ListAppCenterUsers is intended as a demo for how to use the App Center REST API for account management. No warranties about its correctness or performance are offered, but best-effort has been made to ensure that it runs correctly.

### Building the code
ListAppCenterUsers uses .NET Core 3.0, which you can download from [dot.net](https://dotnet.microsoft.com/download/dotnet-core/3.0). Build the project by running

    dotnet build ListAppCenterUsers.csproj

or by using your favorite editor or IDE.

### Output file

All output is shown in the terminal window, and optionally also written to an output file.

### Examples:
List all users associated with an App Center organization:

    ListAppCenterUsers --apitoken myApiToken --organization My-Organization-Name

Show help:

    ListAppCenterUsers --help
    ListAppCenterUsers /?

### Parameters:

  `--apitoken` _(required)_ The API token for your organization

  `--organization` _(required)_ The name of your organization (i.e. the slug used in AppCenter URL's)

  `--outputfile` The name of the output file

  `--help | /?` Show help message

