using AtlasBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace AtlasBlog.Services
{
    public class DataService
    {
        //Calling a method or instruction that executes the migrations
        readonly ApplicationDbContext _dbcontext;

        public DataService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task SetupDbAsync()
        {
            //Run the migrations async
           await _dbcontext.Database.MigrateAsync();
        }
    }
}
