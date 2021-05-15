using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JPlayer.Data;
using JPlayer.Data.Dao;
using JPlayer.Data.Dto.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JPlayer.Business.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ILogger<DashboardService> logger, ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        /// <summary>
        ///     Return available menu items for a specific user functions
        /// </summary>
        /// <param name="functions">current authenticated user functions</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetMenuList(string[] functions)
        {
            this._logger.LogInformation("Getting menu list with functions {Functions}",
                string.Join(", ", functions));


            List<string> types = new() {GlobalLabelCodes.DefaultMenu};
            types.AddRange(
                await this._dbContext.Functions
                    .Where(f => functions.Contains(f.FunctionCode))
                    .Select(f => f.Type)
                    .ToListAsync()
            );

            return types.Distinct();
        }

        /// <summary>
        ///     Return the available tiles for Administration dashboard for current user functions
        /// </summary>
        /// <param name="functions">current authenticated user functions</param>
        /// <returns></returns>
        public async Task<IEnumerable<TileCollectionItem>> GetAdminTiles(string[] functions)
        {
            this._logger.LogInformation("Getting administration tiles information with functions {Functions}",
                string.Join(", ", functions));
            List<TileCollectionItem> tiles = new();

            if (functions.Contains(JPlayerRoles.UserRead))
                tiles.Add(new TileCollectionItem
                {
                    Label = GlobalLabelCodes.UserTileLabel,
                    Icon = GlobalLabelCodes.UserTileIcon,
                    Link = GlobalLabelCodes.UserTileLink,
                    Count = await this._dbContext.Users.CountAsync()
                });

            if (functions.Contains(JPlayerRoles.ProfileRead))
            {
                tiles.Add(new TileCollectionItem
                {
                    Label = GlobalLabelCodes.ProfileTileLabel,
                    Icon = GlobalLabelCodes.ProfileTileIcon,
                    Link = GlobalLabelCodes.ProfileTileLink,
                    Count = await this._dbContext.Profiles.CountAsync()
                });

                tiles.Add(new TileCollectionItem
                {
                    Label = GlobalLabelCodes.FunctionTileLabel,
                    Icon = GlobalLabelCodes.FunctionTileIcon,
                    Link = GlobalLabelCodes.FunctionTileLink,
                    Count = await this._dbContext.Functions.CountAsync()
                });
            }

            this._logger.LogInformation("{Count} tiles return", tiles.Count);

            return tiles;
        }
    }
}