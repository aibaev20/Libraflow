﻿using System.Threading.Tasks;
using BookDepoSystem.Services.Common.Models;
using Essentials.Results;

namespace BookDepoSystem.Services.Common.Contracts;

public interface IEmailSender
{
    Task<StandardResult> SendEmailAsync(EmailModel model);
}