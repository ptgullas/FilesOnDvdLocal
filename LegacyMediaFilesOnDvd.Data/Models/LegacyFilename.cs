﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LegacyMediaFilesOnDvd.Data.Models;

public partial class LegacyFilename
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    public string Name { get; set; }

    public long? GenreId { get; set; }

    public long? DiscId { get; set; }

    public long? SeriesId { get; set; }

    public string Notes { get; set; }

    public long? UnknownActors { get; set; }

    public string Unwatched { get; set; }

    [ForeignKey("DiscId")]
    [InverseProperty("LegacyFilenames")]
    public virtual LegacyDisc Disc { get; set; }

    [ForeignKey("GenreId")]
    [InverseProperty("LegacyFilenames")]
    public virtual LegacyGenre Genre { get; set; }

    [ForeignKey("SeriesId")]
    [InverseProperty("LegacyFilenames")]
    public virtual LegacySeries Series { get; set; }

    [ForeignKey("FilenameId")]
    [InverseProperty("Filenames")]
    public virtual ICollection<LegacyPerformer> Performers { get; } = new List<LegacyPerformer>();
}