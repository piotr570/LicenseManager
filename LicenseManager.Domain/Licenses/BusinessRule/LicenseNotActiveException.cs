namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseNotActiveException(string message) : Exception(message);