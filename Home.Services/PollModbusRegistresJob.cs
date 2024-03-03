using System.Net;
using FluentModbus;
using Home.Common.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Home.Services
{
    public class PollModbusRegistresJob : IJob
    {
        private readonly HomeHeatingService _heatingService;

        public PollModbusRegistresJob(
            HomeHeatingService heatingService)
        {
            _heatingService = heatingService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _heatingService.UpdateDataPoints();

            return Task.FromResult(true);
        }
    }
}
