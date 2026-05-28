namespace db;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

public class DesignTimeServices : IDesignTimeServices {
	public void ConfigureDesignTimeServices(IServiceCollection services) {
		services.AddSingleton<IPluralizer, TableEntityMapper>();
	}
}