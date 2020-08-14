using System.Collections.Generic;

namespace SweetAndSavory.Models
{
  public class Savory
  {
    public Savory()
    {
        this.Sweets = new HashSet<SweetSavory>();
    }
    public int SavoryId { get; set; }
    public string Name { get; set; }
    public virtual ApplicationUser User {get; set;}
    public virtual ICollection<SweetSavory> Sweets { get; set; }
  }
}