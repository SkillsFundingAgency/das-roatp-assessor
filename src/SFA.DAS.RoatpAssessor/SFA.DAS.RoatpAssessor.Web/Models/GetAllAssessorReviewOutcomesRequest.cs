using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetAllAssessorReviewOutcomesRequest
    {
        public Guid ApplicationId { get; set; }
        public int AssessorType { get; set; }
        public string UserId { get; set; }
    }
}
