namespace Schefco.TaskFlow.API.Services
{
    public class TimeService : ITimeService
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
