using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;

using Telegram.Bot.Types;

using Bazis.Hubs;

 

namespace Bazis.Controllers

{

   [ApiController]

   [Route("api/telegram")]

   public class TelegramWebhookController : ControllerBase

   {

       private readonly IHubContext<ChatHub> _hubContext;

       private readonly long _managerChatId;

 

       public TelegramWebhookController(

           IHubContext<ChatHub> hubContext,

           IConfiguration config)

       {

           _hubContext    = hubContext;

           _managerChatId = config.GetValue<long>("Telegram:ManagerChatId");

       }

 

       [HttpPost("update")]

       public async Task<IActionResult> Post([FromBody] Update update)

       {

           // Проверяем, что это сообщение от пользователя (не callback, не прочее)

           if (update.Message != null && update.Message.Chat.Id == _managerChatId)

           {

               var text     = update.Message.Text;

               var userName = update.Message.From.Username ?? update.Message.From.FirstName;

 

               // 1) Отправляем обратно всем клиентам на сайте

               await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Менеджер", text);

           }

 

           return Ok();

       }

   }

}