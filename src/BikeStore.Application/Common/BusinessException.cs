namespace BikeStore.Application.Common;

public sealed class BusinessException(string message) : Exception(message);

public sealed class NotFoundException(string message) : Exception(message);

public sealed class ConflictException(string message) : Exception(message);
