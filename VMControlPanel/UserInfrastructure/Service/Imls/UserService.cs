﻿using Core.Dtos;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserInfrastructure.Data;
using UserInfrastructure.Service.Interfaces;

namespace UserInfrastructure.Service.Imls
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckIfUserHasAccountAsync(long telegramId)
        {
            return await _context.Users.Where(_ => _.TelegramId == telegramId).AnyAsync();
        }

        public async Task<bool> CheckIfAccountWithUserNameExistAsync(string? userName)
        {
            return await _context.Users.Where(_ => _.UserName == userName).AnyAsync();
        }

        public async Task<AuthResponse> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.Where(_ => _.UserName == dto.UserName && _.PasswordHash == ComputeSha256Hash(dto.Password)).FirstOrDefaultAsync();

            return user == null ? AuthResponse.BadCredentials : AuthResponse.SuccessesLogin;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDto dto)
        {
            if (await CheckIfAccountWithUserNameExistAsync(dto.UserName))
            {
                return AuthResponse.AlreadyRegistered;
            }

            await CreateAsync(dto);

            return AuthResponse.SuccessesRegister;
        }

        private async Task CreateAsync(RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                PasswordHash = ComputeSha256Hash(dto.Password),
                Email = dto.Email
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        private string ComputeSha256Hash(string? data)
        {
            if (data == null)
            {
                return "";
            }

            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
                var strBuilder = new StringBuilder();

                foreach (var _ in bytes)
                {
                    strBuilder.Append(_.ToString("x2"));
                }

                return strBuilder.ToString();
            }
        }
    }
}