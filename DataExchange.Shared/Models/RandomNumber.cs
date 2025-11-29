using System;

namespace DataExchange.Shared.Models
{
    public class RandomNumber
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
