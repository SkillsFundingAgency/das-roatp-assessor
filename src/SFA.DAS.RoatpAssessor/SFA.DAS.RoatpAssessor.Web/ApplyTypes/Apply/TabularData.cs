﻿using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class TabularData
    {
        public string Caption { get; set; }
        public List<string> HeadingTitles { get; set; }
        public List<TabularDataRow> DataRows { get; set; }
    }

    public class TabularDataRow
    {
        public string Id { get; set; }
        public List<string> Columns { get; set; }
    }
}
