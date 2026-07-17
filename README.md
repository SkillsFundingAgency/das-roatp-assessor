## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

## RoATP Assessor

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-roatp-assessor/blob/master/LICENSE)

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Apprenticeships%20Providers/das-roatp-assessor?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2151&branchName=master)
[![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-assessor&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-roatp-assessor)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


## About
This project is part of the Digital Apprenticeship Service (DAS) and is used to assess applications from training providers to be added to the Register of Apprenticeship Training Providers (RoATP).

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* Azurite or similar local storage emulator
* Visual studio or similar IDE

### Dependencies
- Apply internal api: https://github.com/SkillsFundingAgency/das-apply-service/tree/master/src/SFA.DAS.ApplyService.InternalApi
- DfE Signin

### Config

* Create a Configuration table in your (Development) local storage account.
* Obtain the local config json from the das-employer-config for [das-roatp-assessor](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-assessor/SFA.DAS.RoatpAssessor.json) 
  * PartitionKey: LOCAL
  * RowKey: SFA.DAS.Roatp.Assessor.Web_1.0
  * Data: {The contents of the local config json file}

  In the web project, if not exist already, add `AppSettings.Development.json` file with following content:
```json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "cdn": {
    "url": "https://das-prd-frnt-end.azureedge.net"
  },
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.RoatpAssessor,SFA.DAS.Provider.DfeSignIn",
  "ConnectionStrings": {
    "Redis": ""
  },
  "EnvironmentName": "LOCAL"
  
}
```  

## Technologies
* .Net 10.0
* NUnit
* Moq
