# Digital Apprenticeships Service

## RoATP Assessor

Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-roatp-assessor/blob/master/LICENSE.txt)

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|RoATP Assessor|
| Info | A service which allows EFSA staff members to assess & moderate RoATP applications. |
| Build | [![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Apprenticeships%20Providers/das-roatp-assessor?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2151&branchName=master) |
| Web  | https://localhost:5015/  |


### Developer Setup

#### Requirements

- Install [.NET Core 2.2 SDK](https://www.microsoft.com/net/download)
- Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
- Install [SQL Server 2017 Developer Edition](https://go.microsoft.com/fwlink/?linkid=853016)
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on at least v5.3)
- Install [Azure Storage Explorer](http://storageexplorer.com/) 
- Administrator Access

#### Setup

- Clone this repository
- Open Visual Studio as an administrator

##### Publish Database
RoATP Assessor does not have a database it accesses the RoATP database via its internal APIs.

##### Config

- Get the das-roatp-assessor configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-assessor/SFA.DAS.RoatpAssessor.json); which is a non-public repository.
- Create a Configuration table in your (Development) local Azure Storage account.
- Add a row to the Configuration table with fields: PartitionKey: LOCAL, RowKey: SFA.DAS.RoatpAssessor_1.0, Data: {{The contents of the local config json file}}.

##### To run a local copy you will also require 
To use RoATP admin functionality; you will need to have the SFA.DAS.RoATPService.Application.Api project running, from the das-roatp-service projects respectively.

- [RoATP Service](https://github.com/SkillsFundingAgency/das-roatp-service)

To access the assessment functionality for training providers:

- [QnA API](https://github.com/SkillsFundingAgency/das-qna-api)

#### Run the solution

- Set SFA.DAS.RoatpAssessor.Web as the startup project
- Running the solution will launch the site and API in your browser
- JSON configuration was created to work with dotnet run

-or-

- Navigate to src/SFA.DAS.RoatpAssessor.Web/
- run `dotnet restore`
- run `dotnet run`
- Open https://localhost:44350
