# Alma API to Ed-Fi API Console App

This .Net Core console application pulls from the Alma API and inserts into the Ed-Fi ODS/API.

## Instructions
1. Download the code: `git clone https://github.com/Ed-Fi-Exchange-OSS/Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp`
2. Compile the application and publish it.
3. Edit the appsettings.json file to include your Source and Destination APIs information.
4. Run the application by executing the EdFi.AlmaToEdFi.Cmd.exe

   4.1 Run the application by passing command line parameters
      #####   	Allowed Parameters:
      ```
       "--schoolYearFilter", "<School_Year_Filter>" The application gets only the information related with this School Year.
       
       "--schoolFilter", "<School_Filter>" The application gets only the information from this School.
       
       "--awsSourceConnectionName", "<your_aws_Source_Connection_Name>" To load the parameters from  AWS, allow to specify your SourceConnectionName If you have multiples SourceConnections(ParameterStoreProvider from appsettings.json should be configured as "AWSParamStore").   
       
	"--awsDestinationConnectionName", "<your_aws_Destination_Connection_Name>" To load the parameters from  AWS, allow to specify your ConnectionDestinationName If you have multiples ConnectionDestinationName(ParameterStoreProvider from appsettings.json should be configured as "AWSParamStore").
	```
### Filter EndPoints by School Year

If we need to get the data filtered by School Year, set the value you want to filter to the `SchoolYearFilter` property in the appsettings.json  (e.g. 2020-2021) and run the application.

###### Or with PowerShell
```powershell 
#Change your directory
 cd  "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\"
 $almaToEdFiExe = "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\EdFi.AlmaToEdFi.Cmd.exe"
 #Set your parameter (The parameter is requiered)
 $params = @(
    "--schoolYearFilter", "<School_Year_Filter>"  #(e.g. 2020-2021)
)
Write-host -ForegroundColor Cyan  $almaToEdFiExe $params
 &  $almaToEdFiExe $params
 
 
```
![image](https://user-images.githubusercontent.com/85459544/170791007-47579716-6033-4732-be8d-e243bce7fea9.png)

### AWS Parameter Store

#### Create AWS Parameters

Instead to keep your config information into appsettings.json file, now you can keep them in AWS Parameter Store.

First, we are goint to add our  appsettings values to AWS Parameter Store

LogIn to your AWS Console, click `Services` then `Management & Governance`, find `Systems Manager` then click on `Parameter Store` and then click on `Create parameter`.

Now we are going to configure the next fields : `Name` and `Value`, for this example lets focus on how to configure `SchoolYearFilter and DestinationLocalEducationAgencyId` of our appsettings.json

Following the appsettings.json structure:

`SchoolYearFilter`

In the Name field, type : `/AlmaApi/Connections/Alma/{SourceConnectionName}/SchoolYearFilter`

In the Value field, type your filter string ( e.g. ***2020-2021***).

`DestinationLocalEducationAgencyId`

In the Name field, type : `/AlmaApi/Connections/EdFi/{TargetConnectionName}/DestinationLocalEducationAgencyId`

In the Value field, type the value for DestinationLocalEducationAgencyId

    `AlmaApi/` is a prefix that the application needs to load the parameters from Aws.


#### Configure the other Connection Parameters
  #####  SourceConnection
  ```
	/AlmaApi/Connections/Alma/{SourceConnectionName}/Url
	/AlmaApi/Connections/Alma/{SourceConnectionName}/Key
	/AlmaApi/Connections/Alma/{SourceConnectionName}/Secret
	/AlmaApi/Connections/Alma/{SourceConnectionName}/District
  ```
   
   
   #####  TargetConnection
  ```
	/AlmaApi/Connections/EdFi/{TargetConnectionName}/Url
	/AlmaApi/Connections/EdFi/{TargetConnectionName}/Key
	/AlmaApi/Connections/EdFi/{TargetConnectionName}/Secret
	/AlmaApi/Connections/EdFi/{TargetConnectionName}/DestinationLocalEducationAgencyId
  ```
```
{

    "Settings": {
        "StartWithProcessor": 1,
        "Logging": {
            "LoggingProvider": "File", // or "AWSCloudWatch"
            "Region": "us-east-1",
            "LogGroup": "AlmaApi",
            "LogStreamNamePrefix": "AlmaLogs",
            "LogLevel": {
                "Default": "Debug",
                "System": "Information",
                "Microsoft": "Information"
            }
        },
        "AlmaAPI": {
            "ParameterStoreProvider": "appSettings", //appSettings  or "AWSParamStore"
            "Connections": {
                "Alma": {
                    "SourceConnection": {
                        "Comment": "SchoolYearFilter={InitialYear-FinalYear(e.g. 2019-2020) ;  SchoolFilter ={SchoolId to be filtered}}",
                        "Url": "",
                        "Key": "",
                        "Secret": "",
                        "District": "",
                        "SchoolYearFilter": "",
                        "SchoolFilter": ""
                    }
                },
                "EdFi": {
                    "RefreshTokenIn": "28", //Minutes
                    "TargetConnection": {
                        "Comment": "This should be your destination Ed-Fi ODS/API",
                        "Url": "",
                        "Key": "",
                        "Secret": "",
                        "DestinationLocalEducationAgencyId": 255901
                    }
                }
            }
        }
    }
}

```
#### Loading From AWS Parameter Store

1. Ensure you aready have configured all your parameters (at least the `Connections ` section)
2. Change the `ParameterStoreProvider` value to "AWSParamStore"
3. Run the application by executing the EdFi.AlmaToEdFi.Cmd.exe
4. If you have multiple sources or destinations , choose your source and destination

```powershell 
#Change your directory
 cd  "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\"
 $almaToEdFiExe = "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\EdFi.AlmaToEdFi.Cmd.exe"
 #Set your parameters(All parameters are requiered)
 $params = @(
    "--awsSourceConnectionName","<Your_Aws_Source_Connection_Name>",
    "--awsDestinationConnectionName", "<Your_Aws_Target_Connection_Name>"
)
Write-host -ForegroundColor Cyan  $almaToEdFiExe $params
 &  $almaToEdFiExe $params
 
```

### Logs

By default, the application creates log files, to review them go to the root directory and find the Log folder.

![image](https://user-images.githubusercontent.com/85459544/170787787-6eea7c24-6f77-41aa-ae6a-5fba26a55792.png)

#### AWS CloudWatch Logs

1. In the appsettings.json file, switch the value for "LoggingProvider" to "AWSCloudWatch".
2. Run the application.


### Support
For any support please create a ticket in the Ed-Fi tracker ticketing system: https://tracker.ed-fi.org/ and make sure you select the "Ed-Fi Support (EDFI)" project. 


## Legal Information

Copyright (c) 2022 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.
