using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Bazis.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ITelegramBotClient _botClient;
        private readonly long               _managerChatId;

        public ChatHub(ITelegramBotClient botClient, IConfiguration config)
        {
            _botClient     = botClient;
            _managerChatId = config.GetValue<long>("Telegram:ManagerChatId");
        }

        /// <summary>
        /// Вызывается клиентом JS: connection.invoke("SendMessage", message);
        /// </summary>
        public async Task SendMessage(string message)
        {
            // 1) Получаем идентификатор текущего пользователя из SignalR Context
            //    По умолчанию это ClaimTypes.NameIdentifier из Identity
            var userId   = Context.UserIdentifier ?? "unknown";
            var userName = Context.User?.Identity?.Name ?? userId;

            // 2) Рассылаем всем подключённым на сайте
            await Clients.All.SendAsync("ReceiveMessage", userName, message);

            // 3) Дублируем менеджеру в Telegram
            //    Форматируем сообщение так, чтобы менеджер видел, от кого
            var text = $"[{userId}] {userName}: {message}";
            try
            {
                await _botClient.SendTextMessageAsync(_managerChatId, text);
            }
            catch (Exception ex)
            {
                // здесь можно залогировать ошибку отправки в Telegram
                Console.Error.WriteLine($"Telegram send failed: {ex}");
            }
        }
    }
}
