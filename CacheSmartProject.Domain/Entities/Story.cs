using CacheSmartProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSmartProject.Domain.Entities
{
    public class Story : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsPublished { get; set; }
    }
}
