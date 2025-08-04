namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository
{
    public interface ICvSubmissionRepository
    {
        Task<bool> UpdateRecruiterNoteAsync(int submissionId, string recruiterNote);
    }

}
