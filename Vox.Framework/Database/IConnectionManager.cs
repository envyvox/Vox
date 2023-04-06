using System.Data;

namespace Vox.Framework.Database;

public interface IConnectionManager
{
    IDbConnection GetConnection();
}
