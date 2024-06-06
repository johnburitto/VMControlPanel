namespace Core.Dtos
{
    public class DeleteAccountDto
    {
        public string? AccountUserName { get; set; }
        public string? AccountPassword { get; set; }
        public long TelegramId { get; set; }
    }
}
