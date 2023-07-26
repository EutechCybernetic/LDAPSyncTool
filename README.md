# iviva LDAP Sync Tool

This program can be used to sync users from an Active Directory (or any LDAP-compatible) server with iviva.

This tool is meant to be run within the network where Active Directory is hosted. Each time it is run, it will query the LDAP server for users and send them to iviva for synchronization.

This tool is non-interactive and designed to run on a scheduled task.

## Prerequisites

The following information is required to use this tool:

* `LDAP Server Address` - The ip/host of the active directory or LDAP server to connect to
* `LDAP User` - A user name for connecting to the LDAP server. This will likely be a fully qualified name like "uid=snow,ou=Users,o=orgunitabc,dc=myorg,dc=com"
* `LDAP Password` - The password for the user name specified
* `LDAP Base DN` - An expression that specifies the base domain name to use for querying for users. Example: `o=org,dc=mydc,dc=com`
* `LDAP Query` - This is the LDAP query to be issued to retrieve all the users from the directory. This query must be crafted to include all the users that need to be synced. Example: `(objectclass=*)`. This retrieves all users who have a objectclass attribute
* `Attributes` - A list of attributes to retrieve for each user, and the corresponding attribute within iviva to map this user to. Consult your iviva contact or documentation for available attributes to configure
* `iviva Url` - The url of the iviva application that user data is supposed to be synced to. 
* `iviva Api Key` - The api key used to connect to iviva. You will need to have an API Key issued to you. The API Key should have the `cansyncldapexternal` app role. (In the user roles configuration UI in iviva, look for "Allowed to run an external LDAP sync tool")

## Configuration

The configuration used by this tool will be read from a YAML file.
The structure of the yaml file is as follows:

```
ldap:
    server: ldap.myserver.com
    dn: "o=orgunitabc,dc=myorg,dc=com"
    user: "uid=snow,ou=Users,o=orgunitabc,dc=myorg,dc=com"
    password: sssshhhhh
    query: "(objectclass=*)"
    getAllAttributes: false
    batchSize: 100
    attributes:
        mail: Email
        displayName: LoginID
iviva:
    url: https://iviva
    apiKey: SC123:234
pageSize: 10
```

Here's what these represent,

### ldap.server

The host/ip of the server to connect to for LDAP or active directory. The tool supports the following formats,

For unsecured connection to the server on standard (**389**) or custom port,

- ldap.jumpcloud.com
- ldap.jumpcloud.com:389

For secured connection to the server on standard (**636**) or custom port,

- ldaps://ldap.jumpcloud.com
- ldaps://ldap.jumpcloud.com:636

**Note:** Invalid port would lead to `Not a valid server port` error.

### ldap.dn

The BaseDN that will be used to search for users.

### ldap.user

The user name to use to connect to LDAP. This will probably have to be a fully qualified expression like `uid=snow,ou=Users,o=orgunitabc,dc=myorg,dc=com`

### ldap.password

The password to use to connect to the server. Instead of setting it here - you can override it with an environment variable called `LDAP_SYNC_TOOL_PASSWORD`.

**Note:** The environment variable takes precedence over what is configured here.

### ldap.query

The actual LDAP query to issue to fetch the required users. This query must return all users that need to be synced with iviva

### ldap.batchSize

When set, limits the number of items that get fetched on each query. Multiple queries will be made until all items are recovered

### ldap.getAllAttributes

When set to true, the query will return all attributes that each user has, regardless of what has been configured in the `attributes` dictionary

### ldap.attributes

This is a dictionary mapping LDAP attributes to the corresponding iviva attributes.
Note that `userAccountControl` is always added and you should not have to explicitly map it here.

At the minimum, iviva expects these attributes:
* LoginID
* Email
* FirstName

So these 3 need to be mapped to corresponding attributes in ldap.

**Example**

```
attributes:
    mail: Email
    displayName: FirstName
    sAMAccountName: LoginID
```

This maps the LDAP attribute `mail` to the iviva attribute `Email`. The ldap `sAMAAccountName` is the iviva user's LoginID.

### iviva.url
The full https url of the iviva application to sync users to.

**Example:** `https://foo.iviva.cloud`

### iviva.apikey
The api key to use to talk to the iviva api.

### pageSize

Number of users to sync to iviva at a time.

## Usage

To run the tool, specify the path to the configuration file using the `-c` parameter.

```
.\LDAPSyncTool.exe -c  e:\ldap.yaml
```

### Simulation Mode

You can run the tool in simulation mode - meaning it will query the LDAP/active directory server but not actually send the data to iviva. It will log it on the console.

Use the `-s` or `--simulation-mode` command line switch for that.

```
.\LDAPSyncTool.exe -c e:\ldap.yaml --simulation-mode
```

## Command Line Options

You can run the tool with the `--help` option to see available command line parameters.

## Logging

Currently all log entries are logged to the console (ie, `stdout`). You can redirect them to a file as required.

You can use the `-v` or `--verbose` command to log extra information for debugging purposes.

## Building the code

### Prerequisites
You need .NET 6 SDK

### Building

Clone this repository and run `dotnet publish -c Release --self-contained -r win10-x64 -o <directory-to-publish-to>`

This will create a folder with the `LDAPSyncTool.exe` executable inside it.

Replace `win10-x64` with any runtime identifier from here: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog.
