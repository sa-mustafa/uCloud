namespace Bps.uCloud.Gateway.Requests
{
    using FluentValidation;

    public class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.UserName)
                .NotNull()
                .WithMessage("User name is not provided!");

            RuleFor(x => x.UserName)
                .Length(1, 50)
                .WithMessage("User name's length exceeded limit of 50!");

            RuleFor(x => x.Password)
                .NotNull()
                .WithMessage("Password is not provided!");

            RuleFor(x => x.Password)
                .Length(1, 50)
                .WithMessage("Password's length exceeded limit of 50!");
        }
    }
}
