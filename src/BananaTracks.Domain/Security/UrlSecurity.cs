using System.Net;
using System.Security.Cryptography;
using System.Text;
using BananaTracks.Domain.Providers;
using Microsoft.Extensions.Configuration;

namespace BananaTracks.Domain.Security;

public class UrlSecurity
{
	public enum ValidationStatus
	{
		MissingParameters,
		ExpiredTimestamp,
		InvalidToken,
		Success
	}

	/// <summary>
	/// The default amount of time in milliseconds that URLs are valid for.
	/// </summary>
	public const int DefaultUrlExpirationInMilliseconds = 1 * 60 * 60 * 1000; // 1 hour

	private const string TimestampParam = "timestamp";
	private const string HashParam = "hash";

	private readonly ITimeProvider _timeProvider;
	private readonly int _urlExpiration;
	private readonly string _salt;

	public UrlSecurity(IConfiguration configuration, ITimeProvider? timeProvider = null, int? urlExpiration = null)
	{
		_timeProvider = timeProvider ?? new UtcTimeProvider();
		_urlExpiration = urlExpiration ?? DefaultUrlExpirationInMilliseconds;
		_salt = configuration["BananaTracks:HashSalt"];
	}

	/// <summary>
	/// Gets a secure URL containing the specified querystring parameters, a timestamp, and a hash token.
	/// </summary>
	/// <param name="baseUrl">An absolute URL for the requested resource.</param>
	/// <param name="parameters">A parameter collection to be used for the URL's querystring.</param>
	public Uri GenerateSecureUrl(string baseUrl, Dictionary<string, object>? parameters)
	{
		if (string.IsNullOrWhiteSpace(baseUrl))
		{
			throw new ArgumentException("URL cannot be null or whitespace.", nameof(baseUrl));
		}

		parameters ??= new Dictionary<string, object>();
		var timestamp = _timeProvider.Now().Ticks;

		var input = GetDelimitedParameters(parameters, timestamp);
		var hash = GetHash(input);

		var uriBuilder = new UriBuilder(baseUrl)
		{
			Query = GetQueryString(parameters, timestamp, hash)
		};

		return uriBuilder.Uri;
	}

	private string GetDelimitedParameters(IReadOnlyDictionary<string, object> parameters, long timestamp)
	{
		var values = parameters.Values.ToList();
		values.Add(timestamp);
		values.Add(_salt);

		return string.Join("|", values);
	}

	private static string GetQueryString(IDictionary<string, object> parameters, long? timestamp, string token)
	{
		var values = parameters.ToDictionary(i => i.Key, i => i.Value);
		values.Add(TimestampParam, timestamp!);
		values.Add(HashParam, token);
		return string.Join("&", values.Select(i => $"{i.Key}={WebUtility.UrlEncode(i.Value.ToString())}"));
	}

	public static string GetHash(string input)
	{
		using var sha256 = SHA256.Create();
		var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
		return Convert.ToBase64String(hash);
	}

	/// <summary>
	/// Returns whether or not a secure URL is valid.
	/// </summary>
	public ValidationStatus ValidateSecureUrl(UrlToken urlToken, params object[] parameters)
	{
		var dictionary = new Dictionary<string, object>();
		for (var i = 0; i < parameters.Length; i++)
		{
			dictionary.Add(FormattableString.Invariant($"{i}"), parameters[i]);
		}

		return ValidateSecureUrl(urlToken, dictionary);
	}

	/// <summary>
	/// Returns whether or not a secure URL is valid.
	/// </summary>
	public ValidationStatus ValidateSecureUrl(UrlToken urlToken, Dictionary<string, object> parameters)
	{
		if (!ValidateParameters(urlToken))
		{
			return ValidationStatus.MissingParameters;
		}

		if (!ValidateTimestamp(urlToken.Timestamp))
		{
			return ValidationStatus.ExpiredTimestamp;
		}

		if (!ValidateToken(urlToken, parameters))
		{
			return ValidationStatus.InvalidToken;
		}

		return ValidationStatus.Success;
	}

	private static bool ValidateParameters(UrlToken urlToken)
	{
		return urlToken.Timestamp > DateTime.MinValue.Ticks && !string.IsNullOrWhiteSpace(urlToken.Hash);
	}

	private bool ValidateTimestamp(long timestamp)
	{
		try
		{
			var dt = new DateTime(timestamp);
			var now = _timeProvider.Now();
			return now.Subtract(dt).TotalMilliseconds <= _urlExpiration;
		}
		catch
		{
			return false;
		}
	}

	private bool ValidateToken(UrlToken urlToken, IReadOnlyDictionary<string, object> parameters)
	{
		var input = GetDelimitedParameters(parameters, urlToken.Timestamp);
		var hash = GetHash(input);

		return hash.Equals(urlToken.Hash, StringComparison.Ordinal);
	}
}
