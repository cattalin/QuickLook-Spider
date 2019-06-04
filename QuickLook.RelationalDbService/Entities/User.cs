using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickLook.RelationalDbService.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        public virtual List<Bookmark> Bookmarks { get; set; }
    }
}
