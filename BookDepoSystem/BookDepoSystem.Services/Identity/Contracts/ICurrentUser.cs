using System;

namespace BookDepoSystem.Services.Identity.Contracts;

public interface ICurrentUser
{
    Guid? UserId { get; }

    bool Exists { get; }
}