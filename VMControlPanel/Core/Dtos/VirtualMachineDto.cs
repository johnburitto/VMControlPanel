﻿namespace Core.Dtos
{
    public class VirtualMachineDto
    {
        public string? Name { get; set; }
        public long UserTelegramId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
    }
}
