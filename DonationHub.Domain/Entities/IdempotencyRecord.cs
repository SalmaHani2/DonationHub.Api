using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class IdempotencyRecord
    {
        //Key, UserId, RequestHash, ResponseStatusCode, ResponseBody, CreatedAt, ExpiresAt.
        public Guid Id { get; set; }
        public string Key { get; set; }
        public Guid UserId { get; set; }
        public string RequestHash { get; set; }
        public int ResponseStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
