using Alma.Api.Sdk.Models;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Helpers
{
    public static class AlmaToEdFi
    {
        public static List<EdFiEducationOrganizationAddress> EducationOrganizationAddress(List<Address> AlmaAddresses)
        {
            var AddressList = new List<EdFiEducationOrganizationAddress>();
            foreach (var address in AlmaAddresses)
            {
                var Data = new EdFiEducationOrganizationAddress("uri://ed-fi.org/AddressTypeDescriptor#Mailing",
                    $"uri://ed-fi.org/StateAbbreviationDescriptor#{address.state}", address.city, address.zip, address.address, null, null, null, null, null, null, null, null, address.country, null);

                AddressList.Add(Data);
            }
            return AddressList;
        }

        public static List<EdFiEducationOrganizationInstitutionTelephone> InstitutionTelephones(List<Phone> AlmaPhones)
        {
            var Telephones = new List<EdFiEducationOrganizationInstitutionTelephone>();
            foreach (var phone in AlmaPhones)
            {
                var Data = new EdFiEducationOrganizationInstitutionTelephone("uri://ed-fi.org/InstitutionTelephoneNumberTypeDescriptor#Main", phone.number);
                Telephones.Add(Data);
            }
            return Telephones;
        }



        public static EdFiEducationOrganizationCategory EducationOrganizationCategoryTranslator(string AlmaOrganizationCategory)
        {
            switch (AlmaOrganizationCategory)
            {
                case "school":
                    return new EdFiEducationOrganizationCategory("uri://ed-fi.org/EducationOrganizationCategoryDescriptor#School");
                case "LEA":
                    return new EdFiEducationOrganizationCategory("uri://ed-fi.org/EducationOrganizationCategoryDescriptor#Local Education Agency");
                case "...":
                    return new EdFiEducationOrganizationCategory("uri://ed-fi.org/EducationOrganizationCategoryDescriptor#Other");
                default:
                    return new EdFiEducationOrganizationCategory("uri://ed-fi.org/EducationOrganizationCategoryDescriptor#School");
            }
        }
        public static List<EdFiSchoolGradeLevel> SchoolGradeLevelTranslator(List<string> AlmaGradeLevels)
        {
            var edfiGradeLevels = new List<EdFiSchoolGradeLevel>();

            foreach (var agl in AlmaGradeLevels)
            {
                switch (agl)
                {
                    case "PK":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Preschool/Prekindergarten"));
                        break;
                    case "K":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Kindergarten"));
                        break;
                    case "1st":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#First grade"));
                        break;
                    case "2nd":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Second grade"));
                        break;
                    case "3rd":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Third grade"));
                        break;
                    case "4th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Fourth grade"));
                        break;
                    case "5th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Fifth grade"));
                        break;
                    case "6th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Sixth grade"));
                        break;
                    case "7th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Seventh grade"));
                        break;
                    case "8th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Eighth grade"));
                        break;
                    case "9th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Ninth grade"));
                        break;
                    case "10th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Tenth grade"));
                        break;
                    case "11th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Eleventh grade"));
                        break;
                    case "12th":
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Twelfth grade"));
                        break;
                    default:
                        edfiGradeLevels.Add(new EdFiSchoolGradeLevel("uri://ed-fi.org/GradeLevelDescriptor#Other"));
                        break;
                }
            }
            return edfiGradeLevels;
        }

    }
}
