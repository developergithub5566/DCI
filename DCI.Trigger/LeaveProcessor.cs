using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Trigger
{
    public class LeaveProcessor
    {
        private readonly DestinationDbContext _destDb;

        public LeaveProcessor(DestinationDbContext destDb)
        {
            _destDb = destDb;
        }

        public async Task ProcessPendingMessages()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}
