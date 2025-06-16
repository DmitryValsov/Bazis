
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Bazis.Hubs;

namespace Bazis.Services
{
    public class TelegramRelayService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly long _managerChatId;

        public TelegramRelayService(
            ITelegramBotClient botClient,
            IHubContext<ChatHub> hubContext,
            IConfiguration config)
        {
            _botClient      = botClient;
            _hubContext     = hubContext;
            _managerChatId  = config.GetValue<long>("Telegram:ManagerChatId");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Запускаем приём обновлений
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } },
                cancellationToken: stoppingToken
            );

            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ct)
        {
            if (update.Type != UpdateType.Message) return;

            var msg = update.Message!;
            // от менеджера ожидаем ответ в формате: "[userId]: ответ"
            if (msg.Chat.Id != _managerChatId || msg.Text is null) return;

            var text = msg.Text.Trim();
            if (!text.StartsWith("[") || !text.Contains("]:")) return;

            var idx = text.IndexOf("]:");
            var userIdStr = text.Substring(1, idx - 1);
            var reply      = text.Substring(idx + 2).Trim();

            // отсылаем обратно на сайт конкретному пользователю
            await _hubContext
                .Clients
                .User(userIdStr)
                .SendAsync("ReceiveMessage", "Менеджер", reply, ct);
        }

        private Task HandleErrorAsync(ITelegramBotClient client, Exception ex, CancellationToken ct)
        {
            // тут можно залогировать
            return Task.CompletedTask;
        }
    }
}
