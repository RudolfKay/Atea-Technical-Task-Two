using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace Atea.Task2.Models;

/// <summary>
/// Represents a record of weather data for a specific location and timestamp.
/// </summary>
/// <remarks>
/// This class is used to store weather data in the database, including temperature readings, humidity, and the timestamp of the record.
/// It is marked with data annotations to enforce required fields and display names.
/// </remarks>
public class WeatherRecord
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required, DisplayName("Minimum Temperature")]
    public double MinTemp { get; set; }
    [Required, DisplayName("Maximum Temperature")]
    public double MaxTemp { get; set; }
    [Required, DisplayName("Current Temperature")]
    public double CurrentTemp { get; set; }
    public int Humidity { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Represents the weather response data returned from the weather API.
/// </summary>
/// <remarks>
/// This class is used to deserialize the JSON response from the weather API into a .NET object.
/// It contains various weather-related information, including coordinates, weather conditions, and system data.
/// </remarks>
public class WeatherResponse
{
    [JsonPropertyName("coord")]
    public Coord Coord { get; set; } = new Coord();

    [JsonPropertyName("weather")]
    public List<Weather> Weather { get; set; } = new List<Weather>();

    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;

    [JsonPropertyName("main")]
    public Main Main { get; set; } = new Main();

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public Wind Wind { get; set; } = new Wind();

    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; } = new Clouds();

    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("sys")]
    public Sys Sys { get; set; } = new Sys();

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("cod")]
    public int Cod { get; set; }
}

public class Coord
{
    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("lat")]
    public double Lat { get; set; }
}

public class Weather
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class Main
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("sea_level")]
    public int SeaLevel { get; set; }

    [JsonPropertyName("grnd_level")]
    public int GrndLevel { get; set; }
}

public class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Deg { get; set; }

    [JsonPropertyName("gust")]
    public double Gust { get; set; }
}

public class Clouds
{
    [JsonPropertyName("all")]
    public int All { get; set; }
}

public class Sys
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}
