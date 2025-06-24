using System.Text.RegularExpressions;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.SynonymService
{
    public class SynonymService : ISynonymService
    {
        private readonly Dictionary<string, List<string>> _synonyms = new()
    {
        { "it", new List<string> { "công nghệ thông tin", "developer", "phần mềm", "lập trình viên", "backend", "frontend", "ai", "an ninh mạng" } }, 
        { "cntt", new List<string> { "công nghệ thông tin", "developer", "phần mềm" } },
        { "phan mem", new List<string> { "phần mềm", "developer" } },
    };

        public List<string> ExpandKeywords(string keyword)
        {
            keyword = keyword.ToLower().Trim();
            var results = new List<string> { keyword };

            if (_synonyms.TryGetValue(keyword, out var synonyms))
            {
                results.AddRange(synonyms);
            }

            return results.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
