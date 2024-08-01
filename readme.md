
# Lite CosmosDB Explorer

A cross-platform client explorer for Microsoft Azure CosmosDB (Windows, MacOS, Linux).

## Features

- Connect to Azure CosmosDB and display databases and containers
- Filter CosmosDB documents
- Create/Read/Update/Delete a document
- Execute a query and view results and RUs
- Save the last executed queries (they are saved automatically and available in a dropdown)

## How to Download
Win-x64, macOS-x64, macOS-arm64 and linux-x64 pre-built binaries can be found on Releases page.

## How to run
Windows
- Right-click -> Open

You can allow windows defender to start this application:

- click on More Info -> Run anyway

## macOS
- Right-click -> Open

You can allow macOS to start this application by enabling Developer tools for Terminal:

System Preferences -> Security & Privacy -> Privacy, select "Developer Tools" on the left, check terminal on the right.

You can make CosmosExplorer.Avalonia file executable by:

- chmod +x CosmosExplorer.Avalonia

## Linux (CentOS, Debian, Fedora, Ubuntu and derivatives)
- Right-click -> Run

## How to Start
Add your connectrion string on tab Connections:
- Connection string name: The display name of your connection string
- Connection string: Copy PRIMARY KEY or SECONDARY CONNECTION STRING from the Keys menu on the Azure Portal.