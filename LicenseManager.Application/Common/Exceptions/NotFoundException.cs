namespace LicenseManager.Application.Common.Exceptions;

public class NotFoundException(string field, Guid id) : Exception(field);