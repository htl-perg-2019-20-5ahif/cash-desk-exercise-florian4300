using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashDesk.Model
{
    public class Deposit : IDeposit
    {
        public int DepositId { get; set; }
        public Membership Membership { get; set; }

        public decimal Amount { get; set; }

        [NotMapped]
        IMembership IDeposit.Membership => Membership;

    }
}
