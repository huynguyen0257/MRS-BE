using System;
namespace MRS.ViewModels
{
    //public Guid Id { get; set; }
    //public string Title { get; set; }
    //public string Content { get; set; }
    //public string Review { get; set; }
    //public DateTime DateCreated { get; set; }
    //public string UserCreated { get; set; }
    //public DateTime? DateUpdated { get; set; }
    //public string UserUpdated { get; set; }
    public class NewsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class NewsCM
    {
        public string Title { get; set; }
        public string Review { get; set; }
        public string Content { get; set; }
    }

    public class NewsDetailVM : NewsUM
    {
        
        public DateTime DateCreated { get; set; }
        public string UserCreated { get; set; }
    }

    public class NewsUM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public string Content { get; set; }
    }
}
