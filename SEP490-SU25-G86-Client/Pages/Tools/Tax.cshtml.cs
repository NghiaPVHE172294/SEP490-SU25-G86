using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages.Tools
{
    public class TaxModel : PageModel
    {
        [BindProperty] public decimal GrossIncome { get; set; }
        [BindProperty] public decimal InsuranceSalary { get; set; }
        [BindProperty] public int Dependents { get; set; }
        [BindProperty] public int Region { get; set; }
        [BindProperty] public string Period { get; set; } = "2025";
        [BindProperty] public string InsuranceOption { get; set; } = "default";

        // Kết quả chi tiết
        public decimal BHXH { get; set; }
        public decimal BHYT { get; set; }
        public decimal BHTN { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal IncomeBeforeTax { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetIncome { get; set; }
        public bool Calculated { get; set; }
        public decimal PersonalDeductionPublic => PersonalDeduction;
        public decimal DependentDeductionPublic => DependentDeduction;

        // Giảm trừ
        private const decimal PersonalDeduction = 11000000;
        private const decimal DependentDeduction = 4400000;

        public void OnPost()
        {
            // 1. Lấy lương đóng bảo hiểm
            decimal baseSalary = InsuranceOption == "custom" && InsuranceSalary > 0
                                ? InsuranceSalary
                                : GrossIncome;

            // 2. Tính bảo hiểm
            BHXH = baseSalary * 0.08m;
            BHYT = baseSalary * 0.015m;
            BHTN = baseSalary * 0.01m;
            TotalInsurance = BHXH + BHYT + BHTN;

            // 3. Thu nhập trước thuế
            IncomeBeforeTax = GrossIncome - TotalInsurance;

            // 4. Thu nhập chịu thuế
            TaxableIncome = IncomeBeforeTax - PersonalDeduction - Dependents * DependentDeduction;
            if (TaxableIncome < 0) TaxableIncome = 0;

            // 5. Thuế TNCN
            TaxAmount = CalculateTax(TaxableIncome);

            // 6. Thu nhập NET
            NetIncome = GrossIncome - TotalInsurance - TaxAmount;

            Calculated = true;
        }

        private decimal CalculateTax(decimal taxable)
        {
            if (taxable <= 0) return 0;

            decimal tax = 0;

            // Bậc 1: 0 → 5 triệu
            if (taxable <= 5000000)
                return taxable * 0.05m;
            tax += 5000000 * 0.05m;

            // Bậc 2: 5 → 10 triệu
            if (taxable <= 10000000)
                return tax + (taxable - 5000000) * 0.10m;
            tax += 5000000 * 0.10m;

            // Bậc 3: 10 → 18 triệu
            if (taxable <= 18000000)
                return tax + (taxable - 10000000) * 0.15m;
            tax += 8000000 * 0.15m;

            // Bậc 4: 18 → 32 triệu
            if (taxable <= 32000000)
                return tax + (taxable - 18000000) * 0.20m;
            tax += 14000000 * 0.20m;

            // Bậc 5: 32 → 52 triệu
            if (taxable <= 52000000)
                return tax + (taxable - 32000000) * 0.25m;
            tax += 20000000 * 0.25m;

            // Bậc 6: 52 → 80 triệu
            if (taxable <= 80000000)
                return tax + (taxable - 52000000) * 0.30m;
            tax += 28000000 * 0.30m;

            // Bậc 7: > 80 triệu
            tax += (taxable - 80000000) * 0.35m;

            return tax;
        }
    }
}
