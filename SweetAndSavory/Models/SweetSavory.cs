namespace SweetAndSavory.Models
{
  public class SweetSavory
    {       
        public int SweetSavoryId { get; set; }
        public int SweetId { get; set; }
        public int SavoryId { get; set; }
        public virtual Sweet Sweet { get; set; }
        public virtual Savory Savory { get; set; }
    }
}