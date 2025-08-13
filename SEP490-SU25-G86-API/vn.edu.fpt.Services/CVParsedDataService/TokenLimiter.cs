namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public class TokenLimiter
    {
        // ước lượng thô: ~4 ký tự / token (tiếng Anh). Với tiếng Việt, dùng 3–4. Dưới đây lấy 4 để an toàn.
        public static string TrimToApproxTokens(string text, int maxTokens)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var approxPerToken = 4.0; // conservative
            var maxChars = (int)(maxTokens * approxPerToken);
            if (text.Length <= maxChars) return text;
            return text.Substring(0, maxChars);
        }
    }
}
