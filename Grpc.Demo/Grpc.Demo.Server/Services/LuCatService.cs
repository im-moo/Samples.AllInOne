using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.Demo.Server
{
    public class LuCatService : LuCat.LuCatBase
    {
        private readonly ILogger<LuCatService> _logger;
        private static readonly List<string> Cats = new List<string>() { "Ӣ��������", "Ӣ�̽𽥲�", "����", "��è", "�껨è", "��è" };
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        public LuCatService(ILogger<LuCatService> logger)
        {
            _logger = logger;
        }

        public override async Task BathTheCat(IAsyncStreamReader<BathTheCatReq> requestStream, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            var bathQueue = new Queue<int>();
            while (await requestStream.MoveNext())
            {
                bathQueue.Enqueue(requestStream.Current.Id);

                _logger.LogInformation($"NO.{requestStream.Current.Id} Enqueue.");
            }

            while (!context.CancellationToken.IsCancellationRequested
                   && bathQueue.TryDequeue(out var catId))
            {
                await responseStream.WriteAsync(new BathTheCatResp() { Message = $"No.{catId} {Cats[catId]} ��ϴ�裡" });
                await Task.Delay(500);
            }

        }

        public override Task<CountCatResult> Count(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new CountCatResult() { Count = Cats.Count });
        }

        public override Task<SuckingCatResult> SuckingCat(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new SuckingCatResult()
            {
                Message = $"������һֻ{Cats[Rand.Next(0, Cats.Count)]}"
            });
        }

    }

}