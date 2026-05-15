using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Entities
{
    public abstract class BaseEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

    }

}
