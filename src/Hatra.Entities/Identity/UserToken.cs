﻿using Hatra.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace Hatra.Entities.Identity
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2577
    /// and http://www.dotnettips.info/post/2578
    /// </summary>
    public class UserToken : IdentityUserToken<int>, IAuditableEntity
    {
        public virtual User User { get; set; }
    }
}