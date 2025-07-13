using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bazis.Services
{
    public class TelegramService
    {
        private readonly string _botToken = "7571397109:AAFD7jN642c8FXYsiUMY7gZNUh7fzJfA8Ms"; // Токен вашего бота
        private readonly long _chatId = 140596009; // Ваш ID чата в Telegram (получить через @userinfobot)

        public async Task SendMessageAsync(string message)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                chat_id = _chatId,
                text = message
            }), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Не удалось отправить сообщение в Telegram.");
                }
            }
        }
    }
}
