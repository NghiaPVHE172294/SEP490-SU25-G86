using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class SalaryCalculatorModel : PageModel
{
    [BindProperty] public decimal Salary { get; set; }
    [BindProperty] public int Dependents { get; set; }
    [BindProperty] public int Region { get; set; }

    [BindProperty] public string InsuranceOption { get; set; } = "official";
    [BindProperty] public decimal? CustomInsuranceSalary { get; set; }

    public decimal BaseSalary { get; set; } = 2340000;
    public decimal SelfDeduction { get; set; } = 11000000;
    public decimal DependentDeduction { get; set; } = 4400000;

    public string? Result { get; set; }

    // === Hàm tính thuế TNCN ===
    private decimal CalculatePersonalIncomeTax(decimal taxableIncome)
    {
        if (taxableIncome <= 0) return 0;

        var brackets = new (decimal limit, decimal rate)[]
        {
            (5000000, 0.05m),
            (10000000, 0.10m),
            (18000000, 0.15m),
            (32000000, 0.20m),
            (52000000, 0.25m),
            (80000000, 0.30m),
            (decimal.MaxValue, 0.35m)
        };

        decimal tax = 0;
        decimal remaining = taxableIncome;
        decimal previousLimit = 0;

        foreach (var (limit, rate) in brackets)
        {
            var range = Math.Min(remaining, limit - previousLimit);
            if (range <= 0) break;

            tax += range * rate;
            remaining -= range;
            previousLimit = limit;
        }

        return tax;
    }

    // === Gross → Net ===
    public void OnPostGrossToNet()
    {
        var insuranceBase = InsuranceOption == "custom" && CustomInsuranceSalary.HasValue
            ? CustomInsuranceSalary.Value
            : Salary;

        var bhxh = insuranceBase * 0.08m;
        var bhyt = insuranceBase * 0.015m;
        var bhtn = insuranceBase * 0.01m;

        var taxableIncome = Salary - bhxh - bhyt - bhtn - SelfDeduction - Dependents * DependentDeduction;
        var tax = CalculatePersonalIncomeTax(taxableIncome);

        var net = Salary - bhxh - bhyt - bhtn - tax;

        Result = $"💡 Lương Net ước tính: <strong>{net:N0}đ</strong> " +
                 $"(BHXH: {bhxh:N0}đ, BHYT: {bhyt:N0}đ, BHTN: {bhtn:N0}đ, Thuế: {tax:N0}đ)";
    }

    // === Net → Gross (dùng Binary Search) ===
    public void OnPostNetToGross()
    {
        decimal targetNet = Salary;

        // Tìm khoảng Gross hợp lý
        decimal low = targetNet;
        decimal high = targetNet * 3;
        decimal gross = targetNet;

        while (low <= high)
        {
            gross = (low + high) / 2;

            // Tính bảo hiểm dựa trên Gross hoặc CustomInsurance
            var insuranceBase = InsuranceOption == "custom" && CustomInsuranceSalary.HasValue
                ? CustomInsuranceSalary.Value
                : gross;

            var bhxh = insuranceBase * 0.08m;
            var bhyt = insuranceBase * 0.015m;
            var bhtn = insuranceBase * 0.01m;

            // Thu nhập chịu thuế
            var taxableIncome = gross - bhxh - bhyt - bhtn - SelfDeduction - Dependents * DependentDeduction;
            if (taxableIncome < 0) taxableIncome = 0;

            // Tính thuế TNCN
            var tax = CalculatePersonalIncomeTax(taxableIncome);

            // Lương Net tạm tính
            var calculatedNet = gross - bhxh - bhyt - bhtn - tax;

            // Kiểm tra sai số
            if (Math.Abs(calculatedNet - targetNet) < 500)
                break;

            if (calculatedNet > targetNet)
                high = gross - 1;
            else
                low = gross + 1;
        }

        Result = $"💡 Lương Gross ước tính: <strong>{gross:N0}đ</strong>";
    }

}
