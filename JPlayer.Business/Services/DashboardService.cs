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
        ///     Return the available tiles for Administration dashboard for current user functions
        /// </summary>
        /// <param name="functions">current authenticated user functions</param>
        /// <returns></returns>
        public async Task<IEnumerable<TileCollectionItem>> GetAdminTiles(string[] functions)
        {
            this._logger.LogInformation($"Getting administration tiles information with functions {string.Join(", ", functions)}");
            List<TileCollectionItem> tiles = new();

            if (functions.Contains(JPlayerRoles.UserRead))
                tiles.Add(new TileCollectionItem
                {
                    Label = GlobalLabelCodes.UserTileLabel,
                    SubLabel = GlobalLabelCodes.UserTileSubLabel,
                    Count = await this._dbContext.Users.CountAsync()
                });

            if (functions.Contains(JPlayerRoles.ProfileRead))
            {
                tiles.Add(new TileCollectionItem
                {
                    Label = GlobalLabelCodes.ProfileTileLabel,
                    SubLabel = GlobalLabelCodes.ProfileTileSubLabel,
                    Count = await this._dbContext.Profiles.CountAsync()
                });

                tiles.Add(new TileCollectionItem
                {
                    Label = GlobalLabelCodes.FunctionTileLabel,
                    SubLabel = GlobalLabelCodes.FunctionTileSubLabel,
                    Count = await this._dbContext.Functions.CountAsync()
                });
            }

            this._logger.LogInformation($"{tiles.Count} tiles return");

            return tiles;
        }
    }
}