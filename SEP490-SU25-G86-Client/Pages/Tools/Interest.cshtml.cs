using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages.Tools
{
    public class InterestModel : PageModel
    {
        [BindProperty] public double Principal { get; set; }
        [BindProperty] public double ContributionPerMonth { get; set; } // Đóng góp hàng tháng
        [BindProperty] public double Years { get; set; }
        [BindProperty] public double InterestRate { get; set; } // %/năm

        public double? FutureValue { get; set; }
        public double? TotalProfit { get; set; }

        public List<string> ChartLabels { get; set; } = new();
        public List<double> ChartPrincipal { get; set; } = new();
        public List<double> ChartFuture { get; set; } = new();

        public void OnPost()
        {
            double r = InterestRate / 100;   // Lãi suất năm
            int m = 12;                      // Ghép lãi hàng tháng (mặc định)
            int totalMonths = (int)(Years * m);
            double periodicRate = r / m;

            // 1. FV vốn gốc
            double futurePrincipal = Principal * Math.Pow(1 + periodicRate, totalMonths);

            // 2. FV đóng góp hàng tháng (PMT)
            double futurePMT = 0;
            if (ContributionPerMonth > 0)
            {
                futurePMT = ContributionPerMonth *
                            (Math.Pow(1 + periodicRate, totalMonths) - 1) / periodicRate;
            }

            // 3. Tổng FV
            FutureValue = futurePrincipal + futurePMT;

            // 4. Tổng vốn bỏ ra
            double totalPrincipal = Principal + ContributionPerMonth * totalMonths;
            TotalProfit = FutureValue - totalPrincipal;

            // 5. Biểu đồ theo từng năm
            for (int year = 0; year <= Years; year++)
            {
                int months = year * m;

                double fvYear = Principal * Math.Pow(1 + periodicRate, months);
                double fvPMTYear = 0;
                if (ContributionPerMonth > 0 && months > 0)
                {
                    fvPMTYear = ContributionPerMonth *
                                (Math.Pow(1 + periodicRate, months) - 1) / periodicRate;
                }

                double totalFVYear = fvYear + fvPMTYear;
                double totalPrincipalYear = Principal + ContributionPerMonth * months;

                ChartLabels.Add("Năm " + year);
                ChartFuture.Add(totalFVYear);
                ChartPrincipal.Add(totalPrincipalYear);
            }
        }
    }
}
