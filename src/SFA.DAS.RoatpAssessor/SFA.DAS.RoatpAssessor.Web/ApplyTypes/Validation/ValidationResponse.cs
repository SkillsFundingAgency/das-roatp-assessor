﻿using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation
{
    public class ValidationResponse
    {
        public ValidationResponse()
        {
            if (Errors == null) { Errors = new List<ValidationErrorDetail>(); }
        }

        public List<ValidationErrorDetail> Errors { get; set; }
        public bool IsValid => Errors.Count == 0;
    }
}
