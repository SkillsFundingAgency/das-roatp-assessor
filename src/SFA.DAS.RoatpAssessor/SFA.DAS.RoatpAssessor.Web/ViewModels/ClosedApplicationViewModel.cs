﻿using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClosedApplicationViewModel : ApplicationViewModel
    {
        public string ModeratorName { get; set; }
        public string OutcomeStatus { get; set; }
        public DateTime OutcomeDate { get; set; }
    }
}