using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.RoatpAssessor.Web.Transformers
{
    public static class ManagementHierarchyTransformer
    {
        private const int FirstNameInputColumn = 0;
        private const int LastNameInputColumn = 1;
        private const int JobRoleInputColumn = 2;
        private const int YearsInputColumn = 3;
        private const int MonthsInputColumn = 4;
        private const int AnotherOrgInputColumn = 5;
        private const int OrgDetailsInputColumn = 6;
        private const int DobMonth = 7;
        private const int DobYear = 8;
        private const int Email = 9;
        private const int ContactNumber = 10;

        private static readonly List<string> InputHeadingTitles = new List<string> { "First name", "Last name","Job role", "Years in role", "Months in role", "Part of another organisation", "Organisation details", "Date of birth", "Email", "Contact number" };
        private static readonly List<string> TransformHeadingTitles = new List<string> { "First name", "Last name", "Job role", "Time in role", "Is this person part of any other organisations?", "Enter the names of all these organisations","Date of birth","Email", "Contact number" };

        public static TabularData Transform(TabularData tabularData)
        {
            var managementHierarchy = new TabularData
            {
                Caption = null,
                HeadingTitles = new List<string>(TransformHeadingTitles),
                DataRows = new List<TabularDataRow>()
            };

            if (tabularData?.DataRows != null && tabularData.DataRows.Any())
            {
                managementHierarchy.DataRows = new List<TabularDataRow>(tabularData.DataRows.Select(dr => { return dr.TransformDataRow(); }));
            }

            return managementHierarchy;
        }

        private static TabularDataRow TransformDataRow(this TabularDataRow tabularDataRow)
        {
            var dataRow = new TabularDataRow
            {
                Id = tabularDataRow.Id,
                Columns = new List<string>()
            };

            if (tabularDataRow.Columns != null && tabularDataRow.Columns.Any())
            {
                dataRow.Columns = tabularDataRow.Columns.TransformColumns();
            }

            return dataRow;
        }

        private static List<string> TransformColumns(this List<string> tabularDataRowColumns)
        {
            var columns = new List<string>();

            if (tabularDataRowColumns.Count - 1 == InputHeadingTitles.Count)
            {
                columns = new List<string>
                {
                    tabularDataRowColumns[FirstNameInputColumn],
                    tabularDataRowColumns[LastNameInputColumn],
                    tabularDataRowColumns[JobRoleInputColumn],
                    TransformYearsAndMonths(tabularDataRowColumns[YearsInputColumn], tabularDataRowColumns[MonthsInputColumn]),
                    tabularDataRowColumns[AnotherOrgInputColumn],
                    !string.IsNullOrEmpty(tabularDataRowColumns[OrgDetailsInputColumn]) ? tabularDataRowColumns[OrgDetailsInputColumn] : "Not applicable",
                    ConvertIntegerToMonth(tabularDataRowColumns[DobMonth]) + " " + tabularDataRowColumns[DobYear],
                    tabularDataRowColumns[Email],
                    tabularDataRowColumns[ContactNumber]
                };
            }

            return columns;
        }

        private static string ConvertIntegerToMonth(string monthInteger)
        {
            switch (monthInteger)
            { 
                case "1": return "Jan";
                case "2": return "Feb";
                case "3": return "Mar";
                case "4": return "Apr";
                case "5": return "May";          
                case "6": return "Jun";          
                case "7": return "Jul";
                case "8": return "Aug";
                case "9": return "Sep";
                case "10": return "Oct";
                case "11": return "Nov";
                case "12": return "Dec";
                default: return "";
 
            }

        }
        private static string TransformYearsAndMonths(string yearString, string monthString)
        {
            int years = 0;
            int months = 0;

            if (int.TryParse(yearString, out var yearResult))
            {
                years = yearResult;
            }

            if (int.TryParse(monthString, out var monthResult))
            {
                months = monthResult;
            }

            StringBuilder sb = new StringBuilder();

            if (years == 1)
            {
                sb.Append($"{years} year");
            }
            else
            {
                sb.Append($"{years} years");
            }

            sb.Append(" ");

            if (months == 1)
            {
                sb.Append($"{months} month");
            }
            else
            {
                sb.Append($"{months} months");
            }

            return sb.ToString();
        }
    }
}
