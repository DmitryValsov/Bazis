using Microsoft.AspNetCore.SignalR;

using Telegram.Bot;

 

namespace Bazis.Hubs

{

   public class ChatHub : Hub

   {

       private readonly ITelegramBotClient _botClient;

       private readonly long _managerChatId;

 

       public ChatHub(ITelegramBotClient botClient, IConfiguration config)

       {

           _botClient     = botClient;

           _managerChatId = config.GetValue<long>("Telegram:ManagerChatId");

       }

 

       // Клиент → сервер (JS вызывает connection.invoke("SendMessage", message))

       public async Task SendMessage(string message)

       {

           var userId   = Context.UserIdentifier ?? "unknown";

           var userName = Context.User?.Identity?.Name ?? userId;

 

           // 1) Рассылаем всем подключённым в браузере

           await Clients.All.SendAsync("ReceiveMessage", userName, message);

 

           // 2) Дублируем менеджеру в Telegram

           var text = $"[{userId}] {userName}: {message}";

           await _botClient.SendTextMessageAsync(_managerChatId, text);

       }

   }

}