using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DCI.Trigger
{
    public class OutboxProcessor
    {
        private readonly AppDbContext _sourceDb;
        private readonly DestinationDbContext _destDb;


        public OutboxProcessor(AppDbContext sourceDb, DestinationDbContext destDb)
        {
            _sourceDb = sourceDb;
            _destDb = destDb;
        }

        public async Task ProcessPendingMessages()
        {
            try
            {

                var pending = await _sourceDb.OutboxQueue
                .Where(x => x.Status == "Pending")
                .Take(10)
                .ToListAsync();

                foreach (var message in pending)
                {
                    try
                    {
                        var log = JsonConvert.DeserializeObject<TblRawLog>(message.Payload);
                        _destDb.TblRawLogs.Add(log);
                        await _destDb.SaveChangesAsync();

                        message.Status = "Processed";
                        message.LastError = null;
                    }
                    catch (Exception ex)
                    {
                        message.RetryCount++;
                        message.LastError = ex.Message;

                        if (message.RetryCount >= 5)
                            message.Status = "Failed";
                    }

                    await _sourceDb.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}
