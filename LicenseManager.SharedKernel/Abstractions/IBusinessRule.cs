namespace LicenseManager.SharedKernel.Abstractions;

public interface IBusinessRule
{
    bool IsBroken();
    string? Message { get; }
}

