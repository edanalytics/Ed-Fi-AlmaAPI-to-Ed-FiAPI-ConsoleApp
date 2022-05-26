# Alma API to Ed-Fi API Console App

This .Net Core console application pulls from the Alma API and inserts into the Ed-Fi ODS/API.

## Instructions
1. Download the code: git clone git@github.com:Ed-Fi-Exchange-OSS/Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp.git
2. Compile the application and publish it.
3. Edit the appsettings.json file to include your Source and Destination APIs information.
4. Run the application by executing the EdFi.AlmaToEdFi.Cmd.exe
	
### Filter EndPoints by School Year

If we need to get the data filtered by School Year, set the value you want to filter to the `SchoolYearFilter` property (e.g. 2020-2021) in the appsettings.json 

###### Or with PowerShell
```powershell 
#Change your paths
 cd  "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\"
 $almaToEdFiExe = "C:\Ed-Fi\Exchange-OSS\Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp\EdFi.OdsApi.SdkClient\bin\Debug\netcoreapp3.1\EdFi.AlmaToEdFi.Cmd.exe"
 #Set your parameter
 $params = @(
    "--schoolYearFilter", "2020-2021"
)
Write-host -ForegroundColor Cyan  $apiLoaderExe $params
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
