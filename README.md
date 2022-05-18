# Alma API to Ed-Fi API Console App

This .Net Core console application pulls from the Alma API and inserts into the Ed-Fi ODS/API.

## Instructions
1. Download the code: git clone git@github.com:Ed-Fi-Exchange-OSS/Ed-Fi-AlmaAPI-to-Ed-FiAPI-ConsoleApp.git
2. Compile the application and publish it.
3. Edit the appsettings.json file to include your Source and Destination APIs information.
4. Run the application by executing the EdFi.AlmaToEdFi.Cmd.exe
### Filter EndPoints by SchoolYearId

If we need to get the data filtered by SchoolYear, Set the SchoolYear section in the appsettings.json. StartDate && EndDate Should be valid dates with the format "YYYY-MM-DD" ,emty if dont want to filter by SchoolYearId


### AWS Parameter Store and AWS CloudWatch

#### Loading From AWS Parameter Store

Instead to keep your config information into appsettings.json file, now you can keep them in AWS Parameter Store.

First, we are goint to add our  appsettings values to AWS Parameter Store

LogIn to your AWS Console,click "Services" then "Management & Governance", find "Systems Manager" then click on "Parameter Store" and then click on "Create parameter".

Now we are going to configure the next fields : Name and Value, for this example lets focus on how to configure StartDate and EndDate from SchoolYear of our appsettings.json

Following the appsettings.json Structure:

{

    "Settings": {
        "AwsConfiguration": {
            "AWSAccessKey": "",
            "AWSSecretKey": "",
            "AWSRegion": "",
            "AWSLoggingGroupName": "AlmaApi"
        },

        "SourceAlmaAPISettings": {
            "Url": "",
            "Key": "",
            "Secret": "",
            "District": "",
            "SchoolYear": {
                "Comment": "StartDate && EndDate Should follow the format YYYY-MM-DD,emty if dont want to filter by SchoolYearId",
                "StartDate": "",
                "EndDate": ""
            }
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

**For StartDate**.

In the Name fiel, type : AlmaApi/Settings/SourceAlmaAPISettings/SchoolYear/StartDate
In the Value fiel, type: **2020-08-24** , the date should be a valid date and existing in Alma Api.

**For EndDate**.

In the Name fiel,type : AlmaApi/Settings/SourceAlmaAPISettings/SchoolYear/EndDate

In the Value fiel, type:  **2020-10-15** , the date should be a valid date and existing in Alma Api.

 AlmaApi/ is a prefix and is important to use that name to the aplication works, if you want to change the prefix name you need to change it in the application code as well;
 
#### Change AlmaApi Prefix.
	1. Go to the Program.cs of the application.
	2. Find the line with the code  configBuilder.AddSystemsManager("/AlmaApi" and Set your best prefix name.
	
	
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
