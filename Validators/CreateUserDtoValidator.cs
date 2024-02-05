using FluentValidation;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email jest wymagany.")
                             .EmailAddress().WithMessage("Nieprawidłowy format adresu email.");

        RuleFor(x => x.Password).NotEmpty()
            .MinimumLength(8).WithMessage("Hasło musi mieć przynajmniej 8 znaków długości.")
            .Matches(@"[A-Z]").WithMessage("Hasło musi zawierać przynajmniej jedną wielką literę.")
            .Matches(@"[a-z]").WithMessage("Hasło musi zawierać przynajmniej jedną małą literę.")
            .Matches(@"[0-9]").WithMessage("Hasło musi zawierać przynajmniej jedną cyfrę.")
            .Matches(@"[\^$*.\[\]{}\(\)?\-""!@#%&/\\,><':;|_~`]").WithMessage("Hasło musi zawierać przynajmniej jeden znak specjalny.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role can't be empty.")
            .Must(role => role == "user" || role == "owner")
            .WithMessage("Are you a user or an owner?");
    }
}
