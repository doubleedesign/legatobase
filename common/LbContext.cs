using Microsoft.EntityFrameworkCore;
using Legatobase.Core;

namespace Legatobase.Common;

public class LbContext(DbContextOptions<LbContextBase> options) : LbContextBase(options) {
	private string? _dbPath = new ContextUtils().GetDbPath();

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
		optionsBuilder.UseSqlite($"Data Source={this._dbPath}");
	}
}