using SEP490_SU25_G86_API.Models;
using vn.edu.fpt.DTOs;

namespace vn.edu.fpt.DTOs.GeminiDTO
{
    public static class MatchedCvandJobPostMapper
    {
        public static MatchedCvandJobPostDto ToDto(MatchedCvandJobPost entity)
        {
            return new MatchedCvandJobPostDto
            {
                MatchedCvandJobPostId = entity.MatchedCvandJobPostId,
                CvparsedDataId = entity.CvparsedDataId,
                JobPostCriteriaId = entity.JobPostCriteriaId,
                ExperienceScore = entity.ExperienceScore,
                SkillsScore = entity.SkillsScore,
                EducationLevelScore = entity.EducationLevelScore,
                JobTitlesScore = entity.JobTitlesScore,
                LanguagesScore = entity.LanguagesScore,
                CertificationsScore = entity.CertificationsScore,
                SummaryScore = entity.SummaryScore,
                WorkHistoryScore = entity.WorkHistoryScore,
                ProjectsScore = entity.ProjectsScore,
                
                
                TotalScore = entity.TotalScore
            };
        }
    }
}
