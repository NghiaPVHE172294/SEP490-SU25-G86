namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService
{
    public interface ICvSubmissionService
    {
        Task<bool> UpdateRecruiterNoteAsync(int submissionId, string recruiterNote);
    }

}
