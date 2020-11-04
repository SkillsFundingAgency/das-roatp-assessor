using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class Apply
    {
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string ApplicationStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public string ModerationStatus { get; set; }

        public string Assessor1UserId { get; set; }
        public string Assessor1Name { get; set; }
        public string Assessor1ReviewStatus { get; set; }
        public string Assessor2UserId { get; set; }
        public string Assessor2Name { get; set; }
        public string Assessor2ReviewStatus { get; set; }

        public ApplyData ApplyData { get; set; }

        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }

    public class ApplyData
    {
        public ApplyDetails ApplyDetails { get; set; }
        public List<ApplySequence> Sequences { get; set; }
        public ModeratorReviewDetails ModeratorReviewDetails { get; set; }
    }

    public class ApplyDetails
    {
        public string ReferenceNumber { get; set; }
        public string UKPRN { get; set; }
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public int ProviderRoute { get; set; }
        public string ProviderRouteName { get; set; }
        public DateTime? ApplicationSubmittedOn { get; set; }
        public Guid? ApplicationSubmittedBy { get; set; }        
    }

    public class ApplySequence
    {
        public Guid SequenceId { get; set; }
        public int SequenceNo { get; set; }
        public List<ApplySection> Sections { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool NotRequired { get; set; }
        public bool Sequential { get; set; }
        public string Description { get; set; }
    }

    public class ApplySection
    {
        public Guid SectionId { get; set; }
        public int SectionNo { get; set; }
        public string Status { get; set; }
        public bool NotRequired { get; set; }
    }

    public class ModeratorReviewDetails
    {
        public string ModeratorName { get; set; }
        public string ModeratorUserId { get; set; }
        public DateTime? OutcomeDateTime { get; set; }
        public DateTime? ClarificationRequestedOn { get; set; }
        public string ModeratorComments { get; set; }
    }
}
