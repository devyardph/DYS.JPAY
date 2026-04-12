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

    }

}
