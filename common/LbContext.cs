using Microsoft.EntityFrameworkCore;
using Legatobase.Core;

namespace Legatobase.Common;

public class LbContext(DbContextOptions<LbContextBase> options) : LbContextBase(options) {
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.UseSqlite($"Data Source={Config.GetDbPath()}");
	}
}