namespace MyReadsApp.Core.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
