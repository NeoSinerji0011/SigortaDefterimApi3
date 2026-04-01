using Swashbuckle.AspNetCore.Filters;

namespace SigortaDefterimV2API.Models.Examples
{
    public class LoginRequestExample : IExamplesProvider<LoginInput>
    {
        public LoginInput GetExamples()
        {
            return new LoginInput
            {
                Email = "mehmetust@gmail.com",
                Password = "222222"
            };
        }
    }
}
