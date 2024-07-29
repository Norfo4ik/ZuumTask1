# Azure Functions Data Fetch and Storage

## Overview

This project demonstrates the implementation of a serverless architecture using Azure Functions, Azure Storage (Table and Blob), and .NET Core 6. The solution fetches data from a public API every minute, logs the results in Azure Table Storage, stores the full payload in Azure Blob Storage, and provides APIs to list logs and retrieve payloads.

## Features

- **Periodic Data Fetching**: An Azure Function that triggers every minute to fetch data from a public API.
- **Logging**: Logs success or failure of each API call in Azure Table Storage.
- **Payload Storage**: Stores the full payload of successful API calls in Azure Blob Storage.
- **API Endpoints**:
  - List logs within a specific time period.
  - Fetch the payload from the blob for a specific log entry.

## Architecture

The architecture consists of the following components:
- **FetchDataFunction**: Timer-triggered Azure Function that fetches data from the API and logs the results.
- **ListLogsFunction**: HTTP-triggered Azure Function that lists logs within a specified time period.
- **FetchPayloadFunction**: HTTP-triggered Azure Function that fetches the payload from blob storage for a specific log entry.

### Diagram

![image](https://github.com/user-attachments/assets/430792a6-db5e-49e5-bfa3-75ed8803ab4e)

### Project Structure
## Functions:
- **FetchDataFunction**: Fetches data from the public API every minute.
- **ListLogsFunction**: Lists logs within a specified time period.
- **FetchPayloadFunction**: Fetches payloads from blob storage for a specific log entry.

## Services:
- **ApiService**: Handles API calls to the public API.
- **LoggingService**: Manages logging in Azure Table Storage.
- **BlobStorageService**: Manages storage of payloads in Azure Blob Storage.
  
##Models:
- **ApiResponse**: Represents the response from the public API.
- **LogEntity**: Represents a log entity in Azure Table Storage.
