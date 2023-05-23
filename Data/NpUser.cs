using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalADO.Data
{
    public class NpUser // Набор параметров пользователя
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ConfirmCode { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
