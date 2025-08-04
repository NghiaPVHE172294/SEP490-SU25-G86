using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService
{
    public class CvSubmissionService : ICvSubmissionService
    {
        private readonly ICvSubmissionRepository _repository;

        public CvSubmissionService(ICvSubmissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> UpdateRecruiterNoteAsync(int submissionId, string recruiterNote)
        {
            return await _repository.UpdateRecruiterNoteAsync(submissionId, recruiterNote);
        }
    }

}
