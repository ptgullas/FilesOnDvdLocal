﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LegacyMediaFilesOnDvd.Data.Models;

public partial class LegacyPublisher
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; }

    [InverseProperty("Publisher")]
    public virtual ICollection<LegacySeries> LegacySeries { get; } = new List<LegacySeries>();
}