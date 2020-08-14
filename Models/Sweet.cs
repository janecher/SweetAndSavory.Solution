using System.Collections.Generic;

namespace SweetAndSavory.Models
{
  public class Sweet
  {
    public Sweet()
    {
        this.Savories = new HashSet<SweetSavory>();
    }
    public int SweetId { get; set; }
    public string Name { get; set; }
    public virtual ICollection<SweetSavory> Savories { get; set; }
  }
}