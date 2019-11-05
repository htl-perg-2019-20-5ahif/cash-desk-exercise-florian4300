﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashDesk.Model
{
    public class Membership : IMembership
    {
        public int MembershipId { get; set; }
        public Member Member { get; set; }
        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public bool IsActive { get; set; }

        public List<Deposit> Deposits { get; set; }

        [NotMapped]
        IMember IMembership.Member => Member;

        
    }
}
