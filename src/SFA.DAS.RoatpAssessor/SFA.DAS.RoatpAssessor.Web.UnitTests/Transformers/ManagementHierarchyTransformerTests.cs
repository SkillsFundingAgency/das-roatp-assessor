using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Transformers;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Transformers
{
    [TestFixture]
    public class ManagementHierarchyTransformerTests
    {
        private readonly List<string> _InputHeadingTitles = new List<string> { "Name", "Job role", "Years in role", "Months in role", "Part of another organisation", "Organisation details" };
        private readonly List<string> _TransformedHeadingTitles = new List<string> { "Full name", "Job role", "Time in role", "Is this person part of any other organisations?", "Enter the names of all these organisations" };
        private readonly string _NotApplicable = "Not applicable";     

        [TestCase("name", "role", "1", "1", "1 year 1 month", "No", null)]
        [TestCase("name", "role", "1", "1", "1 year 1 month", "Yes", "organisation")]
        [TestCase("name", "role", "1", "2", "1 year 2 months", "No", null)]
        [TestCase("name", "role", "1", "2", "1 year 2 months", "Yes", "organisation")]
        [TestCase("name", "role", "2", "1", "2 years 1 month", "No", null)]
        [TestCase("name", "role", "2", "1", "2 years 1 month", "Yes", "organisation")]
        [TestCase("name", "role", "2", "2", "2 years 2 months", "No", null)]
        [TestCase("name", "role", "2", "2", "2 years 2 months", "Yes", "organisation")]
        public void Transformer_Produces_Expected_Result(string name, string jobRole, string years, string months, string yearsAndMonths, string anotherOrg, string orgDetails)
        {
            var input = new TabularData
            {
                HeadingTitles = _InputHeadingTitles,
                DataRows = new List<TabularDataRow>
                {
                    new TabularDataRow
                    {
                           Columns = new List<string> { name, jobRole, years, months, anotherOrg, orgDetails }
                    }
                }
            };

            var expectedResult = new TabularData
            {
                HeadingTitles = _TransformedHeadingTitles,
                DataRows = new List<TabularDataRow>
                {
                    new TabularDataRow
                    {
                           Columns = new List<string> { name, jobRole, yearsAndMonths, anotherOrg, !string.IsNullOrWhiteSpace(orgDetails) ? orgDetails : _NotApplicable }
                    }
                }
            };

            var actualResult = ManagementHierarchyTransformer.Transform(input);

            Assert.IsNotNull(actualResult);
            Assert.IsNotNull(actualResult.HeadingTitles);
            Assert.IsNotNull(actualResult.DataRows);
            CollectionAssert.AreEquivalent(expectedResult.HeadingTitles, actualResult.HeadingTitles); 
            CollectionAssert.AreEquivalent(expectedResult.DataRows[0].Columns, actualResult.DataRows[0].Columns);
        }
    }
}
