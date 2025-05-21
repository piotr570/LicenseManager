namespace LicenseManager.Domain.Licenses.BusinessRule;

public class PolicyViolationException(string message) : Exception(message);