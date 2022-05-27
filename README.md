# Alma API to Ed-Fi API Console App

This .Net Core console application pulls from the Alma API and inserts into the Ed-Fi ODS/API.

## Instructions
1. Download the code: git clone git@github.com:Ed-Fi-Exchange-OSS/Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp.git
2. Compile the application and publish it.
3. Edit the appsettings.json file to include your Source and Destination APIs information.
4. Run the application by executing the EdFi.AlmaToEdFi.Cmd.exe
	
### Filter EndPoints by School Year

If we need to get the data filtered by School Year, set the value you want to filter to the `SchoolYearFilter` property (e.g. 2020-2021) in the appsettings.json and run the application.

###### Or with PowerShell
```powershell 
#Change your directory
 cd  "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\"
 $almaToEdFiExe = "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\EdFi.AlmaToEdFi.Cmd.exe"
 #Set your parameter (The parameter is requiered)
 $params = @(
    "--schoolYearFilter", "2020-2021"
)
Write-host -ForegroundColor Cyan  $almaToEdFiExe $params
 &  $almaToEdFiExe $params
 
 
```
![image](https://user-images.githubusercontent.com/85459544/170791007-47579716-6033-4732-be8d-e243bce7fea9.png)

### AWS Parameter Store

#### Loading From AWS Parameter Store

Instead to keep your config information into appsettings.json file, now you can keep them in AWS Parameter Store.

First, we are goint to add our  appsettings values to AWS Parameter Store

LogIn to your AWS Console, click `Services` then `Management & Governance`, find `Systems Manager` then click on `Parameter Store` and then click on `Create parameter`.

Now we are going to configure the next fields : `Name` and `Value`, for this example lets focus on how to configure `SchoolYearFilter` of our appsettings.json

Following the appsettings.json structure:

In the Name field, type : `AlmaApi/Settings/SourceAlmaAPISettings/SchoolYearFilter`

    `AlmaApi/` is a prefix and is important to use that name to the aplication works.
In the Value field, type your filter string ( e.g. ***2020-2021***).



```
{
    "Settings": {
    
        "SourceAlmaAPISettings": {
            "Url": "",
            "Key": "",
            "Secret": "",
            "District": "",
            "SchoolYearFilter": ""
        },

        "DestinationEdFiAPISettings": {
            "Comment": "This should be your destination Ed-Fi ODS/API",
            "Url": "",
            "Key": "",
            "Secret": "",
            "DestinationLocalEducationAgencyId": 255901
        }
    }
}
```
 Now you can run this script with PowerShell to Run the application getting your values from Aws Parameter Store.

```powershell 
#Change your directory
 cd  "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\"
 $almaToEdFiExe = "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\EdFi.AlmaToEdFi.Cmd.exe"
 #Set your parameters(All parameters are requiered)
 $params = @(
    "--awsKey", "",
    "--awsSecret", "",
    "--awsRegion", ""
)
Write-host -ForegroundColor Cyan  $almaToEdFiExe $params
 &  $almaToEdFiExe $params
 
```

### Logs

By default, the application creates log files, to review them go to the root directory and find the Log folder.

![image](https://user-images.githubusercontent.com/85459544/170787787-6eea7c24-6f77-41aa-ae6a-5fba26a55792.png)

#### AWS CloudWatch Logs

If you already have CloudWatch enabled for logs, run the next Script with PowerShell to execute the application with Aws CloudWatch logs.


```powershell 
#Change your directory
 cd  "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\"
 $almaToEdFiExe = "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\EdFi.AlmaToEdFi.Cmd.exe"
 #Set your parameters(All parameters are requiered)
 $params = @(
    "--awsKey", "",
    "--awsSecret", "",
    "--awsRegion", "",
    "--awsLoggingGroupName", ""
)
Write-host -ForegroundColor Cyan  $almaToEdFiExe $params
 &  $almaToEdFiExe $params
 
```
### Support
For any support please create a ticket in the Ed-Fi tracker ticketing system: https://tracker.ed-fi.org/ and make sure you select the "Ed-Fi Support (EDFI)" project. 


## Legal Information

Copyright (c) 2021 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.
