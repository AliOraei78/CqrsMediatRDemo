using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Application.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}