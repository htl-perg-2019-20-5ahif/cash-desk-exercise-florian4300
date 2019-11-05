using CashDesk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashDesk
{
    /// <inheritdoc />
    public class DataAccess : IDataAccess
    {
        private MemberContext context;
        /// <inheritdoc />
        public async Task InitializeDatabaseAsync()
        {
            if(context != null)
            {
                throw new InvalidOperationException();
            }
            context = new MemberContext();
            return;
        }

        /// <inheritdoc />
        public async Task<int> AddMemberAsync(string firstName, string lastName, DateTime birthday)
        {
            if(firstName == null || lastName == null || birthday == null)
            {
                throw new ArgumentException();
            }
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var existingMembers = this.context.Members.ToList();
            var existingMember = existingMembers.Where(m => m.LastName.Equals(lastName));
            if(existingMember.Any())
            {
                throw new DuplicateNameException();
            }
            Member member = new Member();
            
            member.Birthday = birthday;
            member.FirstName = firstName;
            member.LastName = lastName;

            await this.context.AddAsync(member);
            var number = await this.context.SaveChangesAsync();
            return member.MemberNumber;
        }

        /// <inheritdoc />
        public async Task DeleteMemberAsync(int memberNumber)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var existingMember = this.context.Members.Find(memberNumber);
            if (existingMember == null)
            {
                throw new ArgumentException();
            }

            this.context.Remove(existingMember);
            await this.context.SaveChangesAsync();
            return;
        }

        /// <inheritdoc />
        public async Task<IMembership> JoinMemberAsync(int memberNumber)
        {

            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var existingMember = this.context.Members.Find(memberNumber);
            if(existingMember == null)
            {
                throw new NoMemberException();
            }
            if (existingMember.Membership != null )
                
            {
                if(existingMember.Membership.IsActive == true)
                {
                    throw new AlreadyMemberException();
                }
                
            }
            Membership mbs = new Membership();
            mbs.Begin = System.DateTime.Now;
            mbs.Member = existingMember;
            mbs.IsActive = true;
            existingMember.Membership = mbs;
            await this.context.AddAsync(mbs);
            await this.context.SaveChangesAsync();
            return mbs;
        }

        /// <inheritdoc />
        public async Task<IMembership> CancelMembershipAsync(int memberNumber)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var existingMember = this.context.Members.Find(memberNumber);
            if (existingMember == null)
            {
                throw new ArgumentException();
            }
            if(existingMember.Membership == null)
            {
                throw new NoMemberException();
            }
            existingMember.Membership.End = System.DateTime.Now;
            existingMember.Membership.IsActive = false;
            this.context.Update(existingMember);
            await this.context.SaveChangesAsync();
            return existingMember.Membership;
        }

        /// <inheritdoc />
        public async Task DepositAsync(int memberNumber, decimal amount)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var existingMember = this.context.Members.Find(memberNumber);
            if (existingMember == null || amount <= 0)
            {
                throw new ArgumentException();
            }
            if (existingMember.Membership == null)
            {
                throw new NoMemberException();
            }
            Deposit dp = new Deposit();
            dp.Membership = existingMember.Membership;
            dp.Amount = amount;
            if(existingMember.Membership.Deposits == null)
            {
                existingMember.Membership.Deposits = new List<Deposit>();
            }
            existingMember.Membership.Deposits.Add(dp);
            this.context.Add(dp);
            await this.context.SaveChangesAsync();
            return;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IDepositStatistics>> GetDepositStatisticsAsync()
        {
            
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            
            
            List<DepositStatistics> statistics = new List<DepositStatistics>();

            foreach(Deposit dp in this.context.Deposits.ToArray())
            {
                if (dp.Membership == null)
                {
                    continue;
                }
                if (statistics.Find(st => st.Member == dp.Membership.Member) == null)
                {
                    DepositStatistics dps = new DepositStatistics();
                    dps.Member = dp.Membership.Member;
                    dps.TotalAmount = dp.Amount;
                    dps.Year = dp.Membership.End.Year;
                    statistics.Add(dps);
                }
                else
                {
                    var statistic = statistics.Find(st => st.Member == dp.Membership.Member);
                    statistic.TotalAmount = Decimal.Add(dp.Amount, statistic.TotalAmount);
                }
            }
            return statistics;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if(context != null)
            {
                this.context.Dispose();
            }   
        }
    }
}
