using Legatobase.Common;
using Legatobase.Core;
using Microsoft.EntityFrameworkCore;
namespace Legatobase.API;

public class LbContext(DbContextOptions<LbContextBase> options) : LbContextBase(options) {
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.UseSqlite($"Data Source={Config.GetDbPath()}");
	}
}