# Frends.AmazonCloudWatch.GetMetricData

Retrieves metric data from Amazon CloudWatch.

[![Build status](https://github.com/FrendsPlatform/Frends.AmazonCloudWatch/actions/workflows/GetMetricData_main_release.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.AmazonCloudWatch/actions/workflows/GetMetricData_main_release.yml)
![Code Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.AmazonCloudWatch/Frends.AmazonCloudWatch.GetMetricData|main)
[![Repository License](https://img.shields.io/github/license/FrendsPlatform/Frends.AmazonCloudWatch?label=License)](https://github.com/FrendsPlatform/Frends.AmazonCloudWatch/blob/main/LICENSE)

## Installing

You can install the Task via Frends UI Task View.

## Defining queries

Queries can be provided in two ways, selected with `Input.QueryInputMode`:

- `Form` (default) - define the queries using the form fields in `Input.Queries`.
- `Json` - provide a JSON array matching the AWS `MetricDataQuery` structure in `Input.QueriesJson`. 
   Each query must contain exactly one of `MetricStat` or `Expression`. For `MetricStat`, `Period` is required. 
   Units use AWS notation (e.g. `Bytes/Second`). JSON is deserialized into AWS SDK objects, and AWS validates the query parameters.

```json
[
  {
    "Id": "cpu",
    "MetricStat": {
      "Metric": {
        "Namespace": "AWS/EC2",
        "MetricName": "CPUUtilization",
        "Dimensions": [{ "Name": "InstanceId", "Value": "i-1234567890abcdef0" }]
      },
      "Period": 300,
      "Stat": "Average",
      "Unit": "Percent"
    }
  },
  { "Id": "cpu2x", "Expression": "cpu * 2", "Label": "CPU x2" }
]
```


## Building

### Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.AmazonCloudWatch.git`

### Build the project

`dotnet build`

### Run tests

Run the tests

`dotnet test`

### Create a NuGet package

`dotnet pack --configuration Release`
