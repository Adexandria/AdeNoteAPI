using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AdeNote.Models
{
    public class RecoveryCode : IBaseEntity
    {

        protected RecoveryCode()
        {
                
        }

        public RecoveryCode(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Codes = GenerateCodes();
        }


        private string GenerateCodes()
        {
            var stringBuilder = new StringBuilder();

            for(int i = 0; i < 10; i++)
            {
                var generatedCode = Guid.NewGuid().ToString("N")[..10];

                var code = generatedCode.Insert(5, "-");

                if(i != 0)
                {
                    stringBuilder.Append(',');
                }
                stringBuilder.Append(code);
                
            }

            return stringBuilder.ToString();
        }



        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public string Codes { get; set; }
    }
}
