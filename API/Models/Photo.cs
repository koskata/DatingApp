using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    
    public required string Url { get; set; }

    public bool IsMain { get; set; }

    public string? PublicId { get; set; }

    //Navigation props

    public int AppUserId { get; set; }

    public AppUser AppUser { get; set; } = null!;
}
