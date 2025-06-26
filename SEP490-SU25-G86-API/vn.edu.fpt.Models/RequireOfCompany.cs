using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class RequireOfCompany
    {
        public bool RequireOfCompanyId { get; set; }
        public int? CompanyId { get; set; }
        public int SendByUserId { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Company? Company { get; set; }
        public virtual User SendByUser { get; set; } = null!;
    }
}
