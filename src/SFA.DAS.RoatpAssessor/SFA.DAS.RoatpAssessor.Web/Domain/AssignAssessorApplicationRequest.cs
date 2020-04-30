﻿using System;

namespace SFA.DAS.RoatpAssessor.Web.Domain
{
    public class AssignAssessorApplicationRequest
    {
        public AssignAssessorApplicationRequest(int assessorNumber, string assessorUserId, string assessorName)
        {
            AssessorNumber = assessorNumber;
            AssessorUserId = assessorUserId;
            AssessorName = assessorName;
        }

        public int AssessorNumber { get; }
        public string AssessorUserId { get; }
        public string AssessorName { get; }
    }
}
