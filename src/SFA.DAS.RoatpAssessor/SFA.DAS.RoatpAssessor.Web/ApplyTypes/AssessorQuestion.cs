﻿namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes
{
    public class AssessorQuestion
    {
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public string QuestionBodyText { get; set; }
        public string InputType { get; set; }
        public string InputPrefix { get; set; }
        public string InputSuffix { get; set; }
    }
}