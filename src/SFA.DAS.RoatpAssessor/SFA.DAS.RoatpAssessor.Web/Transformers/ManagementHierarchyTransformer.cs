using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.RoatpAssessor.Web.Transformers
{
    public static class ManagementHierarchyTransformer
    {
        private static int NameInputColumn = 0;
        private static int JobRoleInputColumn = 1;
        private static int YearsInputColumn = 2;
        private static int MonthsInputColumn = 3;
        private static int AnotherOrgInputColumn = 4;
        private static int OrgDetailsInputColumn = 5;

        private static List<string> InputHeadingTitles = new List<string> { "Name", "Job role", "Years in role", "Months in role", "Part of another organisation", "Organisation details" };
        private static List<string> TransformHeadingTitles = new List<string> { "Full name", "Job role", "Time in role", "Is this person part of any other organisations?", "Enter the names of all these organisations" };

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

            if (tabularDataRowColumns.Count == InputHeadingTitles.Count)
            {
                columns = new List<string>
                {
                    tabularDataRowColumns[NameInputColumn],
                    tabularDataRowColumns[JobRoleInputColumn],
                    TransformYearsAndMonths(tabularDataRowColumns[YearsInputColumn], tabularDataRowColumns[MonthsInputColumn]),
                    tabularDataRowColumns[AnotherOrgInputColumn],
                    !string.IsNullOrEmpty(tabularDataRowColumns[OrgDetailsInputColumn]) ? tabularDataRowColumns[OrgDetailsInputColumn] : "Not applicable",
                };
            }

            return columns;
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

            if(years == 1)
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
