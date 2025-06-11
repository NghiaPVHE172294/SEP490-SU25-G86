using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class MatchedCvandJobPost
    {
        public int MatchedCvandJobPostId { get; set; }
        public int CvparsedDataId { get; set; }
        public int JobPostCriteriaId { get; set; }
        public double? MatchedScore { get; set; }

        public virtual CvparsedDatum CvparsedData { get; set; } = null!;
        public virtual JobCriterion JobPostCriteria { get; set; } = null!;
    }
}
