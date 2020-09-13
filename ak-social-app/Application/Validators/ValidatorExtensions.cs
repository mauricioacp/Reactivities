using FluentValidation;

namespace Application.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
            .NotEmpty().MinimumLength(6)
                .WithMessage("Passsword must be at least 6 character")
                .Matches("[A-Z]").WithMessage("Passsword must contain 1 uppercase letter")
                .Matches("[a-z]").WithMessage("Passsword must contain at least 1 lowercase character")
                .Matches("[0-9]").WithMessage("Password must contain a number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain non alphanumeric");

            return options;
        }
    }
}