using Hw10.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace Hw10.Services.CachedCalculator;

public class MathCachedCalculatorService : IMathCalculatorService
{
	private readonly IMemoryCache _memoryCache;
	private readonly IMathCalculatorService _simpleCalculator;

	public MathCachedCalculatorService(IMemoryCache memoryCache, IMathCalculatorService simpleCalculator)
	{
		_memoryCache = memoryCache;
		_simpleCalculator = simpleCalculator;
	}

	public async Task<CalculationMathExpressionResultDto> CalculateMathExpressionAsync(string? expression)
	{
		if (expression is null)
			return await _simpleCalculator.CalculateMathExpressionAsync(expression);
		if (_memoryCache.TryGetValue(expression, out double res))
		{
			return new CalculationMathExpressionResultDto(res);
		}

		var result = await _simpleCalculator.CalculateMathExpressionAsync(expression);
		if (!result.IsSuccess) return result;

		_memoryCache.Set(expression, result.Result);

		return result;
	}
}