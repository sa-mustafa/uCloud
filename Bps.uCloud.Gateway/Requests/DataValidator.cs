namespace Bps.uCloud.Gateway.Requests
{
    using Bps.Common;
    using Bps.uCloud.Contracts;
    using FluentValidation;

    public class DataValidator : AbstractValidator<Data>
    {
        public DataValidator(Settings.IAppSettings appSettings)
        {
            var maxFileSize = (int)appSettings.MaxUploadSize.Size(FileSize.Unit.Byte);
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Format)
                .NotEmpty()
                .WithMessage("Data format is not set!");

            RuleFor(x => (int)x.Type)
                .ExclusiveBetween((int)DataTypes.None, (int)DataTypes.Reserved)
                 .WithMessage("Data type is unknown!");

            RuleFor(x => x.Size)
                .ExclusiveBetween(0, maxFileSize)
                .WithMessage($"Maximum file size of {maxFileSize} exceeded!");

        }
    }
}
