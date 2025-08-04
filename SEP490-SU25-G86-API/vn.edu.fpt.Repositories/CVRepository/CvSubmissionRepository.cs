using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository
{
    public class CvSubmissionRepository : ICvSubmissionRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public CvSubmissionRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateRecruiterNoteAsync(int submissionId, string recruiterNote)
        {
            var submission = await _context.Cvsubmissions.FindAsync(submissionId);
            if (submission == null)
                return false;

            submission.RecruiterNote = recruiterNote;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
