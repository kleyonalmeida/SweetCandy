using Mapster;

namespace Application.Common.Mappings;

public static class MapsterSettings
{
  public static TypeAdapterConfig Config { get; } = CreateConfig();

  public static TypeAdapterConfig IgnoreNullValues { get; } = Config;

  private static TypeAdapterConfig CreateConfig()
  {
    var config = new TypeAdapterConfig();
    config.Default.IgnoreNullValues(true);
    config.ForType<string?, string?>().MapWith(src => src == null ? null : src.Trim());
    return config;
  }
}